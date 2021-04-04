using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using TvdP.Collections;
using TvdP.Threading;

namespace Obtics.Values
{
    //Below follows a section concerned with translating standard not observable object method calls
    //into Implicitly observable ones.

    internal enum ParameterMapInfo
    {
        Direct,             //The parameter is not mapped observably as:        int S( int prm )        -> IValueProvider<int> M( int prm ) 
        ObservableValue,    //The parameter is mapped as observable value as:   int S( int prm )        -> IValueProvider<int> M( IValueProvider<int> prm )
        ObservableLambda    //The parameter is mapped as observable lambda as:  int S( Func<int> prm )  -> IValueProvider<int> M( Func<IValueProvider<int>> prm )
    }

    internal class MapInfo
    {
        public MemberInfo SourceMember;

        /// <summary>
        /// The first element of the Tuple contains the actual method, 
        /// the second parts are the indexes of the parameters of that method that are truely observable and not just plain copies from the source member.
        /// So S(int,string,Func&lt;double&gt;) -> M(IValueProvider&lt;int&gt;,string,Func&lt;IValueProvider&lt;double&gt;&gt;) would have a second part of new int[] { 0 , 2 }
        /// 
        /// Note there must always be a smallest common product target method, whose int[] array contains all values contained in the int[] for all other target methods.
        /// </summary>
        public Tuple<MethodInfo,int[]>[] TargetMethods;

        /// <summary>
        /// How all parameters are mapped towards the parameters of the most observable target method of all target methods (the smallest common product target method)
        /// </summary>
        public ParameterMapInfo[] Parameters;

        /// <summary>
        /// The number of type arguments for the sourceMember and its declaring type together (and number of type arguments for the target methods)
        /// 0 in case we are not mapping generic type definitions.
        /// </summary>
        public int GenericPrmsCount;

        /// <summary>
        ///if true:
        ///The result of the target methods are declared as same type as source method but is observable
        ///(like IEnumerable -> IEnumerable with INotifyCollectionChanged)
        ///
        ///if false:
        ///The target methods return an IValueProvider&lt;TResult&gt;
        /// </summary>
        public bool ResultDirectlyObservable; 

        /// <summary>
        /// In combination with ResultDirectlyObservable. The result of the Target methods is a specialization
        /// of the result of the source method and should be upcast to the source type to ensure compatibility. 
        /// So source return A and target returns B:A. Then when the mapping is used the result of the target method needs to be upcast to A.
        /// </summary>
        public bool NeedsUpcast;
    }

    /// <summary>
    /// Helper class for working with observable mappings
    /// </summary>
    /// <remarks>
    /// </remarks>
    public static class ExpressionObserverMappingHelper
    {
        #region static methods for creating mapping from non-live methods to live methods

        static Type[] lambdaTypes = new Type[] {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>),
            typeof(Func<,,,,,,>),
            typeof(Func<,,,,,,,>),
            typeof(Func<,,,,,,,,>),
            typeof(Func<,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,,>)
        };

        //checks if type is a lambda and what the arguments are
        static bool CheckLambdaType(Type type, out Type[] genericArguments)
        {
            genericArguments = null;

            if (!type.IsGenericType || !lambdaTypes.Contains(type.GetGenericTypeDefinition()))
                return false;

            genericArguments = type.GetGenericArguments();

            return true;
        }

        static bool IsAssignableFrom(IDictionary<Type, Type> typeArgsMap, Type sourceType, Type targetType)
        {
            if (sourceType.IsAssignableFrom(targetType))
                return true;

            if (sourceType.IsGenericType)
            {
                var sourceTypeArgArray = sourceType.GetGenericArguments();
                var tgtTypeArgArray = new Type[sourceTypeArgArray.Length];

                for (int i = 0, end = tgtTypeArgArray.Length; i < end; ++i)
                    if (!typeArgsMap.TryGetValue(sourceTypeArgArray[i], out tgtTypeArgArray[i]))
                        return false;

                return sourceType.GetGenericTypeDefinition().MakeGenericType(tgtTypeArgArray).IsAssignableFrom(targetType);
            }

            return false;
        }

