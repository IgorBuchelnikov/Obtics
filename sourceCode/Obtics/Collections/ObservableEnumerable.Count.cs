using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Gives the number of items in a sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence</typeparam>
        /// <param name="source">The sequence to count the number of items of</param>
        /// <returns>An <see cref="IValueProvider{Int32}"/>, whose Value property gives the number of items in the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<int> Count<TSource>(this IEnumerable<TSource> source)
        { return _Count(source).Concrete(); }

        internal static IInternalValueProvider<int> _Count<TSource>(this IEnumerable<TSource> source)
        { return CountAggregate<TSource>.Create(source); }

        /// <summary>
        /// Gives the number of items in a sequence that satisfy a given predicate.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence</typeparam>
        /// <param name="source">The sequence to count the number of items of</param>
        /// <param name="predicate">The predicate to test the individual items with</param>
        /// <returns>An <see cref="IValueProvider{Int32}"/>, whose Value property gives the number of items in the sequence that match the predicate, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<int> Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return _Count(source, predicate).Concrete(); }

        internal static IInternalValueProvider<int> _Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return CountAggregate<TSource>.Create(source.Where(predicate)); }

        /// <summary>
        /// Gives the number of items in a sequence that satisfy a given predicate dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence</typeparam>
        /// <param name="source">The sequence to count the number of items of</param>
        /// <param name="predicate">The predicate to test the individual items with. It returns an <see cref="IValueProvider{Boolean}"/> whose boolean Value property indicates if the given element satisfies the predicate.</param>
        /// <returns>An <see cref="IValueProvider{Int32}"/>, whose Value property gives the number of items in the sequence that match the predicate, or null if either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<int> Count<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return _Count(source, predicate).Concrete(); }
        
        internal static IInternalValueProvider<int> _Count<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return CountAggregate<TSource>.Create(source.Where(predicate)); }
    }
}
