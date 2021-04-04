using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Threading;
using Obtics.Values;

namespace Obtics.Collections
{
    struct DictionaryReturnPathFuncStruct<TBase, TKey, TValue>
    {
        public Action<TBase, TKey, TValue> _Add;
        public Action<TBase, TKey, TValue> _Insert;
        public Func<TBase, TKey, bool> _Remove;
        public Func<TBase, TKey, TValue, bool> _Remove2;
        public Action<TBase> _Clear;
        public Func<TBase, IValueProvider<bool>> _IsReadOnly;
        public Func<TBase, bool> _IsSynchronized;
        public Func<TBase, object> _SyncRoot;

        public override int GetHashCode()
        {
            return
                Hasher.CreateFromRef(this.GetType())
                    .AddRef(_Add)
                    .AddRef(_Insert)
                    .AddRef(_Remove)
                    .AddRef(_Remove2)
                    .AddRef(_Clear)
                    .AddRef(_IsReadOnly)
                    .AddRef(_IsSynchronized)
                    .AddRef(_SyncRoot);
        }

        internal bool Equals(ref DictionaryReturnPathFuncStruct<TBase, TKey, TValue> other)
        {
            return
                ObticsEqualityComparer<Action<TBase, TKey, TValue>>.Default.Equals(_Add, other._Add)
                && ObticsEqualityComparer<Action<TBase, TKey, TValue>>.Default.Equals(_Insert, other._Insert)
                && ObticsEqualityComparer<Func<TBase, TKey, bool>>.Default.Equals(_Remove, other._Remove)
                && ObticsEqualityComparer<Func<TBase, TKey, TValue, bool>>.Default.Equals(_Remove2, other._Remove2)
                && ObticsEqualityComparer<Action<TBase>>.Default.Equals(_Clear, other._Clear)
                && ObticsEqualityComparer<Func<TBase, IValueProvider<bool>>>.Default.Equals(_IsReadOnly, other._IsReadOnly)
                && ObticsEqualityComparer<Func<TBase, bool>>.Default.Equals(_IsSynchronized, other._IsSynchronized)
                && ObticsEqualityComparer<Func<TBase, object>>.Default.Equals(_SyncRoot, other._SyncRoot)
            ;
        }
    }

    public class DictionaryReturnPathBase<TBase, TKey, TValue>
    {
        DictionaryReturnPathFuncStruct<TBase, TKey, TValue> _Funcs;

        internal DictionaryReturnPathBase(DictionaryReturnPathFuncStruct<TBase, TKey, TValue> funcs)
        { _Funcs = funcs; }

        internal int GetElementsHashCode()
        { return _Funcs.GetHashCode(); }

        internal bool ElementsEqual(DictionaryReturnPathBase<TBase, TKey, TValue> other)
        { return _Funcs.Equals(ref other._Funcs); }

        #region IDictionaryReturnPath<TKey,TValue> Members

