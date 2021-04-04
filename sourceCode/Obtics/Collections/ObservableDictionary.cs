using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections;
using System.Threading;
using Obtics.Collections.Transformations;

namespace Obtics.Collections
{
    /// <summary>
    /// An observable dictionary
    /// </summary>
    /// <typeparam name="TKey">Type of the keys</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    /// <remarks>
    /// The order of elements is important for this dictionary in the sense that it is always preserved, even though
    /// the actual order is arbitrary. To be able to preserve this order the dictionary is emplemented as a binary tree
    /// variation and not as a hash table. 
    /// </remarks>
#if !SILVERLIGHT   
    [Serializable]
#endif
    public sealed class ObservableDictionary<TKey,TValue> 
        : IObservableDictionary<TKey,TValue>
            , IVersionedEnumerable<KeyValuePair<TKey, TValue>>
            , ICollection
            , INotifyCollectionChanged
            , INotifyPropertyChanged
#if !SILVERLIGHT
            , ISerializable
#endif
    {
        class Node : SortedSkiplist<Node, int>.Node
        {
            public TKey _Key;
            public TValue _Value;
        }

        class SkipList : SortedSkiplist<Node, int>
        {
            static bool _MightBeNull = typeof(TKey).IsByRef;
            internal IEqualityComparer<TKey> _Comparer;

            internal int GetHash(TKey key)
            { return _MightBeNull && key == null ? 0 : _Comparer.GetHashCode(key); }

            protected override int SelectKey(ObservableDictionary<TKey, TValue>.Node node)
            { return GetHash(node._Key); }

            protected override int Compare(int first, int second)
            {
                return
                    first < second ? -1 :
                    first > second ? 1 :
                    0
                ;
            }
        }

        SkipList _SkipList;

        /// <summary>
        /// Creates an instance of ObservableDictionary using the given <paramref name="comparer"/> to compare keys.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> to comparer keys with.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="comparer"/> is null.</exception>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _SkipList = new SkipList { _Comparer = comparer };
        }

        /// <summary>
        /// Creates an instance of ObservableDictionary using the default comparer to compare keys.
        /// </summary>
        public ObservableDictionary()
            : this(ObticsEqualityComparer<TKey>.Default)
        {}

        /// <summary>
        /// Creates an instance of ObservableDictionary using the default comparer to compare keys and
        /// copies all key value pairs from the <paramref name="source"/> dictionary into this dictionary.
        /// </summary>
        /// <param name="source">The source <see cref="IDictionary{TKey,TValue}"/> to copy key value pairs from.</param>
        /// <exception cref="ArgumentException">When <paramref name="source"/> contains duplicate keys.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is null.</exception>
        public ObservableDictionary(IDictionary<TKey, TValue> source)
            : this(source, ObticsEqualityComparer<TKey>.Default)
        {}

        /// <summary>
        /// Creates an instance of ObservableDictionary using the given <paramref name="comparer"/> to compare keys and
        /// copies all key value pairs from the <paramref name="source"/> dictionary into this dictionary.
        /// </summary>
        /// <param name="source">The source <see cref="IDictionary{TKey,TValue}"/> to copy key value pairs from.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> to comparer keys with.</param>
        /// <exception cref="ArgumentException">When <paramref name="source"/> contains duplicate keys.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> or <paramref name="comparer"/> is null.</exception>
        public ObservableDictionary(IDictionary<TKey, TValue> source, IEqualityComparer<TKey> comparer)
            : this(comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (var kvp in source)
                this.Add(kvp.Key, kvp.Value);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        ObservableDictionary(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            var comparer = (IEqualityComparer<TKey>)serializationInfo.GetValue("EqualityComparer", typeof(IEqualityComparer<TKey>));
            var items = (KeyValuePair<TKey, TValue>[])serializationInfo.GetValue("Items", typeof(KeyValuePair<TKey, TValue>[]));

            if (items == null)
                throw new SerializationException();

            _SkipList = new SkipList { _Comparer = comparer };

            foreach (var kvp in items)
                this.Add(kvp.Key, kvp.Value);
        }
#endif

        #region IObservableDictionary<TKey,TValue> Members

        IObservableDictionary<TKey, TValue> TransformedDictionary
        { get { return this.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); } }

        /// <summary>
        /// Gets a live value from this dictionary with a live key.
        /// </summary>
        /// <param name="key">The <see cref="Obtics.Values.IValueProvider{TKey}"/> for the key</param>
        /// <returns>An <see cref="Obtics.Values.IValueProvider{TValue}"/> for the value. If <paramref name="key"/> equals null then null will be returned instead.</returns>
        /// <remarks>
        /// Whenever the collection in the dictionary or the key delivered by <paramref name="key"/> changes the
        /// value delivered by the result provider will be updated.
        /// </remarks>
        public Obtics.Values.IValueProvider<TValue> this[Obtics.Values.IValueProvider<TKey> key]
        { get { return TransformedDictionary[key]; } }

