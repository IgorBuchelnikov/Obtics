using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Collections.Transformations;
using Obtics.Values;
using System.Collections;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Returns the value of a parameterless function when a sequence changes.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of <paramref name="func"/> and the type of the Value property of the result.</typeparam>
        /// <param name="source">The sequence to monitor for changes.</param>
        /// <param name="func">A function whose result will be returned.</param>
        /// <returns>An <see cref="IValueProvider{TResult}"/> whose Value property will give the result of <paramref name="func"/> or null when either <paramref name="source"/> or <paramref name="func"/> is null.</returns>
        public static IValueProvider<TResult> Aggregate<TResult>(this IEnumerable source, Func<TResult> func)
        { return _Aggregate(source, func).Concrete(); }

        internal static IInternalValueProvider<TResult> _Aggregate<TResult>(this IEnumerable source, Func<TResult> func)
        { return IndependentAggregate<TResult>.Create(source, func); }

        /// <summary>
        /// Applies an accumulator function over a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the resulting value.</typeparam>
        /// <param name="source">The sequence to aggregate over.</param>
        /// <param name="func">An accumulator function to be invoked on the entire sequence.</param>
        /// <returns>An <see cref="IValueProvider{TResult}"/> whose Value property will give the result of <paramref name="func"/> or null when either <paramref name="source"/> or <paramref name="func"/> is null.</returns>
        public static IValueProvider<TResult> Aggregate<TSource, TResult>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, TResult> func)
        { return _Aggregate(source, func).Concrete(); }

        internal static IInternalValueProvider<TResult> _Aggregate<TSource, TResult>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, TResult> func)
        { return global::Obtics.Collections.Transformations.Aggregate<TSource, TResult>.Create(source, func); }

        /// <summary>
        /// Applies an accumulator function over a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An IEnumerable&lt;T&gt; to aggregate over.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>The final accumulator value.</returns>
        public static IValueProvider<TSource> Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        { return _Aggregate(source, func).Concrete(); }

        internal static IInternalValueProvider<TSource> _Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        { return _Aggregate(source, default(TSource), func); }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified seed value
        ///     is used as the initial accumulator value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <param name="source">The IEnumerable&lt;T&gt; to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>The final accumulator value.</returns>
        public static IValueProvider<TAccumulate> Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        { return _Aggregate(source, seed, func).Concrete(); }

        internal static IInternalValueProvider<TAccumulate> _Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        { return _Aggregate(source, seed, func, r => r); }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified seed value
        ///     is used as the initial accumulator value, and the specified function is used
        ///     to select the result value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <typeparam name="TResult">The type of the resulting value.</typeparam>
        /// <param name="source">The IObservableEnumerable&lt;T&gt; to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
        /// <returns>The transformed final accumulator value.</returns>
        public static IValueProvider<TResult> Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        { return _Aggregate(source, seed, func, resultSelector).Concrete(); }

        internal static IInternalValueProvider<TResult> _Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        { return AccumulativeAggregate<TSource, TAccumulate, TResult>.Create(source, seed, func, resultSelector); }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified seed value
        ///     is used as the initial accumulator value, and the specified function is used
        ///     to select the result value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <typeparam name="TResult">The type of the resulting value.</typeparam>
        /// <param name="source">The IObservableEnumerable&lt;T&gt; to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
        /// <returns>The transformed final accumulator value.</returns>
        public static IValueProvider<TResult> Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, IValueProvider<TResult>> resultSelector)
        { return _Aggregate(source, seed, func, resultSelector).Concrete(); }

        internal static IInternalValueProvider<TResult> _Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, IValueProvider<TResult>> resultSelector)
        { return AccumulativeAggregate<TSource, TAccumulate, IValueProvider<TResult>>.Create(source, seed, func, resultSelector)._Cascade(); }
    }
}
