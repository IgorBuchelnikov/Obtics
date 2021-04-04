using System;
using System.Collections.Generic;
using Obtics.Values;
using System.Linq;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IObservableOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IObservableOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        { return ThenBy(source,keySelector, Comparer<TKey>.Default); }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order by using a given comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IObservableOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IComparer{TKey}"/> used to compare key values.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IObservableOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
                return null;

            return source.CreateOrderedEnumerable(keySelector, comparer, false); 
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IObservableOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to order the given element by.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IObservableOrderedEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector)
        { return ThenBy(source,keySelector, Comparer<TKey>.Default); }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order dynamicaly by using a given comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IObservableOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IComparer{TKey}"/> used to compare key values. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to order the given element by.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IObservableOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IObservableOrderedEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
                return null;

            return source.CreateOrderedEnumerable(keySelector, comparer, false); 
        }

        // These methods are created for compatibility with System.Linq.Enumerable
        // if the specified source is not an IObservableOrderedEnumerable as generated by 
        // the ObservableEnumerable methods then the result of these methods are not observable and
        // the returned values of the below methods are not guaranteed equal when created with equal 
        // arguments.        

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        /// <remarks>The result of this method will only be reactive and observable if source is in fact an <see cref="IObservableOrderedEnumerable{TSource}"/>.</remarks>
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        { return ThenBy(source, keySelector, Comparer<TKey>.Default); }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order by using a given comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IObservableOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IComparer{TKey}"/> used to compare key values.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        /// <remarks>The result of this method will only be reactive and observable if source is in fact an <see cref="IObservableOrderedEnumerable{TSource}"/>.</remarks>
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null || keySelector == null || comparer == null)
                return null;

            var asObservable = source as IObservableOrderedEnumerable<TSource>;

            return asObservable != null ? asObservable.CreateOrderedEnumerable(keySelector, comparer, false) : source.CreateOrderedEnumerable(keySelector, comparer, false);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to order the given element by.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        /// <remarks>The result of this method will only be reactive and observable if source is in fact an <see cref="IObservableOrderedEnumerable{TSource}"/>.</remarks>
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector)
        { return ThenBy(source, keySelector, Comparer<TKey>.Default); }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order dynamicaly by using a given comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">An <see cref="IObservableOrderedEnumerable{TSource}"/> that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to order the given element by.</param>
        /// <param name="comparer">An <see cref="IComparer{TKey}"/> used to compare key values.</param>
        /// <returns>An <see cref="IObservableOrderedEnumerable{TSource}"/>, whose elements are sorted according to a key, or null when either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        /// <remarks>The result of this method will only be reactive and observable if source is in fact an <see cref="IObservableOrderedEnumerable{TSource}"/>.</remarks>
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer)
        {
            if (source == null || keySelector == null || comparer == null)
                return null;

            var asObservable = source as IObservableOrderedEnumerable<TSource>;

            return asObservable != null ? asObservable.CreateOrderedEnumerable(keySelector, comparer, false) : source.CreateOrderedEnumerable( FuncExtender<TSource>.Extend(keySelector, (i,ks) => ks(i).Value), comparer, false);
        }    
    }
}