        static bool EqualityCompareTypes(IDictionary<Type, Type> typeArgsMap, Type sourceType, Type targetType)
        {
            Type temp;

            if (sourceType == targetType || typeArgsMap.TryGetValue(sourceType, out temp) && temp == targetType)
                return true;

            if (sourceType.IsArray && targetType.IsArray && sourceType.GetArrayRank() == targetType.GetArrayRank())
                return EqualityCompareTypes(typeArgsMap, sourceType.GetElementType(), targetType.GetElementType());

            if (sourceType.IsGenericType && targetType.IsGenericType && sourceType.GetGenericTypeDefinition() == targetType.GetGenericTypeDefinition())
            {
                var sourceArgs = sourceType.GetGenericArguments();
                var targetArgs = targetType.GetGenericArguments();

                if (sourceArgs.Length == targetArgs.Length)
                {
                    for (int i = 0, end = sourceArgs.Length; i < end; ++i)
                        if (!EqualityCompareTypes(typeArgsMap, sourceArgs[i], targetArgs[i]))
                            return false;

                    return true;
                }
            }

            return false;
        }

        //checks if targetType is a 'made observable' lambda version of sourceType
        static bool IsMappedLambda(IDictionary<Type, Type> typeArgsMap, Type sourceType, Type targetType)
        {
            Type[] sourceTypeArgs, targetTypeArgs;

            if (
                CheckLambdaType(sourceType, out sourceTypeArgs)
                && CheckLambdaType(targetType, out targetTypeArgs)
            )
            {

                var typeArgs_Length = sourceTypeArgs.Length;

                if (typeArgs_Length == targetTypeArgs.Length)
                {
                    for (int i = 0, end = typeArgs_Length - 1; i < end; ++i)
                        if (!EqualityCompareTypes(typeArgsMap, sourceTypeArgs[i], targetTypeArgs[i]))
                            return false;

                    return EqualityCompareTypes(typeArgsMap, typeof(IValueProvider<>).MakeGenericType(sourceTypeArgs[typeArgs_Length - 1]), targetTypeArgs[typeArgs_Length - 1]);
                }
            }

            return false;
        }

        //checks if targetType is a 'made observable' value version of sourceType
        static bool IsMappedValue(IDictionary<Type, Type> typeArgsMap, Type sourceType, Type targetType)
        {
            return EqualityCompareTypes(typeArgsMap, typeof(IValueProvider<>).MakeGenericType(sourceType), targetType);
        }

        //determines if sourceParam maps to targetParam and how.
        static ParameterMapInfo? MapParameter(IDictionary<Type, Type> typeArgsMap, Type sourceParam_ParameterType, Type targetParam_ParameterType)
        {
            return
                EqualityCompareTypes(typeArgsMap, sourceParam_ParameterType, targetParam_ParameterType) ? ParameterMapInfo.Direct :
                IsMappedValue(typeArgsMap, sourceParam_ParameterType, targetParam_ParameterType) ? ParameterMapInfo.ObservableValue :
                IsMappedLambda(typeArgsMap, sourceParam_ParameterType, targetParam_ParameterType) ? ParameterMapInfo.ObservableLambda :
                (ParameterMapInfo?)null;
        }

        //determines if all sourceParams map to targetParams and how.
        static ParameterMapInfo[] MapParameters(IDictionary<Type, Type> typeArgsMap, Type[] sourceParams, Type[] targetParams)
        {
            var sourceParams_Length = sourceParams.Length;

            if (sourceParams_Length != targetParams.Length)
                return null;

            var res = new ParameterMapInfo[sourceParams_Length];

            for (int i = 0; i != sourceParams_Length; ++i)
            {
                var map = MapParameter(typeArgsMap, sourceParams[i], targetParams[i]);

                if (!map.HasValue)
                    return null;

                res[i] = map.Value;
            }

            return res;
        }

