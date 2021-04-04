using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections;
using System.Collections.Specialized;
using System.Collections;

namespace Obtics.Values.Transformations
{
    internal sealed class RangeTransformation : NCSourcedObjectToVE<int,Tuple<IInternalValueProvider<int>,IInternalValueProvider<int>>>
    {
        enum State
        {
            Initial = 0,
            Clients = 1,
            Visible = 2,
            Hidden = 3,
            Excepted = 4,
            Cached = 5
        }


        #region Bitflags

        const Int32 StateMask = 7 << NCSourcedObjectToVE<int, Tuple<IInternalValueProvider<int>, IInternalValueProvider<int>>>.BitFlagIndexEnd;
        const Int32 StateOffset = NCSourcedObjectToVE<int, Tuple<IInternalValueProvider<int>, IInternalValueProvider<int>>>.BitFlagIndexEnd;

        #endregion

        State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        public static RangeTransformation Create(IInternalValueProvider<int> begin, IInternalValueProvider<int> end)
        {
            if (begin == null || end == null)
                return null;

            return Carrousel.Get<RangeTransformation, Tuple<IInternalValueProvider<int>, IInternalValueProvider<int>>>(Tuple.Create(begin, end));
        }

        public override bool IsMostUnordered
        { get { return true; } }

        #region IVersionedEnumerable<TType> Members

        VersionNumber _ContentVersion;

        #endregion

        #region SourcePropertyChangedEventHandler

        protected override void SourceValueChangeEvent(object sender)
        {
            INCEventArgs[] res = null;

            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            try
            {
                switch (GetState(ref flags))
                {
                    case State.Visible:

                        Tuple<int, int> oldRange = _BeginBuffer < _EndBuffer ? Tuple.Create(_BeginBuffer, _EndBuffer) : Tuple.Create(_EndBuffer, _BeginBuffer);
                        Tuple<int, int> newRange;

                        if (object.ReferenceEquals(sender, _Prms.First))
                        {
                            //begin
                            var newBegin = _Prms.First.Value;

                            newRange = newBegin < _EndBuffer ? Tuple.Create(newBegin, _EndBuffer) : Tuple.Create(_EndBuffer, newBegin);

                            _BeginBuffer = newBegin;
                        }
                        else if (object.ReferenceEquals(sender, _Prms.Second))
                        {
                            //end
                            var newEnd = _Prms.Second.Value;

                            newRange = _BeginBuffer < newEnd ? Tuple.Create(_BeginBuffer, newEnd) : Tuple.Create(newEnd, _BeginBuffer);

                            _EndBuffer = newEnd;
                        }
                        else
                        {
                            res = null;
                            break;
                        }

                        if (newRange.First != oldRange.First)
                        {
                            if (newRange.Second != oldRange.Second)
                            {
                                SetState(ref flags, State.Hidden);
                                res = new INCEventArgs[] { INCEventArgs.CollectionReset(_ContentVersion = _ContentVersion.Next) };
                            }
                            else if (newRange.First < oldRange.First)
                                _ContentVersion = AddRange(_ContentVersion, newRange.First, oldRange.First, 0, out res);
                            else
                                _ContentVersion = RemoveRange(_ContentVersion, oldRange.First, newRange.First, 0, out res);
                        }
                        else if (newRange.Second != oldRange.Second)
                        {
                            if (newRange.Second < oldRange.Second)
                                _ContentVersion = RemoveRange(_ContentVersion, newRange.Second, oldRange.Second, newRange.Second - oldRange.First, out res);
                            else
                                _ContentVersion = AddRange(_ContentVersion, oldRange.Second, newRange.Second, oldRange.Second - oldRange.First, out res);
                        }
                        break;

                    case State.Excepted :
                        SetState(ref flags, State.Hidden);
                        res = new INCEventArgs[] { INCEventArgs.CollectionReset(_ContentVersion = _ContentVersion.Next) };
                        break;

                    default:
                        res = null;
                        break;
                }
            }
            finally
            {
                Commit(ref flags);
            }

            if(res != null)
                SendMessages(ref flags, res);
        }

        #endregion

        protected override void SourceExceptionEvent(object sender, INExceptionEventArgs changeArgs)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            switch (GetState(ref flags))
            {
                case State.Hidden:
                case State.Visible:
                    SetState(ref flags, State.Excepted);
                    if (!Commit(ref flags))
                        goto retry;
                    goto case State.Excepted;
                case State.Excepted:
                    SendMessage(ref flags, changeArgs);
                    break;
            }
        }

