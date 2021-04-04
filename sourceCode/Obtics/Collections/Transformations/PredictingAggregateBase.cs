using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using Obtics.Values;
using System.Diagnostics;


namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Base class for aggregates that can update their result values based on CollectionChange information.
    /// They only need to know how the collection changes instead of using the entire collection to
    /// recalculate the value.
    /// </summary>
    /// <typeparam name="TOut">The type of the calculated aggregate value.</typeparam>
    /// <typeparam name="TPrms">The type of the parameters struct.</typeparam>
    internal abstract class PredictingAggregateBase<TOut, TPrms> : NCSourcedObjectToVP<TOut, TPrms>
    {
        protected enum State
        {
            Initial = 0,
            Clients = 1,
            Visible = 2,
            Cached = 3,
            HiddenKnown = 4,
            HiddenUnknown = 5,
            Excepted = 6
        }

        #region Bitflags

        const Int32 StateMask = 7 << NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd;

        protected new const Int32 BitFlagIndexEnd = NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd + 3;

        #endregion

        protected State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        protected State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        protected abstract void SubscribeOnSources();
        protected abstract void UnsubscribeFromSources();

        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            //flagsState is locked
            if (GetState(ref flagsState) == State.Initial)
                SetState(ref flagsState, State.Clients);

            return true;
        }

        protected virtual void ClearBuffer(ref FlagsState flags)
        { }

        protected abstract VersionNumber InitializeBuffer(ref FlagsState flags);
        protected abstract TOut GetValueDirect();
        protected abstract TOut GetValueFromBuffer(ref FlagsState flags);

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            //flagsState is locked

            switch (GetState(ref flagsState))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flagsState))
                        return false;
                    goto case State.Visible;

                case State.HiddenKnown:
                case State.Visible:
                    ClearBuffer(ref flagsState);
                    goto case State.HiddenUnknown;

                case State.Excepted:
                case State.HiddenUnknown:
                    UnsubscribeFromSources();
                    goto case State.Clients;

                case State.Clients:
                    SetState(ref flagsState, State.Initial);
                    break;

                case State.Initial:
                    break;

                default:
                    throw new InvalidOperationException("Unexpected state");
            }


            return true;
        }


        #region Source property changed listeners

        VersionNumber _SourceContentVersion;

        protected enum Change
        {
            None,
            Controled,
            Destructive
        }

#if HLG
        struct SourceCollectionEvent_LoggInfo
        {
            public INCollectionChangedEventArgs CollectionEvent;
            public State State;
        }
#endif

        protected override void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Visible:
                case State.HiddenKnown:
                case State.Cached:
                    if (!Lock(ref flags))
                        goto retry;

                    break;
            }

#if HLG
            Logg(
                this,
                new SourceCollectionEvent_LoggInfo { CollectionEvent = collectionEvent, State = state }
            );
