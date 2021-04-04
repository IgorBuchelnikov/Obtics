using System.Collections.Generic;
using Obtics.Collections.Transformations;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Produces the set union of two sequences by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">A sequence whose distinct elements form the first set for the union.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements from both input sequences, excluding duplicates, or null if either <paramref name="first"/> or <paramref name="second"/> is null.</returns>
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        { return Union(first, second, ObticsEqualityComparer<TSource>.Default); }

        /// <summary>
        /// Produces the set union of two sequences by using a given <see cref="IEqualityComparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">A sequence whose distinct elements form the first set for the union.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TSource}"/> to compare values.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements from both input sequences, excluding duplicates, or null if either <paramref name="first"/>, <paramref name="second"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        { return Distinct( Concat(first, second), comparer); }
    }
}
