
using System.ComponentModel;
using Obtics.Values.Transformations;
using System;
using TvdP.Collections;
using System.Linq.Expressions;
using System.Reflection;
using SL = System.Linq;


namespace Obtics.Values
{
    /// <summary>
    /// Provides a set of static methods for querying objects that implement <see cref="IValueProvider{TType}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The majority of the methods in this class are defined as extension methods that extend <see cref="IValueProvider{TType}"/>. This means they can be called like an instance method on any object that implements <see cref="IValueProvider{TType}"/>.
    /// </para><para>
    /// Methods that are used in a query that returns a value provider do not consume the target data until the query objects Value property getter is accessed. This is known as deferred execution.
    /// </para>
    /// </remarks>
    public static partial class ValueProvider
    {
        internal static IInternalValueProvider<TSource> Patched<TSource>(this IValueProvider<TSource> source)
        {
            var concrete = source as ConcreteValueProvider<TSource>;

            if (concrete != null)
                return concrete.Source;

            var res = source as IInternalValueProvider<TSource>;

            if (res != null)
                return res;
          
            return 
                NPCToNC<TSource>.Create(source);
        }


#if !FULL_REFLECTION
        public
#endif
        class PatchFactory
        {
            internal virtual IInternalValueProvider CreatePatch(IValueProvider source)
            { return NPCToNC.Create(source); }
        }

#if !FULL_REFLECTION
        public
#endif
        class PatchFactory<TType> : PatchFactory
        {
            internal override IInternalValueProvider CreatePatch(IValueProvider source)
            { return ((IValueProvider<TType>)source).Patched(); }
        }


        static Cache<Type, PatchFactory> _PatchedMap = new Cache<Type, PatchFactory>();

        /// <summary>
        /// This method is for internal use and is not intended to be used from client code.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static IInternalValueProvider Patched(this IValueProvider source)
        {
            var concrete = source as ConcreteValueProvider;

            if (concrete != null)
                return concrete.Source;

            var asInternalValueProvider = source as IInternalValueProvider;

            if (asInternalValueProvider != null)
                return asInternalValueProvider;

            var patchedMap = _PatchedMap;
            var sourceType = source.GetType();
            PatchFactory mapper;

            if (!patchedMap.TryGetItem(sourceType, out mapper))
            {
                var vpInterface =
                    SL.Enumerable.FirstOrDefault(
                        sourceType.GetInterfaces(),
                        type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IValueProvider<>)
                    )
                    ??
                    SL.Enumerable.FirstOrDefault(
                        sourceType.GetInterfaces(),
                        type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IValueProvider<>)
                    );

                if (vpInterface != null)
                {
                    var prm = Expression.Parameter(typeof(IValueProvider), "_1");

                    mapper = (PatchFactory)Activator.CreateInstance(typeof(ValueProvider.PatchFactory<>).MakeGenericType(vpInterface.GetGenericArguments()));
                }
                else
                    mapper = (PatchFactory)Activator.CreateInstance(typeof(ValueProvider.PatchFactory));

                mapper = patchedMap.GetOldest(sourceType, mapper);
            }

            return mapper.CreatePatch(source);
        }

        static WeakDictionary<IValueProvider[], IInternalValueProvider[]> ArrayPatchCache = new WeakDictionary<IValueProvider[], IInternalValueProvider[]>();

        internal static IInternalValueProvider[] Patched(this IValueProvider[] sources)
        {
            if (sources == null)
                return null;

            return
                 ArrayPatchCache.GetOrAdd(
                    sources,
                    ss =>
                    {
                        var patchedSources = new IInternalValueProvider[ss.Length];

                        for (int i = 0, end = ss.Length; i < end; ++i)
                            patchedSources[i] = ss[i].Patched();

                        return patchedSources;
                    }
                )
            ;
        }

        internal static IValueProvider Concrete(this IInternalValueProvider source)
        { return source == null ? null : ConcreteValueProvider.Create(source); }

        internal static IValueProvider<T> Concrete<T>(this IInternalValueProvider<T> source)
        { return source == null ? null : ConcreteValueProvider<T>.Create(source); }

        internal static IValueProvider[] Concrete(this IInternalValueProvider[] sources)
        { return sources == null ? null : SL.Enumerable.ToArray(SL.Enumerable.Select(sources, s => s.Concrete())); }
    }
}
