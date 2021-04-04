using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Returns the minimum value in a sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{NullableDecimal}"/>, whose Value property gives a nullable <see cref="Decimal"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<Decimal?> Min(this IEnumerable<Decimal?> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Decimal?> _Min(this IEnumerable<Decimal?> source)
        { return ExtremityAggregateNullableValueDescending<Decimal>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{Decimal}"/>, whose Value property gives a <see cref="Decimal"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Decimal}"/> will be default(<see cref="Decimal"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Decimal> Min(this IEnumerable<Decimal> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Decimal> _Min(this IEnumerable<Decimal> source)
        { return ExtremityAggregateValueDescending<Decimal>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value property gives a nullable <see cref="Double"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<Double?> Min(this IEnumerable<Double?> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Double?> _Min(this IEnumerable<Double?> source)
        { return ExtremityAggregateNullableValueDescending<Double>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Double"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value property gives a <see cref="Double"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Double}"/> will be default(<see cref="Double"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Double> Min(this IEnumerable<Double> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Double> _Min(this IEnumerable<Double> source)
        { return ExtremityAggregateValueDescending<Double>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable <see cref="System.Single"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="System.Single"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{NullableSingle}"/>, whose Value property gives a nullable <see cref="System.Single"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<Single?> Min(this IEnumerable<Single?> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Single?> _Min(this IEnumerable<Single?> source)
        { return ExtremityAggregateNullableValueDescending<Single>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="System.Single"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="System.Single"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{Single}"/>, whose Value property gives a <see cref="System.Single"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Single}"/> will be default(<see cref="System.Single"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Single> Min(this IEnumerable<Single> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Single> _Min(this IEnumerable<Single> source)
        { return ExtremityAggregateValueDescending<Single>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt32}"/>, whose Value property gives a nullable <see cref="Int32"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<Int32?> Min(this IEnumerable<Int32?> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Int32?> _Min(this IEnumerable<Int32?> source)
        { return ExtremityAggregateNullableValueDescending<Int32>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Int32"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{Int32}"/>, whose Value property gives a <see cref="Int32"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Int32}"/> will be default(<see cref="Int32"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Int32> Min(this IEnumerable<Int32> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Int32> _Min(this IEnumerable<Int32> source)
        { return ExtremityAggregateValueDescending<Int32>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt64}"/>, whose Value property gives a nullable <see cref="Int64"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        public static IValueProvider<Int64?> Min(this IEnumerable<Int64?> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Int64?> _Min(this IEnumerable<Int64?> source)
        { return ExtremityAggregateNullableValueDescending<Int64>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Int64"/> values to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{Int64}"/>, whose Value property gives a <see cref="Int64"/> that corresponds to the minimum value in the sequence, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Int64}"/> will be default(<see cref="Int64"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Int64> Min(this IEnumerable<Int64> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<Int64> _Min(this IEnumerable<Int64> source)
        { return ExtremityAggregateValueDescending<Int64>.Create(source); }

        /// <summary>
        /// Returns the minimum value in a sequence.
        /// </summary>
        /// <typeparam name="TOut">Type of the elements of the sequence.</typeparam>
        /// <param name="source">A sequence to determine the minimum value of.</param>
        /// <returns>An <see cref="IValueProvider{TOut}"/>, whose Value property gives a <typeparamref name="TOut"/> that corresponds to the minimum value in the sequence or default(<typeparamref name="TOut"/>) if the source sequence is empty, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{TOut}"/> will be default(<typeparamref name="TOut"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequences. 
        /// </remarks>
        public static IValueProvider<TOut> Min<TOut>(this IEnumerable<TOut> source)
        { return _Min(source).Concrete(); }

        internal static IInternalValueProvider<TOut> _Min<TOut>(this IEnumerable<TOut> source)
        { return ExtremityAggregate<TOut>.Create(source, Comparer.DefaultInverted<TOut>()); }

        /// <summary>
        /// Returns the minimum value in a sequence using a given <see cref="IComparer{TOut}"/>.
        /// </summary>
        /// <typeparam name="TOut">Type of the elements of the sequence.</typeparam>
        /// <param name="source">A sequence to determine the minimum value of.</param>
        /// <param name="comparer">An <see cref="IComparer{TOut}"/> to compare elements of the source sequence.</param>
        /// <returns>An <see cref="IValueProvider{TOut}"/>, whose Value property gives a <typeparamref name="TOut"/> that corresponds to the minimum value in the sequence or default(<typeparamref name="TOut"/>) if the source sequence is empty, or null when either <paramref name="source"/> or <paramref name="comparer"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{TOut}"/> will be default(<typeparamref name="TOut"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<TOut> Min<TOut>(this IEnumerable<TOut> source, IComparer<TOut> comparer)
        { return _Min(source, comparer).Concrete(); }

        internal static IInternalValueProvider<TOut> _Min<TOut>(this IEnumerable<TOut> source, IComparer<TOut> comparer)
        { return ExtremityAggregate<TOut>.Create(source, Comparer.Inverted(comparer)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Decimal"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDecimal}"/>, whose Value property gives a nullable <see cref="Decimal"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Decimal?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
        { return ExtremityAggregateNullableValueDescending<Decimal>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Decimal"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Decimal}"/>, whose Value property gives a <see cref="Decimal"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Decimal"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Decimal}"/> will be default(<see cref="Decimal"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Decimal> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
        { return ExtremityAggregateValueDescending<Decimal>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Double"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value property gives a nullable <see cref="Double"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Double?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Double?> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Double?> selector)
        { return ExtremityAggregateNullableValueDescending<Double>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Double"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value property gives a <see cref="Double"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Double"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Double}"/> will be default(<see cref="Double"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Double> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Double> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Double> selector)
        { return ExtremityAggregateValueDescending<Double>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="System.Single"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableSingle}"/>, whose Value property gives a nullable <see cref="System.Single"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Single?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Single?> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Single?> selector)
        { return ExtremityAggregateNullableValueDescending<Single>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="System.Single"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Single}"/>, whose Value property gives a <see cref="System.Single"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="System.Single"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Single}"/> will be default(<see cref="System.Single"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Single> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Single> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Single> selector)
        { return ExtremityAggregateValueDescending<Single>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Int32"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt32}"/>, whose Value property gives a nullable <see cref="Int32"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Int32?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32?> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32?> selector)
        { return ExtremityAggregateNullableValueDescending<Int32>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Int32"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Int32}"/>, whose Value property gives a <see cref="Int32"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Int32"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Int32}"/> will be default(<see cref="Int32"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Int32> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int32> selector)
        { return ExtremityAggregateValueDescending<Int32>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Int64"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt64}"/>, whose Value property gives a nullable <see cref="Int64"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Int64?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64?> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64?> selector)
        { return ExtremityAggregateNullableValueDescending<Int64>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="Int64"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{Int64}"/>, whose Value property gives a <see cref="Int64"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Int64"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Int64}"/> will be default(<see cref="Int64"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Int64> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int64> selector)
        { return ExtremityAggregateValueDescending<Int64>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum result.
        /// </summary>
        /// <typeparam name="TOut">Type of the result of the function.</typeparam>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IValueProvider{TOut}"/>, whose Value property gives a <typeparamref name="TOut"/> that corresponds to the minimum value return by <paramref name="selector"/> or default(<typeparamref name="TOut"/>) if the source sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{TOut}"/> will be default(<typeparamref name="TOut"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<TOut> Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<TOut> _Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> selector)
        { return ExtremityAggregate<TOut>.Create(source.Select(selector), Comparer.DefaultInverted<TOut>()); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum result determined by using a given <see cref="IComparer{TOut}"/> to compare result values.
        /// </summary>
        /// <typeparam name="TOut">Type of the result of the function.</typeparam>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="comparer">An <see cref="IComparer{TOut}"/> to compare the result values of <paramref name="selector"/>.</param>
        /// <returns>An <see cref="IValueProvider{TOut}"/>, whose Value property gives a <typeparamref name="TOut"/> that corresponds to the minimum value return by <paramref name="selector"/> or default(<typeparamref name="TOut"/>) if the source sequence is empty, or null when either <paramref name="source"/>, <paramref name="selector"/> or <paramref name="comparer"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{TOut}"/> will be default(<typeparamref name="TOut"/>).
        /// This may be a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<TOut> Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> selector, IComparer<TOut> comparer)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<TOut> _Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> selector, IComparer<TOut> comparer)
        { return ExtremityAggregate<TOut>.Create(source.Select(selector), Comparer.Inverted(comparer)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Decimal"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableDecimal}"/> whose nullable <see cref="Decimal"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{NullableDecimal}"/>, whose Value property gives a nullable <see cref="Decimal"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Decimal?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal?>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal?>> selector)
        { return ExtremityAggregateNullableValueDescending<Decimal>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Decimal"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Decimal}"/> whose <see cref="Decimal"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{Decimal}"/>, whose Value property gives a <see cref="Decimal"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Decimal"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Decimal}"/> will be default(<see cref="Decimal"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Decimal> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Decimal> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Decimal>> selector)
        { return ExtremityAggregateValueDescending<Decimal>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Double"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableDouble}"/> whose nullable <see cref="Double"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{NullableDouble}"/>, whose Value property gives a nullable <see cref="Double"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Double?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double?>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double?>> selector)
        { return ExtremityAggregateNullableValueDescending<Double>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Double"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Double}"/> whose <see cref="Double"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{Double}"/>, whose Value property gives a <see cref="Double"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Double"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Double}"/> will be default(<see cref="Double"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Double> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Double> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Double>> selector)
        { return ExtremityAggregateValueDescending<Double>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="System.Single"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableSingle}"/> whose nullable <see cref="System.Single"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{NullableSingle}"/>, whose Value property gives a nullable <see cref="System.Single"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Single?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single?>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single?>> selector)
        { return ExtremityAggregateNullableValueDescending<Single>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="System.Single"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Single}"/> whose <see cref="System.Single"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{Single}"/>, whose Value property gives a <see cref="System.Single"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="System.Single"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Single}"/> will be default(<see cref="System.Single"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Single> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Single> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Single>> selector)
        { return ExtremityAggregateValueDescending<Single>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Int32"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableInt32}"/> whose nullable <see cref="Int32"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt32}"/>, whose Value property gives a nullable <see cref="Int32"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Int32?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32?>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32?>> selector)
        { return ExtremityAggregateNullableValueDescending<Int32>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Int32"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Int32}"/> whose <see cref="Int32"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{Int32}"/>, whose Value property gives a <see cref="Int32"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Int32"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Int32}"/> will be default(<see cref="Int32"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Int32> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int32> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int32>> selector)
        { return ExtremityAggregateValueDescending<Int32>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Int64"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{NullableInt64}"/> whose nullable <see cref="Int64"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{NullableInt64}"/>, whose Value property gives a nullable <see cref="Int64"/> that corresponds to the minimum value returned by <paramref name="selector"/> or null if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IValueProvider<Int64?> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64?>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64?> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64?>> selector)
        { return ExtremityAggregateNullableValueDescending<Int64>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum nullable <see cref="Int64"/> value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{Int64}"/> whose <see cref="Int64"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{Int64}"/>, whose Value property gives a <see cref="Int64"/> that corresponds to the minimum value returned by <paramref name="selector"/> or default(<see cref="Int64"/>) if the sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{Int64}"/> will be default(<see cref="Int64"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<Int64> Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<Int64> _Min<TSource>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<Int64>> selector)
        { return ExtremityAggregateValueDescending<Int64>.Create(source.Select(selector)); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum result.
        /// </summary>
        /// <typeparam name="TOut">Type of the result of the function.</typeparam>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{TOutl}"/> whose <typeparamref name="TOut"/> Value property gives the value to determine the minimum with.</param>
        /// <returns>An <see cref="IValueProvider{TOut}"/>, whose Value property gives a <typeparamref name="TOut"/> that corresponds to the minimum value return by <paramref name="selector"/> or default(<typeparamref name="TOut"/>) if the source sequence is empty, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{TOut}"/> will be default(<typeparamref name="TOut"/>).
        /// This infact is a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<TOut> Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TOut>> selector)
        { return _Min(source, selector).Concrete(); }

        internal static IInternalValueProvider<TOut> _Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TOut>> selector)
        { return _Min(source, selector, Comparer.DefaultInverted<TOut>()); }

        /// <summary>
        /// Invokes a transform function on each element of a sequence dynamicaly and returns the minimum result determined by using a given <see cref="IComparer{TOut}"/> to compare result values.
        /// </summary>
        /// <typeparam name="TOut">Type of the result of the function.</typeparam>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{TOutl}"/> whose <typeparamref name="TOut"/> Value property gives the value to determine the minimum with.</param>
        /// <param name="comparer">An <see cref="IComparer{TOut}"/> to compare the result values of <paramref name="selector"/>.</param>
        /// <returns>An <see cref="IValueProvider{TOut}"/>, whose Value property gives a <typeparamref name="TOut"/> that corresponds to the minimum value return by <paramref name="selector"/> or default(<typeparamref name="TOut"/>) if the source sequence is empty, or null when either <paramref name="source"/>, <paramref name="selector"/> or <paramref name="comparer"/> is null.</returns>
        /// <remarks>
        /// When <paramref name="source"/> is an empty sequence the Value property of the result <see cref="IValueProvider{TOut}"/> will be default(<typeparamref name="TOut"/>).
        /// This may be a non-sensical response. It is upto the developer to detect an empty source sequence. 
        /// </remarks>
        public static IValueProvider<TOut> Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TOut>> selector, IComparer<TOut> comparer)
        { return _Min(source, selector, comparer).Concrete(); }

        internal static IInternalValueProvider<TOut> _Min<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TOut>> selector, IComparer<TOut> comparer)
        { return ExtremityAggregate<TOut>.Create(source.Select(selector), Comparer.Inverted(comparer)); }
    }
}
