﻿using System;
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
        static IInternalValueProvider<Tuple<bool, TSource>> _SingleBase<TSource>(this IEnumerable<TSource> source)
        {
            return
                source._Aggregate(
                    Tuple.Create(false, default(TSource)),
                    (acc, elt) =>
                    {
                        if (acc.First)
                            throw new InvalidOperationException(); //TODO: text

                        return Tuple.Create(true, elt);
                    }
                )
            ;
        }

        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence or default(<typeparamref name="TSource"/>) if no such element is found, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<TSource> Single<TSource>(this IEnumerable<TSource> source)
        { return _Single(source).Concrete(); }

        internal static IInternalValueProvider<TSource> _Single<TSource>(this IEnumerable<TSource> source)
        {
            return
                _SingleBase(source)
                ._Select(
                    t =>
                    {
                        if (!t.First)
                            throw new InvalidOperationException(); //TODO: text

                        return t.Second;
                    }
                )
            ;
        }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence that passes the test in the specified predicate function or default(<typeparamref name="TSource"/>) if no such element is found, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<TSource> Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return _Single(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        { return source.Where(predicate)._Single(); }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition dynamicaly or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition. It returns an <see cref="IValueProvider{Boolean}"/> whose <see cref="Boolean"/> Value property indicates if the given element satisfies the condition.</param>
        /// <returns>An <see cref="IValueProvider{TSource}"/>, whose Value property gives the first element in the sequence that passes the test in the specified predicate function or default(<typeparamref name="TSource"/>) if no such element is found, or null when either <paramref name="source"/> or <paramref name="predicate"/> is null.</returns>
        public static IValueProvider<TSource> Single<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return _Single(source, predicate).Concrete(); }

        internal static IInternalValueProvider<TSource> _Single<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<bool>> predicate)
        { return source.Where(predicate)._Single(); }
    }
}