        //determines which parameter mappings are reactive mappings and give the result as a bitmap
        static int[] ParameterMapIndex(ParameterMapInfo[] map)
        {
            return 
                map
                    .Select((pmi, ix) => Tuple.Create(pmi, ix))
                    .Where(t => t.First != ParameterMapInfo.Direct)
                    .Select(t => t.Second)
                    .ToArray()
            ;
        }

        //determines how many bits are set in mapIndex. The more bits the more reactive the mapping
        static int ParameterMapScore(List<int> mapIndex)
        {
            return mapIndex.Count;
        }

        static IDictionary<Type, Type> GetTypeArgsMap(MemberInfo sourceMember, MethodInfo targetMethod)
        {
            var typeArgsMap = new Dictionary<Type, Type>();

            var sourceMethod = sourceMember as MethodInfo;
            var sourceTypeArgs = sourceMethod != null && sourceMethod.IsGenericMethodDefinition ? sourceMethod.GetGenericArguments() : Type.EmptyTypes;

            //if defining type of sourceMethod is Generic type definition then add
            //type args of defining type to typeArgsMap.
            //so class A<X>{ X m<Y>( Y p ); } should be mapped by something like class A2{ IValueProvider<X> m<X,Y>( IValueProvider<X> p ) }
            //note: we do not go down the hierarchy of nested types. The direct parent has all generic parameters.

            var sourceDeclaringType = sourceMember.DeclaringType;

            if(sourceDeclaringType.IsGenericTypeDefinition)
                sourceTypeArgs = sourceDeclaringType.GetGenericArguments().Concat(sourceTypeArgs).ToArray();

            var targetTypeArgs = targetMethod.IsGenericMethodDefinition ? targetMethod.GetGenericArguments() : new Type[0] ;

            if (sourceTypeArgs.Length != targetTypeArgs.Length)
                return null; //mismatch in 'genericity'

            for (int i = 0, end = sourceTypeArgs.Length; i < end; ++i)
                typeArgsMap.Add(sourceTypeArgs[i], targetTypeArgs[i]);

            return typeArgsMap;
        }

        static bool IsStatic(MemberInfo member)
        {
            var method = member as MethodInfo;

            if (method != null)
                return ( method.Attributes & MethodAttributes.Static ) != 0;

            var prop = member as PropertyInfo;

            if (prop != null)
            {
                var acc = prop.GetAccessors();
                return acc.Length > 0 && (acc[0].Attributes & MethodAttributes.Static) != 0;
            }

            var field = member as FieldInfo;

            return field != null && (field.Attributes & FieldAttributes.Static) != 0;
        }

        static Type[] GetParameters(MemberInfo member)
        {
            var method = member as MethodInfo;

            var sourceParameters = method == null ? Enumerable.Empty<ParameterInfo>() : method.GetParameters().AsEnumerable();

            if (method == null)
            {
                var prop = member as PropertyInfo;

                if (prop != null)
                {
                    sourceParameters = prop.GetIndexParameters();
                }
            }

            return ( IsStatic(member) ? Type.EmptyTypes : new Type[] { member.DeclaringType } ).Concat( sourceParameters.Select(prmInfo => prmInfo.ParameterType) ).ToArray();
        }

        static Type GetReturnType(MemberInfo member)
        {
            var method = member as MethodInfo;

            if (method != null)
                return method.ReturnType;

            var prop = member as PropertyInfo;

            if (prop != null)
                return prop.PropertyType;

            var field = member as FieldInfo;

            return field != null ? field.FieldType : null ;
        }


