using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the element at the specified position in the source sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource> ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        { return _ElementAt(source, index).Concrete(); }

        internal static IInternalValueProvider<TSource> _ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        { return ElementAggregate<TSource>.Create(source, index); }

        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="index">An <see cref="IValueProvider{Int32}"/> whose Value property gives the zero-based index of the element to retrieve.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the element at the specified position in the source sequence, or null when <paramref name="source"/> or <paramref name="index"/> is null.</returns>
        public static IValueProvider<TSource> ElementAt<TSource>(this IEnumerable<TSource> source, IValueProvider<int> index)
        { return _ElementAt(source, index.Patched()).Concrete(); }

        internal static IInternalValueProvider<TSource> _ElementAt<TSource>(this IEnumerable<TSource> source, IInternalValueProvider<int> index)
        {
            return
                index._Convert(
                    FuncExtender<IValueProvider<int>>.Extend(
                        source,
                        (indexCV, src) =>
                            (IValueProvider<TSource>)src._ElementAt(indexCV.Value)
                    )
                )
            ;
        }
    }
}
