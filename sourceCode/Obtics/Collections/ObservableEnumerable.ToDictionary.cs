using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        static IObservableDictionary<TKey, TValue> _CreateDictionaryTransformation<TKey, TValue>(IEnumerable<Tuple<TValue, TKey>> s, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TValue> returnPath)
        {
            return
                returnPath == null ? (IObservableDictionary<TKey, TValue>)DictionaryTransformation<TKey, TValue>.Create(s, comparer) :
                (IObservableDictionary<TKey, TValue>)DictionaryWithReturnPathTransformation<TKey, TValue>.Create(s, comparer, returnPath);
        }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        { return ToDictionary(source, keySelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IDictionaryReturnPath<TKey,TSource> returnPath)
        { return ToDictionary(source, keySelector, ObticsEqualityComparer<TKey>.Default, returnPath); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function and key comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        { return _ToDictionary(source, keySelector, comparer, null); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function and key comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey,TSource> returnPath)
        {
            if (returnPath == null)
                return null;

            return _ToDictionary(source, keySelector, comparer, returnPath); 
        }

        static IObservableDictionary<TKey, TSource> _ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TSource> returnPath)
        {
            return _CreateDictionaryTransformation<TKey, TSource>(
                ConvertToPairsTransformation<TSource, TKey>.Create(source, keySelector),
                comparer,
                returnPath
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector)
        { return ToDictionary(source, keySelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IDictionaryReturnPath<TKey,TSource> returnPath)
        { return ToDictionary(source, keySelector, ObticsEqualityComparer<TKey>.Default, returnPath); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function and key comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        { return _ToDictionary(source, keySelector, comparer, null); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function and key comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains keys and values, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null </returns>
        public static IObservableDictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey,TSource> returnPath)
        {
            if (returnPath == null)
                return null;

            return _ToDictionary(source, keySelector, comparer, returnPath); 
        }

        static IObservableDictionary<TKey, TSource> _ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey,TSource> returnPath)
        {
            return _CreateDictionaryTransformation<TKey, TSource>(
                NotifyVpcTransformation<TSource, TKey>.Create(source, keySelector),
                comparer,
                returnPath
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector and element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector and element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IDictionaryReturnPath<TKey, TElement> returnPath)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function, a comparer, and an element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        { return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, null); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function, a comparer, and an element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TElement> returnPath )
        {
            if (returnPath == null)
                return null;

            return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); 
        }

        static IObservableDictionary<TKey, TElement> _ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey,TElement> returnPath )
        {
            if (elementSelector == null)
                return null;

            return _CreateDictionaryTransformation<TKey, TElement>(
                (IVersionedEnumerable<Tuple<TElement, TKey>>)
                    ConvertToPairsTransformation<TSource, TKey>
                        .Create(source, keySelector)
                        .Select(
                            elementSelector,
                            (kvp, elsel) => Tuple.Create(elsel(kvp.First), kvp.Second)
                        ),
                comparer,
                returnPath
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector and element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector and element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IDictionaryReturnPath<TKey, TElement> returnPath)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function, a comparer, and an element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        { return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, null); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function, a comparer, and an element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TElement> returnPath)
        {
            if (returnPath == null)
                return null;

            return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); 
        }

        static IObservableDictionary<TKey, TElement> _ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey,TElement> returnPath )
        {
            if (elementSelector == null)
                return null;

            return _CreateDictionaryTransformation<TKey, TElement>(
                (IVersionedEnumerable<Tuple<TElement, TKey>>)
                    NotifyVpcTransformation<TSource, TKey>
                        .Create(source, keySelector)
                        .Select(
                            elementSelector,
                            (kvp, elsel) => Tuple.Create(elsel(kvp.First), kvp.Second)
                        ),
                comparer,
                returnPath
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector and dynamic element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector and dynamic element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IDictionaryReturnPath<TKey, TElement> returnPath)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function, a comparer, and a dynamic element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        { return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, (IDictionaryReturnPath<TKey, TElement>)null); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified key selector function, a comparer, and a dynamic element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TElement> returnPath)
        {
            if (returnPath == null)
                return null;

            return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); 
        }

        static IObservableDictionary<TKey, TElement> _ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TElement> returnPath)
        {
            if (elementSelector == null)
                return null;

            return _CreateDictionaryTransformation<TKey, TElement>(
                (IVersionedEnumerable<Tuple<TElement, TKey>>)
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
                                                (element, kvp2) =>
                                                    Tuple.Create(element, kvp2.Second)
                                            )
                                        )
                        ),
                comparer,
                returnPath
            );
        }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector and dynamic element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector and dynamic element selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IDictionaryReturnPath<TKey, TElement> returnPath)
        { return ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function, a comparer, and a dynamic element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        { return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, (IDictionaryReturnPath<TKey, TElement>)null); }

        /// <summary>
        /// Creates an <see cref="IObservableDictionary{TKey,TSource}"/> from an sequence according to a specified dynamic key selector function, a comparer, and a dynamic element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">A sequence to create a dictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to index the given element by.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the result element to project the given element into.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to comparer keys.</param>
        /// <param name="returnPath">An <see cref="IDictionaryReturnPath{TKey,TSource}"/> that provides a return path for the dictionary.</param>
        /// <returns>An <see cref="IObservableDictionary{TKey,TSource}"/>, that contains values of type TElement selected from the input sequence, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null </returns>
        public static IObservableDictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TElement> returnPath)
        {
            if (returnPath == null)
                return null;

            return _ToDictionary(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default, returnPath); 
        }

        static IObservableDictionary<TKey, TElement> _ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey, TElement> returnPath)
        {
            if (elementSelector == null)
                return null;

            return 
                _CreateDictionaryTransformation<TKey, TElement>(
                    (IVersionedEnumerable<Tuple<TElement, TKey>>)
                        NotifyVpcTransformation<TSource, TKey>
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
                                                    (element, kvp2) =>
                                                        Tuple.Create(element, kvp2.Second)
                                                )
                                            )
                            ),
                    comparer,
                    returnPath
                );
        }
    }
}
