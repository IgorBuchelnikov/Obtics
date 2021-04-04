using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Filters a sequence based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains elements from the input sequence that satisfy the condition, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return FilterTransformation<TSource>.Create(source, predicate); }

        /// <summary>
        /// Filters a sequence based on a predicate. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to filter.</param>
        /// <param name="predicate">A function to test each source element for a condition. The second parameter of the function represents the index of the source element.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains elements from the input sequence that satisfy the condition, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        {
            return
                NotifyVpcTransformation<TSource, bool>
                    .Create(source, predicate)
                    .Where(t => t.Second)
                    .Select(t => t.First);
        }

        /// <summary>
        /// Filters a sequence based on a dynamic predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to filter.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns an <see cref="IValueProvider{Boolean}"/> whose <see cref="Boolean"/> Value property indicates if the given element satisfies the predicate.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains elements from the input sequence that satisfy the condition, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (predicate == null)
                return null;

            return
                source
                    .Select(
                        predicate,
                        (item, ix, pred) => Tuple.Create(item, pred(item, ix))
                    )
                    .Where(t => t.Second)
                    .Select(t => t.First);
        }

        /// <summary>
        /// Filters a sequence based on a dynamic predicate. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to filter.</param>
        /// <param name="predicate">A function to test each source element for a condition. The second parameter of the function represents the index of the source element. It returns an <see cref="IValueProvider{Boolean}"/> whose <see cref="Boolean"/> Value property indicates if the given element satisfies the predicate.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains elements from the input sequence that satisfy the condition, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, IValueProvider<bool>> predicate)
        {
            if (predicate == null)
                return null;

            return 
                NotifyVpcTransformation<Tuple<TSource, int>, bool>
                    .Create( 
                        (IVersionedEnumerable<Tuple<TSource,int>>)source.Select((Func<TSource, int, Tuple<TSource, int>>)Tuple.Create), 
                        FuncExtender<Tuple<TSource,int>>.Extend(predicate, (kvp, pred) => pred(kvp.First, kvp.Second)) 
                    )
                    .Where(t => t.Second)
                    .Select(t => t.First.First);
        }
    }
}
