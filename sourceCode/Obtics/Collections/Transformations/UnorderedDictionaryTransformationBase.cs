using System;
using System.Collections.Generic;
using System.ComponentModel;
using SL = System.Linq;
using Obtics.Values;
using System.Collections;

namespace Obtics.Collections.Transformations
{
    internal abstract class UnorderedDictionaryTransformationBase<TKey, TOut,TPrms> : ConvertTransformationBase<Tuple<SL.IGrouping<TKey, TOut>, TOut>, KeyValuePair<TKey, TOut>, UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>, TPrms>, IObservableDictionary<TKey, TOut>, ICollection
    {
        public override bool IsMostUnordered
        { get { return true; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override KeyValuePair<TKey, TOut> ConvertValue(Tuple<SL.IGrouping<TKey, TOut>, TOut> value)
        { return new KeyValuePair<TKey, TOut>(value.First.Key, value.Second); }

        UnorderedLookupTransformation<TKey, TOut> Lookup
        { get { return (UnorderedLookupTransformation<TKey, TOut>)Source._Source; } }

        #region IObservableDictionary<TKey,TOut> Members

        //only accessing truly static parameters.. No locking
        public ICollection<TKey> Keys
        { get { return ListTransformation<TKey>.Create(ConvertTransformation<SL.IGrouping<TKey, TOut>, TKey>.Create(Lookup, grp => grp.Key)); } }

        //only accessing truly static parameters.. No locking
        public ICollection<TOut> Values
        { get { return ListTransformation<TOut>.Create(ConvertTransformation<KeyValuePair<TKey, TOut>, TOut>.Create(this, kvp => kvp.Value)); } }

        //only accessing truly static parameters.. No locking
        public IValueProvider<TOut> this[IValueProvider<TKey> key]
        { get { return ElementAggregate<TOut>.Create(Lookup[key].Patched(), 0); } }

        //only accessing truly static parameters.. No locking
        public IValueProvider<bool> ContainsKey(IValueProvider<TKey> key)
        { return Lookup.Contains(key); }

        #endregion

        private static void RaiseNotSupportedException()
        { throw new NotSupportedException(); }

        #region IDictionary<TKey,TOut> Members

        //Only raising exception.. No Locking
        public virtual void Add(TKey key, TOut value)
        { RaiseNotSupportedException(); }

        //Only raising exception.. No Locking
        public virtual void Insert(TKey key, TOut value)
        { RaiseNotSupportedException(); }

        //Only accessing truly static parameters.. No locking
        public bool ContainsKey(TKey key)
        { return Lookup.Contains(key); }

        //Only raising exception.. No Locking
        public virtual bool Remove(TKey key)
        { RaiseNotSupportedException(); return false; }

        //Only accessing truly static parameters and local variables.. No locking
        public bool TryGetValue(TKey key, out TOut value)
        {
            using (IEnumerator<TOut> items = Lookup[key].GetEnumerator())
            {
                if (items.MoveNext())
                {
                    value = items.Current;
                    return true;
                }
                else
                {
                    value = default(TOut);
                    return false;
                }
            }
        }

        public virtual TOut this[TKey key]
        {
            //Only accessing truly static parameters and local variables.. No locking
            get
            {
                using (IEnumerator<TOut> items = Lookup[key].GetEnumerator())
                    return items.MoveNext() ? items.Current : default(TOut);
            }
            //Only raising exception.. No Locking
            set
            { RaiseNotSupportedException(); }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TOut>> Members

        //Only raising exception.. No Locking
        public void Add(KeyValuePair<TKey, TOut> item)
        { Add(item.Key,item.Value); }

        //Only raising exception.. No Locking
        public virtual void Clear()
        { RaiseNotSupportedException(); }

        //TryGetValue is public, only using local variables.. No Locking
        public bool Contains(KeyValuePair<TKey, TOut> item)
        {
            TOut value;
            return TryGetValue(item.Key, out value) && ObticsEqualityComparer<TOut>.Default.Equals(value, item.Value); //TODO:OK?
        }

        //GetEnumerator() is public, only using local variables.. No locking
        public void CopyTo(KeyValuePair<TKey, TOut>[] array, int arrayIndex)
        {
            using (var enumerator = this.GetEnumerator())
                while (enumerator.MoveNext())
                    array[arrayIndex++] = enumerator.Current;
        }


        public const string CountPropertyName = SICollection.CountPropertyName;

        public int Count
        {
            get
            {
                int c = 0;

                using (var enm = this.GetEnumerator())
                    while (enm.MoveNext())
                        ++c;

                return c;
            }
        }

        public virtual bool IsReadOnly
        { get { return true; } }

        //Only raising exception.. No Locking
        public virtual bool Remove(KeyValuePair<TKey, TOut> item)
        { RaiseNotSupportedException(); return false; }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        { this.CopyTo((KeyValuePair<TKey, TOut>[])array, index); }

        public virtual bool IsSynchronized
        { get { return true; } }

        public virtual object SyncRoot
        { get { return null; } }

        #endregion
    }
}
