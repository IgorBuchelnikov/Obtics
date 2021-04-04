using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Collections.Transformations;
using VT = Obtics.Values.Transformations;
using CT = Obtics.Collections.Transformations;
using System.Collections;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Filters the elements of an <see cref="IEnumerable"/> based on a specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to filter the elements of the sequence on.</typeparam>
        /// <param name="source">The sequence to filter.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, containing elements from the source sequence, or null if <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// No true casting is performed. The elements must be instances of or instances of types derived from the requested type to pass through the filter.
        /// </remarks>
        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            var seqSource = source.Patched();

            return
                seqSource == null ? null :
                UpCast<TResult>(seqSource) ??
                    (seqSource as IEnumerable<object> ?? TypeConvertTransformation<object>.Create(seqSource))
                        .Where(o => o is TResult)
                        .Select(o => (TResult)o); 
        }

        /// <summary>
        /// Casts the elements of an <see cref="IEnumerable{TSource}"/> up to a specified base type.
        /// </summary>
        /// <typeparam name="TResult">The type to cast the elements of the sequence up to.</typeparam>
        /// <typeparam name="TSource">The type of the sequence elements. This type is constrained to <typeparamref name="TSource"/> or types derived of <typeparamref name="TSource"/>.</typeparam>
        /// <param name="source">The sequence to filter.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, containing elements from the source sequence, or null if <paramref name="source"/> is null.</returns>
        public static IEnumerable<TResult> OfType<TSource, TResult>(this IEnumerable<TSource> source) where TSource : TResult
        { return source.Select(e => (TResult)e); }
    }
}
