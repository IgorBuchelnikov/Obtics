using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Obtics.Values;

namespace Obtics
{
    /// <summary>
    /// This class maps internally used INotifyChanged events to public INotifyPropertyChanged events.
    /// </summary>
    /// <remarks>
    /// It passes the original INotifyChanged sender as sender of the INotifyPropertyChanged event.
    /// </remarks>
    internal sealed class NCToNPC : ObservableObjectBase<INotifyChanged>, IReceiveChangeNotification, INotifyPropertyChanged
    {
        public static NCToNPC Create(INotifyChanged source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<NCToNPC, INotifyChanged>(source);
        }

        #region INotifyPropertyChanged Members

        event PropertyChangedEventHandler mPropertyChanged;

        /// <summary>
        /// PropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                FlagsState flags;

                while (!GetAndLockFlags(out flags)) ;

                if (mPropertyChanged == null)
                    _Prms.SubscribeINC(this);

                mPropertyChanged += value;

                //shield against clients that get value first
                //and only after register for changes.
                IValueProvider asVp = _Prms as IValueProvider;

                if (asVp != null)
                    GC.KeepAlive(asVp.Value);

                Commit(ref flags);
            }

            remove
            {
                FlagsState flags;

                while (!GetAndLockFlags(out flags)) ;

                bool hadSome = mPropertyChanged != null;

                mPropertyChanged -= value;

                if ( hadSome && mPropertyChanged == null )
                    _Prms.UnsubscribeINC(this);

                Commit(ref flags);
            }
        }

        #endregion


        #region IReceiveChangeNotification Members

        public void NotifyChanged(object sender, INCEventArgs changeArgs)
        {
            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            PropertyChangedEventHandler handler = mPropertyChanged;

            Commit(ref flags);

            if (handler != null)
            {
                switch (changeArgs.Type)
                {
                    case INCEventArgsTypes.Exception:
                        throw ((INExceptionEventArgs)changeArgs).Exception;

                    case INCEventArgsTypes.ValueChanged:
                        handler(_Prms, SIValueProvider.ValuePropertyChangedEventArgs);
                        break;

                    case INCEventArgsTypes.IsReadOnlyChanged:
                        handler(_Prms, SIValueProvider.IsReadOnlyPropertyChangedEventArgs);
                        break;

                    case INCEventArgsTypes.CollectionReplace:
                        handler(_Prms, SIList.ItemsIndexerPropertyChangedEventArgs);
                        break;

                    case INCEventArgsTypes.CollectionReset:
                    case INCEventArgsTypes.CollectionAdd:
                    case INCEventArgsTypes.CollectionRemove:
                        handler(_Prms, SIList.ItemsIndexerPropertyChangedEventArgs);
                        handler(_Prms, SICollection.CountPropertyChangedEventArgs);
                        break;

                    default:
                        throw new Exception("Unexpected INCEventArgsType");
                }
            }
        }

        #endregion
    }
}
