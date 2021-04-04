using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        private static IEnumerable<TSource> BuildSkipWhile<TSource>(IEnumerable<TSource> source, IEnumerable<bool> markers)
        {
            return
                IndexOfFirstAggregate<bool>
                    .Create(markers, b => !b)
                    ._Select(
                        FuncExtender<int?>.Extend(source, (ix,src) => src.Skip(ix.GetValueOrDefault(int.MaxValue)))
                    )
                    .Cascade();
        }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements from the input sequence starting at the first element that does not pass the test specified by predicate, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return BuildSkipWhile<TSource>(source,source.Select(predicate)); }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true dynamicaly and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns an <see cref="IValueProvider{Boolean}"/> whos <see cref="Boolean"/> Value property indicates if the given element satisfies the condition.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements from the input sequence starting at the first element that does not pass the test specified by predicate, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return BuildSkipWhile<TSource>(source, source.Select(predicate)); }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements from the input sequence starting at the first element that does not pass the test specified by predicate, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        { return BuildSkipWhile<TSource>(source, source.Select(predicate)); }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true dynamicaly and then returns the remaining elements. The element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element. It returns an <see cref="IValueProvider{Boolean}"/> whos <see cref="Boolean"/> Value property indicates if the given element satisfies the condition.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the elements from the input sequence starting at the first element that does not pass the test specified by predicate, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, IValueProvider<bool>> predicate)
        { return BuildSkipWhile<TSource>(source, source.Select(predicate)); }
    }
}
