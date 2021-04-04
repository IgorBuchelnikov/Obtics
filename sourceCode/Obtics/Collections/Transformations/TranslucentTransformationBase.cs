using System;
using System.Collections.Specialized;
using System.Collections.Generic;


namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Base class for collection transformations
    /// Translucent means that the immediate result depends both on buffered information and source information.
    /// This means that propblems will occure when the source and buffer happen to be out of step.
    /// TranslucentTransformationBase resolves that by sending a Reset collection changed event to the clients if
    /// such a situation is detected.
    /// </summary>
    /// <typeparam name="TOut">The type of the elements of the result collection</typeparam>
    /// <typeparam name="TSource">The type of our source</typeparam>
    /// <typeparam name="TPrms">The type of the parameters struct.</typeparam>
    internal abstract class TranslucentTransformationBase<TOut, TSource, TPrms> : NCSourcedObjectToVE<TOut, TPrms>
        where TSource : IVersionedEnumerable
    {
        protected enum State
        {
            Initial = 0,
            Clients = 1,
            Visible = 2,
            Cached = 3,
            Hidden = 4,
            Excepted = 5
        }

        #region Bitflags

        const Int32 StateMask = 7 << NCSourcedObjectToVE<TOut, TPrms>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVE<TOut, TPrms>.BitFlagIndexEnd;

        protected new const Int32 BitFlagIndexEnd = NCSourcedObjectToVE<TOut, TPrms>.BitFlagIndexEnd + 3;

        #endregion

        protected VersionNumber _SourceContentVersion;
        protected VersionNumber _ContentVersion;


        protected VersionNumber AdvanceContentVersion()
        { return _ContentVersion = _ContentVersion.Next; }

        protected static State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        protected static State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        protected abstract void SubscribeOnSources();
        protected abstract void UnsubscribeFromSources();

        protected virtual void ClearBuffer(ref FlagsState flags)
        { }

        protected abstract VersionNumber InitializeBuffer(ref FlagsState flags);

        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flags)
        {
            if (GetState(ref flags) == State.Initial)
                SetState(ref flags, State.Clients);

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flags)
        {
            switch (GetState(ref flags))
            {
                case State.Cached:
                    ClearExternalCache(ref flags);
                    goto case State.Visible;

                case State.Visible:
                    ClearBuffer(ref flags);
                    goto case State.Hidden;

                case State.Excepted:
                case State.Hidden:
                    UnsubscribeFromSources();
                    goto case State.Clients;

                case State.Clients:
                    SetState(ref flags, State.Initial);
                    break;

                case State.Initial:
                    break;
            }

            return true;
        }

        protected override void SourceExceptionEvent(object sender, INExceptionEventArgs args)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags))
                        goto retry;

                    goto case State.Visible;

                case State.Visible:
                    if (!Lock(ref flags))
                        goto retry;

                    ClearBuffer(ref flags);
                    goto case State.Hidden;

                case State.Hidden:

                    SetState(ref flags, State.Excepted);
                    if (!Commit(ref flags))
                        goto retry;

                    goto case State.Excepted;

                case State.Excepted:
                    SendMessage(ref flags, args);
                    break;

                case State.Initial:
                case State.Clients:
                    break;
            }
        }


        protected enum Change { None, Controled, Destructive };

        protected override void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Initial:
                case State.Hidden:
                case State.Clients:
                    return;
            }

            if (!Lock(ref flags))
                goto retry;

            INCEventArgs message1 = null;
            INCEventArgs[] message2 = null;

            try
            {
                var historicRelation = collectionEvent.VersionNumber.IsInRelationTo(_SourceContentVersion);

                if (historicRelation != VersionRelation.Past)
                {
                    _SourceContentVersion = collectionEvent.VersionNumber;

                    if(state == State.Cached)
                        ClearExternalCache(ref flags);

                    if (
                        historicRelation == VersionRelation.Future
                        || collectionEvent.Type == INCEventArgsTypes.CollectionReset
                        || state == State.Excepted
                    )
                    {
                        if (state != State.Excepted)
                            ClearBuffer(ref flags);

                        SetState(ref flags, State.Hidden);
                        message1 = INCEventArgs.CollectionReset(_ContentVersion = _ContentVersion.Next);
                    }
                    else
                    {
                        try
                        {
                            switch (ProcessIncrementalChangeEvent(ref flags, collectionEvent, out message1, out message2))
                            {
                                case Change.None:
                                case Change.Controled:
                                    SetState(ref flags, State.Visible);
                                    break;
                                case Change.Destructive:
                                    ClearBuffer(ref flags);
                                    SetState(ref flags, State.Hidden);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            ClearBuffer(ref flags);
                            SetState(ref flags, State.Excepted);
                            message1 = INCEventArgs.Exception(ex);
                            message2 = null;
                        }
                    }
                }
            }
            finally
            { Commit(ref flags); }

            if (message1 != null)
                SendMessage(ref flags, message1);

            if (message2 != null)
                foreach (var message in message2)
                    SendMessage(ref flags, message);
        }

        protected abstract Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs collectionEvent, out INCEventArgs message1, out INCEventArgs[] message2);

        protected abstract IEnumerator<TOut> GetEnumeratorDirect();
        protected abstract Tuple<bool,IEnumerator<TOut>> GetEnumeratorFromBuffer();

        protected override IVersionedEnumerator<TOut> GetEnumeratorEvent()
        {
            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            try
            {
                switch (GetState(ref flags))
                {
                    case State.Excepted:
                    case State.Cached:
                    case State.Initial:
                        return VersionedEnumerator.WithContentVersion(GetEnumeratorDirect(), _ContentVersion); //because we access _ContentVersion we need to be locked here

                    case State.Clients:
                        SubscribeOnSources();
                        _ContentVersion = _ContentVersion.Next;
                        SetState(ref flags, State.Hidden);
                        goto case State.Hidden;

                    case State.Hidden:
                        _SourceContentVersion = InitializeBuffer(ref flags);                        
                        SetState(ref flags, State.Visible);
                        goto case State.Visible;
                    
                    case State.Visible:
                        var enm = GetEnumeratorFromBuffer();

                        if (enm.First)
                        {
                            if (GetIsPivotNode(ref flags))
                            {
                                var safeEnum = new SafeEnumeratorProvider(enm.Second, _ContentVersion);

                                base.SetExternalCache(ref flags, safeEnum);

                                SetState(ref flags, State.Cached);

                                return safeEnum.GetEnumerator();
                            }

                            return VersionedEnumerator.WithContentVersion(enm.Second, _ContentVersion);
                        }
                        else
                        {
                            ClearExternalCache(ref flags);
                            SetState(ref flags, State.Excepted);
                            return 
                                VersionedEnumerator.WithContentVersion(
                                    enm.Second ?? GetEnumeratorDirect(), 
                                    _ContentVersion
                                )
                            ;
                        }
                    default:
                        throw new Exception("Unexpected State.");
                }
            }
            catch (Exception)
            {
                switch (GetState(ref flags))
                {
                    case State.Initial:
                    case State.Clients:
                    case State.Excepted:
                        break;

                    case State.Cached:
                        ClearExternalCache(ref flags);
                        goto case State.Visible;

                    case State.Visible:
                        ClearBuffer(ref flags);
                        goto case State.Hidden;

                    case State.Hidden:
                        SetState(ref flags, State.Excepted);
                        break;
                }

                throw;
            }
            finally
            { Commit(ref flags); }
        }
    }
}
