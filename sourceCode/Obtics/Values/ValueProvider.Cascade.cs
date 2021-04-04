using System.Collections.Generic;
using System.Linq;
using Obtics.Values.Transformations;
using Obtics.Collections.Transformations;
using Obtics.Collections;
using CT = Obtics.Collections.Transformations;
using VT = Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Converts an IValueProvider&lt;IValueProvider&lt;TType&gt;&gt; to an IValueProvider&lt;TType&gt;. The Value property
        /// of the result represents the Value property of the inner IValueProvider of the source. If the Value property of the
        /// outer IValueProvider has value null, then te result IValueProvider will have default(TType) as value for its Value property.
        /// </summary>
        /// <typeparam name="TType">Type of the Value property of the inner IValueProvider of the source and the type of the Value property of the result.</typeparam>
        /// <param name="source">The source <see cref="IValueProvider{VP}"/> of <see cref="IValueProvider{TType}"/>.</param>
        /// <returns>An IValueProvider&lt;TType&gt; of which the Value property represents the Value property of the inner IValueProvider of the source.</returns>
        public static IValueProvider<TType> Cascade<TType>(this IValueProvider<IValueProvider<TType>> source)
        { return _Cascade(source.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _Cascade<TType>(this IInternalValueProvider<IValueProvider<TType>> source)
        { return VT.CascadeTransformation<TType, IValueProvider<TType>>.GeneralCreate(source); }

        public static IValueProvider<TType> Cascade<TType>(this IValueProvider<IValueProvider<TType>> source, TType fallbackValue)
        { return _Cascade(source.Patched(), fallbackValue).Concrete(); }

        internal static IInternalValueProvider<TType> _Cascade<TType>(this IInternalValueProvider<IValueProvider<TType>> source, TType fallbackValue)
        { return VT.CascadeWithDefaultTransformation<TType, IValueProvider<TType>>.GeneralCreate(source, fallbackValue); }

        /// <summary>
        /// Converts an <see cref="IValueProvider{TSource}"/> of <typeparamref name="TSource"/> to an <see cref="IEnumerable{TType}"/> of <typeparamref name="TType"/>. The elements of the result collection
        /// will be the elements of the sequence returned by the source <see cref="IValueProvider{TSource}"/>. If the Value property of the
        /// outer IValueProvider has value null, then the result collection will be empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the source sequence. This type is constrained to an <see cref="IEnumerable{TType}"/> of <typeparamref name="TType"/>.</typeparam>
        /// <typeparam name="TType">Type of the elements of the sequence returned by <paramref name="source"/> and type of the elements of the result sequence.</typeparam>
        /// <param name="source">The source <see cref="IValueProvider{TSource}"/> of <typeparamref name="TSource"/>.</param>
        /// <returns>An <see cref="IEnumerable{TType}"/> of <typeparamref name="TType"/> containing the elements returned by the sequence returned by <paramref name="source"/>, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// The <paramref name="source"/> parameter of this method has two levels of observability. First, on the outer layer, the IValueProvider can 
        /// change its Value property. In other words; the sequence (<see cref="IVersionedEnumerable{TType}"/>) gets replaced completely. Second, on the inner layer,
        /// the sequence can alter its contents. 
        /// 
        /// Cascade merges these two layers into one resulting sequence whose contents will be exactly the contents of the source inner sequence or, if 
        /// the Value property of the outer IValueProvider is null, will be empty.
        /// 
        /// Any change events from the outer layer will be translated into 'reset' events for the result sequence. Any change events from the inner
        /// layer will passed directly to the result sequence.
        /// </remarks>
        public static IEnumerable<TType> Cascade<TSource, TType>(IValueProvider<TSource> source) where TSource : IEnumerable<TType>
        { return _Cascade<TSource,TType>(source.Patched()); }

        internal static IEnumerable<TType> _Cascade<TSource, TType>(IInternalValueProvider<TSource> source) where TSource : IEnumerable<TType>
        { return CT.CascadeTransformation<TType, TSource>.GeneralCreate(AsCollectionNullableTransformation<TSource>.GeneralCreate(source, v => v != null)); }

        /// <summary>
        /// Converts an IValueProvider&lt;IEnumerable&lt;TType&gt;&gt; to an IEnumerable&lt;TType&gt; The elements of the result collection
        /// will be the elements of the IEnumerable&lt;TType&gt; returned by the source IValueProvider. If the Value property of the
        /// outer IValueProvider has value null, then the result collection will be empty.
        /// </summary>
        /// <typeparam name="TType">Type of the elements of the IEnumerable returned by the source IValueProvider and type of the elements of the result collection.</typeparam>
        /// <param name="source">The source <see cref="IValueProvider{VP}"/> of <see cref="IVersionedEnumerable{TType}"/>.</param>
        /// <returns>An IEnumerable&lt;TType&gt; containing the elements returned by the IEnumerable&lt;TType&gt; returned by the source IValueProvider.</returns>
        /// <remarks>
        /// The <paramref name="source"/> parameter of this method has two levels of observability. First, on the outer layer, the IValueProvider can 
        /// change its Value property. In other words; the sequence (<see cref="IVersionedEnumerable{TType}"/>) gets replaced completely. Second, on the inner layer,
        /// the sequence can alter its contents. 
        /// 
        /// Cascade merges these two layers into one resulting sequence whose contents will be exactly the contents of the source inner sequence or, if 
        /// the Value property of the outer IValueProvider is null, will be empty.
        /// 
        /// Any change events from the outer layer will be translated into 'reset' events for the result sequence. Any change events from the inner
        /// layer will passed directly to the result sequence.
        /// </remarks>
        public static IEnumerable<TType> Cascade<TType>(this IValueProvider<IVersionedEnumerable<TType>> source)
        { return Cascade<IVersionedEnumerable<TType>, TType>(source); }

        internal static IEnumerable<TType> _Cascade<TType>(this IInternalValueProvider<IVersionedEnumerable<TType>> source)
        { return _Cascade<IVersionedEnumerable<TType>, TType>(source); }

        /// <summary>
        /// Converts an IValueProvider&lt;IEnumerable&lt;TType&gt;&gt; to an IEnumerable&lt;TType&gt; The elements of the result collection
        /// will be the elements of the IEnumerable&lt;TType&gt; returned by the source IValueProvider. If the Value property of the
        /// outer IValueProvider has value null, then the result collection will be empty.
        /// </summary>
        /// <typeparam name="TType">Type of the elements of the IEnumerable returned by the source IValueProvider and type of the elements of the result collection.</typeparam>
        /// <param name="source">The source <see cref="IValueProvider{VP}"/> of <see cref="IEnumerable{TType}"/>.</param>
        /// <returns>An IEnumerable&lt;TType&gt; containing the elements returned by the IEnumerable&lt;TType&gt; returned by the source IValueProvider.</returns>
        /// <remarks>
        /// The <paramref name="source"/> parameter of this method has two levels of observability. First, on the outer layer, the IValueProvider can 
        /// change its Value property. In other words; the sequence (<see cref="IEnumerable{TType}"/>) gets replaced completely. Second, on the inner layer,
        /// the sequence can alter its contents. 
        /// 
        /// Cascade merges these two layers into one resulting sequence whose contents will be exactly the contents of the source inner sequence or, if 
        /// the Value property of the outer IValueProvider is null, will be empty.
        /// 
        /// Any change events from the outer layer will be translated into 'reset' events for the result sequence. Any change events from the inner
        /// layer will passed directly to the result sequence.
        /// </remarks>
        public static IEnumerable<TType> Cascade<TType>(this IValueProvider<IEnumerable<TType>> source)
        { return Cascade<IEnumerable<TType>, TType>(source); }

        /// <summary>
        /// Converts an IValueProvider&lt;IOrderedEnumerable&lt;TType&gt;&gt; to an IEnumerable&lt;TType&gt; The elements of the result collection
        /// will be the elements of the IEnumerable&lt;TType&gt; returned by the source IValueProvider. If the Value property of the
        /// outer IValueProvider has value null, then the result collection will be empty.
        /// </summary>
        /// <typeparam name="TType">Type of the elements of the IEnumerable returned by the source IValueProvider and type of the elements of the result collection.</typeparam>
        /// <param name="source">The source <see cref="IValueProvider{VP}"/> of <see cref="IOrderedEnumerable{TType}"/>.</param>
        /// <returns>An IEnumerable&lt;TType&gt; containing the elements returned by the IEnumerable&lt;TType&gt; returned by the source IValueProvider.</returns>
        /// <remarks>
        /// The <paramref name="source"/> parameter of this method has two levels of observability. First, on the outer layer, the IValueProvider can 
        /// change its Value property. In other words; the sequence (<see cref="IOrderedEnumerable{TType}"/>) gets replaced completely. Second, on the inner layer,
        /// the sequence can alter its contents. 
        /// 
        /// Cascade merges these two layers into one resulting sequence whose contents will be exactly the contents of the source inner sequence or, if 
        /// the Value property of the outer IValueProvider is null, will be empty.
        /// 
        /// Any change events from the outer layer will be translated into 'reset' events for the result sequence. Any change events from the inner
        /// layer will passed directly to the result sequence.
        /// </remarks>
        public static IEnumerable<TType> Cascade<TType>(this IValueProvider<IOrderedEnumerable<TType>> source)
        { return Cascade<IOrderedEnumerable<TType>, TType>(source); }

        /// <summary>
        /// Converts an IValueProvider&lt;IObservableOrderedEnumerable&lt;TType&gt;&gt; to an IEnumerable&lt;TType&gt; The elements of the result collection
        /// will be the elements of the IEnumerable&lt;TType&gt; returned by the source IValueProvider. If the Value property of the
        /// outer IValueProvider has value null, then the result collection will be empty.
        /// </summary>
        /// <typeparam name="TType">Type of the elements of the IEnumerable returned by the source IValueProvider and type of the elements of the result collection.</typeparam>
        /// <param name="source">The source <see cref="IValueProvider{VP}"/> of <see cref="IObservableOrderedEnumerable{TType}"/>.</param>
        /// <returns>An IEnumerable&lt;TType&gt; containing the elements returned by the IEnumerable&lt;TType&gt; returned by the source IValueProvider.</returns>
        /// <remarks>
        /// The <paramref name="source"/> parameter of this method has two levels of observability. First, on the outer layer, the IValueProvider can 
        /// change its Value property. In other words; the sequence (<see cref="IObservableOrderedEnumerable{TType}"/>) gets replaced completely. Second, on the inner layer,
        /// the sequence can alter its contents. 
        /// 
        /// Cascade merges these two layers into one resulting sequence whose contents will be exactly the contents of the source inner sequence or, if 
        /// the Value property of the outer IValueProvider is null, will be empty.
        /// 
        /// Any change events from the outer layer will be translated into 'reset' events for the result sequence. Any change events from the inner
        /// layer will passed directly to the result sequence.
        /// </remarks>
        public static IEnumerable<TType> Cascade<TType>(this IValueProvider<IObservableOrderedEnumerable<TType>> source)
        { return Cascade<IObservableOrderedEnumerable<TType>, TType>(source); }

        /// <summary>
        /// Converts an <see cref="IValueProvider{TGrouping}"/> whose Value property provides an <see cref="IGrouping{TKey,TType}"/> to an <see cref="IEnumerable{TType}"/>. The elements of the result sequence
        /// will be the elements of the grouping returned by the source value provider. If the Value property of the
        /// source is null, then the result sequence will be empty.
        /// </summary>
        /// <typeparam name="TKey">Type of the Key properties of the <see cref="IGrouping{TKey,TType}"/> provided by <paramref name="source"/>.</typeparam>
        /// <typeparam name="TType">Type of the elements of the <see cref="IGrouping{TKey,TType}"/> provided by <paramref name="source"/> and type of the elements of the result sequence.</typeparam>
        /// <param name="source">An <see cref="IValueProvider{TGroup}"/> whose <see cref="IGrouping{TKey,TType}"/> Value property provides the sequence to build the result with.</param>
        /// <returns>An <see cref="IEnumerable{TType}"/> containing the elements of the grouping provided by <paramref name="source"/>.</returns>
        /// <remarks>
        /// The <paramref name="source"/> parameter of this method has two levels of observability. First, on the outer layer, the IValueProvider can 
        /// change its Value property. In other words; the sequence (<see cref="IGrouping{TKey,TType}"/>) gets replaced completely. Second, on the inner layer,
        /// the sequence can alter its contents. 
        /// 
        /// Cascade merges these two layers into one resulting sequence whose contents will be exactly the contents of the source inner sequence or, if 
        /// the Value property of the outer IValueProvider is null, will be empty.
        /// 
        /// Any change events from the outer layer will be translated into 'reset' events for the result sequence. Any change events from the inner
        /// layer will passed directly to the result sequence.
        /// </remarks>
        public static IEnumerable<TType> Cascade<TKey, TType>(this IValueProvider<IGrouping<TKey, TType>> source)
        { return Cascade<IGrouping<TKey, TType>, TType>(source); }

        internal static IEnumerable<TType> _Cascade<TKey, TType>(this IInternalValueProvider<IGrouping<TKey, TType>> source)
        { return _Cascade<IGrouping<TKey, TType>, TType>(source); }
    }
}