        protected override bool ClientSubscribesEvent(ref FlagsState flagsState)
        {
            if (GetState(ref flagsState) == State.Initial)
                SetState(ref flagsState, State.Clients);

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref FlagsState flagsState)
        {
            switch (GetState(ref flagsState))
            {
                case State.Cached:
                    if (!ClearExternalCache(ref flagsState))
                        return false;
                    goto case State.Visible;

                case State.Visible:
                case State.Excepted:
                case State.Hidden:
                    _Prms.First.UnsubscribeINC(this);
                    _Prms.Second.UnsubscribeINC(this);
                    goto case State.Clients;

                case State.Clients:
                    SetState(ref flagsState, State.Initial);
                    break;
            }

            return true;
        }

        new class SafeEnumeratorProvider : IVersionedEnumerable<int>
        {
            public int _Begin;
            public int _End;
            public VersionNumber SN;


            public IVersionedEnumerator<int> GetEnumerator()
            { return GetRangeEnumerator(_Begin, _End, SN); }

            IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
            { return this.GetEnumerator(); }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            { return this.GetEnumerator(); }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            { return this.GetEnumerator(); }
        }

        protected override IVersionedEnumerator<int> GetEnumeratorEvent()
        {
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
                    case State.Excepted:
                    case State.Initial:
                        return
                            GetRangeEnumerator(_Prms.First.Value, _Prms.Second.Value, _ContentVersion);

                    case State.Clients:

                        _Prms.First.SubscribeINC(this);
                        _Prms.Second.SubscribeINC(this);
                        _ContentVersion = _ContentVersion.Next;
                        SetState(ref flags, state = State.Hidden);

                        goto case State.Hidden;

                    case State.Hidden:

                        _BeginBuffer = _Prms.First.Value;
                        _EndBuffer = _Prms.Second.Value;

                        SetState(ref flags, state = State.Visible);
                        goto case State.Visible;

                    case State.Visible:

                        IVersionedEnumerator<int> res;

                        if (GetIsPivotNode(ref flags))
                        {
                            var safeEnum = new SafeEnumeratorProvider{ _Begin = _BeginBuffer, _End = _EndBuffer, SN = _ContentVersion };

                            base.SetExternalCache(ref flags, safeEnum);

                            SetState(ref flags, state = State.Cached);

                            res = safeEnum.GetEnumerator();
                        }
                        else
                            res = GetRangeEnumerator(_BeginBuffer, _EndBuffer, _ContentVersion);

                        Commit(ref flags);
                        return res;

                    default:
                        throw new Exception("Unexpected State value.");
                }
            }
            catch (Exception)
            {
                switch (state)
                {
                    case State.Cached:
                        if (!Lock(ref flags))
                            goto retry;

                        ClearExternalCache(ref flags);
                        goto case State.Hidden;

                    case State.Visible:
                    case State.Hidden:
                        SetState(ref flags, State.Excepted);
                        goto case State.Clients;

                    case State.Clients:
                        Commit(ref flags);
                        break;
                }

                throw;
            }

        }

        static Obtics.Collections.IVersionedEnumerator<int> GetRangeEnumerator(int begin, int end, VersionNumber sn)
        {
            return Obtics.Collections.VersionedEnumerator.WithContentVersion(end > begin ? System.Linq.Enumerable.Range(begin, end - begin) : System.Linq.Enumerable.Range(end, begin - end), sn);
        }

        int _BeginBuffer;
        int _EndBuffer;

        VersionNumber AddRange(VersionNumber ccn, int begin, int end, int ix, out INCEventArgs[] args)
        {
            args = new INCEventArgs[end - begin];

            for (int i = begin; i < end; ++i)
                args[i - begin] = INCEventArgs.CollectionAdd(ccn = ccn.Next, ix++, i);

            return ccn;
        }

        VersionNumber RemoveRange(VersionNumber ccn, int begin, int end, int ix, out INCEventArgs[] args)
        {
            args = new INCEventArgs[end - begin];

            for (int i = begin; i < end; ++i)
                args[i - begin] = INCEventArgs.CollectionRemove(ccn = ccn.Next, ix, i);

            return ccn;
        }
    }
}
