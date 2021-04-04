using System.Collections.Generic;
using OT = Obtics.Collections.Transformations;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Creates an array from a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">A sequence to create an Array from.</param>
        /// <returns>An <see cref="IValueProvider{Array}"/>, whose Value property gives an array that contains the elements from the input sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource[]> ToArray<TSource>(this IEnumerable<TSource> source)
        { return _ToArray(source).Concrete(); }

        internal static IInternalValueProvider<TSource[]> _ToArray<TSource>(this IEnumerable<TSource> source)
        { return OT.Aggregate<TSource, TSource[]>.Create(source, s => System.Linq.Enumerable.ToArray(s)); }
    }
}
