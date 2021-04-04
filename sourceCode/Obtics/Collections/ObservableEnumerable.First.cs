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
        /// Returns the first element of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource> First<TSource>(this IEnumerable<TSource> source)
        { return _First(source).Concrete(); }

        internal static IInternalValueProvider<TSource> _First<TSource>(this IEnumerable<TSource> source)
        { return FirstAggregate<TSource>.Create(source); }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence that passes the test in the specified predicate function, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<TSource> First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return _First(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return FindFirstAggregate<TSource>.Create(source, predicate); }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns an <see cref="IValueProvider{Boolean}"/> whose boolean Value property indicates if the given element satisfies the condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence that passes the test in the specified predicate function, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<TSource> First<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return _First(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _First<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        {
            return
                FindFirstAggregate<Tuple<TSource, bool>>.Create(
                    NotifyVpcTransformation<TSource, bool>.Create(source, predicate),
                    vpcPair => vpcPair.Second
                )
                ._Convert(
                    vpcPair => vpcPair.Value.First
                )
            ;
        }
    }
}
