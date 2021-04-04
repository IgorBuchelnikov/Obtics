using System;
using System.Collections.Generic;
using Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Represents an <see cref="IValueProvider{TSource}"/> as a sequence with exactly one item. This item is equal to the value of the Value property of the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the Value of the source and the type of the elements of the result enumerable.</typeparam>
        /// <param name="source">An <see cref="IValueProvider{TSource}"/> to take the Value from.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, with the value of the Value property of <paramref name="source"/> as it's one and only element, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> AsEnumerable<TSource>(this IValueProvider<TSource> source)
        { return AsCollectionTransformation<TSource>.GeneralCreate(source.Patched()); }

        /// <summary>
        /// Represents an <see cref="IValueProvider{TSource}"/> as an sequence with exactly one item. This item is equal to the value of the Value property of the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the Value of the source and the type of the elements of the result enumerable.</typeparam>
        /// <param name="source">An <see cref="IValueProvider{TSource}"/> to take the Value from.</param>
        /// <param name="predicate">A delegate that returns true if the result collection should have a member and false if it should not.</param>
        /// <returns>an <see cref="IEnumerable{TSource}"/>, with the value of the Value property of <paramref name="source"/> as it's one possible element, or null when <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> AsEnumerable<TSource>(this IValueProvider<TSource> source, Func<TSource, bool> predicate)
        { return AsCollectionNullableTransformation<TSource>.GeneralCreate(source.Patched(), predicate); }
    }
}