        //Attempts to determin a parameter mapping of sourceMethod to targetMethod
        //source can be plain and target equaly plain
        //or source can be a generic definition and target a generic definition.
        //return null if no mapping can be found.
        static ParameterMapInfo[] GetParameterMapInfo(IDictionary<Type, Type> typeArgsMap, MemberInfo sourceMember, MethodInfo targetMethod)
        {
            if (typeArgsMap == null)
                return null;

            var sourceParameters = GetParameters(sourceMember);

            return MapParameters(typeArgsMap, sourceParameters, targetMethod.GetParameters().Select(prmInfo => prmInfo.ParameterType).ToArray());
        }

        /// <summary>
        /// Construct a MapInfo class with distilled information about the mapping from sourceMember to targetMethods.
        /// The targetMethods must be valid mappings.
        /// </summary>
        /// <param name="sourceMember"></param>
        /// <param name="targetMethods"></param>
        /// <returns></returns>
        internal static MapInfo CreateMap(MemberInfo sourceMember, MethodInfo[] targetMethods)
        {
            if (targetMethods.Length < 1)
                throw new MappingException("There must be at least one target method.");

            var sourceParams = GetParameters(sourceMember);
            var params_Length = sourceParams.Length;
            var returnType = GetReturnType(sourceMember);

            var mapps =
                targetMethods
                    .Select(
                        m =>
                        {
                            // check if the target method is a valid mapping and if it matches the source method >>>>>>>>>>>>>>>

                            if (!m.IsStatic)
                                throw new MappingException("All target methods need to be static methods.");

                            if (m.GetParameters().Length != params_Length)
                                throw new MappingException("Source and target method have incompatible number of input parameters.");

                            var typeArgsMap = GetTypeArgsMap(sourceMember, m);

                            if (typeArgsMap == null)
                                throw new MappingException("Source and target method have incompatible number of type parameters.");

                            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                            //check the type of result mapping. >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                            bool equalResultTypes;
                            bool needsUpcast;

                            //test before testing potential upcast to catch object -> IValueProvider<object> mapping
                            if(EqualityCompareTypes(typeArgsMap, returnType, m.ReturnType))
                            {
                                equalResultTypes = true;
                                needsUpcast = false;
                            }
                            else if (EqualityCompareTypes(typeArgsMap, typeof(IValueProvider<>).MakeGenericType(returnType), m.ReturnType))
                            {
                                equalResultTypes = false;
                                needsUpcast = false;
                            }
                            //potentially upcastable
                            else if (IsAssignableFrom(typeArgsMap, returnType, m.ReturnType))
                            {
                                equalResultTypes = true;
                                needsUpcast = true;
                            }
                            else if (
                                //Explicit exception for ToList and ToDictionary. Potential errors during rewriting
                                //TODO:What to do about this? generalize it? What about rewrite errors?
                                (sourceMember.Name == "ToList" || sourceMember.Name == "ToDictionary")
                                && typeof(Enumerable).GetMethods().Contains(sourceMember)
                            )
                            {
                                equalResultTypes = true;
                                needsUpcast = false;
                            }
                            else
                                throw new MappingException("All target methods need be upcastable to the source result type or all need to return IValueProvider<TSourceResult>.");

                            var prmsMap = GetParameterMapInfo(typeArgsMap, sourceMember, m);

                            return new {
                                sourceMethod = m,
                                equalResultTypes,
                                needsUpcast,
                                prmsMap,
                                mapIndex = ParameterMapIndex(prmsMap),
                                typeArgsMap
                            };
                        }
                    )
                ;

            //check if all target methods found have the same type of result.
            if(mapps.GroupBy(m => m.equalResultTypes).Count() > 1)
                throw new MappingException("All target methods need be upcastable to the source result type or all need to return IValueProvider<TSourceResult>.");

            //check if there exists more than one target method with exactly the same
            //parameter mapping. All parameter mappings must be unique.
            if (mapps.GroupBy(m => new ArrayStructuredEqualityWrapper<int>(m.mapIndex)).Any(grp => grp.Count() > 1))
                throw new MappingException("Multiple target methods with same parameter mapping.");

            var mostReactive = 
                mapps.OrderByDescending(m => m.mapIndex.Length).First();

            //check if there exists a target method with a parameter mapping that is not covered
            //by the target method with the most extended mapping. This most extended mapping must cover
            //the mappings of all less extended mappings.
            if (mapps.FirstOrDefault(m => m.mapIndex.Except(mostReactive.mapIndex).Any()) != null)
                throw new MappingException("There must be one target method whose mapped parameters cover the mapped parameters of all other target methods.");

            return
                new MapInfo
                {
                    GenericPrmsCount = mostReactive.typeArgsMap.Count,
                    ResultDirectlyObservable = mostReactive.equalResultTypes,
                    NeedsUpcast = mostReactive.needsUpcast,
                    SourceMember = sourceMember,
                    Parameters = mostReactive.prmsMap, //most reactive target method
                    TargetMethods = 
                        mapps.Select(
                            m => Tuple.Create(
                                m.sourceMethod,
                                m.mapIndex
                            )
                        ).ToArray()
                }
            ;
        }

