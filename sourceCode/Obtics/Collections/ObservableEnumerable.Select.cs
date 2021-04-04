using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        { return ConvertTransformation<TSource, TResult>.Create(source, selector); }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating an extra value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue">The type of the extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value">A value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter receives the given extra value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue, TResult>(this IEnumerable<TSource> source, TValue value, Func<TSource, TValue, TResult> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender1.Extend(value, selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating two extra values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue1">The type of the first extra value.</typeparam>
        /// <typeparam name="TValue2">The type of the second extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value1">The first value to specialize the transformation function with.</param>
        /// <param name="value2">The second value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second and third parameters receives the given first and second extra value respecively.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue1, TValue2, TResult>(this IEnumerable<TSource> source, TValue1 value1, TValue2 value2, Func<TSource, TValue1, TValue2, TResult> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender2.Extend(value1, value2, selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating three extra values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue1">The type of the first extra value.</typeparam>
        /// <typeparam name="TValue2">The type of the second extra value.</typeparam>
        /// <typeparam name="TValue3">The type of the third extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value1">The first value to specialize the transformation function with.</param>
        /// <param name="value2">The second value to specialize the transformation function with.</param>
        /// <param name="value3">The third value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second, third and fourth parameters receives the given first, second and third extra value respecively.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue1, TValue2, TValue3, TResult>(this IEnumerable<TSource> source, TValue1 value1, TValue2 value2, TValue3 value3, Func<TSource, TValue1, TValue2, TValue3, TResult> selector)
        { 
            if (selector == null)
                return null;

            return Select(source, FuncExtender3.Extend(value1, value2, value3, selector)); 
        }

        /// <summary>
        /// Projects each element of a sequence into a new form dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result of the projection.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TResult>> selector)
        {
            return
                ConvertTransformation<Tuple<TSource, TResult>, TResult>.Create(
                    NotifyVpcTransformation<TSource, TResult>.Create(
                        source,
                        selector
                    ),
                    t => t.Second
                );
        }

        /// <summary>
        /// Projects each element of a sequence into a new form dynamicaly by incorporating an extra value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue">The type of the extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value">A value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter receives the given extra value. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result of the projection.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue, TResult>(this IEnumerable<TSource> source, TValue value, Func<TSource, TValue, IValueProvider<TResult>> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender1.Extend(value, selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form dynamicaly by incorporating two extra values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue1">The type of the first extra value.</typeparam>
        /// <typeparam name="TValue2">The type of the second extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value1">The first value to specialize the transformation function with.</param>
        /// <param name="value2">The second value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second and third parameters receives the given first and second extra value respecively. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result of the projection.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue1, TValue2, TResult>(this IEnumerable<TSource> source, TValue1 value1, TValue2 value2, Func<TSource, TValue1, TValue2, IValueProvider<TResult>> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender2.Extend(value1, value2, selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form dynamicaly by incorporating three extra values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue1">The type of the first extra value.</typeparam>
        /// <typeparam name="TValue2">The type of the second extra value.</typeparam>
        /// <typeparam name="TValue3">The type of the third extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value1">The first value to specialize the transformation function with.</param>
        /// <param name="value2">The second value to specialize the transformation function with.</param>
        /// <param name="value3">The third value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second, third and fourth parameters receives the given first, second and third extra value respecively. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result of the projection.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue1, TValue2, TValue3, TResult>(this IEnumerable<TSource> source, TValue1 value1, TValue2 value2, TValue3 value3, Func<TSource, TValue1, TValue2, TValue3, IValueProvider<TResult>> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender3.Extend(value1, value2, value3, selector));
        }


        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter is the index of the element in the sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            return
                IndexedConvertTransformation<TSource, TResult>.Create(
                    source,
                    selector
                );
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index and an extra value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue">The type of the extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value">A value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter is the index of the element in the sequence and the third parameter receives the given extra value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue, TResult>(this IEnumerable<TSource> source, TValue value, Func<TSource, int, TValue, TResult> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender1.Extend(value,selector)); 
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index and two extra values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue1">The type of the first extra value.</typeparam>
        /// <typeparam name="TValue2">The type of the second extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value1">The first value to specialize the transformation function with.</param>
        /// <param name="value2">The second value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter is the index of the element in the sequence and the third and fourth parameters receive the given first and second extra values respectively.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue1, TValue2, TResult>(this IEnumerable<TSource> source, TValue1 value1, TValue2 value2, Func<TSource, int, TValue1, TValue2, TResult> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender2.Extend(value1, value2, selector));
        }


        /// <summary>
        /// Projects each element of a sequence into a new form dynamicaly by incorporating the element's index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter is the index of the element in the sequence. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result of the projection.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IValueProvider<TResult>> selector)
        {
            if (selector == null)
                return null;

            return
                Select(
                    IndexedConvertTransformation<TSource, Tuple<TSource, int>>.Create(
                        source,
                        Tuple.Create
                    ),
                    FuncExtender<Tuple<TSource, int>>.Extend(selector, (pair, sel) => sel(pair.First, pair.Second))
                );
        }

        /// <summary>
        /// Projects each element of a sequence into a new form dynamicaly by incorporating the element's index and an extra value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue">The type of the extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value">A value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter is the index of the element in the sequence and the third parameter receives the given extra value. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result of the projection.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue, TResult>(this IEnumerable<TSource> source, TValue value, Func<TSource, int, TValue, IValueProvider<TResult>> selector)
        { 
            if (selector == null)
                return null;

            return Select(source, FuncExtender1.Extend(value, selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form dynamicaly by incorporating the element's index and two extra values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TValue1">The type of the first extra value.</typeparam>
        /// <typeparam name="TValue2">The type of the second extra value.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="value1">The first value to specialize the transformation function with.</param>
        /// <param name="value2">The second value to specialize the transformation function with.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter is the index of the element in the sequence and the third and fourth parameters receive the given first and second extra values respectively. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result of the projection.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the transform function on each element of source, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> Select<TSource, TValue1, TValue2, TResult>(this IEnumerable<TSource> source, TValue1 value1, TValue2 value2, Func<TSource, int, TValue1, TValue2, IValueProvider<TResult>> selector)
        {
            if (selector == null)
                return null;

            return Select(source, FuncExtender2.Extend(value1, value2, selector));
        }

    }
}
