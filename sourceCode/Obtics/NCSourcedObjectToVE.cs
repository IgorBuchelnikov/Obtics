using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections;
using System.Collections.Specialized;

namespace Obtics
{
    internal abstract class NCSourcedObjectToVE<TOut, TPrms> : NCObservableObjectBase<TPrms>, IInternalEnumerable<TOut>, IReceiveChangeNotification, INotifyCollectionChanged
    {
        #region IInternalEnumerable<TOut> Members

        public virtual IInternalEnumerable<TOut> UnorderedForm
        {
            get
            {
                if (IsMostUnordered)
                    return this;

                throw new NotImplementedException("UnorderedForm");
            }
        }

        #endregion

        #region IInternalEnumerable Members

        IInternalEnumerable IInternalEnumerable.UnorderedForm
        { get { return this.UnorderedForm; } }

        public virtual bool IsMostUnordered
        { get { return false; } }

        public virtual bool HasSafeEnumerator
        { get { return false; } }

        #endregion

        #region IVersionedEnumerable Members

        IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IVersionedEnumerable<TOut> Members

        protected class SafeEnumeratorProvider : IVersionedEnumerable<TOut> 
        {
            public TOut[] Items;
            public VersionNumber SN;

            public SafeEnumeratorProvider(IVersionedEnumerator<TOut> source)
                : this(source, source.ContentVersion)
            {}

            public SafeEnumeratorProvider(IEnumerator<TOut> source, VersionNumber version)
            {
                var acc = new List<TOut>();

                while (source.MoveNext())
                    acc.Add(source.Current);

                Items = acc.ToArray();
                SN = version;
            }

            public IVersionedEnumerator<TOut> GetEnumerator()
            { return VersionedEnumerator.WithContentVersion(Items, SN); }

            IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
            { return this.GetEnumerator(); }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            { return this.GetEnumerator(); }

            IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator()
            { return this.GetEnumerator(); }
        }

        public IVersionedEnumerator<TOut> GetEnumerator()
        {
            if (PeekExternalCacheHint())
            {
                //ExternalCacheHint is true. That means there is probably a valid value in the external cache.
                object cachedValue;

                //try to get it; if we can return it otherwise proceed as normal.
                if (TryGetExternalCache(out cachedValue))
                    return ((IVersionedEnumerable<TOut>)cachedValue).GetEnumerator();
            }

            return GetEnumeratorEvent();
        }

        protected abstract IVersionedEnumerator<TOut> GetEnumeratorEvent();

        #endregion

        #region IEnumerable<TOut> Members

        IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { Obtics.NCToNCC.Create(this).CollectionChanged += value; }
            remove { Obtics.NCToNCC.Create(this).CollectionChanged -= value; }
        }

        #endregion

        #region IReceiveChangeNotification Members

        void IReceiveChangeNotification.NotifyChanged(object sender, INCEventArgs changeArgs)
        {
            switch (changeArgs.Type)
            {
                case INCEventArgsTypes.ValueChanged:
                    SourceValueChangeEvent(sender);
                    break;
                case INCEventArgsTypes.Exception:
                    SourceExceptionEvent(sender, (INExceptionEventArgs)changeArgs);
                    break;
                case INCEventArgsTypes.IsReadOnlyChanged:
                    SourceIsReadOnlyEvent(sender);
                    break;
                //case INCEventArgsTypes.CollectionReset:
                //case INCEventArgsTypes.CollectionAdd:
                //case INCEventArgsTypes.CollectionRemove:
                //case INCEventArgsTypes.CollectionReplace:
                default:
                    SourceCollectionEvent(sender, (INCollectionChangedEventArgs)changeArgs);
                    break;
            }
        }

        protected virtual void SourceValueChangeEvent(object sender)
        { }

        protected virtual void SourceExceptionEvent(object sender, INExceptionEventArgs changeArgs)
        { }

        protected virtual void SourceIsReadOnlyEvent(object sender)
        { }

        protected virtual void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        { }

        #endregion
    }
}
