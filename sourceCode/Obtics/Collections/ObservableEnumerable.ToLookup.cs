using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        { return ToLookup(source, keySelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified key selector function and key comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null </returns>
        public static IObservableLookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return LookupTransformation<TKey, TSource>.Create(
                ConvertToPairsTransformation<TSource, TKey>.Create(source, keySelector),
                comparer
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector)
        { return ToLookup(source, keySelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function and key comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null </returns>
        public static IObservableLookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            return LookupTransformation<TKey, TSource>.Create(
                NotifyVpcTransformation<TSource, TKey>.Create(source, keySelector),
                comparer
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified key selector and element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        { return ToLookup(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified key selector function, a comparer, and an element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (elementSelector == null)
                return null;

            return LookupTransformation<TKey, TElement>.Create(
                ConvertToPairsTransformation<TSource, TKey>
                    .Create(source, keySelector)
                    .Select(
                        elementSelector,
                        (kvp, elsel) => Tuple.Create(elsel(kvp.First), kvp.Second)
                    ),
                comparer
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified dynamic key selector and element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector)
        { return ToLookup(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function, a comparer, and an element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (elementSelector == null)
                return null;

            return LookupTransformation<TKey, TElement>.Create(
                (IVersionedEnumerable<Tuple<TElement,TKey>>)
                    NotifyVpcTransformation<TSource, TKey>
                        .Create(source, keySelector)
                        .Select(
                            elementSelector,
                            ( kvp, elsel ) => Tuple.Create(elsel(kvp.First), kvp.Second)
                        ),
                comparer
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified key selector and dynamic element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return ToLookup(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified key selector function, a comparer, and a dynamic element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (elementSelector == null)
                return null;

            return LookupTransformation<TKey, TElement>.Create(
                (IVersionedEnumerable<Tuple<TElement,TKey>>)
                    ConvertToPairsTransformation<TSource, TKey>
                        .Create(source, keySelector)
                        .Select(
                            elementSelector,
                            (kvp, elsel) => 
                                (IValueProvider<Tuple<TElement,TKey>>)
                                    elsel(kvp.First)
                                        .Patched()
                                        ._Select(
                                            FuncExtender<TElement>.Extend( 
                                                kvp, 
                                                ( element, kvp2 ) => 
                                                    Tuple.Create(element, kvp2.Second) 
                                            )
                                        )
                        ),
                comparer
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified dynamic key selector and dynamic element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return ToLookup(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableLookup{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function, a comparer, and a dynamic element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a lookup from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableLookup{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableLookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (elementSelector == null)
                return null;

            return LookupTransformation<TKey, TElement>.Create(
                (IVersionedEnumerable<Tuple<TElement, TKey>>)
                    NotifyVpcTransformation<TSource, TKey>
                        .Create(source, keySelector)
                        .Select(
                            elementSelector,
                            ( kvp, elsel ) => 
                                (IValueProvider<Tuple<TElement,TKey>>)
                                    elsel(kvp.First)
                                        .Patched()
                                        ._Select(
                                            FuncExtender<TElement>.Extend( 
                                                kvp, 
                                                (element, kvp2) => Tuple.Create(element, kvp2.Second)
                                            )
                                        )
                        ),
                comparer
            );
        }
    }
}
