using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Collections.Transformations;
using VT = Obtics.Values.Transformations;
using CT = Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Returns the last element of a sequence or a default value if no such element can be found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to return the last element of.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property will give the element at the last position in the source sequence or default(<typeparamref name="TSource"/>) if the sequence is empty, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource> Last<TSource>(this IEnumerable<TSource> source)
        { return _Last(source).Concrete(); }

        internal static IInternalValueProvider<TSource> _Last<TSource>(this IEnumerable<TSource> source)
        { return LastAggregate<TSource>.Create(source); }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a given condition or a default value if no such element can be found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to return an element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property will give the last element that satisfies the condition in the source sequence or default(<typeparamref name="TSource"/>) if such an element can not be found, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource> Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return _Last(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return FindLastAggregate<TSource>.Create(source, predicate); }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a given condition dynamicaly or a default value if no such element can be found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to return an element of.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns an <see cref="IValueProvider{Boolean}"/> whoser boolean Value property indicates if a given element satisfies the condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property will give the last element that satisfies the condition in the source sequence or default(<typeparamref name="TSource"/>) if such an element can not be found, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource> Last<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return _Last(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _Last<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        {
            return
                FindLastAggregate<Tuple<TSource, bool>>
                    .Create(
                        NotifyVpcTransformation<TSource, bool>.Create(source, predicate),
                        vpcPair => vpcPair.Second
                    )
                    ._Select(vpcPair => vpcPair.First);
        }
    }
}
