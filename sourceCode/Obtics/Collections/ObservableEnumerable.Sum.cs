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
        /// Computes the sum of a sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableDecimal}"/>, whose Value property is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Decimal?> Sum(this IEnumerable<Decimal?> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Decimal?> _Sum(this IEnumerable<Decimal?> source)
        { return SumAggregateNullableDecimal.Create(source); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableDecimal}"/>, whose Value property is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Decimal> Sum(this IEnumerable<Decimal> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Decimal> _Sum(this IEnumerable<Decimal> source)
        { return SumAggregateDecimal.Create(source); }


        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Sum(this IEnumerable<Double?> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Double?> _Sum(this IEnumerable<Double?> source)
        { return SumAggregateNullableDouble.Create(source); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double> Sum(this IEnumerable<Double> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Double> _Sum(this IEnumerable<Double> source)
        { return SumAggregateDouble.Create(source); }




        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Single"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="System.Single"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableSingle}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Single?> Sum(this IEnumerable<Single?> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Single?> _Sum(this IEnumerable<Single?> source)
        { return SumAggregateNullableSingle.Create(source); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Single"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="System.Single"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableSingle}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Single> Sum(this IEnumerable<Single> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Single> _Sum(this IEnumerable<Single> source)
        { return SumAggregateSingle.Create(source); }


        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Int32?> Sum(this IEnumerable<Int32?> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Int32?> _Sum(this IEnumerable<Int32?> source)
        { return SumAggregateNullableInt32.Create(source); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{Int32}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Int32> Sum(this IEnumerable<Int32> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Int32> _Sum(this IEnumerable<Int32> source)
        { return SumAggregateInt32.Create(source); }



        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Int64?> Sum(this IEnumerable<Int64?> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Int64?> _Sum(this IEnumerable<Int64?> source)
        { return SumAggregateNullableInt64.Create(source); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the sum of.</param>
        /// <returns>A <see cref="IValueProvider{Int64}"/>, whose Value is the sum of the sequence of values, or null when <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Int64> Sum(this IEnumerable<Int64> source)
        { return _Sum(source).Concrete(); }

        internal static IInternalValueProvider<Int64> _Sum(this IEnumerable<Int64> source)
        { return SumAggregateInt64.Create(source); }




        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Decimal"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDecimal}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
        { return SumAggregateNullableDecimal.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Decimal"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDecimal}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
        { return SumAggregateDecimal.Create(source.Select(selector)); }




        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Double"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Double?> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Double?> selector)
        { return SumAggregateNullableDouble.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Double"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Double> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Double> selector)
        { return SumAggregateDouble.Create(source.Select(selector)); }


        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Single"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="System.Single"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableSingle}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Single?> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Single?> selector)
        { return SumAggregateNullableSingle.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Single"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="System.Single"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableSingle}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Single> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Single> selector)
        { return SumAggregateSingle.Create(source.Select(selector)); }


        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Int32"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///     empty or contains only values that are null.
        ///</returns>
        public static IValueProvider<Int32?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32?> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32?> selector)
        { return SumAggregateNullableInt32.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Int32"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Int32> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32> selector)
        { return SumAggregateInt32.Create(source.Select(selector)); }


        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Int64"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Int64?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64?> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64?> selector)
        { return SumAggregateNullableInt64.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Int64"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Int64> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64> selector)
        { return SumAggregateInt64.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Decimal"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableDecimal}"/> whose nullable <see cref="Decimal"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDecimal}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal?>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal?>> selector)
        { return SumAggregateNullableDecimal.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Decimal"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Decimal}"/> whose <see cref="Decimal"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{Decimal}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal>> selector)
        { return SumAggregateDecimal.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Double"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableDouble}"/> whose nullable <see cref="Double"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double?>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double?>> selector)
        { return SumAggregateNullableDouble.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Double"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Double}"/> whose <see cref="Double"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double>> selector)
        { return SumAggregateDouble.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Single"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="System.Single"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableSingle}"/> whose nullable <see cref="System.Single"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{NullableSingle}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single?>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single?>> selector)
        { return SumAggregateNullableSingle.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Single"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="System.Single"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Single}"/> whose <see cref="System.Single"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{Single}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single>> selector)
        { return SumAggregateSingle.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Int32"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableInt32}"/> whose nullable <see cref="Int32"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt32}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Int32?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32?>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32?>> selector)
        { return SumAggregateNullableInt32.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Int32"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Int32}"/> whose <see cref="Int32"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{Int32}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Int32> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32>> selector)
        { return SumAggregateInt32.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Int64"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableInt64}"/> whose nullable <see cref="Int64"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt64}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Int64?> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64?>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64?> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64?>> selector)
        { return SumAggregateNullableInt64.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Int64"/> values that
        ///     are obtained dynamicaly by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Int64}"/> whose <see cref="Int64"/> Value property gives the value to add to the total sum for the given element.</param>
        /// <returns>An <see cref="IValueProvider{Int64}"/>, whose Value is the sum of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Int64> Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64>> selector)
        { return _Sum(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64> _Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64>> selector)
        { return SumAggregateInt64.Create(source.Select(selector)); }
    }
}
