using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;

namespace Obtics.Collections
{
    internal sealed class StaticEnumerable<TType> : IVersionedEnumerable<TType>, INotifyChanged
    {
        StaticEnumerable(params TType[] items)
        { _Items = (TType[])items.Clone(); }

        public static StaticEnumerable<TType> Create(params TType[] items)
        {
            if (items == null)
                return null;

            return new StaticEnumerable<TType>(items);
        }

        StaticEnumerable(IEnumerable<TType> items)
        { _Items = SL.Enumerable.ToArray(items); }

        public static StaticEnumerable<TType> Create(IEnumerable<TType> items)
        {
            if (items == null)
                return null;

            return new StaticEnumerable<TType>(items);
        }

        TType[] _Items;

        #region IEnumerable<TType> Members

        IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return this.GetEnumerator();  }

        #endregion

        #region IEquatable

        public override bool Equals(object obj)
        { return Equals(obj as StaticEnumerable<TType>); }

        public override int GetHashCode()
        { return Hasher.CreateFromRef(typeof(StaticEnumerable<TType>)).AddValue(new ArrayStructuredEqualityWrapper<TType>(_Items)); }

        public bool Equals(StaticEnumerable<TType> other)
        { return other != null && (object.ReferenceEquals(this, other) || new ArrayStructuredEqualityWrapper<TType>(_Items).Equals(new ArrayStructuredEqualityWrapper<TType>(other._Items))); }

        #endregion    
    
        #region IVersionedEnumerable<TType> Members

        public IVersionedEnumerator<TType> GetEnumerator()
        { return VersionedEnumerator.WithContentVersion(_Items, new VersionNumber()); }

        #endregion

        #region IVersionedEnumerable Members

        public VersionNumber ContentVersion
        { get { return new VersionNumber(); } }

        IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region INotifyChanged Members

        public void SubscribeINC(IReceiveChangeNotification receiver)
        {}

        public void UnsubscribeINC(IReceiveChangeNotification receiver)
        {}

        #endregion
    }
}
