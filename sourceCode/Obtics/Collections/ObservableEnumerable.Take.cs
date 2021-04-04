using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to take elements from.</param>
        /// <param name="count">The number of elements to take from the sequence.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the specified number of elements from the start of the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        { 
            return 
                TakeTransformation<TSource>.Create(
                    source, 
                    count
                ); 
        }

        /// <summary>
        /// Returns a variable number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to take elements from.</param>
        /// <param name="count">An <see cref="IValueProvider{Int32}"/> whose Value property gives number of elements to take from the sequence.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> that contains the specified number of elements from the start of the sequence, or null when either <paramref name="source"/> or <paramref name="count"/> is null.</returns>
        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, IValueProvider<int> count)
        { 
            return 
                count.Patched()
                    ._Select(
                        FuncExtender<int>.Extend(source, (v, src) => src.Take(v))
                    )
                    .Cascade(); 
        }
    }
}
