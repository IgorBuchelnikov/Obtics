using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using Obtics.Collections;

namespace Obtics
{
    public static class BindingHelper
    {
        public class ConcreteValueProviderBase<TProvider> : INotifyPropertyChanged
            where TProvider : class, IValueProvider
        {
            internal ConcreteValueProviderBase(TProvider vp)
            { _VP = vp; }

            protected readonly TProvider _VP;

            public bool IsReadOnly
            { get { return _VP.IsReadOnly; } }

            #region INotifyPropertyChanged Members

            event PropertyChangedEventHandler _PropertyChanged;

            public event PropertyChangedEventHandler PropertyChanged
            {
                add
                {
                    bool register = _PropertyChanged == null;

                    _PropertyChanged += value;

                    if (register)
                    {
                        var npc = _VP as INotifyPropertyChanged;

                        if (npc != null)
                            npc.PropertyChanged += npc_PropertyChanged;
                    }
                }
                remove
                {
                    _PropertyChanged -= value;

                    if (_PropertyChanged == null)
                    {
                        var npc = _VP as INotifyPropertyChanged;

                        if (npc != null)
                            npc.PropertyChanged -= npc_PropertyChanged;
                    }
                }
            }

            void npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                var pc = _PropertyChanged ;

                if (pc != null)
                    pc(this, e);
            }

            #endregion
        }

        public sealed class ConcreteValueProvider : ConcreteValueProviderBase<IValueProvider>, IValueProvider
        {
            internal ConcreteValueProvider(IValueProvider vp)
                : base(vp)
            { }

            #region IValueProvider Members

            public object Value
            {
                get { return _VP.Value; }
                set { _VP.Value = value; }
            }

            #endregion
        }

        public sealed class ConcreteValueProvider<TType> : ConcreteValueProviderBase<IValueProvider<TType>>, IValueProvider<TType>
        {
            internal ConcreteValueProvider(IValueProvider<TType> vp) 
                : base( vp )
            {}

            #region IValueProvider<TType> Members

            public TType Value
            {
                get { return _VP.Value; }
                set { _VP.Value = value; }
            }

            #endregion

            #region IValueProvider Members

            object IValueProvider.Value
            {
                get { return _VP.Value; }
                set { _VP.Value = (TType)value; }
            }

            #endregion
        }

