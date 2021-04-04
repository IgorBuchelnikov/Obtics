using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Values.Transformations
{
    internal abstract class ConvertTransformationBase<TOut, TPrms> : NCSourcedObjectToVP<TOut, TPrms>
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
            Cached = 5
        }

        protected State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        protected State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        protected abstract void SubscribeOnSources();

        protected abstract void UnsubscribeFromSources();

        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            //flagsState is locked
            if (GetState(ref flagsState) == ConvertTransformationBase<TOut, TPrms>.State.Initial)
                SetState(ref flagsState, ConvertTransformationBase<TOut, TPrms>.State.Clients);

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            //flagsState is locked

            switch (GetState(ref flagsState))
            {
                case ConvertTransformationBase<TOut,TPrms>.State.Cached:
                    if (!ClearExternalCache(ref flagsState))
                        return false;
                    goto case ConvertTransformationBase<TOut, TPrms>.State.Visible;

                case ConvertTransformationBase<TOut, TPrms>.State.Hidden:
                case ConvertTransformationBase<TOut, TPrms>.State.Visible:
                    UnsubscribeFromSources();
                    goto case ConvertTransformationBase<TOut, TPrms>.State.Clients;

                case ConvertTransformationBase<TOut, TPrms>.State.Clients:
                    SetState(ref flagsState, ConvertTransformationBase<TOut, TPrms>.State.Initial);
                    break;

                case ConvertTransformationBase<TOut, TPrms>.State.Initial:
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

            while(!GetFlags(out flags));

            switch (GetState(ref flags))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags))
                        goto retry;
                    goto case State.Visible;

                case State.Visible:
                    SetState(ref flags, State.Hidden);
                    if (!Commit(ref flags))
                        goto retry;
                    SendMessage(ref flags, INCEventArgs.PropertyChanged());
                    break;
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
                    goto case State.Hidden;

                case State.Hidden:
                    SetState(ref flags, State.Visible);
                    if (!Commit(ref flags))
                        goto retry;
                    goto case State.Visible;

                case State.Visible:
                    SendMessage(ref flags, evt);
                    break;
            }
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
                case State.Clients:
                case State.Hidden:
                case State.Visible:
                    if (!Lock(ref flags))
                        goto retry;
                    break;
            }

            try
            {
                switch (GetState(ref flags))
                {
                    case State.Cached:
                    case State.Initial:
                        return GetValue();

                    case State.Clients:
                        SubscribeOnSources();
                        goto case State.Hidden;

                    case State.Hidden:
                        SetState(ref flags, state = State.Visible);
                        goto case State.Visible;

                    case State.Visible:
                        var res = GetValue();

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
                    case State.Hidden:
                        SetState(ref flags, State.Visible);
                        goto case State.Visible;

                    case State.Clients:
                    case State.Visible:
                        Commit(ref flags);
                        break;
                }

                throw;
            }
        }

        protected abstract TOut GetValue();
    }
}
