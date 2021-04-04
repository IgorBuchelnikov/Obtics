using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TSource}"/> and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function on each element of the input sequence, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (selector == null)
                return null;

            return 
                source
                    .Select(selector)
                    .Concat(); 
        }


        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TResult}"/> and flattens the resulting sequences into one sequence. The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter of the function represents the index of the source element.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function on each element of the input sequence, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (selector == null)
                return null;

            return 
                source
                    .Select(selector)
                    .Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TSource}"/> dynamicaly and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element. It returns an <see cref="IValueProvider{TResultEnum}"/> whose <see cref="IEnumerable{TResult}"/> Value property gives the sequence the given element is projected into.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function on each element of the input sequence, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<IEnumerable<TResult>>> selector)
        { 
            return 
                source
                    .Select(selector)
                    .Concat(); 
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TResult}"/> dynamicaly and flattens the resulting sequences into one sequence. The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element. The second parameter of the function represents the index of the source element. It returns an <see cref="IValueProvider{TResultEnum}"/> whose <see cref="IEnumerable{TResult}"/> Value property gives the sequence the given element is projected into.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function on each element of the input sequence, or null when either <paramref name="source"/> or <paramref name="selector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IValueProvider<IEnumerable<TResult>>> selector)
        { 
            return 
                source
                    .Select(selector)
                    .Concat(); 
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/>, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (collectionSelector == null)
                return null;

            return 
                source
                    .Select(
                        collectionSelector, 
                        resultSelector, 
                        (item, colsel, ressel) => 
                            colsel(item)
                                .Select(
                                    ressel, 
                                    item, 
                                    (cItem, ressel2, itm2) => 
                                        ressel2(itm2, cItem)
                                )
                    )
                    .Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/>, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.  The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence. The second parameter of the function represents the index of the source element.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (collectionSelector == null || resultSelector == null )
                return null;

            return source.Select(collectionSelector, resultSelector, (item, index, colsel, ressel) => colsel(item, index).Select(ressel, item, (cItem, ressel2, itm2) => ressel2(itm2, cItem))).Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/>, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein dynamicaly.  The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence. The second parameter of the function represents the index of the source element.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given projected element.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, IValueProvider<TResult>> resultSelector)
        { 
            if (collectionSelector == null || resultSelector == null )
                return null;

            return source.Select(collectionSelector, resultSelector, (item, colsel, ressel) => colsel(item).Select(ressel, item, (cItem, ressel2, itm2) => ressel2(itm2, cItem))).Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/>, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein dynamicaly.  The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence. The second parameter of the function represents the index of the source element.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given projected element.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, IValueProvider<TResult>> resultSelector)
        {
            if( collectionSelector == null || resultSelector == null ) 
                return null;

            return source.Select(collectionSelector, resultSelector, (item, index, colsel, ressel) => colsel(item, index).Select(ressel, item, (cItem, ressel2, itm2) => ressel2(itm2, cItem))).Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/> dynamicaly, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence. It returns an <see cref="IValueProvider{TResultEnum}"/> whose <see cref="IEnumerable{TResult}"/> Value property gives the sequence the given element is projected into.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<IEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if( collectionSelector == null || resultSelector == null ) 
                return null;

            return source.Select(collectionSelector, resultSelector, (item, colsel, ressel) => colsel(item).Cascade().Select(ressel, item, (cItem, ressel2, itm2) => ressel2(itm2, cItem))).Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/> dynamicaly, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.  The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence. The second parameter of the function represents the index of the source element. It returns an <see cref="IValueProvider{TResultEnum}"/> whose <see cref="IEnumerable{TResult}"/> Value property gives the sequence the given element is projected into.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IValueProvider<IEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if( collectionSelector == null || resultSelector == null ) 
                return null;

            return source.Select(collectionSelector, resultSelector, (item, index, colsel, ressel) => colsel(item, index).Cascade().Select(ressel, item, (cItem, ressel2, itm2) => ressel2(itm2, cItem))).Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/> dynamicaly, flattens the resulting sequences into one sequence, and invokes a result selector function dynamicaly on each element therein.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence. It returns an <see cref="IValueProvider{TResultEnum}"/> whose <see cref="IEnumerable{TResult}"/> Value property gives the sequence the given element is projected into.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given projected element.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<IEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, IValueProvider<TResult>> resultSelector)
        {
            if( collectionSelector == null || resultSelector == null ) 
                return null;

            return source.Select(collectionSelector, resultSelector, (item, colsel, ressel) => colsel(item).Cascade().Select(ressel, item, (cItem, ressel2, itm2) => ressel2(itm2, cItem))).Concat();
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{TCollection}"/> dynamicaly, flattens the resulting sequences into one sequence, and invokes a result selector function dynamicaly on each element therein.  The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence. The second parameter of the function represents the index of the source element. It returns an <see cref="IValueProvider{TResultEnum}"/> whose <see cref="IEnumerable{TResult}"/> Value property gives the sequence the given element is projected into.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given projected element.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, whose elements are the result of invoking the one-to-many transform function collectionSelector on each element of source and then mapping each of those sequence elements and their corresponding source element to a result element, or null when either <paramref name="source"/>, <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IValueProvider<IEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, IValueProvider<TResult>> resultSelector)
        {
            if( collectionSelector == null || resultSelector == null ) 
                return null;

            return source.Select(collectionSelector, resultSelector, (item, index, colsel, ressel) => colsel(item, index).Cascade().Select(ressel, item, (cItem, ressel2, itm2) => ressel2(itm2, cItem))).Concat();
        }
    }
}
