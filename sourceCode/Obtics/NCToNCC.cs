using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Obtics.Collections;

namespace Obtics
{
    /// <summary>
    /// This class maps internally used INotifyChanged events to public INotifyCollectionChanged events.
    /// </summary>
    /// <remarks>
    /// It passes the original INotifyChanged sender as sender of the INotifyCollectionChanged event.
    /// </remarks>
    internal sealed class NCToNCC : ObservableObjectBase<IInternalEnumerable>, IReceiveChangeNotification, INotifyCollectionChanged
    {
        #region Bitflags
        const Int32 HaveCollectionChangedListeningClientsMask = 1 << (ObservableObjectBase<INotifyChanged>.BitFlagIndexEnd + 0);

        //protected new const Int32 BitFlagIndexEnd = ObservableObjectBase<INotifyChanged>.BitFlagIndexEnd + 1;
        #endregion

        public static NCToNCC Create(IInternalEnumerable source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<NCToNCC, IInternalEnumerable>(source);
        }

        #region INotifyCollectionChanged Members

        event NotifyCollectionChangedEventHandler mCollectionChanged;

        /// <summary>
        /// CollectionChanged event
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                FlagsState flags;

                while (!GetAndLockFlags(out flags)) ;

                try
                {
                    mCollectionChanged += value;

                    if (!flags.GetBitFlag(HaveCollectionChangedListeningClientsMask))
                    {
                        _Prms.SubscribeINC(this);
                        flags.SetBitFlag(HaveCollectionChangedListeningClientsMask, true);
                    }
                }
                finally 
                { 
                    Commit(ref flags); 
                }

                //Get enumerator to shield against clients that get enumerator first
                //and only then register for changes.
                var disposable = _Prms.GetEnumerator() as IDisposable;

                if (disposable != null)
                    disposable.Dispose();
            }

            remove
            {
                FlagsState flags;

                while (!GetAndLockFlags(out flags)) ;

                try
                {
                    mCollectionChanged -= value;

                    if (mCollectionChanged == null && flags.GetBitFlag(HaveCollectionChangedListeningClientsMask))
                    {
                        _Prms.UnsubscribeINC(this);
                        flags.SetBitFlag(HaveCollectionChangedListeningClientsMask, false);
                    }
                }
                finally
                {
                    Commit(ref flags);
                }
            }
        }

        #endregion


        #region IReceiveChangeNotification Members

        public void NotifyChanged(object sender, INCEventArgs changeArgs)
        {
            NotifyCollectionChangedEventHandler handler = null;            

            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            try
            {
                handler = mCollectionChanged;
            }
            finally
            { Commit(ref flags); }


            if (handler != null)
            {
                NotifyCollectionChangedEventArgs message ;

                switch (changeArgs.Type)
                {
                    case INCEventArgsTypes.ValueChanged:
                    case INCEventArgsTypes.IsReadOnlyChanged:
                        message = null;
                        break;

                    case INCEventArgsTypes.CollectionAdd:
                        {
                            var addArgs = (SingleItemINCollectionEventArgs)changeArgs;
                            message = SIOrderedNotifyCollectionChanged.Create(addArgs.VersionNumber, NotifyCollectionChangedAction.Add, addArgs.Item, addArgs.Index);
                        }
                        break;

                    case INCEventArgsTypes.CollectionRemove:
                        {
                            var removeArgs = (SingleItemINCollectionEventArgs)changeArgs;
                            message = SIOrderedNotifyCollectionChanged.Create(removeArgs.VersionNumber, NotifyCollectionChangedAction.Remove, removeArgs.Item, removeArgs.Index);
                        }
                        break;
                    case INCEventArgsTypes.CollectionReplace:
                        {
                            var replaceArgs = (INCollectionReplaceEventArgs)changeArgs;
                            message = SIOrderedNotifyCollectionChanged.Create(replaceArgs.VersionNumber, NotifyCollectionChangedAction.Replace, replaceArgs.NewItem, replaceArgs.OldItem, replaceArgs.Index);
                        }
                        break;
                    case INCEventArgsTypes.CollectionReset:
                        {
                            var resetArgs = (INCollectionResetEventArgs)changeArgs;
                            message = SIOrderedNotifyCollectionChanged.Create(resetArgs.VersionNumber, NotifyCollectionChangedAction.Reset);
                        }
                        break;

                    default:
                        throw new Exception("Unexpected INCEventArgsType");
                }

                if (message != null)
                    handler(_Prms, message);
            }
        }

        #endregion
    }
}
