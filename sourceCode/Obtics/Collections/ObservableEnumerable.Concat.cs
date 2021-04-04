using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using System.Linq;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Concatenates two sequences
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The sequence to concatenate to the first sequence.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the concatenated elements of the two input sequences, or null when either <paramref name="first"/> or <paramref name="second"/> is null.</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        { return CascadeTransformation<TSource,IEnumerable<TSource>>.GeneralCreate(StaticEnumerable<IEnumerable<TSource>>.Create(first, second)); }

        /// <summary>
        /// Concatenates all sequences in a sequence of child sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the child sequences.</typeparam>
        /// <param name="source">The sequence to concatenate to the child sequences of.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the concatenated elements of the child sequences of the input sequence, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<IVersionedEnumerable<TSource>> source)
        { return CascadeTransformation<TSource,IVersionedEnumerable<TSource>>.GeneralCreate(source); }

        /// <summary>
        /// Concatenates all sequences in a sequence of child sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the child sequences.</typeparam>
        /// <param name="source">The sequence to concatenate to the child sequences of.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the concatenated elements of the child sequences of the input sequence, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<IEnumerable<TSource>> source)
        { return CascadeTransformation<TSource, IEnumerable<TSource>>.GeneralCreate(source); }

        /// <summary>
        /// Concatenates all elements in a sequence of groupings.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements of the grouping-elements of the input sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys of the grouping-elements of the input sequence.</typeparam>
        /// <param name="source">The sequence of groupings to concatenate.</param>
        /// <returns>An <see cref="IEnumerable{TElement}"/>, that contains the concatenated elements of the groups, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TElement> Concat<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source)
        { return CascadeTransformation<TElement, IGrouping<TKey, TElement>>.GeneralCreate(source); }

        /// <summary>
        /// Concatenates all sequences in a sequence of sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the child sequences.</typeparam>
        /// <param name="source">The sequence to concatenate to the child sequences of.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the concatenated elements of the child sequences of the input sequence, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<IOrderedEnumerable<TSource>> source)
        { return CascadeTransformation<TSource, IOrderedEnumerable<TSource>>.GeneralCreate(source); }

        /// <summary>
        /// Concatenates all sequences in a sequence of sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the child sequences.</typeparam>
        /// <param name="source">The sequence to concatenate to the child sequences of.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the concatenated elements of the child sequences of the input sequence, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<IObservableOrderedEnumerable<TSource>> source)
        { return CascadeTransformation<TSource, IObservableOrderedEnumerable<TSource>>.GeneralCreate(source); }
    }
}
