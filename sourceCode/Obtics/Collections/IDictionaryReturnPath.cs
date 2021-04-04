using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;

namespace Obtics.Collections
{
    /// <summary>
    /// Interface for objects that provide a path for mutations on a dictionary
    /// </summary>
    /// <typeparam name="TKey">Type of the keys of the dictionary</typeparam>
    /// <typeparam name="TValue">Type of the values of the dictionary</typeparam>
    public interface IDictionaryReturnPath<TKey,TValue>
    {
        /// <summary>
        /// Called when adding a key and value pair to the dictionary
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to be added.</param>
        /// <param name="value">The value to be added.</param>
        void Add(IDictionary<TKey, TValue> dictionary, TKey key, TValue value);

        /// <summary>
        /// Called when inserting a key and value pair into the dictionary, possibly replacing a key and value pair with an equal key.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to be added.</param>
        /// <param name="value">The value to be added.</param>
        void Insert(IDictionary<TKey, TValue> dictionary, TKey key, TValue value);

        /// <summary>
        /// Called when a key and value pair needs to be removed from the dictionary when the key is equal to the given <paramref name="key"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to find the key and value pair with.</param>
        /// <returns>True if a key and value pair has actually been removed; false otherwise.</returns>
        bool Remove(IDictionary<TKey, TValue> dictionary, TKey key);

        /// <summary>
        /// Called when a key and value pair needs to be removed from the dictionary when the 
        /// key is equal to the given <paramref name="key"/> and the value is equal to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to find the key and value pair with.</param>
        /// <param name="value">The value to match the found value with.</param>
        /// <returns>True if a key and value pair has actually been removed; false otherwise.</returns>
        bool Remove(IDictionary<TKey, TValue> dictionary, TKey key, TValue value);

        /// <summary>
        /// Called when the entire contents of the dictionary needs to be cleared.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        void Clear(IDictionary<TKey, TValue> dictionary);

        /// <summary>
        /// Gives an IValueProvider of bool that indicates if the dictionary should be regarded as readonly or not.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>An IValueProvider of bool whose Value property is true when the dictionary is readonly and false otherwise.</returns>
        IValueProvider<bool> IsReadOnly(IDictionary<TKey, TValue> dictionary);

        /// <summary>
        /// Indicates wether the given dictionary is synchronised
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>True if the dictionary is synchronized; false otherwise.</returns>
        bool IsSynchronized(IDictionary<TKey, TValue> dictionary);

        /// <summary>
        /// Returns an object that serves as syncroot for the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>An object.</returns>
        object SyncRoot(IDictionary<TKey, TValue> dictionary);
    }
}
