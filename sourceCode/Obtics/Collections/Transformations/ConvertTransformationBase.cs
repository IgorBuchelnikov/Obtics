using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using SL = System.Linq;
using System;

namespace Obtics.Collections.Transformations
{
    internal abstract class ConvertTransformationBase<TIn, TOut, TSource, TPrms> : NCSourcedObjectToVE<TOut, TPrms>
        where TSource : IInternalEnumerable<TIn>
    {
        #region Bitflags

        const Int32 StateMask = 3 << NCSourcedObjectToVE<TOut, TPrms>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVE<TOut, TPrms>.BitFlagIndexEnd;

        protected new const Int32 BitFlagIndexEnd = NCSourcedObjectToVE<TOut, TPrms>.BitFlagIndexEnd + 2;

        #endregion

        //no hidden state
        //we do not check the order of events. A reset may be received out of order.
        //in that case clients can legaly ignore it and still have a valid image of the contents.
        //
        //no excepted state for the same reason.

        [Flags]
        protected enum State
        {
            Initial = 0,
            Clients = 1,
            Visible = 2,
            Cached = 3
        }

        protected State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        protected State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        protected virtual void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected virtual void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        protected override bool ClientSubscribesEvent(ref FlagsState flagsState)
        {
            //flagsState is locked
            if (GetState(ref flagsState) == State.Initial)
                SetState(ref flagsState, State.Clients);

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref FlagsState flagsState)
        {
            //flagsState is locked

            switch (GetState(ref flagsState))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flagsState))
                        return false;
                    goto case State.Visible;

                case State.Visible:
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

        protected abstract TOut ConvertValue(TIn value);

        protected override void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            INCEventArgs message = null;

            switch (GetState(ref flags))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags))
                        goto retry;

                    SetState(ref flags, State.Visible);

                    if (!Commit(ref flags))
                        goto retry;

                    goto case State.Visible;

                case State.Visible:

                    try
                    {
                        switch (collectionEvent.Type)
                        {
                            case INCEventArgsTypes.CollectionReset:
                                message = collectionEvent;
                                break;
                            case INCEventArgsTypes.CollectionAdd:
                                var addArgs = (INCollectionAddEventArgs<TIn>)collectionEvent;
                                message = INCEventArgs.CollectionAdd(addArgs.VersionNumber, addArgs.Index, ConvertValue(addArgs.Item));
                                break;
                            case INCEventArgsTypes.CollectionRemove:
                                var removeArgs = (INCollectionRemoveEventArgs<TIn>)collectionEvent;
                                message = INCEventArgs.CollectionRemove(removeArgs.VersionNumber, removeArgs.Index, ConvertValue(removeArgs.Item));
                                break;
                            case INCEventArgsTypes.CollectionReplace:
                                var replaceArgs = (INCollectionReplaceEventArgs<TIn>)collectionEvent;
                                message =
                                    INCEventArgs.CollectionReplace(
                                        replaceArgs.VersionNumber,
                                        replaceArgs.Index,
                                        ConvertValue(replaceArgs.NewItem),
                                        ConvertValue(replaceArgs.OldItem)
                                    )
                                ;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        message = INCEventArgs.Exception(ex);
                    }

                    break;

                case State.Initial:
                case State.Clients:
                    break;
            }

            if (message != null)
                SendMessage(ref flags, message);
        }

        protected override void SourceExceptionEvent(object sender, INExceptionEventArgs evt)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flags))
                        goto retry;

                    SetState(ref flags, State.Visible);

                    if (!Commit(ref flags))
                        goto retry;

                    goto case State.Visible;

                case State.Visible:
                    SendMessage(ref flags, evt);
                    break;

                case State.Clients:
                case State.Initial:
                    break;
            }
        }

        internal protected abstract TSource Source { get; }

        internal class Enumerator : IVersionedEnumerator<TOut>
        {
            internal IVersionedEnumerator<TIn> _Source;
            internal ConvertTransformationBase<TIn, TOut, TSource, TPrms> _Owner;

            #region IVersionedEnumerator Members

            VersionNumber IVersionedEnumerator.ContentVersion
            { get { return _Source.ContentVersion; } }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            { get { return _Owner.ConvertValue(_Source.Current); } }

            bool IEnumerator.MoveNext()
            { return _Source.MoveNext(); }

            void IEnumerator.Reset()
            { _Source.Reset(); }

            #endregion

            #region IEnumerator<TOut> Members

            TOut IEnumerator<TOut>.Current
            { get { return _Owner.ConvertValue(_Source.Current); } }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            { _Source.Dispose(); }

            #endregion
        }

        new IVersionedEnumerator<TOut> GetEnumerator()
        { return new Enumerator { _Owner = this, _Source = Source.GetEnumerator() }; }

        protected override IVersionedEnumerator<TOut> GetEnumeratorEvent()
        {
            //not locked
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Clients:
                    if (!Lock(ref flags))
                        goto retry;

                    SubscribeOnSources();
                    SetState(ref flags, State.Visible);

                    if (GetIsPivotNode(ref flags))                        
                        goto case State.Visible;

                    Commit(ref flags);

                    goto case State.Initial;

                case State.Visible:
                    if (GetIsPivotNode(ref flags))
                    {
                        if (!Lock(ref flags))
                            goto retry;

                        try
                        {
                            using(var res = GetEnumerator())
                            {
                                var safeEnum = new SafeEnumeratorProvider(res);

                                base.SetExternalCache(ref flags, safeEnum);

                                SetState(ref flags, State.Cached);

                                return safeEnum.GetEnumerator();
                            }
                        }
                        finally
                        {
                            //locked so commit should succeed
                            Commit(ref flags);
                        }
                    }
                    goto case State.Initial;

                case State.Cached:
                case State.Initial:
                    return GetEnumerator();

                default:
                    throw new InvalidOperationException("Unexpected state.");
            }
        }
    }
}
