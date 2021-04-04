using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;

namespace Obtics.Collections.Transformations
{
    internal sealed class TypeConvertTransformation<TOut> : NCSourcedObjectToVE<TOut, IInternalEnumerable>
    {
        public static TypeConvertTransformation<TOut> Create(IEnumerable s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<TypeConvertTransformation<TOut>, IInternalEnumerable>(source);
        }

        #region Bitflags

        const Int32 StateMask = 3 << NCSourcedObjectToVE<TOut, IInternalEnumerable>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVE<TOut, IInternalEnumerable>.BitFlagIndexEnd;

        #endregion

        [Flags]
        enum State
        {
            Initial = 0,
            Clients = 1,
            Visible = 2,
            Cached = 3
        }

        State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        public override bool IsMostUnordered
        { get { return _Prms.IsMostUnordered; } }

        public override IInternalEnumerable<TOut> UnorderedForm
        { get { return IsMostUnordered ? this : Create(_Prms.UnorderedForm); } }

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
                    _Prms.UnsubscribeINC(this);
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
                                var addArgs = (SingleItemINCollectionEventArgs)collectionEvent;
                                message = INCEventArgs.CollectionAdd(addArgs.VersionNumber, addArgs.Index, (TOut)addArgs.Item);
                                break;
                            case INCEventArgsTypes.CollectionRemove:
                                var removeArgs = (SingleItemINCollectionEventArgs)collectionEvent;
                                message = INCEventArgs.CollectionRemove(removeArgs.VersionNumber, removeArgs.Index, (TOut)removeArgs.Item);
                                break;
                            case INCEventArgsTypes.CollectionReplace:
                                var replaceArgs = (INCollectionReplaceEventArgs)collectionEvent;
                                message =
                                    INCEventArgs.CollectionReplace(
                                        replaceArgs.VersionNumber,
                                        replaceArgs.Index,
                                        (TOut)replaceArgs.NewItem,
                                        (TOut)replaceArgs.OldItem
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


        class Enumerator : IVersionedEnumerator<TOut>
        {
            internal IVersionedEnumerator _Source;

            #region IVersionedEnumerator Members

            VersionNumber IVersionedEnumerator.ContentVersion
            { get { return _Source.ContentVersion; } }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            { get { return (TOut)_Source.Current; } }

            bool IEnumerator.MoveNext()
            { return _Source.MoveNext(); }

            void IEnumerator.Reset()
            { _Source.Reset(); }

            #endregion

            #region IEnumerator<TOut> Members

            TOut IEnumerator<TOut>.Current
            { get { return (TOut)_Source.Current; } }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                var sourceAsDisposable = _Source as IDisposable;

                if (sourceAsDisposable != null)
                    sourceAsDisposable.Dispose();
            }

            #endregion
        }

        IVersionedEnumerator<TOut> BuildEnumerator()
        { return new Enumerator { _Source = _Prms.GetEnumerator() }; }

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

                    _Prms.SubscribeINC(this);
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
                            using (var res = BuildEnumerator())
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
                    return BuildEnumerator();

                default:
                    throw new InvalidOperationException("Unexpected state.");
            }
        }
    }
}
