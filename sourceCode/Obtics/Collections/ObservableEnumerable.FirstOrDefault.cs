using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence or default(<typeparamref name="TSource"/>) if no such element is found, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource> FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        { return _FirstOrDefault(source).Concrete(); }

        internal static IInternalValueProvider<TSource> _FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        { return ElementAggregateWithDefault<TSource>.Create(source, 0); }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence that passes the test in the specified predicate function or default(<typeparamref name="TSource"/>) if no such element is found, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<TSource> FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return _FirstOrDefault(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return FindFirstOrDefaultAggregate<TSource>.Create(source, predicate); }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns an <see cref="IValueProvider{Boolean}"/> whose boolean Value property indicates if the given element satisfies the condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence that passes the test in the specified predicate function or default(<typeparamref name="TSource"/>) if no such element is found, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<TSource> FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return _FirstOrDefault(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        {
            return
                FindFirstOrDefaultAggregate<Tuple<TSource,bool>>.Create(
                    NotifyVpcTransformation<TSource, bool>.Create(source, predicate),
                    vpcPair => vpcPair.Second
                )
                ._Select(vpcPair => vpcPair == null ? default(TSource) : vpcPair.First)
            ;
        }
    }
}
