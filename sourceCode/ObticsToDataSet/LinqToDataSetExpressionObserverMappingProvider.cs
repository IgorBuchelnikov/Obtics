using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;
using System.Data;
using System.Reflection;

namespace Obtics.Data
{
    [ObticsRegistration]
    public class LinqToDataSetExpressionObserverMappingProvider : IExpressionObserverMappingProvider
    {
        Type[] _MappedTypes = new Type[] {
                typeof(DataSet),
                typeof(DataTable),
                typeof(DataTableExtensions),
                typeof(DataRow),
                typeof(DataRowExtensions),
                typeof(DataRowCollection),
                typeof(TypedTableBaseExtensions),
                typeof(EnumerableRowCollectionExtensions)
            };

        //trick to not to have to use dynamic types.
        interface IIntegerType { int Value { get; } };

        sealed class IntegerType<TH, TR> : IIntegerType
            where TH : IIntegerType, new()
            where TR : IIntegerType, new()
        {
            static int _Value = (new TH().Value << 1) + new TR().Value;

            public int Value { get { return _Value; } }
        }

        sealed class ZeroType : IIntegerType
        { public int Value { get { return 0; } } }

        sealed class OneType : IIntegerType
        { public int Value { get { return 1; } } }

        static Type GetIntegerType(int v)
        {
            return
                v == 0 ? typeof(ZeroType) :
                v == 1 ? typeof(OneType) :
                typeof(IntegerType<,>).MakeGenericType(GetIntegerType(v >> 1), GetIntegerType(v & 1));
        }

        sealed class MappingHelper<TOrd>
            where TOrd : IIntegerType, new()
        {
            static int _Ord = new TOrd().Value;

            public static IValueProvider<TType> GetRow<TRow, TType>(TRow row)
                where TRow : DataRow
            { return ObservableExtensions.Item(row, _Ord).Select(v => (TType)v); }
        }

        #region IExpressionObserverMappingProvider Members

        public IEnumerable<MethodInfo> GetMappings(MemberInfo memberToMap)
        {
            IEnumerable<MethodInfo> res = null;

            var declaringType = memberToMap.DeclaringType;

            if (_MappedTypes.Contains(declaringType))
                res = ExpressionObserverMappingHelper.FindMappings(memberToMap, typeof(ObservableExtensions));

            if (res == null || !res.Any())
            {               
                if (
                    declaringType.IsSubclassOf(typeof(DataRow))
                    && memberToMap.MemberType == MemberTypes.Property
                )
                {
                    var dataSetType = declaringType.DeclaringType;
                    var propToMap = (PropertyInfo)memberToMap;
                    var propGetMethod = propToMap.GetGetMethod();

                    if(
                        propGetMethod != null
                        && propGetMethod.IsPublic 
                        && !propGetMethod.IsStatic
                        && dataSetType != null
                        && dataSetType.IsSubclassOf(typeof(DataSet))
                        && dataSetType.GetConstructor(Type.EmptyTypes) != null
                    )
                    {
                        var dataSet = (DataSet)Activator.CreateInstance(dataSetType);

                        foreach(DataTable dataTable in dataSet.Tables)
                        {
                            for(
                                var tableType = dataTable.GetType();
                                tableType != typeof(object);
                                tableType = tableType.BaseType
                            )
                                if(tableType.IsGenericType && tableType.GetGenericTypeDefinition() == typeof(global::System.Data.TypedTableBase<>))
                                {
                                    var rowType = tableType.GetGenericArguments()[0];  
                              
                                    if(rowType == declaringType)
                                    {
                                        var column = dataTable.Columns.Cast<DataColumn>().FirstOrDefault(c => c.ColumnName == propToMap.Name);

                                        if (column != null)
                                        {
                                            var method =
                                                typeof(MappingHelper<>)
                                                    .MakeGenericType(GetIntegerType(column.Ordinal))
                                                    .GetMethod("GetRow")
                                                    .MakeGenericMethod(rowType, propToMap.PropertyType)
                                            ;

                                            res = Enumerable.Repeat(method, 1);

                                            goto lookupBreak;
                                        }
                                    }
                                }

                        }
                    }
                }

            lookupBreak: ;
            }

            return res;
        }

        #endregion
    }
}
