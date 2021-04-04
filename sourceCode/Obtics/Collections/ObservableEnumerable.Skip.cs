using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return elements from.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements that occur after the specified index in the input sequence, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        { return SkipTransformation<TSource>.Create(source, count); }

        /// <summary>
        /// Bypasses a variable number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return elements from.</param>
        /// <param name="count">An <see cref="IValueProvider{Int32}"/> whose Value property gives the number of elements to skip before returning the remaining elements.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements that occur after the specified index in the input sequence, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, IValueProvider<int> count)
        { 
            return 
                count.Patched()
                    ._Select(
                        FuncExtender<int>.Extend(source, (v,src) => src.Skip(v))                        
                    )
                    .Cascade(); 
        }
    }
}