        public class ConcreteEnumerableBase<TEnumerable> : INotifyCollectionChanged
            where TEnumerable : IEnumerable
        {
            internal ConcreteEnumerableBase(TEnumerable e)
            { _E = e; }

            protected readonly TEnumerable _E;


            #region INotifyCollectionChanged Members

            event NotifyCollectionChangedEventHandler _CollectionChanged;

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    var register = _CollectionChanged == null;

                    _CollectionChanged += value;


                    if (register)
                    {
                        var ncc = _E as INotifyCollectionChanged;

                        if (ncc != null)
                            ncc.CollectionChanged += ncc_CollectionChanged;
                    }
                }
                remove
                {
                    _CollectionChanged -= value;

                    if (_CollectionChanged == null)
                    {

                        var ncc = _E as INotifyCollectionChanged;

                        if (ncc != null)
                            ncc.CollectionChanged -= ncc_CollectionChanged;
                    }
                }
            }

            void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                var cc = _CollectionChanged;

                if (cc != null)
                    cc(this, e);
            }

            #endregion
        }

        public sealed class ConcreteEnumerable : ConcreteEnumerableBase<IEnumerable>, IEnumerable
        {
            internal ConcreteEnumerable(IEnumerable e)
                : base(e)
            { }

            #region IEnumerable Members

            public IEnumerator GetEnumerator()
            { return _E.GetEnumerator(); }

            #endregion
        }

        public  class ConcreteTypedEnumerableBase<TEnumerable,TType> : ConcreteEnumerableBase<TEnumerable>, IEnumerable<TType>
            where TEnumerable : IEnumerable<TType>
        {
            internal ConcreteTypedEnumerableBase(TEnumerable e)
                : base(e)
            { }

            #region IEnumerable<TType> Members

            public IEnumerator<TType> GetEnumerator()
            { return _E.GetEnumerator(); }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            { return _E.GetEnumerator(); }

            #endregion
        }

        public sealed class ConcreteEnumerable<TType> : ConcreteTypedEnumerableBase<IEnumerable<TType>,TType>
        {
            internal ConcreteEnumerable(IEnumerable<TType> e)
                : base(e)
            { }
        }

        public sealed class ConcreteOrderedEnumerable<TType> : ConcreteTypedEnumerableBase<IOrderedEnumerable<TType>, TType>, IOrderedEnumerable<TType>
        {
            internal ConcreteOrderedEnumerable(IOrderedEnumerable<TType> e)
                : base(e)
            { }

            #region IOrderedEnumerable<TType> Members

            public IOrderedEnumerable<TType> CreateOrderedEnumerable<TKey>(Func<TType, TKey> keySelector, IComparer<TKey> comparer, bool descending)
            { return _E.CreateOrderedEnumerable(keySelector, comparer, descending); }

            #endregion
        }

        public sealed class ConcreteObservableOrderedEnumerable<TType> : ConcreteTypedEnumerableBase<IObservableOrderedEnumerable<TType>, TType>, IObservableOrderedEnumerable<TType>
        {
            internal ConcreteObservableOrderedEnumerable(IObservableOrderedEnumerable<TType> e)
                : base(e)
            { }


            #region IObservableOrderedEnumerable<TType> Members

            public IObservableOrderedEnumerable<TType> CreateOrderedEnumerable<TKey>(Func<TType, TKey> keySelector, IComparer<TKey> comparer, bool descending)
            { return _E.CreateOrderedEnumerable(keySelector, comparer, descending); }

            public IObservableOrderedEnumerable<TType> CreateOrderedEnumerable<TKey>(Func<TType, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer, bool descending)
            { return _E.CreateOrderedEnumerable(keySelector, comparer, descending); }

            #endregion

            #region IOrderedEnumerable<TType> Members

            IOrderedEnumerable<TType> IOrderedEnumerable<TType>.CreateOrderedEnumerable<TKey>(Func<TType, TKey> keySelector, IComparer<TKey> comparer, bool descending)
            { return _E.CreateOrderedEnumerable(keySelector, comparer, descending); }

            #endregion
        }

        public sealed class ConcreteGrouping<TKey, TValue> : ConcreteEnumerableBase<IGrouping<TKey, TValue>>, IGrouping<TKey, TValue>
        {
            internal ConcreteGrouping(IGrouping<TKey, TValue> e)
                : base(e)
            { }

            #region IGrouping<TKey,TValue> Members

            public TKey Key
            { get { return _E.Key; } }

            #endregion

            #region IEnumerable<TValue> Members

            public IEnumerator<TValue> GetEnumerator()
            { return _E.GetEnumerator(); }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            { return _E.GetEnumerator(); }

            #endregion
        }

        public class ConcreteDictionaryBase<TDictionary,TKey,TValue> : ConcreteTypedEnumerableBase<TDictionary,KeyValuePair<TKey,TValue>>, IDictionary<TKey, TValue>
            where TDictionary : IDictionary<TKey,TValue>
        {
            internal ConcreteDictionaryBase(TDictionary e)
                : base(e)
            { }

            #region IDictionary<TKey,TValue> Members

            public void Add(TKey key, TValue value)
            { _E.Add(key,value); }

            public bool ContainsKey(TKey key)
            { return _E.ContainsKey(key); }

            public ICollection<TKey> Keys
            { get { return _E.Keys; } }

            public bool Remove(TKey key)
            { return _E.Remove(key); }

            public bool TryGetValue(TKey key, out TValue value)
            { return _E.TryGetValue(key, out value); }

            public ICollection<TValue> Values
            { get { return _E.Values; } }

            public TValue this[TKey key]
            {
                get { return _E[key]; }
                set { _E[key] = value; }
            }

            #endregion

            #region ICollection<KeyValuePair<TKey,TValue>> Members

            public void Add(KeyValuePair<TKey, TValue> item)
            { _E.Add(item); }

            public void Clear()
            { _E.Clear(); }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            { return _E.Contains(item); }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            { _E.CopyTo(array, arrayIndex); }

            public int Count
            { get { return _E.Count; } }

            public bool IsReadOnly
            { get { return _E.IsReadOnly; } }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            { return _E.Remove(item); }

            #endregion
        }

        public sealed class ConcreteDictionary<TKey, TValue> : ConcreteDictionaryBase<IDictionary<TKey, TValue>, TKey, TValue>
        {
            internal ConcreteDictionary(IDictionary<TKey, TValue> e)
                : base(e)
            { }
        }

        public sealed class ConcreteObservableDictionary<TKey, TValue> : ConcreteDictionaryBase<IObservableDictionary<TKey, TValue>, TKey, TValue>, IObservableDictionary<TKey,TValue>
        {
            internal ConcreteObservableDictionary(IObservableDictionary<TKey, TValue> e)
                : base(e)
            { }

            #region IObservableDictionary<TKey,TValue> Members

            public IValueProvider<TValue> this[IValueProvider<TKey> key]
            { get { return _E[key]; } }

            public IValueProvider<bool> ContainsKey(IValueProvider<TKey> key)
            { return _E.ContainsKey(key); }

            #endregion
        }


        public class ConcreteLookupBase<TLookup, TKey, TValue> : ConcreteTypedEnumerableBase<TLookup, IGrouping<TKey, TValue>>, ILookup<TKey, TValue>
            where TLookup : ILookup<TKey, TValue>
        {
            internal ConcreteLookupBase(TLookup e)
                : base(e)
            { }

            #region ILookup<TKey,TValue> Members

            public bool Contains(TKey key)
            { return _E.Contains(key); }

            public int Count
            { get { return _E.Count ; } }

            public IEnumerable<TValue> this[TKey key]
            { get { return _E[key]; } }

            #endregion
        }

        public sealed class ConcreteLookup<TKey, TValue> : ConcreteLookupBase<ILookup<TKey, TValue>, TKey, TValue>
        {
            internal ConcreteLookup(ILookup<TKey, TValue> e)
                : base(e)
            { }
        }

        public sealed class ConcreteObservableLookup<TKey, TValue> : ConcreteLookupBase<IObservableLookup<TKey, TValue>, TKey, TValue>, IObservableLookup<TKey, TValue>
        {
            internal ConcreteObservableLookup(IObservableLookup<TKey, TValue> e)
                : base(e)
            { }

            #region IObservableLookup<TKey,TValue> Members

            public IEnumerable<TValue> this[IValueProvider<TKey> key]
            { get { return _E[key]; } }

            public IValueProvider<bool> Contains(IValueProvider<TKey> key)
            { return _E.Contains(key); }

            #endregion
        }

        /// <summary>
        /// Creates a <see cref="ConcreteValueProvider"/> that serves as a concrete proxy for the given <see cref="IValueProvider"/>.
        /// </summary>
        /// <param name="source">The <see cref="IValueProvider"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteValueProvider"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        [Obsolete]
        public static ConcreteValueProvider Concrete(this IValueProvider source)
        { return source == null ? null : new ConcreteValueProvider(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteValueProvider{TType}"/> that serves as a concrete proxy for the given <see cref="IValueProvider{TType}"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the Value property of <paramref name="source"/> and the returned <see cref="ConcreteValueProvider{TType}"/>.</typeparam>
        /// <param name="source">The <see cref="IValueProvider{TType}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteValueProvider{TType}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        [Obsolete]
        public static ConcreteValueProvider<TType> Concrete<TType>(this IValueProvider<TType> source)
        { return source == null ? null : new ConcreteValueProvider<TType>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteEnumerable"/> that serves as a concrete proxy for the given <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteEnumerable"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteEnumerable Concrete(this IEnumerable source)
        { return source == null ? null : new ConcreteEnumerable(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteEnumerable{TType}"/> that serves as a concrete proxy for the given <see cref="IEnumerable{TType}"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of <paramref name="source"/> and the returned <see cref="ConcreteEnumerable{TType}"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TType}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteEnumerable{TType}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteEnumerable<TType> Concrete<TType>(this IEnumerable<TType> source)
        { return source == null ? null : new ConcreteEnumerable<TType>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteOrderedEnumerable{TType}"/> that serves as a concrete proxy for the given <see cref="IOrderedEnumerable{TType}"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of <paramref name="source"/> and the returned <see cref="ConcreteOrderedEnumerable{TType}"/>.</typeparam>
        /// <param name="source">The <see cref="IOrderedEnumerable{TType}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteOrderedEnumerable{TType}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteOrderedEnumerable<TType> Concrete<TType>(this IOrderedEnumerable<TType> source)
        { return source == null ? null : new ConcreteOrderedEnumerable<TType>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteObservableOrderedEnumerable{TType}"/> that serves as a concrete proxy for the given <see cref="IObservableOrderedEnumerable{TType}"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of <paramref name="source"/> and the returned <see cref="ConcreteObservableOrderedEnumerable{TType}"/>.</typeparam>
        /// <param name="source">The <see cref="IObservableOrderedEnumerable{TType}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteObservableOrderedEnumerable{TType}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteObservableOrderedEnumerable<TType> Concrete<TType>(this IObservableOrderedEnumerable<TType> source)
        { return source == null ? null : new ConcreteObservableOrderedEnumerable<TType>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteGrouping{TKey, TValue}"/> that serves as a concrete proxy for the given <see cref="IGrouping{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the key of <paramref name="source"/> and the returned <see cref="ConcreteGrouping{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The type of the elements of <paramref name="source"/> and the returned <see cref="ConcreteGrouping{TKey, TValue}"/>.</typeparam>
        /// <param name="source">The <see cref="IGrouping{TKey, TValue}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteGrouping{TKey, TValue}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteGrouping<TKey, TValue> Concrete<TKey, TValue>(this IGrouping<TKey, TValue> source)
        { return source == null ? null : new ConcreteGrouping<TKey, TValue>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteDictionary{TKey, TValue}"/> that serves as a concrete proxy for the given <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys of <paramref name="source"/> and the returned <see cref="ConcreteDictionary{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The type of the values of <paramref name="source"/> and the returned <see cref="ConcreteDictionary{TKey, TValue}"/>.</typeparam>
        /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteDictionary{TKey, TValue}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteDictionary<TKey, TValue> Concrete<TKey, TValue>(this IDictionary<TKey, TValue> source)
        { return source == null ? null : new ConcreteDictionary<TKey, TValue>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteObservableDictionary{TKey, TValue}"/> that serves as a concrete proxy for the given <see cref="IObservableDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys of <paramref name="source"/> and the returned <see cref="ConcreteObservableDictionary{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The type of the values of <paramref name="source"/> and the returned <see cref="ConcreteObservableDictionary{TKey, TValue}"/>.</typeparam>
        /// <param name="source">The <see cref="IObservableDictionary{TKey, TValue}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteObservableDictionary{TKey, TValue}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteObservableDictionary<TKey, TValue> Concrete<TKey, TValue>(this IObservableDictionary<TKey, TValue> source)
        { return source == null ? null : new ConcreteObservableDictionary<TKey, TValue>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteLookup{TKey, TValue}"/> that serves as a concrete proxy for the given <see cref="ILookup{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys of <paramref name="source"/> and the returned <see cref="ConcreteLookup{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The type of the values of <paramref name="source"/> and the returned <see cref="ConcreteLookup{TKey, TValue}"/>.</typeparam>
        /// <param name="source">The <see cref="ILookup{TKey, TValue}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteLookup{TKey, TValue}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteLookup<TKey, TValue> Concrete<TKey, TValue>(this ILookup<TKey, TValue> source)
        { return source == null ? null : new ConcreteLookup<TKey, TValue>(source); }

        /// <summary>
        /// Creates a <see cref="ConcreteObservableLookup{TKey, TValue}"/> that serves as a concrete proxy for the given <see cref="IObservableLookup{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys of <paramref name="source"/> and the returned <see cref="ConcreteObservableLookup{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The type of the values of <paramref name="source"/> and the returned <see cref="ConcreteObservableLookup{TKey, TValue}"/>.</typeparam>
        /// <param name="source">The <see cref="IObservableLookup{TKey, TValue}"/> to create a concrete proxy for.</param>
        /// <returns>A <see cref="ConcreteObservableLookup{TKey, TValue}"/> instance that is a concrete proxy for <paramref name="source"/> or null if <paramref name="source"/> is null.</returns>
        public static ConcreteObservableLookup<TKey, TValue> Concrete<TKey, TValue>(this IObservableLookup<TKey, TValue> source)
        { return source == null ? null : new ConcreteObservableLookup<TKey, TValue>(source); }

    }
}
