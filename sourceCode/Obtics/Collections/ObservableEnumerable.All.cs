using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Determines if all elements of a sequence satisfy a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property is true if every element of the source sequence passes the test in the specified predicate or if the sequence is empty and false otherwise, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<bool> All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return _All(source, predicate).Concrete(); }

        internal static IInternalValueProvider<bool> _All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return AllAggregate.Create(source.Select(predicate)); }

        /// <summary>
        /// Determines if all elements of a sequence satisfy a condition dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns an <see cref="IValueProvider{Boolean}"/> whose Value property indicates if an elements satisfies the condition.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property is true if every element of the source sequence passes the test in the specified predicate or if the sequence is empty and false otherwise, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<bool> All<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return _All(source, predicate).Concrete(); }

        internal static IInternalValueProvider<bool> _All<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return AllAggregate.Create(source.Select(predicate)); }
    }
}
