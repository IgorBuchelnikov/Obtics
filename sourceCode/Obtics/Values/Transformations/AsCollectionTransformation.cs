using System;
using System.Collections.Generic;
using Obtics.Collections;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Obtics.Values.Transformations
{
    internal abstract class AsCollectionTransformationBase<TType, TPrms> : NCSourcedObjectToVE<TType, TPrms>
    {
        protected enum State
        {
            Initial = 0,
            Clients = 1,
            Visible = 2,
            Hidden = 3,
            Excepted = 4,
            Cached = 5
        }


        #region Bitflags

        const Int32 StateMask = 7 << NCSourcedObjectToVE<TType, TPrms>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVE<TType, TPrms>.BitFlagIndexEnd;

        protected new const Int32 BitFlagIndexEnd = NCSourcedObjectToVE<TType, TPrms>.BitFlagIndexEnd + 3;

        #endregion

        protected State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        protected State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        #region IVersionedEnumerable<TType> Members

        VersionNumber _ContentVersion;

        #endregion

        #region SourcePropertyChangedEventHandler

        protected override void SourceValueChangeEvent(object sender)
        {
            DelayedActionRegistry.Register(
                () =>
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

                        case State.Excepted:
                        case State.Visible:
                            if (!Lock(ref flags))
                                goto retry;
                            var msg = INCEventArgs.CollectionReset(_ContentVersion = _ContentVersion.Next);
                            SetState(ref flags, State.Hidden);
                            Commit(ref flags);
                            SendMessage(ref flags, msg);
                            break;

                        default:
                            msg = null;
                            break;
                    }
                }
            );
        }

        #endregion

        protected override IVersionedEnumerator<TType> GetEnumeratorEvent()
        {
            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            try
            {
                switch (GetState(ref flags))
                {
                    case State.Excepted:
                    case State.Initial:
                        return VersionedEnumerator.WithContentVersion(BuildEnumerator(), _ContentVersion);

                    case State.Clients:

                        Source.SubscribeINC(this);
                        _ContentVersion = _ContentVersion.Next;

                        goto case State.Hidden;

                    
                    case State.Hidden:

                        SetState(ref flags, State.Visible);
                        goto case State.Visible;

                    case State.Cached:
                    case State.Visible:

                        if (GetIsPivotNode(ref flags))
                        {
                            using (var res = BuildEnumerator())
                            {
                                var safeEnum = new SafeEnumeratorProvider(res, _ContentVersion);

                                base.SetExternalCache(ref flags, safeEnum);

                                SetState(ref flags, State.Cached);

                                return safeEnum.GetEnumerator();
                            }
                        }
                        else
                            return VersionedEnumerator.WithContentVersion(BuildEnumerator(), _ContentVersion);

                    default:
                        throw new Exception("Unexpected State value.");
                }
            }
            catch (Exception)
            {
                switch (GetState(ref flags))
                {
                    case State.Cached:
                        ClearExternalCache(ref flags);
                        goto case State.Visible;

                    case State.Hidden :
                    case State.Visible :
                        SetState(ref flags, State.Excepted);
                        break;
                }

                throw;
            }
            finally
            {
                Commit(ref flags);
            }
        }

        protected override void SourceExceptionEvent(object sender, INExceptionEventArgs changeArgs)
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

                case State.Hidden:
                case State.Visible :
                    SetState(ref flags, State.Excepted);
                    if (!Commit(ref flags))
                        goto retry;
                    goto case State.Excepted;
                case State.Excepted :
                    SendMessage(ref flags, changeArgs);
                    break;
            }
        }

        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            if (GetState(ref flagsState) == State.Initial)
                SetState(ref flagsState, State.Clients);

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<TPrms>.FlagsState flagsState)
        {
            switch (GetState(ref flagsState))
            {
                case State.Cached :
                    if (!ClearExternalCache(ref flagsState))
                        return false;
                    goto case State.Visible;

                case State.Visible :
                case State.Excepted:
                case State.Hidden:
                    Source.UnsubscribeINC(this);
                    goto case State.Clients;

                case State.Clients:
                    SetState(ref flagsState, State.Initial);
                    break;
            }

            return true;
        }

        protected abstract IInternalValueProvider<TType> Source { get; }

        protected abstract IEnumerator<TType> BuildEnumerator();
    }

    /// <summary>
    /// AsCollectionNullableTransformation represents an IValueProvider as a Sequence (IEnumerable) with
    /// exactly one or no members. The result sequence will have no member if and only if 'predicate'
    /// returns false for the Value property of the source.
    /// </summary>
    /// <typeparam name="TType">Type of the Value property of the source and the members of the result sequence.</typeparam>
    internal sealed class AsCollectionNullableTransformation<TType> : AsCollectionTransformationBase<TType, Tuple<IInternalValueProvider<TType>, Func<TType, bool>>>
    {
        public static IEnumerable<TType> GeneralCreate(IInternalValueProvider<TType> source, Func<TType, bool> predicate)
        {
            var sourceAsStatic = source as StaticValueProvider<TType>;

            if (sourceAsStatic != null)
            {
                if (predicate == null)
                    return null;

                var val = sourceAsStatic.Value;
                return predicate(val) ? StaticEnumerable<TType>.Create(val) : StaticEnumerable<TType>.Create();
            }

            return Create(source, predicate);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        public static AsCollectionNullableTransformation<TType> Create(IInternalValueProvider<TType> source, Func<TType, bool> predicate)
        {
            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<AsCollectionNullableTransformation<TType>, IInternalValueProvider<TType>, Func<TType, bool>>(source, predicate);
        }

        protected override IInternalValueProvider<TType> Source
        {
            get { return _Prms.First; }
        }

        protected override IEnumerator<TType> BuildEnumerator()
        {
            var value = _Prms.First.Value;
            if (_Prms.Second(value))
                yield return value;
        }
    }

    /// <summary>
    /// AsCollectionTransformation represents an IValueProvider as a Sequence (IEnumerable) with
    /// exactly one member. 
    /// </summary>
    /// <typeparam name="TType">Type of the Value property of the source and the members of the result sequence.</typeparam>
    internal sealed class AsCollectionTransformation<TType> : AsCollectionTransformationBase<TType, IInternalValueProvider<TType>>
    {
        public static IEnumerable<TType> GeneralCreate(IInternalValueProvider<TType> source)
        {
            var sourceAsStatic = source as StaticValueProvider<TType>;

            if (sourceAsStatic != null)
                return StaticEnumerable<TType>.Create(sourceAsStatic.Value);

            return Create(source);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        public static AsCollectionTransformation<TType> Create(IInternalValueProvider<TType> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<AsCollectionTransformation<TType>, IInternalValueProvider<TType>>(source);
        }

        protected override IInternalValueProvider<TType> Source
        { get { return _Prms; } }

        protected override IEnumerator<TType> BuildEnumerator()
        { yield return _Prms.Value; }
    }
}
