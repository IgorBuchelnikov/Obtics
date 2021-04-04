using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Collections.Transformations;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Converts a sequence to an <see cref="IList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence</typeparam>
        /// <param name="source">A sequence to convert to an <see cref="IList{TSource}"/>.</param>
        /// <returns>An <see cref="IList{TSource}"/> with elements from <paramref name="source"/>, or null when <paramref name="source"/> is null.</returns>
        [Obsolete]
        public static IList<TSource> Cap<TSource>(this IEnumerable<TSource> source)
        { return source as ListTransformation<TSource> ?? ListTransformation<TSource>.Create(source); }
    }
}
