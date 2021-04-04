using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using Obtics.Values;

namespace Obtics.Collections
{
    /// <summary>
    /// Observable variation of the IDictionary interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    /// <typeparam name="TValue">Type of the values.</typeparam>
    public interface IObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets a live value from this dictionary with a live key.
        /// </summary>
        /// <param name="key">The <see cref="Obtics.Values.IValueProvider{TKey}"/> for the key</param>
        /// <returns>An <see cref="Obtics.Values.IValueProvider{TValue}"/> for the value. If <paramref name="key"/> equals null then null will be returned instead.</returns>
        /// <remarks>
        /// Whenever the collection in the dictionary or the key delivered by <paramref name="key"/> changes the
        /// value delivered by the result provider will be updated.
        /// </remarks>
        IValueProvider<TValue> this[IValueProvider<TKey> key] { get; }

        /// <summary>
        /// Determines live if a certain live key exists in the dictionary.
        /// </summary>
        /// <param name="key">The <see cref="Obtics.Values.IValueProvider{TKey}"/> for the key</param>
        /// <returns>An <see cref="Obtics.Values.IValueProvider{Boolean}"/> of a boolean indicating if the dictionary contains the key. If <paramref name="key"/> equals null then null will be returned instead.</returns>
        /// <remarks>
        /// Whenever the collection in the dictionary or the key delivered by <paramref name="key"/> changes the
        /// value delivered by the result provider will be updated.
        /// </remarks>
        IValueProvider<bool> ContainsKey(IValueProvider<TKey> key);
    }
}
