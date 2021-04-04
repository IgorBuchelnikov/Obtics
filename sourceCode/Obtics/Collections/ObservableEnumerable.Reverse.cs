using System.Collections.Generic;
using Obtics.Collections.Transformations;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Inverts the order of the elements in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to reverse.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, whose elements correspond to those of the input sequence in reverse order, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        { return ReverseTransformation<TSource>.Create(source); }
    }
}
