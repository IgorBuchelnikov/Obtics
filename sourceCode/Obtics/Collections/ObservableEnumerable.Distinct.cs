using System.Collections.Generic;
using Obtics.Collections.Transformations;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Removes duplicates from a sequence. Equality is determined using the default <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence</typeparam>
        /// <param name="source">The seqeuence to remove duplicates from</param>
        /// <returns>The sequence with all duplicates removed</returns>
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        { return Distinct(source, ObticsEqualityComparer<TSource>.Default); }

        /// <summary>
        /// Removes duplicates from a sequence. Equality is determined using a given <see cref="IEqualityComparer{T}"/> object.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence.</typeparam>
        /// <param name="source">The seqeuence to remove duplicates from.</param>
        /// <param name="comparer">Equality comparer that determines if two items are equal and therefore are duplicates.</param>
        /// <returns>The sequence with all duplicates removed</returns>
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            return
                DistinctTransformation<TSource>.Create(
                    source,
                    comparer
                );
        }
    }
}