        /// <summary>
        /// To add a give value with a given key to the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Add method.</param>
        /// <param name="key">The key to add the value with.</param>
        /// <param name="value">The value to add.</param>
        /// <remarks>
        /// This method can be represented by the given 'add' delegate or constructed with a given 'insert' delegate.
        /// If neither an 'add' or 'insert' delegate are given then this method will remain 'NotSupported'
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither an 'add' delegate nor an 'insert' delegate have been supplied.</exception>
        /// <exception cref="InvalidOperationException">When an implementation has been constructed using an 'insert' delegate; this exception will be reised
        /// when attempting to add a duplicate key.</exception>
        public void Add(IDictionary<TKey, TValue> dictionary, TBase b, TKey key, TValue value)
        {
            if (_Funcs._Add != null)
                _Funcs._Add(b, key, value);
            else if (_Funcs._Insert != null)
            {
                var syncRoot = SyncRoot(dictionary, b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    if (!dictionary.ContainsKey(key))
                        _Funcs._Insert(b, key, value);
                    else
                        throw new InvalidOperationException("The dictionary already contains a value with the given key.");
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// To insert a value into the dictionary under a given key, potentialy replacing an existing entry under the given key.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Insert method.</param>
        /// <param name="key">The key to insert the value with.</param>
        /// <param name="value">The value to insert.</param>
        /// <remarks>
        /// This method can be represented by the given 'insert' delegate or constructed with a given 'add' delegate combined
        /// with either a 'remove' or 'removeWithValueCheck' delegate.
        /// If neither an 'insert' delegate or an 'add' and 'remove' or 'removeWithValueCheck' delegate combination are given 
        /// then this method will remain 'NotSupported'
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither an 'insert' delegate or an 'add' and 'remove' or 'removeWithValueCheck' delegate combination have been supplied.</exception>
        public void Insert(IDictionary<TKey, TValue> dictionary, TBase b, TKey key, TValue value)
        {
            if (_Funcs._Insert != null)
                _Funcs._Insert(b, key, value);
            else if (_Funcs._Add != null && (_Funcs._Remove != null || _Funcs._Remove2 != null))
            {
                var syncRoot = SyncRoot(dictionary,b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    if (_Funcs._Remove != null)
                        _Funcs._Remove(b, key);
                    else
                    {
                        TValue oldValue;
                        if (dictionary.TryGetValue(key, out oldValue))
                            _Funcs._Remove2(b, key, oldValue);
                    }

                    _Funcs._Add(b, key, value);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// To remove an entry under the given key from the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Remove method.</param>
        /// <param name="key">The keys whose entry is to be removed from the dictionary.</param>
        /// <returns>A boolean value the will be true if an entry has been successfuly removed from the dictionary.</returns>
        /// <remarks>
        /// This method can be represented by the given 'remove' delegate or constructed using a 'removeWithValueCheck' delegate.
        /// If neither a 'remove' or 'removeWithValueCheck' delegate have been given then the method will remain 'NotSupported'.
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither a 'remove' or 'removeWithValueCheck' delegate has been supplied.</exception>
        public bool Remove(IDictionary<TKey, TValue> dictionary, TBase b, TKey key)
        {
            if (_Funcs._Remove != null)
                return _Funcs._Remove(b, key);
            else if (_Funcs._Remove2 != null)
            {
                var syncRoot = SyncRoot(dictionary,b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    TValue value;

                    return dictionary.TryGetValue(key, out value) && _Funcs._Remove2(b, key, value);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// To remove an entry under a given key from the dictionary, but only if this entry contains the given value.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Remove method.</param>
        /// <param name="key">The keys whose entry is to be removed from the dictionary.</param>
        /// <param name="value">The value that must be contained in the found entry for it to be removed.</param>
        /// <returns>A boolean value the will be true if an entry has been successfuly removed from the dictionary.</returns>
        /// <remarks>
        /// This method can be represented by the given 'removeWithValueCheck' delegate or constructed using a 'remove' delegate.
        /// If neither a 'removeWithValueCheck' or 'remove' delegate have been given then the method will remain 'NotSupported'.
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither a 'removeWithValueCheck' or 'remove' delegate has been supplied.</exception>
        public bool Remove(IDictionary<TKey, TValue> dictionary, TBase b, TKey key, TValue value)
        {
            if (_Funcs._Remove2 != null)
                return _Funcs._Remove2(b, key, value);
            else if (_Funcs._Remove != null)
            {
                var syncRoot = SyncRoot(dictionary,b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    TValue currentValue;

                    return
                        dictionary.TryGetValue(key, out currentValue)
                        && ObticsEqualityComparer<TValue>.Default.Equals(currentValue, value)
                        && _Funcs._Remove(b, key);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// To remove all entries from the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Clear method.</param>
        /// <remarks>
        /// This method can be represented by the given 'clear' delegate or constructed using a 'remove' or 'removeWithValueCheck' delegate.
        /// If neither a 'clear, 'removeWithValueCheck' or 'remove' delegate have been given then the method will remain 'NotSupported'.
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither a 'clear', 'removeWithValueCheck' or 'remove' delegate has been supplied.</exception>
        public void Clear(IDictionary<TKey, TValue> dictionary, TBase b)
        {
            if (_Funcs._Clear != null)
                _Funcs._Clear(b);
            else if (_Funcs._Remove != null || _Funcs._Remove2 != null)
            {
                var syncRoot = SyncRoot(dictionary,b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    foreach (var kvp in SL.Enumerable.ToArray(dictionary))
                        if (_Funcs._Remove != null)
                            _Funcs._Remove(b, kvp.Key);
                        else
                            _Funcs._Remove2(b, kvp.Key, kvp.Value);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// To indicate if the dictionary is read only.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the IsReadOnly property.</param>
        /// <returns>A boolean value which is true if the dictionary is to be regarded as read only and false otherwise.</returns>
        /// <remarks>This method can be represented by the given 'isReadOnly' delegate. If no 'isReadOnly' delegate has been given then
        /// this method will always return false.
        /// 
        /// The value of this 'property' will not be automatically checked by any of the other methods.
        /// </remarks>
        public IValueProvider<bool> IsReadOnly(IDictionary<TKey, TValue> dictionary, TBase b)
        { return _Funcs._IsReadOnly != null ? _Funcs._IsReadOnly(b) : null; }

        /// <summary>
        /// To indicate if the dictionary is synchronized.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the IsSynchronized property.</param>
        /// <returns>A boolean value which is true if the dictionary is to be regarded as synchronized and false otherwise.</returns>
        /// <remarks> This method can be represented by the given 'isSynchronized' delegate or constructed using a given 'syncRoot' delegate.
        /// If neither an 'isSynchronized' or 'syncRoot' delegate has been given then the result will always be 'false'.
        /// </remarks>
        public bool IsSynchronized(IDictionary<TKey, TValue> dictionary, TBase b)
        {
            return
                _Funcs._IsSynchronized != null ? _Funcs._IsSynchronized(b) :
                _Funcs._SyncRoot != null ? _Funcs._SyncRoot(b) != null :
                false
            ;
        }

        /// <summary>
        /// To give the synchronization root for the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the SyncRoot property.</param>
        /// <returns>An object that is the sync root or null of no such root exists.</returns>
        /// <remarks>This method can be represented by the given 'syncRoot' delegate. If no 'syncRoot' delegate has been
        /// given then this method will always return null.</remarks>
        public object SyncRoot(IDictionary<TKey, TValue> dictionary, TBase b)
        { return _Funcs._SyncRoot == null ? null : _Funcs._SyncRoot(b); }

        #endregion

    }

    /// <summary>
    /// Helper class that creates an <see cref="IDictionaryReturnPath{TKey,TValue}"/> implementation based on delegates.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys of the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values of the dictionary.</typeparam>
    public class DictionaryReturnPath<TKey,TValue> : DictionaryReturnPathBase<IDictionary<TKey,TValue>,TKey,TValue>, IDictionaryReturnPath<TKey,TValue>, IEquatable<DictionaryReturnPath<TKey,TValue>>
    {
        internal DictionaryReturnPath(DictionaryReturnPathFuncStruct<IDictionary<TKey, TValue>, TKey, TValue> funcs)
            : base(funcs)
        {
            _HashCode =
                Hasher.CreateFromRef(this.GetType()).AddValue(base.GetElementsHashCode());
        }

        readonly int _HashCode;

        public override bool Equals(object obj)
        {
            return
                Equals(obj as DictionaryReturnPath<TKey, TValue>);
        }

        public override int GetHashCode()
        { return _HashCode; }

        #region IEquatable<DictionaryReturnPath<TKey,TValue>> Members

        public bool Equals(DictionaryReturnPath<TKey, TValue> other)
        {
            return
                object.ReferenceEquals(this, other)
                ||
                    other != null
                    && base.ElementsEqual(other);
        }

        #endregion


        #region IDictionaryReturnPath<TKey,TValue> Members

        /// <summary>
        /// To add a give value with a given key to the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Add method.</param>
        /// <param name="key">The key to add the value with.</param>
        /// <param name="value">The value to add.</param>
        /// <remarks>
        /// This method can be represented by the given 'add' delegate or constructed with a given 'insert' delegate.
        /// If neither an 'add' or 'insert' delegate are given then this method will remain 'NotSupported'
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither an 'add' delegate nor an 'insert' delegate have been supplied.</exception>
        /// <exception cref="InvalidOperationException">When an implementation has been constructed using an 'insert' delegate; this exception will be reised
        /// when attempting to add a duplicate key.</exception>
        public void Add(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        { base.Add(dictionary, dictionary, key, value ); }

        /// <summary>
        /// To insert a value into the dictionary under a given key, potentialy replacing an existing entry under the given key.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Insert method.</param>
        /// <param name="key">The key to insert the value with.</param>
        /// <param name="value">The value to insert.</param>
        /// <remarks>
        /// This method can be represented by the given 'insert' delegate or constructed with a given 'add' delegate combined
        /// with either a 'remove' or 'removeWithValueCheck' delegate.
        /// If neither an 'insert' delegate or an 'add' and 'remove' or 'removeWithValueCheck' delegate combination are given 
        /// then this method will remain 'NotSupported'
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither an 'insert' delegate or an 'add' and 'remove' or 'removeWithValueCheck' delegate combination have been supplied.</exception>
        public void Insert(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        { base.Insert(dictionary, dictionary, key, value); }

        /// <summary>
        /// To remove an entry under the given key from the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Remove method.</param>
        /// <param name="key">The keys whose entry is to be removed from the dictionary.</param>
        /// <returns>A boolean value the will be true if an entry has been successfuly removed from the dictionary.</returns>
        /// <remarks>
        /// This method can be represented by the given 'remove' delegate or constructed using a 'removeWithValueCheck' delegate.
        /// If neither a 'remove' or 'removeWithValueCheck' delegate have been given then the method will remain 'NotSupported'.
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither a 'remove' or 'removeWithValueCheck' delegate has been supplied.</exception>
        public bool Remove(IDictionary<TKey, TValue> dictionary, TKey key)
        { return base.Remove(dictionary, dictionary, key); }

        /// <summary>
        /// To remove an entry under a given key from the dictionary, but only if this entry contains the given value.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Remove method.</param>
        /// <param name="key">The keys whose entry is to be removed from the dictionary.</param>
        /// <param name="value">The value that must be contained in the found entry for it to be removed.</param>
        /// <returns>A boolean value the will be true if an entry has been successfuly removed from the dictionary.</returns>
        /// <remarks>
        /// This method can be represented by the given 'removeWithValueCheck' delegate or constructed using a 'remove' delegate.
        /// If neither a 'removeWithValueCheck' or 'remove' delegate have been given then the method will remain 'NotSupported'.
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither a 'removeWithValueCheck' or 'remove' delegate has been supplied.</exception>
        public bool Remove(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        { return base.Remove(dictionary, dictionary, key, value); }

        /// <summary>
        /// To remove all entries from the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the Clear method.</param>
        /// <remarks>
        /// This method can be represented by the given 'clear' delegate or constructed using a 'remove' or 'removeWithValueCheck' delegate.
        /// If neither a 'clear, 'removeWithValueCheck' or 'remove' delegate have been given then the method will remain 'NotSupported'.
        /// </remarks>
        /// <exception cref="NotSupportedException">Raised if neither a 'clear', 'removeWithValueCheck' or 'remove' delegate has been supplied.</exception>
        public void Clear(IDictionary<TKey, TValue> dictionary)
        { base.Clear(dictionary, dictionary); }

        /// <summary>
        /// To indicate if the dictionary is read only.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the IsReadOnly property.</param>
        /// <returns>A boolean value which is true if the dictionary is to be regarded as read only and false otherwise.</returns>
        /// <remarks>This method can be represented by the given 'isReadOnly' delegate. If no 'isReadOnly' delegate has been given then
        /// this method will always return false.
        /// 
        /// The value of this 'property' will not be automatically checked by any of the other methods.
        /// </remarks>
        public IValueProvider<bool> IsReadOnly(IDictionary<TKey, TValue> dictionary)
        { return base.IsReadOnly(dictionary, dictionary); }

        /// <summary>
        /// To indicate if the dictionary is synchronized.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the IsSynchronized property.</param>
        /// <returns>A boolean value which is true if the dictionary is to be regarded as synchronized and false otherwise.</returns>
        /// <remarks> This method can be represented by the given 'isSynchronized' delegate or constructed using a given 'syncRoot' delegate.
        /// If neither an 'isSynchronized' or 'syncRoot' delegate has been given then the result will always be 'false'.
        /// </remarks>
        public bool IsSynchronized(IDictionary<TKey, TValue> dictionary)
        { return base.IsSynchronized( dictionary, dictionary ); }

        /// <summary>
        /// To give the synchronization root for the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary on which this method implements the SyncRoot property.</param>
        /// <returns>An object that is the sync root or null of no such root exists.</returns>
        /// <remarks>This method can be represented by the given 'syncRoot' delegate. If no 'syncRoot' delegate has been
        /// given then this method will always return null.</remarks>
        public object SyncRoot(IDictionary<TKey, TValue> dictionary)
        { return base.SyncRoot(dictionary, dictionary); }

        #endregion
    }

    /// <summary>
    /// Helper class to conveniently create <see cref="DictionaryReturnPath{TKey,TValue}"/> objects with diffent combinations of return path methods.
    /// </summary>
    /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
    /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
    /// <remarks>
    /// The <see cref="ObservableEnumerable"/> ToDictionary method can be passed an <see cref="IDictionaryReturnPath{TKey,TValue}"/> object that tells
    /// the resulting <see cref="IDictionary{TKey,TValue}"/> how to treat various methods that would manipulate the collection.
    /// 
    /// Instead of creating a class that implements <see cref="IDictionaryReturnPath{TKey,TValue}"/>, this factory class can be used
    /// to contruct such a class based on different delegates.
    /// 
    /// Not all methods need to have an implementation. Any combination can be used.
    /// </remarks>
    public class DictionaryReturnPathFactory<TKey, TValue>
    {
        Action<IDictionary<TKey, TValue>, TKey, TValue> _Add;
        public Action<IDictionary<TKey, TValue>, TKey, TValue> Add 
        { get { return _Add; } set { _Add = value; _Cache = null; } }

        Action<IDictionary<TKey, TValue>, TKey, TValue> _Insert;
        public Action<IDictionary<TKey, TValue>, TKey, TValue> Insert
        { get { return _Insert; } set { _Insert = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, TKey, bool> _Remove;
        public Func<IDictionary<TKey, TValue>, TKey, bool> Remove
        { get { return _Remove; } set { _Remove = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, TKey, TValue, bool> _RemoveWithValueCheck;
        public Func<IDictionary<TKey, TValue>, TKey, TValue, bool> RemoveWithValueCheck
        { get { return _RemoveWithValueCheck; } set { _RemoveWithValueCheck = value; _Cache = null; } }

        Action<IDictionary<TKey, TValue>> _Clear;
        public Action<IDictionary<TKey, TValue>> Clear
        { get { return _Clear; } set { _Clear = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, IValueProvider<bool>> _IsReadOnly;
        public Func<IDictionary<TKey, TValue>, IValueProvider<bool>> IsReadOnly
        { get { return _IsReadOnly; } set { _IsReadOnly = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, bool> _IsSynchronized;
        public Func<IDictionary<TKey, TValue>, bool> IsSynchronized
        { get { return _IsSynchronized; } set { _IsSynchronized = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, object> _SyncRoot;
        public Func<IDictionary<TKey, TValue>, object> SyncRoot
        { get { return _SyncRoot; } set { _SyncRoot = value; _Cache = null; } }

        DictionaryReturnPath<TKey, TValue> _Cache;

        /// <summary>
        /// Creates a new <see cref="DictionaryReturnPath{TKey,TValue}"/> object with the supplied delegates.
        /// </summary>
        /// <returns>A newly created <see cref="DictionaryReturnPath{TKey,TValue}"/>.</returns>
        public DictionaryReturnPath<TKey, TValue> CreateReturnPath()
        {
            return
                _Cache ??
                (
                    _Cache = new DictionaryReturnPath<TKey,TValue>(
                        new DictionaryReturnPathFuncStruct<IDictionary<TKey, TValue>, TKey, TValue>
                        {
                            _Add = Add,
                            _Insert = Insert,
                            _Remove = Remove,
                            _Remove2 = RemoveWithValueCheck,
                            _Clear = Clear,
                            _IsReadOnly = IsReadOnly,
                            _IsSynchronized = IsSynchronized,
                            _SyncRoot = SyncRoot
                        }
                    )
                )
            ;
        }

        /// <summary>
        /// Implicitly converts the given <see cref="DictionaryReturnPathFactory{TKey,TValue}"/> to a new <see cref="DictionaryReturnPath{TKey,TValue}"/> object.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>A newly created <see cref="DictionaryReturnPath{TKey,TValue}"/>.</returns>
        public static implicit operator DictionaryReturnPath<TKey, TValue>(DictionaryReturnPathFactory<TKey, TValue> t)
        { return t.CreateReturnPath(); }
    }

    public class ToDictionaryFactory<TKey, TValue>
    {
        public ToDictionaryFactory(Func<IDictionaryReturnPath<TKey, TValue>, IObservableDictionary<TKey, TValue>> creator)
        {
            _Creator = creator;
            _ReturnPathFactory = new DictionaryReturnPathFactory<TKey, TValue>();
        }

        Func<IDictionaryReturnPath<TKey, TValue>, IObservableDictionary<TKey, TValue>> _Creator;
        DictionaryReturnPathFactory<TKey, TValue> _ReturnPathFactory;

        public IObservableDictionary<TKey, TValue> Create()
        { return _Creator(_ReturnPathFactory.CreateReturnPath()); }

        public ToDictionaryFactory<TKey, TValue> Add(Action<IDictionary<TKey, TValue>, TKey, TValue> add)
        {
            _ReturnPathFactory.Add = add;
            return this;
        }

        public ToDictionaryFactory<TKey, TValue> Insert(Action<IDictionary<TKey, TValue>, TKey, TValue> insert)
        {
            _ReturnPathFactory.Insert = insert;
            return this;
        }

        public ToDictionaryFactory<TKey, TValue> Remove(Func<IDictionary<TKey, TValue>, TKey, bool> remove)
        {
            _ReturnPathFactory.Remove = remove;
            return this;
        }

        public ToDictionaryFactory<TKey, TValue> RemoveWithValueCheck(Func<IDictionary<TKey, TValue>, TKey, TValue, bool> removeWithValueCheck)
        {
            _ReturnPathFactory.RemoveWithValueCheck = removeWithValueCheck;
            return this;
        }

        public ToDictionaryFactory<TKey, TValue> Clear(Action<IDictionary<TKey, TValue>> clear)
        {
            _ReturnPathFactory.Clear = clear;
            return this;
        }

        public ToDictionaryFactory<TKey, TValue> IsReadOnly(Func<IDictionary<TKey, TValue>, IValueProvider<bool>> isReadOnly)
        {
            _ReturnPathFactory.IsReadOnly = isReadOnly;
            return this;
        }

        public ToDictionaryFactory<TKey, TValue> IsSynchronized(Func<IDictionary<TKey, TValue>, bool> isSynchronized)
        {
            _ReturnPathFactory.IsSynchronized = isSynchronized;
            return this;
        }

        public ToDictionaryFactory<TKey, TValue> SyncRoot(Func<IDictionary<TKey, TValue>, object> syncRoot)
        {
            _ReturnPathFactory.SyncRoot = syncRoot;
            return this;
        }
    }

    public class DictionaryReturnPath<TParam, TKey, TValue> : IDictionaryReturnPath<TKey, TValue>, IEquatable<DictionaryReturnPath<TParam, TKey, TValue>>
    {
        readonly TParam _Parameter;
        readonly DictionaryReturnPathBase<Tuple<IDictionary<TKey, TValue>, TParam>, TKey, TValue> _Base;
        readonly int _HashCode;

        internal DictionaryReturnPath(DictionaryReturnPathBase<Tuple<IDictionary<TKey,TValue>,TParam>, TKey, TValue> b, TParam param)
        {
            _Parameter = param;
            _Base = b;
            _HashCode = Hasher.CreateFromRef(this.GetType()).AddValue(_Base.GetElementsHashCode()).Add(_Parameter);
        }

        public override bool Equals(object obj)
        { return Equals(obj as DictionaryReturnPath<TParam,TKey,TValue>); }

        public override int GetHashCode()
        { return _HashCode; }

        #region IEquatable<DictionaryReturnPath<TParameter,TKey,TValue>> Members

        public bool Equals(DictionaryReturnPath<TParam, TKey, TValue> other)
        {
            return
                object.ReferenceEquals(this, other)
                ||
                    other != null
                    && ObticsEqualityComparer<TParam>.Equals(_Parameter, other._Parameter)
                    && _Base.ElementsEqual(other._Base);
        }

        #endregion

        #region IDictionaryReturnPath<TKey,TValue> Members

        Tuple<IDictionary<TKey, TValue>, TParam> DP(IDictionary<TKey, TValue> dictionary)
        { return Tuple.Create(dictionary, _Parameter); }

        public void Add(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        { _Base.Add(dictionary, DP(dictionary), key, value ); }

        public void Insert(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        { _Base.Insert(dictionary, DP(dictionary), key, value); }

        public bool Remove(IDictionary<TKey, TValue> dictionary, TKey key)
        { return _Base.Remove(dictionary, DP(dictionary), key); }

        public bool Remove(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        { return _Base.Remove(dictionary, DP(dictionary), key, value ); }

        public void Clear(IDictionary<TKey, TValue> dictionary)
        { _Base.Clear(dictionary, DP(dictionary)); }

        public IValueProvider<bool> IsReadOnly(IDictionary<TKey, TValue> dictionary)
        { return _Base.IsReadOnly(dictionary, DP(dictionary)); }

        public bool IsSynchronized(IDictionary<TKey, TValue> dictionary)
        { return _Base.IsSynchronized(dictionary, DP(dictionary)); }

        public object SyncRoot(IDictionary<TKey, TValue> dictionary)
        { return _Base.SyncRoot(dictionary, DP(dictionary)); }

        #endregion
    }

    /// <summary>
    /// Helper class to conveniently create <see cref="DictionaryReturnPath{TKey,TValue}"/> objects with diffent combinations of return path methods.
    /// </summary>
    /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
    /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
    /// <remarks>
    /// The <see cref="ObservableEnumerable"/> ToDictionary method can be passed an <see cref="IDictionaryReturnPath{TKey,TValue}"/> object that tells
    /// the resulting <see cref="IDictionary{TKey,TValue}"/> how to treat various methods that would manipulate the collection.
    /// 
    /// Instead of creating a class that implements <see cref="IDictionaryReturnPath{TKey,TValue}"/>, this factory class can be used
    /// to contruct such a class based on different delegates.
    /// 
    /// Not all methods need to have an implementation. Any combination can be used.
    /// </remarks>
    public class DictionaryReturnPathFactory<TParam, TKey, TValue>
    {
        Action<IDictionary<TKey, TValue>, TParam, TKey, TValue> _Add;
        public Action<IDictionary<TKey, TValue>, TParam, TKey, TValue> Add
        { get { return _Add; } set { _Add = value; _Cache = null; } }

        Action<IDictionary<TKey, TValue>, TParam, TKey, TValue> _Insert;
        public Action<IDictionary<TKey, TValue>, TParam, TKey, TValue> Insert
        { get { return _Insert; } set { _Insert = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, TParam, TKey, bool> _Remove;
        public Func<IDictionary<TKey, TValue>, TParam, TKey, bool> Remove
        { get { return _Remove; } set { _Remove = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, TParam, TKey, TValue, bool> _RemoveWithValueCheck;
        public Func<IDictionary<TKey, TValue>, TParam, TKey, TValue, bool> RemoveWithValueCheck
        { get { return _RemoveWithValueCheck; } set { _RemoveWithValueCheck = value; _Cache = null; } }

        Action<IDictionary<TKey, TValue>, TParam> _Clear;
        public Action<IDictionary<TKey, TValue>, TParam> Clear
        { get { return _Clear; } set { _Clear = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, TParam, IValueProvider<bool>> _IsReadOnly;
        public Func<IDictionary<TKey, TValue>, TParam, IValueProvider<bool>> IsReadOnly
        { get { return _IsReadOnly; } set { _IsReadOnly = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, TParam, bool> _IsSynchronized;
        public Func<IDictionary<TKey, TValue>, TParam, bool> IsSynchronized
        { get { return _IsSynchronized; } set { _IsSynchronized = value; _Cache = null; } }

        Func<IDictionary<TKey, TValue>, TParam, object> _SyncRoot;
        public Func<IDictionary<TKey, TValue>, TParam, object> SyncRoot
        { get { return _SyncRoot; } set { _SyncRoot = value; _Cache = null; } }

        DictionaryReturnPathBase<Tuple<IDictionary<TKey, TValue>, TParam>, TKey, TValue> _Cache;

        /// <summary>
        /// Creates a new <see cref="DictionaryReturnPath{TKey,TValue}"/> object with the supplied delegates.
        /// </summary>
        /// <returns>A newly created <see cref="DictionaryReturnPath{TKey,TValue}"/>.</returns>
        public DictionaryReturnPath<TParam, TKey, TValue> CreateReturnPath(TParam param)
        {
            if (_Cache == null)
            {
                var fs = new DictionaryReturnPathFuncStruct<Tuple<IDictionary<TKey, TValue>, TParam>, TKey, TValue>();

                if (Add != null) fs._Add = (kvp, k, v) => Add(kvp.First,kvp.Second,k,v);
                if (Insert != null) fs._Insert = (kvp, k, v) => Insert(kvp.First, kvp.Second, k, v);
                if (Remove != null) fs._Remove = (kvp,k) => Remove(kvp.First, kvp.Second, k);
                if (RemoveWithValueCheck != null) fs._Remove2 = (kvp,k, v) => RemoveWithValueCheck(kvp.First, kvp.Second, k, v);
                if (Clear != null) fs._Clear = kvp => Clear(kvp.First, kvp.Second);
                if (IsReadOnly != null) fs._IsReadOnly = kvp => IsReadOnly(kvp.First, kvp.Second);
                if (IsSynchronized != null) fs._IsSynchronized = kvp => IsSynchronized(kvp.First, kvp.Second);
                if (SyncRoot != null) fs._SyncRoot = kvp => SyncRoot(kvp.First, kvp.Second);

                _Cache = new DictionaryReturnPathBase<Tuple<IDictionary<TKey, TValue>, TParam>, TKey, TValue>(fs);
            }

            return new DictionaryReturnPath<TParam, TKey, TValue>(_Cache, param);
        }
    }

    public class ToDictionaryFactory<TParam, TKey, TValue>
    {
        public ToDictionaryFactory(Func<IDictionaryReturnPath<TKey, TValue>, IObservableDictionary<TKey, TValue>> creator, TParam param)
        {
            _Creator = creator;
            _ReturnPathFactory = new DictionaryReturnPathFactory<TParam, TKey, TValue>();
            _Parameter = param;
        }

        Func<IDictionaryReturnPath<TKey, TValue>, IObservableDictionary<TKey, TValue>> _Creator;
        DictionaryReturnPathFactory<TParam,TKey, TValue> _ReturnPathFactory;
        TParam _Parameter;

        public IObservableDictionary<TKey, TValue> Create()
        { return _Creator(_ReturnPathFactory.CreateReturnPath(_Parameter)); }

        public ToDictionaryFactory<TParam, TKey, TValue> Add(Action<IDictionary<TKey, TValue>, TParam, TKey, TValue> add)
        {
            _ReturnPathFactory.Add = add;
            return this;
        }

        public ToDictionaryFactory<TParam, TKey, TValue> Insert(Action<IDictionary<TKey, TValue>, TParam, TKey, TValue> insert)
        {
            _ReturnPathFactory.Insert = insert;
            return this;
        }

        public ToDictionaryFactory<TParam, TKey, TValue> Remove(Func<IDictionary<TKey, TValue>, TParam, TKey, bool> remove)
        {
            _ReturnPathFactory.Remove = remove;
            return this;
        }

        public ToDictionaryFactory<TParam, TKey, TValue> RemoveWithValueCheck(Func<IDictionary<TKey, TValue>, TParam, TKey, TValue, bool> removeWithValueCheck)
        {
            _ReturnPathFactory.RemoveWithValueCheck = removeWithValueCheck;
            return this;
        }

        public ToDictionaryFactory<TParam, TKey, TValue> Clear(Action<IDictionary<TKey, TValue>, TParam> clear)
        {
            _ReturnPathFactory.Clear = clear;
            return this;
        }

        public ToDictionaryFactory<TParam, TKey, TValue> IsReadOnly(Func<IDictionary<TKey, TValue>, TParam, IValueProvider<bool>> isReadOnly)
        {
            _ReturnPathFactory.IsReadOnly = isReadOnly;
            return this;
        }

        public ToDictionaryFactory<TParam, TKey, TValue> IsSynchronized(Func<IDictionary<TKey, TValue>, TParam, bool> isSynchronized)
        {
            _ReturnPathFactory.IsSynchronized = isSynchronized;
            return this;
        }

        public ToDictionaryFactory<TParam, TKey, TValue> SyncRoot(Func<IDictionary<TKey, TValue>, TParam, object> syncRoot)
        {
            _ReturnPathFactory.SyncRoot = syncRoot;
            return this;
        }
    }

    public static class DictionaryReturnPath
    {
        public static ToDictionaryFactory<TKey, TSource> ToDictionaryFactory<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        { return new ToDictionaryFactory<TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, rp)); }

        public static ToDictionaryFactory<TKey, TSource> ToDictionaryFactory<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, comparer, rp)); }

        public static ToDictionaryFactory<TKey, TSource> ToDictionaryFactory<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector)
        { return new ToDictionaryFactory<TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, rp)); }

        public static ToDictionaryFactory<TKey, TSource> ToDictionaryFactory<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, comparer, rp)); }

        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp)); }

        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp)); }

        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp)); }

        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp)); }

        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp)); }

        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp)); }

        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp)); }
        
        public static ToDictionaryFactory<TKey, TElement> ToDictionaryFactory<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp)); }


        public static ToDictionaryFactory<TParam, TKey, TSource> ToDictionaryFactory<TParam, TSource, TKey>(this IEnumerable<TSource> source, TParam param, Func<TSource, TKey> keySelector)
        { return new ToDictionaryFactory<TParam, TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TSource> ToDictionaryFactory<TParam, TSource, TKey>(this IEnumerable<TSource> source, TParam param, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TParam, TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, comparer, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TSource> ToDictionaryFactory<TParam, TSource, TKey>(this IEnumerable<TSource> source, TParam param, Func<TSource, IValueProvider<TKey>> keySelector)
        { return new ToDictionaryFactory<TParam, TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TSource> ToDictionaryFactory<TParam, TSource, TKey>(this IEnumerable<TSource> source, TParam param, Func<TSource, IValueProvider<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TParam, TKey, TSource>(rp => ObservableEnumerable.ToDictionary(source, keySelector, comparer, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, rp), param); }

        public static ToDictionaryFactory<TParam, TKey, TElement> ToDictionaryFactory<TParam, TSource, TKey, TElement>(this IEnumerable<TSource> source, TParam param, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        { return new ToDictionaryFactory<TParam, TKey, TElement>(rp => ObservableEnumerable.ToDictionary(source, keySelector, elementSelector, comparer, rp), param); }    
    }
}
