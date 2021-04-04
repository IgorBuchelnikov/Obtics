using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using Obtics.Values;

namespace Obtics.Collections
{
    /// <summary>
    /// An observable variation of the <see cref="System.Linq.IOrderedEnumerable{TElement}"/> interface.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements of the sequence represented by this interface.</typeparam>
    public interface IObservableOrderedEnumerable<TElement> : SL.IOrderedEnumerable<TElement>, IEnumerable<TElement>
    {
        /// <summary>
        /// CreateOrderedEnumerable overload that returns an <see cref="IObservableOrderedEnumerable{TElement}"/>.
        /// It performs a subsequent ordering on the elements of an <see cref="IObservableOrderedEnumerable{TElement}"/>
        ///     according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key produced by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">The <see cref="System.Func{T,TResult}"/> used to extract the key for each element.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> used to compare keys for placement in the returned sequence.</param>
        /// <param name="descending">true to sort the elements in descending order; false to sort the elements in ascending order.</param>
        /// <returns>An <see cref="System.Linq.IOrderedEnumerable{TElement}"/> whose elements are sorted according to a key.</returns>
        /// <remarks>The <see cref="System.Linq.IOrderedEnumerable{TElement}.CreateOrderedEnumerable{TKey}"/> implementation should return an
        /// <see cref="IObservableOrderedEnumerable{TElement}"/> instance up casted to <see cref="System.Linq.IOrderedEnumerable{TElement}"/>.</remarks>
        new IObservableOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);

        /// <summary>
        /// Performs a subsequent ordering on the elements of an <see cref="IObservableOrderedEnumerable{TElement}"/>
        ///     according to a dynamic key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key produced by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">The <see cref="System.Func{T,TResultProvider}"/> used to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property provides the key for the given element.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> used to compare keys for placement in the returned sequence.</param>
        /// <param name="descending">true to sort the elements in descending order; false to sort the elements in ascending order.</param>
        /// <returns>An <see cref="System.Linq.IOrderedEnumerable{TElement}"/> whose elements are sorted according to a key.</returns>
        IObservableOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer, bool descending);
    }
}
