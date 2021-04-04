using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{NullableDecimal}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Decimal?> Average(this IEnumerable<Decimal?> source)
        { return AverageAggregateNullableDecimal.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{Decimal}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Decimal> Average(this IEnumerable<Decimal> source)
        { return AverageAggregateDecimal.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average(this IEnumerable<Double?> source)
        { return AverageAggregateNullableDouble.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average(this IEnumerable<Double> source)
        { return AverageAggregateDouble.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Single"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="System.Single"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{NullableSingle}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Single?> Average(this IEnumerable<Single?> source)
        { return AverageAggregateNullableSingle.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Single"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="System.Single"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{Single}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Single> Average(this IEnumerable<Single> source)
        { return AverageAggregateSingle.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average(this IEnumerable<Int32?> source)
        { return AverageAggregateNullableInt32.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average(this IEnumerable<Int32> source)
        { return AverageAggregateInt32.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average(this IEnumerable<Int64?> source)
        { return AverageAggregateNullableInt64.Create(source); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the average of.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if <paramref name="source"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average(this IEnumerable<Int64> source)
        { return AverageAggregateInt64.Create(source); }


        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Decimal"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDecimal}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
        { return AverageAggregateNullableDecimal.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Decimal"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Decimal}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
        { return AverageAggregateDecimal.Create(source.Select(selector)); }




        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Double"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Double?> selector)
        { return AverageAggregateNullableDouble.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Double"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Double> selector)
        { return AverageAggregateDouble.Create(source.Select(selector)); }




        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Single"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="System.Single"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableSingle}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Single?> selector)
        { return AverageAggregateNullableSingle.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Single"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="System.Single"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Single}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Single> selector)
        { return AverageAggregateSingle.Create(source.Select(selector)); }




        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Int32"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32?> selector)
        { return AverageAggregateNullableInt32.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Int32"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32> selector)
        { return AverageAggregateInt32.Create(source.Select(selector)); }




        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="Int64"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64?> selector)
        { return AverageAggregateNullableInt64.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Int64"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64> selector)
        { return AverageAggregateInt64.Create(source.Select(selector)); }


        /// <summary>
        /// Computes the average of a sequence of dynamic nullable <see cref="Decimal"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableDecimal}"/> whose Value property gives the nullable <see cref="Decimal"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{NullableDecimal}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal?>> selector)
        { return AverageAggregateNullableDecimal.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic <see cref="Decimal"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Decimal}"/> whose Value property gives the <see cref="Decimal"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{Decimal}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Decimal> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal>> selector)
        { return AverageAggregateDecimal.Create(source.Select(selector)); }


        /// <summary>
        /// Computes the average of a sequence of dynamic nullable <see cref="Double"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableDouble}"/> whose Value property gives the nullable <see cref="Double"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double?>> selector)
        { return AverageAggregateNullableDouble.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic <see cref="Double"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Double}"/> whose Value property gives the <see cref="Double"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double>> selector)
        { return AverageAggregateDouble.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic nullable <see cref="System.Single"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="System.Single"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableSingle}"/> whose Value property gives the nullable <see cref="System.Single"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{NullableSingle}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single?>> selector)
        { return AverageAggregateNullableSingle.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic <see cref="System.Single"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="System.Single"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Single}"/> whose Value property gives the <see cref="System.Single"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{Single}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Single> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single>> selector)
        { return AverageAggregateSingle.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic nullable <see cref="Int32"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableInt32}"/> whose Value property gives the nullable <see cref="Int32"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32?>> selector)
        { return AverageAggregateNullableInt32.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic <see cref="Int32"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Int32}"/> whose Value property gives the <see cref="Int32"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32>> selector)
        { return AverageAggregateInt32.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic nullable <see cref="Int64"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableInt64}"/> whose Value property gives the nullable <see cref="Int64"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double?> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64?>> selector)
        { return AverageAggregateNullableInt64.Create(source.Select(selector)); }

        /// <summary>
        /// Computes the average of a sequence of dynamic <see cref="Int64"/> values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Int64}"/> whose Value property gives the <see cref="Int64"/> to assimilate in the average.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value is the average of the sequence of values, or null if either <paramref name="source"/> or <paramref name="selector"/> is null.
        ///</returns>
        public static IValueProvider<Double> Average<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64>> selector)
        { return AverageAggregateInt64.Create(source.Select(selector)); }
    }
}
