using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Values.Transformations
{
    internal abstract class CachedTransformationBase<TOut, TPrms> : NCSourcedObjectToVP<TOut, TPrms>
    {
        #region Bitflags

        const Int32 StateMask = 7 << NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd;

        protected new const Int32 BitFlagIndexEnd = NCSourcedObjectToVP<TOut, TPrms>.BitFlagIndexEnd + 3;

        #endregion

        [Flags]
        protected enum State
        {
            Initial = 0,
            Clients = 2,
            Visible = 1,
            Hidden = 3,
            Cached = 4,
            Excepted = 5
        }
        protected State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        protected State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        protected abstract void SubscribeOnSources();

        protected abstract void UnsubscribeFromSources();

        protected virtual void ClearBuffer()
        { }

        protected abstract void InitializeBuffer();
        protected abstract TOut GetValueDirect();
        protected abstract TOut GetValueFromBuffer();

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

                case State.Visible:
                    ClearBuffer();
                    goto case State.Hidden;

                case State.Excepted:
                case State.Hidden:
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

        protected override void SourceChangeEvent(object sender)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Hidden:
                case State.Clients:
                case State.Initial:
                    return;
            }

            INCEventArgs message = null;

            if (!Lock(ref flags))
                goto retry;

            try
            {
                switch (state)
                {
                    case State.Cached:
                        base.ClearExternalCache(ref flags);
                        SetState(ref flags, state = State.Visible);
                        goto case State.Visible;

                    case State.Visible:
                        ClearBuffer();
                        goto case State.Excepted;

                    case State.Excepted:
                        SetState(ref flags, state = State.Hidden);
                        message = INCEventArgs.PropertyChanged();
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected State.");
                }
            }
            catch (Exception ex)
            {
                switch (state)
                {
                    case State.Visible:
                        ClearBuffer();
                        SetState(ref flags, State.Excepted);
                        break;
                }

                message = INCEventArgs.Exception(ex);
            }
            finally
            { Commit(ref flags); }

            if (message != null)
                SendMessage(ref flags, message);
        }

        protected override void SourceExceptionEvent(object sender, INCEventArgs evt)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Clients:
                case State.Initial:
                    return;
            }

            INCEventArgs message = null;

            if (!Lock(ref flags))
                goto retry;

            try
            {
                switch (GetState(ref flags))
                {
                    case State.Cached:
                        ClearExternalCache(ref flags);
                        SetState(ref flags, state = State.Visible);
                        goto case State.Visible;

                    case State.Visible:
                        ClearBuffer();
                        goto case State.Hidden;

                    case State.Hidden:
                        SetState(ref flags, state = State.Excepted);
                        goto case State.Excepted;

                    case State.Excepted:
                        message = evt;
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected State.");
                }
            }
            finally
            { Commit(ref flags); }

            if (message != null)
                SendMessage(ref flags, message);
        }

        protected override TOut GetValueEvent()
        {
            //not locked
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Excepted:
                case State.Cached:
                case State.Initial:
                    return GetValueDirect();
            }

            if (!Lock(ref flags))
                goto retry;

            try
            {
                switch (state)
                {
                    case State.Clients:
                        SubscribeOnSources();
                        SetState(ref flags, state = State.Hidden);
                        goto case State.Hidden;

                    case State.Hidden:
                        InitializeBuffer();
                        SetState(ref flags, state = State.Visible);
                        goto case State.Visible;

                    case State.Visible:
                        TOut res = GetValueFromBuffer();

                        if (GetIsPivotNode(ref flags))
                        {
                            base.SetExternalCache(ref flags, res);
                            SetState(ref flags, state = State.Cached);
                        }
                        return res;

                    default:
                        throw new InvalidOperationException("Unexpected state");
                }
            }
            catch (Exception)
            {
                switch (state)
                {
                    case State.Visible:
                        ClearBuffer();
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
