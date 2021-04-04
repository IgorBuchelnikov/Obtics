using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key using the default comparer for the key type.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">Type of the key as returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to sort.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        { return OrderBy(source, keySelector, Comparer<TKey>.Default); }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key using a given <see cref="IComparer{TKey}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">Type of the key as returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to sort.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">An <see cref="IComparer{TKey}"/> to comparer key values.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted to a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return
                SortTransformation<TSource, TKey>.Create(
                    source,
                    keySelector,
                    comparer
                );
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key dynamicaly using the default comparer for the key type.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">Type of the key as returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to sort.</param>
        /// <param name="keySelector">A function to extract a key from an element. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to order the given element by.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector)
        { return OrderBy(source, keySelector, Comparer<TKey>.Default); }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key dynamicaly using a given <see cref="IComparer{TKey}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">Type of the key as returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to sort.</param>
        /// <param name="keySelector">A function to extract a key from an element. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to order the given element by.</param>
        /// <param name="comparer">An <see cref="IComparer{TKey}"/> to comparer key values.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted to a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer)
        {
            return
                DynamicSortTransformation<TSource, TKey>.Create(
                    source,
                    keySelector,
                    comparer
                );
        }
    }
}