        /// <summary>
        /// Determines live if a certain live key exists in the dictionary.
        /// </summary>
        /// <param name="key">The <see cref="Obtics.Values.IValueProvider{TKey}"/> for the key</param>
        /// <returns>An <see cref="Obtics.Values.IValueProvider{Boolean}"/> of a boolean indicating if the dictionary contains the key. If <paramref name="key"/> equals null then null will be returned instead.</returns>
        /// <remarks>
        /// Whenever the collection in the dictionary or the key delivered by <paramref name="key"/> changes the
        /// value delivered by the result provider will be updated.
        /// </remarks>
        public Obtics.Values.IValueProvider<bool> ContainsKey(Obtics.Values.IValueProvider<TKey> key)
        { return TransformedDictionary.ContainsKey(key); }

        #endregion

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds an element with the provided <paramref name="key"/> and <paramref name="value"/> to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists in the dictionary.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public void Add(TKey key, TValue value)
        {
            NotifyCollectionChangedEventArgs msg;

            lock (_SkipList)
            {
                TakeSnapshot();

                if(ContainsKey(key))
                    throw new ArgumentException("An element with the same key already exists in this dictionary.");

                var ix = _SkipList.AddWithIndex(new Node { _Value = value, _Key = key });

                msg = SIOrderedNotifyCollectionChanged.Create(_VersionNumber, NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), ix);
            }

            OnCollectionChanged(msg);
        }

        /// <summary>
        /// Determines whether the dictionary contains an element with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to find the element with.</param>
        /// <returns>true if the dictionary contains an element with the key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(TKey key)
        {
            lock (_SkipList)
            {
                Node dummy;
                return findNode(key, out dummy) != -1;
            }
        }

        /// <summary>
        /// Gets a read only <see cref="ICollection{TKey}"/> containing the keys of the dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        { get { return this.Select( kvp => kvp.Key).ToList() ; } }

        /// <summary>
        /// Removes the element with the specified <paramref name="key"/> from the dictionary.
        /// </summary>
        /// <param name="key">The key to find and remove the element with.</param>
        /// <returns>true if the element is found and removed; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool Remove(TKey key)
        {
            NotifyCollectionChangedEventArgs msg;

            lock (_SkipList)
            {
                TakeSnapshot();

                Node removedNode;
                var ix = findNode(key, out removedNode);

                if (ix >= 0)
                {
                    _SkipList.RemoveAt(ix);
                    msg = SIOrderedNotifyCollectionChanged.Create(_VersionNumber, NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(removedNode._Key, removedNode._Value), ix);
                }
                else
                    msg = null;
            }

            if (msg != null)
            {
                OnCollectionChanged(msg);
                return true;
            }
            else
                return false;
        }

        int findNode(TKey key, out Node node)
        {
            int keyHash = _SkipList.GetHash(key);
            int ix = _SkipList.FindFirst(keyHash, out node);

            if(node != null)
                do
                {
                    if (_SkipList._Comparer.Equals(node._Key, key))
                        return ix;

                    ++ix;
                    node = node._Neighbours[0]._Node;
                }
                while (node != null && _SkipList.GetHash(node._Key) == keyHash);

            return -1;
        }

        /// <summary>
        /// Tries to retrieve a value from the dictionary by key.
        /// </summary>
        /// <param name="key">The key to find the value with.</param>
        /// <param name="value">Out parameter; receives the found value if it can be found; the default value for <typeparamref name="TValue"/> otherwise.</param>
        /// <returns>A boolean value indicating if the value could be found in the dictionary.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_SkipList)
            {
                Node node;

                bool res = findNode(key, out node) != -1;

                value = res ? node._Value : default(TValue);

                return res;
            }
        }

        /// <summary>
        /// Gets the collection of Values in the current dictionary.
        /// </summary>
        public ICollection<TValue> Values
        { get { return this.Select(kvp => kvp.Value).ToList(); } }


        static readonly PropertyChangedEventArgs IndexerPropertyChangedEventArgs = SIList.ItemsIndexerPropertyChangedEventArgs;

        /// <summary>
        /// Gets or sets a single value in the current dictinary. The value is identified with the given key.
        /// </summary>
        /// <param name="key">The key to identify the value with.</param>
        /// <returns>The found value.</returns>
        /// <exception cref="KeyNotFoundException">Raised if no value can be found with the given key.</exception>
        public TValue this[TKey key]
        {
            get
            {
                lock (_SkipList)
                {
                    Node node;

                    if(findNode(key, out node) == -1)
                        throw new KeyNotFoundException("The key does not exist in the collection.");

                    return node._Value;
                }
            }
            set
            {
                NotifyCollectionChangedEventArgs msg;

                lock (_SkipList)
                {
                    TakeSnapshot();

                    Node node;

                    int index;

                    if ((index = findNode(key, out node)) != -1)
                    {
                        msg =
                            SIOrderedNotifyCollectionChanged.Create(
                                _VersionNumber,
                                NotifyCollectionChangedAction.Replace,
                                new KeyValuePair<TKey, TValue>(key, value),
                                new KeyValuePair<TKey, TValue>(key, node._Value),
                                index
                            )
                        ;

                        node._Value = value;
                    }
                    else
                    {
                        index = _SkipList.AddWithIndex(new Node { _Key = key, _Value = value });

                        msg =
                            SIOrderedNotifyCollectionChanged.Create(
                                _VersionNumber,
                                NotifyCollectionChangedAction.Add,
                                new KeyValuePair<TKey, TValue>(key, value),
                                index
                            )
                        ;
                    }
                }

                OnCollectionChanged(msg);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        { Add(item.Key,item.Value); }

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        public void Clear()
        {
            NotifyCollectionChangedEventArgs msg;
            lock (_SkipList)
            {
                TakeSnapshot();

                if (_SkipList.Count > 0)
                {
                    _SkipList.Clear();
                    msg = SIOrderedNotifyCollectionChanged.Create(_VersionNumber, NotifyCollectionChangedAction.Reset);
                }
                else
                    msg = null;
            }

            if (msg != null)
                OnCollectionChanged(msg);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (_SkipList)
            {
                Node node;
                return 
                    findNode(item.Key, out node) != -1 
                    && ObticsEqualityComparer<TValue>.Default.Equals(node._Value, item.Value);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_SkipList)
            {
                foreach (var kvp in this)
                    array[arrayIndex++] = kvp;
            }
        }

        /// <summary>
        /// Gets the total number of entries in the dictionary.
        /// </summary>
        public int Count
        {
            get 
            {
                lock (_SkipList)
                    return _SkipList.Count; 
            }
        }

        /// <summary>
        /// Gets a boolean value indicating if the dictionary is read only.
        /// </summary>
        /// <remarks>Always returns false</remarks>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// removes the given entry from the dictionary if present.
        /// </summary>
        /// <param name="item">The entry to remove.</param>
        /// <returns>A boolean value indicating if the entry could be found and removed; false otherwise.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (_SkipList)
                return ((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item) && Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region INotifyCollectionChanged Members

        void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var cc = CollectionChanged;

            if (cc != null)
                cc(this, args);

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add :
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    OnPropertyChanged(CountPropertyChangedEventArgs);
                    goto case NotifyCollectionChangedAction.Replace;
                case NotifyCollectionChangedAction.Replace:
#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
#endif
                    OnPropertyChanged(IndexerPropertyChangedEventArgs);
                    break;
            }
        }

        /// <summary>
        /// Raises events whenever the contents of the dictionary change.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var pc = PropertyChanged;

            if (pc != null)
                pc(this, args);
        }

        /// <summary>
        /// Raises events whenever a property of the dictionary changes value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

