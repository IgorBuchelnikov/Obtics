using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using Obtics.Values;

namespace Obtics.Collections
{
    /// <summary>
    /// Observable variation of the ILookup interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    /// <typeparam name="TElement">Type of the elements.</typeparam>
    public interface IObservableLookup<TKey, TElement> : SL.ILookup<TKey, TElement>, IEnumerable<SL.IGrouping<TKey, TElement>>
    {
        /// <summary>
        /// Observabley and reactively gets a sequence of elements from this dictionary with a key.
        /// </summary>
        /// <param name="key">The provider of the key</param>
        /// <returns>An observable sequence of elements. If <paramref name="key"/> equal null then null will be returned instead.</returns>
        /// <remarks>
        /// Whenever the collection in the lookup or the key delivered by <paramref name="key"/> changes the
        /// the result sequence will be updated.
        /// </remarks>
        IEnumerable<TElement> this[IValueProvider<TKey> key] { get; }

        /// <summary>
        /// Observably and reactively determines if a certain key exists in the lookup.
        /// </summary>
        /// <param name="key">The provider of the key.</param>
        /// <returns>A provider of a boolean indicating if the lookup contains the key. If <paramref name="key"/> equal null then null will be returned instead.</returns>
        /// <remarks>
        /// Whenever the collection in the lookup or the key delivered by <paramref name="key"/> changes the
        /// value delivered by the result provider will be updated.
        /// </remarks>
        IValueProvider<bool> Contains(IValueProvider<TKey> key);
    }
}