        /// <summary>
        /// Find candidate methods in a given class that would by signature be valid mappings for the given source member.
        /// </summary>
        /// <param name="sourceMember">The member we try to find mappings for.</param>
        /// <param name="mappedToClass">The class wherein we look for the mappings.</param>
        /// <returns>A sequence with 0 or more mapping candidates</returns>
        /// <remarks>
        /// Candidates will need to have the same name as the mapped member or, if that name is prefixed with 'get_', the part of the name after 'get_'.
        /// </remarks>
        public static IEnumerable<MethodInfo> FindMappings(MemberInfo sourceMember, Type mappedToClass)
        { return FindMappings(sourceMember, mappedToClass, null); }

        /// <summary>
        /// Find candidate methods in a given class that would by signature be valid mappings for the given source member. The candidates are
        /// sought using a specifig name <paramref name="targetMethodsName"/>.
        /// </summary>
        /// <param name="sourceMember">The member we try to find mappings for.</param>
        /// <param name="mappedToClass">The class wherein we look for the mappings.</param>
        /// <param name="targetMethodsName">The name of the candidate methods. Can be null.</param>
        /// <returns>A sequence with 0 or more mapping candidates</returns>
        /// <remarks>
        /// Candidates will need to have the specific given name <paramref name="targetMethodsName"/>.
        /// In case that <paramref name="targetMethodsName"/> is left null however candidates will need to have the same name as the mapped member or, if that name is prefixed with 'get_', the part of the name after 'get_'.
        /// </remarks>
        public static IEnumerable<MethodInfo> FindMappings(MemberInfo sourceMember, Type mappedToClass, string targetMethodsName)
        {
            if (null == sourceMember)
                throw new ArgumentNullException("sourceMember");

            if (null == mappedToClass)
                throw new ArgumentNullException("mappedToClass");

            List<MethodInfo> targetMethods;

            var searchedName = targetMethodsName ?? sourceMember.Name;

            targetMethods = FindMappingsBySpecificName(sourceMember, mappedToClass, searchedName);

            if(targetMethods.Count  == 0 && targetMethodsName == null && searchedName.StartsWith("get_") )
                targetMethods = FindMappingsBySpecificName(sourceMember, mappedToClass, searchedName.Substring(4));

            return targetMethods.Count == 0 ? null : targetMethods.ToArray();
        }

        private static List<MethodInfo> FindMappingsBySpecificName(MemberInfo sourceMember, Type mappedToClass, string searchedName)
        {
            List<MethodInfo> targetMethods;
            var sourceParams = GetParameters(sourceMember);
            var params_Length = sourceParams.Length;

            targetMethods = new List<MethodInfo>();

            foreach (var m in mappedToClass.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static))
            {
                if (
                    m.Name == searchedName
                    && m.GetParameters().Length == params_Length
                )
                {
                    if (GetParameterMapInfo(GetTypeArgsMap(sourceMember, m), sourceMember, m) != null)
                        targetMethods.Add(m);
                }
            }