#if !SILVERLIGHT
        #region ISerializable Members

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            lock (_SkipList)
            {
                var comparer = _SkipList._Comparer;
                info.AddValue("EqualityComparer", object.ReferenceEquals(comparer, ObticsEqualityComparer<TKey>.Default) ? null : comparer);
                info.AddValue("Items", this.ToArray());
            }
        }

        #endregion
#endif

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            lock (_SkipList)
                foreach (var node in _SkipList)
                    array.SetValue(new KeyValuePair<TKey,TValue>(node._Key,node._Value) ,index++);
        }

        static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = SICollection.CountPropertyChangedEventArgs ;

        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

        object ICollection.SyncRoot
        {
            get { return _SkipList; }
        }

        #endregion

        #region IVersionedEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerable<KeyValuePair<TKey, TValue>> BuildEnumerable()
        { return System.Linq.Enumerable.Select(_SkipList, n => new KeyValuePair<TKey, TValue>(n._Key, n._Value)); }

        /// <summary>
        /// GetEnumerator override that returns an <see cref="IVersionedEnumerator"/> of <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        /// <returns>An <see cref="IVersionedEnumerator"/> of <see cref="KeyValuePair{TKey,TValue}"/> for this dictionary.</returns>
        public IVersionedEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_SkipList)
                return XLazySnapshotEnumerator.Create<KeyValuePair<TKey, TValue>>(this, _VersionNumber, BuildEnumerable().GetEnumerator());
        }

        /// <summary>
        /// When the contents are being updated this method should be called.
        /// </summary>
        void TakeSnapshot()
        {
            //if an enumerator is still active, copy the old contents and continue enumerating from there.
            XLazySnapshotEnumerator.TakeSnapshot(this, _VersionNumber, () => System.Linq.Enumerable.ToArray(BuildEnumerable()));

            //Advance the sequence number.
            _VersionNumber = _VersionNumber.Next;
        }

        #endregion

        #region IVersionedEnumerable Members

        VersionNumber _VersionNumber;

        /// <summary>
        /// The version of the current content of the dictionary.
        /// </summary>
        /// <remarks>
        /// In a multithreaded environment the returned ContentVersion may be outdated the moment it is returned.
        /// </remarks>
        public VersionNumber ContentVersion
        {
            get 
            { 
                lock (_SkipList)
                    return _VersionNumber;
            }
        }

        /// <summary>
        /// GetEnumerator override that returns an <see cref="IVersionedEnumerator"/>.
        /// </summary>
        /// <returns>An <see cref="IVersionedEnumerator"/> for this dictionary.</returns>
        IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion
    }
}
