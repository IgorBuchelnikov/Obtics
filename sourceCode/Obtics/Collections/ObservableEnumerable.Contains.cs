using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;
using System;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property will be true if the source sequence contains an element that has the specified value and false otherwise, or null if <paramref name="source"/> is null.</returns>
        public static IValueProvider<bool> Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        { return Contains(source, value, ObticsEqualityComparer<TSource>.Default); }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using a specified <see cref="IEqualityComparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/> whose Value property will be true if the source sequence contains an element that has the specified value and false otherwise, or null if either <paramref name="source"/> or <paramref name="comparer"/> is null.</returns>
        public static IValueProvider<bool> Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        { return _Contains(source, value, comparer).Concrete(); }

        internal static IInternalValueProvider<bool> _Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (comparer == null)
                return null;

            return
                AnyAggregate.Create(
                    Select(
                        source,
                        comparer,
                        value,
                        (item, comp, val) => comp.Equals(item, val)
                    )
                );
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">A <see cref="IValueProvider{TSource}"/> whose Value property gives the value to locate in the sequence.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/> whose Value property will be true if the source sequence contains an element that has the specified value and false otherwise, or null if either <paramref name="source"/> or <paramref name="value"/> is null.</returns>
        public static IValueProvider<bool> Contains<TSource>(this IEnumerable<TSource> source, IValueProvider<TSource> value)
        { return Contains(source, value, ObticsEqualityComparer<TSource>.Default); }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using a specified <see cref="IEqualityComparer{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">A <see cref="IValueProvider{TSource}"/> whose Value property gives the value to locate in the sequence.</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/> whose Value property will be true if the source sequence contains an element that has the specified value and false otherwise, or null if either <paramref name="source"/>, <paramref name="value"/> or <paramref name="comparer"/> is null.</returns>
        public static IValueProvider<bool> Contains<TSource>(this IEnumerable<TSource> source, IValueProvider<TSource> value, IEqualityComparer<TSource> comparer)
        { return _Contains(source, value.Patched(), comparer).Concrete(); }

        internal static IInternalValueProvider<bool> _Contains<TSource>(this IEnumerable<TSource> source, IInternalValueProvider<TSource> value, IEqualityComparer<TSource> comparer)
        {
            return
                ValueProvider._Convert(
                    value,
                    FuncExtender<IValueProvider<TSource>>.Extend(
                        source,
                        comparer,
                        (vcv, src, comp) => (IValueProvider<bool>)src._Contains(vcv.Value, comp)
                    )
                );
        }
    }
}
