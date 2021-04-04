using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;
using System.ComponentModel;

namespace Obtics
{
    internal abstract class NCSourcedObjectToVP<TOut, TPrms> : NCObservableObjectBase<TPrms>, IInternalValueProvider<TOut>, IReceiveChangeNotification
    {
        #region IValueProvider<TOut> Members

        /// <summary>
        /// ValuePropertyName
        /// </summary>
        public const string ValuePropertyName = SIValueProvider.ValuePropertyName;

        /// <summary>
        /// Value
        /// </summary>
        public TOut Value
        {
            get
            {
                if (PeekExternalCacheHint())
                {
                    //ExternalCacheHint is true. That means there is probably a valid value in the external cache.
                    object cachedValue;

                    //try to get it; if we can return it otherwise proceed as normal.
                    if (TryGetExternalCache(out cachedValue))
                        return (TOut)cachedValue;
                }

                return GetValueEvent();
            }
            set
            {
                //Setting shouldn't do anything with the internal state or event handlers.. no lock;
                SetValue(value);
            }
        }

        protected abstract TOut GetValueEvent();

        protected virtual void SetValue(TOut value)
        { throw new NotSupportedException(); }

        #endregion

        #region IReceiveChangeNotification Members

#if HLG
        struct NotifyChanged_LoggInfo
        {
            public INCEventArgs ChangeArgs;
        }
#endif

        void IReceiveChangeNotification.NotifyChanged(object sender, INCEventArgs changeArgs)
        {
#if HLG
            Logg(
                this,
                new NotifyChanged_LoggInfo { ChangeArgs = changeArgs }
            );
#endif
            bool daFlag = DelayedActionRegistry.Enter();

            try
            {

                switch (changeArgs.Type)
                {
                    case INCEventArgsTypes.ValueChanged:
                        SourceChangeEvent(sender);
                        break;
                    case INCEventArgsTypes.Exception:
                        SourceExceptionEvent(sender, changeArgs);
                        break;
                    case INCEventArgsTypes.IsReadOnlyChanged:
                        SourceIsReadOnlyEvent(sender);
                        break;
                    //case INCEventArgsTypes.CollectionAdd:
                    //case INCEventArgsTypes.CollectionRemove:
                    //case INCEventArgsTypes.CollectionReplace:
                    //case INCEventArgsTypes.CollectionReset:
                    default:
                        SourceCollectionEvent(sender, (INCollectionChangedEventArgs)changeArgs);
                        break;
                }
            }
            finally
            {
                DelayedActionRegistry.Leave(daFlag);
            }
        }

        protected virtual void SourceChangeEvent(object sender)
        { }

        protected virtual void SourceExceptionEvent(object sender, INCEventArgs changeArgs)
        { }

        protected virtual void SourceIsReadOnlyEvent(object sender)
        { }

        protected virtual void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        { }

        #endregion

        #region IValueProvider Members

        object IValueProvider.Value
        {
            get { return this.Value; }
            set { this.Value = (TOut)value; }
        }

        public bool IsReadOnly
        { get { return GetIsReadOnly(); } }

        protected virtual bool GetIsReadOnly()
        { return true; }

        #endregion
    }
}