            return targetMethods;
        }

        //Attempts to create a MapInfo from sourceMethod (which can be either a plain method or a generic method)
        //into static public member methods from mappedToClass. The methods need to have the same name
        //and type arguments.
        internal static MapInfo CreateMap(MemberInfo sourceMember, Type mappedToClass, string targetMethodsName)
        {
            var targetMethods = FindMappings(sourceMember, mappedToClass, targetMethodsName);

            return targetMethods == null ? null : CreateMap(sourceMember, targetMethods.ToArray());
        }

        //Creates MapInfo from explicit arguments.
        internal static MapInfo CreateMap(Delegate sourceDelegate, params Delegate[] targetDelegates)
        {
            var sourceMethod = sourceDelegate.Method;

            if (sourceMethod.IsGenericMethod)
                sourceMethod = sourceMethod.GetGenericMethodDefinition();

            var targetMethods = targetDelegates.Select(dgt => dgt.Method).Select(m => m.IsGenericMethod ? m.GetGenericMethodDefinition() : m).ToArray();

            return CreateMap(sourceMethod, targetMethods);
        }


        #endregion

        static internal bool TryGetMapping(MemberInfo member, out MapInfo mapping)
        {
            mapping = _providerFinder.GetValue(member);
            return mapping != null;
        }

        class ExpressionObserverMappingAttributeProvider : IExpressionObserverMappingProvider
        {
            #region IExpressionObserverMappingProvider Members

            IEnumerable<MethodInfo>  IExpressionObserverMappingProvider.GetMappings(MemberInfo memberToMap)
            {
                //The attribute can be valid on plain methods or generic method definitions only
                if (memberToMap.MemberType == MemberTypes.Method)
                {
                    var methodToMap = (MethodInfo)memberToMap;

                    if (methodToMap.IsGenericMethod && !methodToMap.IsGenericMethodDefinition)
                        return null;
                }

                //The declaring type must be plain or a generic type definition
                var declaringType = memberToMap.DeclaringType;

                if (declaringType.IsGenericType && !declaringType.IsGenericTypeDefinition)
                    return null;

                var mappingAttribute = (ExpressionObserverMappingAttribute)Attribute.GetCustomAttribute(memberToMap, typeof(ExpressionObserverMappingAttribute));

                return mappingAttribute == null ? null : mappingAttribute.GetMappedMethods(memberToMap);
            }

            #endregion
        }

#if SILVERLIGHT
        static ProviderFinder<IExpressionObserverMappingProvider, MemberInfo, MapInfo> _providerFinder =
            new ProviderFinder<IExpressionObserverMappingProvider, MemberInfo, MapInfo>(
                new IExpressionObserverMappingProvider[] { new ExpressionObserverMappingAttributeProvider() },
                (provider, memberInfo) =>
                {
                    var methods = provider.GetMappings(memberInfo);
                    return methods != null && methods.Any() ? CreateMap(memberInfo, methods.ToArray()) : null;
                },
                memverInfo => null,
                true
            )
        ;
#else
        static ProviderFinder<IExpressionObserverMappingProvider, MemberInfo, MapInfo> _providerFinder =
            new ProviderFinder<IExpressionObserverMappingProvider, MemberInfo, MapInfo>(
                new IExpressionObserverMappingProvider[] { new ExpressionObserverMappingAttributeProvider() }
                    .Concat(Configuration.ObticsConfigurationSection.GetSection().RootMappings.Select(mpce => mpce.Instance))
                    .ToArray(),
                (provider, memberInfo) =>
                {
                    var methods = provider.GetMappings(memberInfo);
                    return methods != null ? CreateMap(memberInfo, methods.ToArray()) : null;
                },
                memverInfo => null,
                Configuration.ObticsConfigurationSection.GetSection().EnableRegistriationThroughReflection
            )
        ;
#endif
    }
}
