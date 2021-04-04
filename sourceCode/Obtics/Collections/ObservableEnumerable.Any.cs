using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The sequence to check for emptiness.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property is true if the source sequence contains any elements and false otherwise, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<bool> Any<TSource>(this IEnumerable<TSource> source)
        { return _Any(source).Concrete(); }

        internal static IInternalValueProvider<bool> _Any<TSource>(this IEnumerable<TSource> source)
        { return IsNotEmptyAggregate<TSource>.Create(source); }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence whose elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property is true if any elements in the source sequence pass the test in the specified
        ///     predicate and false otherwise, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<bool> Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return _Any(source, predicate).Concrete(); }

        internal static IInternalValueProvider<bool> _Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return AnyAggregate.Create(source.Select(predicate)); }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence whose elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns na <see cref="IValueProvider{Boolean}"/> whose Value property indicates if the given value satisfies the condition.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property is true if any elements in the source sequence pass the test in the specified
        ///     predicate and false otherwise, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<bool> Any<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return _Any(source, predicate).Concrete(); }

        internal static IInternalValueProvider<bool> _Any<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return AnyAggregate.Create(source.Select(predicate)); }
    }
}
