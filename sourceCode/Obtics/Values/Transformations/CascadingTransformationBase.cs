using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Obtics.Values.Transformations
{
    internal abstract class CascadingTransformationBase<TOut, TItm, TPrms> : NCSourcedObjectToVP<TOut, TPrms>
    {
        #region Bitflags

        const Int32 StateMask = 7 << NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd;

        protected new const Int32 BitFlagIndexEnd = NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd + 3;

        #endregion

        protected enum State
        {
            Initial = 0,
            Clients = 1,
            Read1Prepared = 2,
            Visible = 3,
            Read1Failed = 4,
            Cached = 5,
            Hidden1 = 6,
            Hidden2 = 7
        }

        State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        protected abstract void SubscribeOnSources();

        protected abstract void UnsubscribeFromSources();

        protected abstract void SubscribeOnItm(TItm itm);

        protected abstract void UnsubscribeFromItm(TItm itm);

        protected TItm _Buffer;

        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            //flagsState is locked
            if (GetState(ref flagsState) == State.Initial)
                SetState(ref flagsState, State.Clients);

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            //flagsState is locked

            switch (GetState(ref flagsState))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flagsState))
                        return false;
                    goto case State.Visible;

                case State.Hidden2:
                case State.Visible:
                    UnsubscribeFromItm(_Buffer);
                    _Buffer = default(TItm);
                    goto case State.Hidden1;

                case State.Hidden1:
                case State.Read1Failed:
                    UnsubscribeFromSources();                    
                    goto case State.Clients;
                    
                case State.Clients:
                    SetState(ref flagsState, State.Initial);
                    break;

                case State.Initial:
                    break;

                case State.Read1Prepared: //shouldn't because of lock.
                default:
                    throw new InvalidOperationException("Unexpected state");
            }

            return true;
        }

        protected override void SourceChangeEvent(object sender)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var oldState = GetState(ref flags);

            switch (oldState)
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags)) //locks
                        goto retry;

                    goto case State.Visible;

                case State.Hidden2:
                case State.Visible:
                    if (!Lock(ref flags))
                        goto retry;

                    UnsubscribeFromItm(_Buffer);
                    _Buffer = default(TItm);
                    goto case State.Read1Failed;

                case State.Read1Failed:
                    SetState(ref flags, State.Hidden1);
                    if (!Commit(ref flags))
                        goto retry;

                    if(oldState != State.Hidden2)
                        SendMessage(ref flags, INCEventArgs.PropertyChanged());

                    break;

                case State.Read1Prepared:
                    Thread.Sleep(0);
                    goto retry;

                case State.Hidden1:
                case State.Clients:
                case State.Initial:
                    break;

                default:
                    throw new InvalidOperationException("Unexpected state");
            }
        }

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
                    goto case State.Visible;

                case State.Hidden2:
                case State.Visible:
                    if (!Lock(ref flags))
                        goto retry;

                    UnsubscribeFromItm(_Buffer);
                    _Buffer = default(TItm);
                    goto case State.Hidden1;

                case State.Hidden1:
                    SetState(ref flags, State.Read1Failed);
                    if (!Commit(ref flags))
                        goto retry;

                    goto case State.Read1Failed ;

                case State.Read1Failed:
                    SendMessage(ref flags, evt);
                    break;

                case State.Read1Prepared:
                    Thread.Sleep(0);
                    goto retry;

                case State.Clients:
                case State.Initial:
                    break;

                default:
                    throw new InvalidOperationException("Unexpected state");
            }
        }

        protected void ItmChangeEvent(object sender)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags)) //locks
                        goto retry;

                    goto case State.Visible;

                case State.Visible:
                    SetState(ref flags, State.Hidden2);
                    if (!Commit(ref flags))
                        goto retry;
                    SendMessage(ref flags, INCEventArgs.PropertyChanged());
                    break;

                case State.Read1Prepared:
                    Thread.Sleep(0);
                    goto retry;

                case State.Hidden2:
                case State.Hidden1:
                case State.Read1Failed:
                case State.Clients:
                case State.Initial:
                    break;

                default:
                    throw new InvalidOperationException("Unexpected state");
            }
        }

        protected void ItmExceptionEvent(object sender, INCEventArgs evt)
        { 
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags)) //locks
                        goto retry;
                    goto case State.Hidden2;

                case State.Hidden2:
                    SetState(ref flags, State.Visible);
                    if (!Commit(ref flags))
                        goto retry;
                    goto case State.Visible;

                case State.Visible:
                    SendMessage(ref flags, evt);
                    break;

                case State.Read1Prepared:
                    Thread.Sleep(0);
                    goto retry;

                case State.Hidden1:
                case State.Read1Failed:
                case State.Clients:
                case State.Initial:
                    break;

                default:
                    throw new InvalidOperationException("Unexpected state");
            }
        }

        protected override TOut GetValueEvent()
        {
            //not locked
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Hidden2:
                    SetState(ref flags, State.Visible);
                    goto case State.Visible;

                case State.Cached:
                case State.Visible:
                    if (!Lock(ref flags))
                        goto retry;

                    if (GetIsPivotNode(ref flags))
                    {
                        try
                        {
                            var res = GetValueFromItm(_Buffer);

                            SetExternalCache(ref flags, res);
                            SetState(ref flags, State.Cached);

                            return res;
                        }
                        finally
                        {
                            //locked so commit should succeed
                            Commit(ref flags);
                        }
                    }
                    else
                    {
                        var itm = _Buffer;
                        Commit(ref flags);

                        return GetValueFromItm(itm);
                    }

                case State.Clients:
                    if (!Lock(ref flags))
                        goto retry;

                    SubscribeOnSources();
                    goto case State.Hidden1;

                case State.Read1Failed:
                case State.Hidden1:
                    SetState(ref flags, State.Read1Prepared);
                    if (!Commit(ref flags))
                        goto retry;

                    try
                    {
                        var itm = GetItmFromSource();

                        while (!GetAndLockFlags(out flags)) ;

                        _Buffer = itm;
                        SubscribeOnItm(itm);
                        SetState(ref flags, State.Visible);
                        goto case State.Visible;
                    }
                    catch
                    {
                        while (true)
                        {
                            if (GetFlags(out flags))
                            {
                                SetState(ref flags, State.Read1Failed);

                                if (Commit(ref flags))
                                    break;
                            }
                        }

                        throw;
                    }
                    

                case State.Initial:
                    return GetValueFromItm(GetItmFromSource());

                case State.Read1Prepared:
                    Thread.Sleep(0);
                    goto retry;
                
                default:
                    throw new InvalidOperationException("Unexpected state");
            }
        }

        protected abstract TOut GetValueFromItm(TItm itm);

        protected abstract TItm GetItmFromSource();
    }
}
