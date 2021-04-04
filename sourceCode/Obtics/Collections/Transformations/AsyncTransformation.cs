using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using Obtics.Async;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    internal sealed class AsyncTransformation<TItem> : ConvertTransformationBase<TItem, TItem, IInternalEnumerable<TItem>, Tuple<IInternalEnumerable<TItem>, IWorkQueue>>
    {
        #region Bitflags
        const Int32 ResetIsPendingMask = 1 << (ConvertTransformationBase<TItem, TItem, IInternalEnumerable<TItem>, Tuple<IInternalEnumerable<TItem>, IWorkQueue>>.BitFlagIndexEnd + 0);
        const Int32 PSCCNIsPendingMask = 1 << (ConvertTransformationBase<TItem, TItem, IInternalEnumerable<TItem>, Tuple<IInternalEnumerable<TItem>, IWorkQueue>>.BitFlagIndexEnd + 1);
        #endregion

        public static AsyncTransformation<TItem> Create(IEnumerable<TItem> s, IWorkQueue workQueue)
        {
            var source = s.Patched();

            if (source == null || workQueue == null)
                return null;

            return Carrousel.Get<AsyncTransformation<TItem>, IInternalEnumerable<TItem>, IWorkQueue>(source, workQueue);
        }

        internal override void Initialize(Tuple<IInternalEnumerable<TItem>, IWorkQueue> prms)
        {
#if PARALLEL
            SetParallelizationForbidden();
#endif
            base.Initialize(prms);
        }

        public override bool IsMostUnordered
        { get { return _Prms.First.IsMostUnordered; } }

        public override IInternalEnumerable<TItem> UnorderedForm
        { get { return AsyncTransformation<TItem>.Create(_Prms.First.UnorderedForm, _Prms.Second); } }


        internal protected override IInternalEnumerable<TItem> Source
        { get { return _Prms.First; } }

        INCollectionChangedEventArgs[] _CollectionChangedQueue;
        int _QueueStart;
        int _QueueCount;

        void Grow()
        {
            if (_CollectionChangedQueue == null)
            {
                _CollectionChangedQueue = new INCollectionChangedEventArgs[16];
                _QueueStart = 0;
                _QueueCount = 0;
            }

            if (_QueueCount == _CollectionChangedQueue.Length)
            {
                var newQueue = new INCollectionChangedEventArgs[_QueueCount << 1];
                var newIndex = 0;

                for (int oldIndex = _QueueStart; oldIndex != _QueueCount; ++oldIndex)
                    newQueue[newIndex++] = _CollectionChangedQueue[oldIndex];

                for (int oldIndex = 0; oldIndex != _QueueStart; ++oldIndex)
                    newQueue[newIndex++] = _CollectionChangedQueue[oldIndex];

                _CollectionChangedQueue = newQueue;
                _QueueStart = 0;
            }
        }

        static int ChangeEffect(INCollectionChangedEventArgs args)
        { return args.Type == INCEventArgsTypes.CollectionReset ? -1 : 1; }

        void Enqueue(INCollectionChangedEventArgs sargs)
        {
            Grow();

            unchecked
            {
                var mask = _CollectionChangedQueue.Length - 1;
                var insertPoint = (_QueueStart + _QueueCount) & mask;

                if (sargs != null)
                {
                    var thisSN = sargs.VersionNumber;

                    while (((_QueueStart - insertPoint) & mask) != 0)
                    {
                        var prevInsertPoint = (insertPoint - 1) & mask;

                        var prevSargs = _CollectionChangedQueue[prevInsertPoint];

                        if (prevSargs == null || prevSargs.VersionNumber.IsInRelationTo(thisSN) == VersionRelation.Past)
                            break;

                        _CollectionChangedQueue[insertPoint] = prevSargs;

                        insertPoint = prevInsertPoint;
                    }
                }

                _CollectionChangedQueue[insertPoint] = sargs;
                ++_QueueCount;
            }
        }

        int QueueCount
        { get { return _CollectionChangedQueue == null ? 0 : _QueueCount; } }

        void ClearQueue()
        {
            _CollectionChangedQueue = null;
        }

        INCollectionChangedEventArgs Dequeue()
        {
            int queueCount = QueueCount;

            if (queueCount == 0)
                return null;

            var mask = _CollectionChangedQueue.Length - 1;
            var res = _CollectionChangedQueue[_QueueStart];
            _QueueStart = (_QueueStart + 1) & mask;
            --_QueueCount;

            if (_QueueCount == 0)
                _CollectionChangedQueue = null;

            return res;
        }

        bool GetResetIsPending(ref FlagsState flags)
        { return flags.GetBitFlag(ResetIsPendingMask); }

        bool SetResetIsPending(ref FlagsState flags, bool value)
        { return flags.SetBitFlag(ResetIsPendingMask, value); }


        bool GetPSCCNIsPending(ref FlagsState flags)
        { return flags.GetBitFlag(PSCCNIsPendingMask); }

        bool SetPSCCNIsPending(ref FlagsState flags, bool value)
        { return flags.SetBitFlag(PSCCNIsPendingMask, value); }

        static void PSCCNCallBack(object me)
        { ((AsyncTransformation<TItem>)me).PSCCNCallBack(); }

        private void PSCCNCallBack()
        {
            FlagsState flags;

            while (true)
            {
                INCollectionChangedEventArgs message = null;
                while (!GetAndLockFlags(out flags)) ;

                try
                {
                    message = Dequeue();

                    if (GetResetIsPending(ref flags))
                        SetResetIsPending(ref flags, false);

                    if (message == null)
                    {
                        ClearQueue();
                        SetPSCCNIsPending(ref flags, false); //no more work
                        break;
                    }
                }
                finally
                { Commit(ref flags); }

                if (message != null)
                    base.SourceCollectionEvent(_Prms.First, message);
            }
        }


        protected override void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        {
            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            try
            {
                if (collectionEvent.Type == INCEventArgsTypes.CollectionReset || GetResetIsPending(ref flags))
                {
                    SetResetIsPending(ref flags, true);

                    //make sure to always have the latest versionnumber for our reset event.
                    VersionNumber newestNumber = collectionEvent.VersionNumber;

                    while (true)
                    {
                        var evt = Dequeue();

                        if (evt == null)
                            break;

                        if (newestNumber.IsInRelationTo(evt.VersionNumber) == VersionRelation.Past)
                            newestNumber = evt.VersionNumber;
                    }

                    Enqueue(INCEventArgs.CollectionReset(newestNumber));
                }
                else
                    Enqueue(collectionEvent);

                if (!GetPSCCNIsPending(ref flags))
                {
                    SetPSCCNIsPending(ref flags, true);
                    _Prms.Second.QueueWorkItem(PSCCNCallBack, this);
                }
            }
            finally
            {
                Commit(ref flags);
            }

        }

        protected override TItem ConvertValue(TItem value)
        { return value; }
    }
}