#endif

            try
            {

                switch (state)
                {
                    case State.Cached:
                        base.ClearExternalCache(ref flags);
                        state = SetState(ref flags, State.Visible);
                        goto case State.Visible;

                    case State.Visible:
                        switch(ProcessSourceChangedNotification(ref flags, sender, collectionEvent))
                        {
                            case Change.Destructive:
                                ClearBuffer(ref flags);
                                state = SetState(ref flags, State.HiddenUnknown);
                                Commit(ref flags);
                                SendMessage(ref flags, INCEventArgs.PropertyChanged());
                                break;
                            case Change.Controled:
                                state = SetState(ref flags, State.HiddenKnown);
                                Commit(ref flags);
                                SendMessage(ref flags, INCEventArgs.PropertyChanged());
                                break;
                            case Change.None:
                                Commit(ref flags);
                                break;
                        }
                        break;

                    case State.HiddenKnown:
                        if (ProcessSourceChangedNotification(ref flags, sender, collectionEvent) == Change.Destructive)
                        {
                            ClearBuffer(ref flags);
                            state = SetState(ref flags, State.HiddenUnknown);
                        }

                        Commit(ref flags);
                        break;

                    case State.Excepted:
                        state = SetState(ref flags, State.HiddenUnknown);

                        if (!Commit(ref flags))
                            goto retry;

                        SendMessage(ref flags, INCEventArgs.PropertyChanged());
                        break;

                    case State.HiddenUnknown:
                    case State.Clients:
                    case State.Initial:
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected State.");
                }
            }
            catch (Exception ex)
            {
                switch(state)
                {
                    case State.Cached:
                        base.ClearExternalCache(ref flags);
                        goto case State.Visible;

                    case State.HiddenKnown:
                    case State.Visible:
                        ClearBuffer(ref flags);                        
                        SetState(ref flags, State.Excepted);
                        Commit(ref flags);
                        SendMessage(ref flags, INCEventArgs.Exception(ex));
                        break;
                    default:
                        throw;
                }
            }
        }

        Change ProcessSourceChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args)
        {
            switch (args.VersionNumber.IsInRelationTo(_SourceContentVersion))
            {
                case VersionRelation.Future:
                    if (args.Type != INCEventArgsTypes.CollectionReset)
                        args = INCEventArgs.CollectionReset(args.VersionNumber);
                    break;
                case VersionRelation.Past:
                    return Change.None;
            }

            _SourceContentVersion = args.VersionNumber;

            return ProcessSourceCollectionChangedNotification(ref flags, sender, args);
        }

        /// <summary>
        /// Override in derived classes to implement processing of collection change notifications.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="flags"></param>
        protected abstract Change ProcessSourceCollectionChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args);

        #endregion

        protected override void SourceExceptionEvent(object sender, INCEventArgs evt)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags)) //will lock flags
                        goto retry;
                    goto case State.HiddenKnown;

                case State.Visible:
                case State.HiddenKnown:
                    if (!Lock(ref flags))
                        goto retry;
                    ClearBuffer(ref flags);
                    goto case State.HiddenUnknown;

                case State.HiddenUnknown:
                    SetState(ref flags, State.Excepted);
                    if (!Commit(ref flags))
                        goto retry;
                    goto case State.Excepted;

                case State.Excepted:
                    SendMessage(ref flags, evt);
                    break;

                case State.Clients:
                case State.Initial:
                    break;

                default:
                    throw new InvalidOperationException("Unexpected State.");
            }
        }

#if HLG
        struct GetValueEvent_LoggInfo
        {
            public State State;
        }
#endif

        protected override TOut GetValueEvent()
        {
            //not locked
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);


            switch (state)
            {
                case State.Clients:
                case State.HiddenUnknown:
                case State.HiddenKnown:
                case State.Visible:
                    if (!Lock(ref flags))
                        goto retry;
                    break;
            }

#if HLG
            Logg(
                this,
                new GetValueEvent_LoggInfo { State = state }
            );
#endif
            try
            {
                switch (GetState(ref flags))
                {
                    case State.Cached:
                    case State.Initial:
                    case State.Excepted:
                        return GetValueDirect();

                    case State.Clients:
                        SubscribeOnSources();
                        SetState(ref flags, state = State.HiddenUnknown);
                        goto case State.HiddenUnknown;

                    case State.HiddenUnknown:
                        _SourceContentVersion = InitializeBuffer(ref flags);
                        goto case State.HiddenKnown;

                    case State.HiddenKnown:
                        SetState(ref flags, state = State.Visible);
                        goto case State.Visible;

                    case State.Visible:
                        TOut res = GetValueFromBuffer(ref flags);

                        if (GetIsPivotNode(ref flags))
                        {
                            base.SetExternalCache(ref flags, res);
                            SetState(ref flags, state = State.Cached);
                        }

                        Commit(ref flags);

                        return res;

                    default:
                        throw new InvalidOperationException("Unexpected state");
                }
            }
            catch (Exception)
            {
                switch (state)
                {
                    case State.HiddenKnown:
                    case State.Visible:
                        ClearBuffer(ref flags);
                        goto case State.HiddenUnknown;

                    case State.HiddenUnknown:
                        SetState(ref flags, State.Excepted);
                        goto case State.Clients;

                    case State.Clients:
                        Commit(ref flags);
                        break;
                }

                throw;
            }
        }
    }
}
