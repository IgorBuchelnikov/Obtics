using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using Obtics.Values;

namespace Obtics.Xml
{
    /// <summary>
    /// Register Linq-to-Xml method mappings for <see cref="ExpressionObserver"/> with an <see cref="ExpressionObserverMaster"/>.    
    /// </summary>
    [ObticsRegistration]
    public class LinqToXmlExpressionObserverMappingProvider : IExpressionObserverMappingProvider
    {
        static readonly Type[] MappedTypes = new Type[] {
            typeof(XAttribute),
            typeof(XElement),
            typeof(XObject),
            typeof(XNode),
            typeof(XComment),
            typeof(XContainer),
            typeof(XDocumentType),
            typeof(XProcessingInstruction),
            typeof(XText),
            typeof(XCData),
            typeof(XDocument),
            typeof(Extensions)
        };

        static Dictionary<Type, string> _converterMaps = 
            new Dictionary<Type, string>() { 
                { typeof(int?), "AsNullableInt" },
                { typeof(int), "AsInt" },
                { typeof(uint?), "AsNullableUnsignedInt"},
                { typeof(uint), "AsUnsignedInt"},
                { typeof(long?), "AsNullableLong"},
                { typeof(long), "AsLong"},
                { typeof(ulong?), "AsNullableUnsignedLong"},
                { typeof(ulong), "AsUnsignedLong"},
                { typeof(bool?), "AsNullableBoolean"},
                { typeof(bool), "AsBoolean"},
                { typeof(float?), "AsNullableSingle"},
                { typeof(float), "AsSingle"},
                { typeof(double?), "AsNullableDouble"},
                { typeof(double),"AsDouble"},
                { typeof(decimal?),"AsNullableDecimal"},
                { typeof(decimal), "AsDecimal" },
                { typeof(Guid?), "AsNullableGuid" },
                { typeof(Guid), "AsGuid" },
                { typeof(TimeSpan?), "AsNullableTimeSpan" },
                { typeof(TimeSpan), "AsTimeSpan" },
                { typeof(DateTime?), "AsNullableDateTime" },
                { typeof(DateTime), "AsDateTime" },
                { typeof(DateTimeOffset?), "AsNullableDateTimeOffset" },
                { typeof(DateTimeOffset), "AsDateTimeOffset" },
                { typeof(string), "AsString" }
            }
        ;

        #region IExpressionObserverMappingProvider Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberToMap"></param>
        /// <returns></returns>
        public IEnumerable<MethodInfo> GetMappings(MemberInfo memberToMap)
        {
            Type declaringType = memberToMap.DeclaringType;
            IEnumerable<MethodInfo> res = null;

            if (
                (declaringType == typeof(XElement) || declaringType == typeof(XAttribute))
                && memberToMap.MemberType == MemberTypes.Method
                && memberToMap.Name == "op_Explicit"
            )
            {
                var methodToMap = (MethodInfo)memberToMap;
                string converterName;

                if (methodToMap.IsStatic && methodToMap.IsPublic && _converterMaps.TryGetValue(methodToMap.ReturnType, out converterName))
                    res = ExpressionObserverMappingHelper.FindMappings(methodToMap, typeof(ObservableExtensions), converterName);
            }

            if (res == null && MappedTypes.Contains(declaringType))
                res = ExpressionObserverMappingHelper.FindMappings(memberToMap, typeof(ObservableExtensions));

            return res;
        }

        #endregion
    }
}
