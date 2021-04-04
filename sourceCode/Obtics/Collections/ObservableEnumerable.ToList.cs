using System.Collections.Generic;
using OT = Obtics.Collections.Transformations;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Creates an <see cref="IList{TSource}"/> from a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The sequence to create a <see cref="IList{TSource}"/> from.</param>
        /// <returns>A <see cref="IList{TSource}"/> that contains elements from the input sequence.</returns>
        public static IList<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            return source as IList<TSource> ?? ListTransformation<TSource>.Create(source);
        }

        /// <summary>
        /// Creates an <see cref="IList{TSource}"/> from a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The sequence to create a <see cref="IList{TSource}"/> from.</param>
        /// <param name="returnPath">An <see cref="IListReturnPath{TSource}"/> of <typeparamref name="TSource"/> that provides a return path for the list.</param>
        /// <returns>A <see cref="IList{TSource}"/> that contains elements from the input sequence.</returns>
        public static IList<TSource> ToList<TSource>(this IEnumerable<TSource> source, IListReturnPath<TSource> returnPath)
        {
            return ListWithReturnPathTransformation<TSource>.Create(source, returnPath);
        }
    }
}
