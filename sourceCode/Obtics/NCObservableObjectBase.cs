using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal abstract class NCObservableObjectBase<TPrms> : ObservableObjectBase<TPrms>, INotifyChanged
    {
        #region Bitflags

        const Int32 HaveChangedListeningClientsMask = 1 << (ObservableObjectBase<TPrms>.BitFlagIndexEnd + 0);
        const Int32 IsPivotNodeMask = 1 << (ObservableObjectBase<TPrms>.BitFlagIndexEnd + 1);
        const Int32 ExternalCacheHintMask = 1 << (ObservableObjectBase<TPrms>.BitFlagIndexEnd + 2);

#if PARALLEL
        const Int32 ParallelizationForbiddenMask = 1 << (ObservableObjectBase<TPrms>.BitFlagIndexEnd + 3);

        protected new const Int32 BitFlagIndexEnd = ObservableObjectBase<TPrms>.BitFlagIndexEnd + 4;
#else

        protected new const Int32 BitFlagIndexEnd = ObservableObjectBase<TPrms>.BitFlagIndexEnd + 3;
#endif
        #endregion

        NotifyChangedReceiverTable _ReceiverTable;

        //if true this is a node with many clients
        protected bool GetIsPivotNode(ref FlagsState flagsState)
        { return flagsState.GetBitFlag(IsPivotNodeMask); }

        protected bool SetIsPivotNode(ref FlagsState flagsState, bool value)
        { return flagsState.SetBitFlag(IsPivotNodeMask, value); }


        //if true suggests that a value is stored in the external cache 
        //(accessable without locking this object)
        protected bool GetExternalCacheHint(ref FlagsState flagsState)
        { return flagsState.GetBitFlag(ExternalCacheHintMask); }

        protected bool SetExternalCacheHint(ref FlagsState flagsState, bool value)
        { return flagsState.SetBitFlag(ExternalCacheHintMask, value); }

        protected bool PeekExternalCacheHint()
        { return PeekFlag(ExternalCacheHintMask); }

#if PARALLEL

        //if true then Parallelization of events is not allowed
        //for this object. True for Value and Collection BufferTransformations
        //there events should be sent using the IWorkQueue provided. 
        protected bool GetParallelizationForbidden(ref FlagsState flagsState)
        { return flagsState.GetBitFlag(ParallelizationForbiddenMask); }

        protected bool SetParallelizationForbidden(ref FlagsState flagsState, bool value)
        { return flagsState.SetBitFlag(ParallelizationForbiddenMask, value); }

        protected void SetParallelizationForbidden()
        {
            FlagsState flags;

            while(!GetAndLockFlags(out flags));

            SetParallelizationForbidden(ref flags, true);

            Commit(ref flags);
        }
#endif

        //Try to get a value from the external cache.
        //This method can be called without locking the object.
        //this should not raise an exception
        protected bool TryGetExternalCache(out object cachedValue)
        { return _ReceiverTable.TryGetExternalCache(out cachedValue); }

        //Set a value in the external cache. The object needs to be locked.
        //this should not raise an exception
        protected void SetExternalCache(ref FlagsState flagsState, object cacheValue)
        { SetExternalCacheHint(ref flagsState, _ReceiverTable.SetExternalCache(cacheValue)); }

        //Clears the external cache. 
        protected bool ClearExternalCache(ref FlagsState flagsState)
        {
            bool res = true;

            if (GetExternalCacheHint(ref flagsState))
            {
                if (Lock(ref flagsState))
                {
                    _ReceiverTable.ClearExternalCache();
                    SetExternalCacheHint(ref flagsState, false);
                }
                else
                    res = false;
            }

            return res;
        }

        #region HaveChangedListeningClients

        //True if we have clients listening for change events.
        protected bool GetHaveChangedListeningClients(ref FlagsState flagsState)
        { return flagsState.GetBitFlag(HaveChangedListeningClientsMask); }

        protected bool SetHaveChangedListeningClients(ref FlagsState flagsState, bool value)
        { return flagsState.SetBitFlag(HaveChangedListeningClientsMask, value); }

        #endregion

        #region INotifyChanged Members

        public void SubscribeINC(IReceiveChangeNotification receiver)
        {
            //TODO:pass IsPivotNode and more than 0 registrars in one go.
            if (_ReceiverTable.SubscribeINC(receiver))
            {
                FlagsState flagsState;

            retry:

                while (!GetFlags(out flagsState)) ;

                if (!GetIsPivotNode(ref flagsState) && _ReceiverTable.IsPivotNode)
                {
                    SetIsPivotNode(ref flagsState, true);

                    if (!Commit(ref flagsState))
                        goto retry;
                }

                if (!GetHaveChangedListeningClients(ref flagsState))
                {
                    if (!Lock(ref flagsState))
                        goto retry;

                    try
                    {
                        if (_ReceiverTable.NCReceiverCount != 0)
                        {
                            SetHaveChangedListeningClients(ref flagsState, true);
                            ClientSubscribesEvent(ref flagsState);
                        }
                    }
                    finally
                    {
                        Commit(ref flagsState);
                    }
                }
            }
        }

        protected abstract bool ClientSubscribesEvent(ref FlagsState flagsState); 

        public void UnsubscribeINC(IReceiveChangeNotification receiver)
        {
            if (_ReceiverTable.UnsubscribeINC(receiver) && _ReceiverTable.NCReceiverCount == 0)
            {
                FlagsState flagsState;

                while (!GetAndLockFlags(out flagsState)) ;

                try
                {
                    if (GetHaveChangedListeningClients(ref flagsState) && _ReceiverTable.NCReceiverCount == 0)
                    {
                        SetHaveChangedListeningClients(ref flagsState, false);
                        ClientUnsubscribesEvent(ref flagsState);
                    }
                }
                finally
                {
                    Commit(ref flagsState);
                }

            }
        }

        protected abstract bool ClientUnsubscribesEvent(ref FlagsState flagsState);

        #endregion


        protected void SendMessages(ref FlagsState flagsState, INCEventArgs[] messages)
        {
            if (messages != null)
            {
                foreach (var message in messages)
                    SendMessage(ref flagsState, message);
            }
        }

#if HLG
        struct SendMessage_LoggInfo
        {
            public INCEventArgs Message;
        }
#endif

        protected void SendMessage(ref FlagsState flagsState, INCEventArgs message)
        {
#if HLG
            Logg(
                this,
                new SendMessage_LoggInfo { Message = message }
            );
#endif
#if PARALLEL
            bool parallelForbidden = GetParallelizationForbidden(ref flagsState);   
            _ReceiverTable.SendMessage(this, message, parallelForbidden);
#else
            _ReceiverTable.SendMessage(this, message);
#endif
        }

    }
}
