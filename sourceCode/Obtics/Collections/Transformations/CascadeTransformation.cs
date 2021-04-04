using System;
using SL = System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    internal sealed class CascadeTransformation<TType, TSeq> : OpaqueTransformationBase<TType, IInternalEnumerable<TSeq>, IInternalEnumerable<TSeq>>
        where TSeq : IEnumerable<TType>
    {
        public static IEnumerable<TType> GeneralCreate(IEnumerable<TSeq> s)
        {
            var staticSource = s as StaticEnumerable<TSeq>;

            if (staticSource != null)
            {
                var count = SL.Enumerable.Count(staticSource);

                if (count == 0)
                    return StaticEnumerable<TType>.Create();
                else if (count == 1)
                    return SL.Enumerable.ElementAt(staticSource, 0);
            }

            return Create(s);
        }

        public static CascadeTransformation<TType,TSeq> Create(IEnumerable<TSeq> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<CascadeTransformation<TType, TSeq>, IInternalEnumerable<TSeq>>(source);
        }

        public override IInternalEnumerable<TType> UnorderedForm
        { get { return UnorderedCascadeTransformation<TType, TSeq>.Create(_Prms); } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        class BufferNodeCC : WeightedSkiplist<BufferNodeCC>.Node, IDisposable, IReceiveChangeNotification
        {
            public ValueHybridList<TType> _SubBuffer; 
            public IInternalEnumerable<TType> _Collection;
            public readonly CascadeTransformation<TType,TSeq> _Owner;
            public VersionNumber _ContentVersion;

            public BufferNodeCC(TSeq rawCollection, CascadeTransformation<TType,TSeq> owner)
            {
                var collection = rawCollection.Patched();
                _Collection = collection;
                _Owner = owner;
                _SubBuffer = new ValueHybridList<TType>();

                if (collection != null)
                {
                    collection.SubscribeINC(this);

                    using (IVersionedEnumerator<TType> enm = collection.GetEnumerator())
                    {
                        _ContentVersion = enm.ContentVersion;

                        while (enm.MoveNext())
                            _SubBuffer.Add(enm.Current);
                    }
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                //only privatly accessed.. no lock
                if (_Collection != null)
                    _Collection.UnsubscribeINC(this);

                _Collection = null;
                _SubBuffer = null;

                GC.SuppressFinalize(this);
            }

            #endregion

            #region IReceiveChangeNotification Members

            public void NotifyChanged(object sender, INCEventArgs changeArgs)
            {
                _Owner.ChildChangedEvent(this, changeArgs);
            }

            #endregion
        }

        WeightedSkiplist<BufferNodeCC> _Buffer;

        protected override void ClearBuffer(ref FlagsState flags)
        {
            foreach (BufferNodeCC bncc in _Buffer)
                bncc.Dispose();

            _Buffer = null;
        }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            _Buffer = new WeightedSkiplist<BufferNodeCC>();

            using (var sourceEnumerator = _Prms.GetEnumerator())
            {
                var bnccList = _Buffer;

                while (sourceEnumerator.MoveNext())
                {
                    var newNode = new BufferNodeCC(sourceEnumerator.Current, this);
                    bnccList.Add(newNode, newNode._SubBuffer.Count);
                }

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs collectionEvent, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            message1 = null;

            switch (collectionEvent.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    return AddSourceAction((INCollectionAddEventArgs<TSeq>)collectionEvent, out message2);
                case INCEventArgsTypes.CollectionRemove:
                    return RemoveSourceAction((INCollectionRemoveEventArgs<TSeq>)collectionEvent, out message2);
                case INCEventArgsTypes.CollectionReplace:
                    return ReplaceSourceAction((INCollectionReplaceEventArgs<TSeq>)collectionEvent, out message2);
                default:
                    message1 = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    message2 = null;
                    return Change.Destructive;
            }
        }


        private Change ReplaceSourceAction(INCollectionReplaceEventArgs<TSeq> args, out INCEventArgs[] res)
        {
            var argsIx = args.Index;
            var cc = _Buffer[argsIx];
            var ccb = cc._SubBuffer;
            var ccCount = ccb.Count;
            var itemsBegin = _Buffer.WeightAt(cc) - ccb.Count;

            var nc = new BufferNodeCC(args.NewItem, this);
            var ncb = nc._SubBuffer;
            var ncCount = ncb.Count;

            _Buffer.RemoveAt(argsIx);
            _Buffer.Insert(argsIx, nc, ncCount);

            res = new INCEventArgs[ccCount + ncCount - Math.Min(ccCount,ncCount)];

            int i = 0;


            for (; i < ccCount && i < ncCount; ++i)
                res[i] = INCEventArgs.CollectionReplace(AdvanceContentVersion(), itemsBegin++, ncb[i], ccb[i]);

            for (; i < ccCount; ++i)
                res[i] = INCEventArgs.CollectionRemove(AdvanceContentVersion(), itemsBegin, ccb[i]);

            for (; i < ncCount; ++i)
                res[i] = INCEventArgs.CollectionAdd(AdvanceContentVersion(), itemsBegin++, ncb[i]);

            cc.Dispose();

            return res.Length > 0 ? Change.Controled : Change.None;
        }

        private Change RemoveSourceAction(INCollectionRemoveEventArgs<TSeq> args, out INCEventArgs[] res)
        {
            var argsIx = args.Index;
            var cc = _Buffer[argsIx];
            var ccb = cc._SubBuffer;
            var itemsBegin = _Buffer.WeightAt(cc) - ccb.Count;
            _Buffer.RemoveAt(argsIx);
            var ccCount = ccb.Count;

            res = new INCEventArgs[ccCount];

            for (int i = 0; i < ccCount; ++i)
                res[i] = INCEventArgs.CollectionRemove(AdvanceContentVersion(), itemsBegin, ccb[i]);

            cc.Dispose();

            return ccCount == 0 ? Change.None : Change.Controled;
        }

        private Change AddSourceAction(INCollectionAddEventArgs<TSeq> args, out INCEventArgs[] res )
        {
            var argsIx = args.Index;
            var itemsBegin = argsIx == 0 ? 0 : _Buffer.WeightAt(_Buffer[argsIx-1]) ;
            var cc = new BufferNodeCC(args.Item, this);
            var ccb = cc._SubBuffer;
            var ccCount = ccb.Count;

            res = new INCEventArgs[ccCount];

            for (int i = 0; i < ccCount; ++i)
                res[i] = INCEventArgs.CollectionAdd(AdvanceContentVersion(), itemsBegin + i, ccb[i]);

            _Buffer.Insert(argsIx, cc, ccCount);

            return ccCount == 0 ? Change.None : Change.Controled;
        }

        void ChildChangedEvent(BufferNodeCC bufferNodeCC, INCEventArgs changeArgs)
        {
            if (changeArgs.Type == INCEventArgsTypes.Exception)
                SourceExceptionEvent(_Prms, (INExceptionEventArgs)changeArgs);
            else if(changeArgs.IsCollectionEvent)
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

                INCEventArgs message = null;
                INCEventArgs[] messages = null;

                try
                {
                    if (bufferNodeCC._Collection != null)
                    {
                        var buffer = _Buffer;

                        var ve = (INCollectionChangedEventArgs)changeArgs;

                        var historicRelation = ve.VersionNumber.IsInRelationTo(bufferNodeCC._ContentVersion);

                        if (historicRelation == VersionRelation.Past)
                            return;

                        bufferNodeCC._ContentVersion = ve.VersionNumber;

                        switch (state)
                        {
                            case State.Cached:
                                ClearExternalCache(ref flags);
                                break;

                            case State.Snapshot:
                                TakeSnapshot();
                                break;
                        }

                        if (
                            historicRelation == VersionRelation.Future
                            || ve.Type == INCEventArgsTypes.CollectionReset
                            || state == State.Excepted
                        )
                        {
                            if (state != State.Excepted)
                                ClearBuffer(ref flags);

                            SetState(ref flags, State.Hidden);
                            message = INCEventArgs.CollectionReset(AdvanceContentVersion());
                        }
                        else
                        {
                            try
                            {
                                switch (ProcessIncrementalChildChangeEvent(bufferNodeCC, ve, out message, out messages))
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
                                messages = null;
                                message = INCEventArgs.Exception(ex);
                            }                        
                        }
                    }
                }
                finally
                { Commit(ref flags); }

                if (message != null)
                    SendMessage(ref flags, message);

                if (messages != null)
                    foreach(var m in messages)
                        SendMessage(ref flags, m);
            }
        }

        Change ProcessIncrementalChildChangeEvent(BufferNodeCC bufferNode, INCollectionChangedEventArgs args, out INCEventArgs message, out INCEventArgs[] messages)
        {
            messages = null;

            var sb = bufferNode._SubBuffer;
            var indexAndWeight = _Buffer.IndexOfAndWeightAt(bufferNode);
            var indexOffset = indexAndWeight._Weight - sb.Count;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;
                        var Ix = addArgs.Index;
                        var item = addArgs.Item;

                        sb.Insert(Ix, item);
                        _Buffer.SetWeightAt(indexAndWeight._Index, sb.Count);

                        message = INCEventArgs.CollectionAdd<TType>(
                            AdvanceContentVersion(),
                            indexOffset + Ix,
                            item
                        );
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;
                        var Ix = removeArgs.Index;
                        var item = removeArgs.Item;

                        sb.RemoveAt(Ix);
                        _Buffer.SetWeightAt(indexAndWeight._Index, sb.Count);

                        message = INCEventArgs.CollectionRemove<TType>(
                            AdvanceContentVersion(),
                            indexOffset + Ix,
                            item
                        );
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;
                        var Ix = replaceArgs.Index;
                        var newItem = replaceArgs.NewItem;

                        sb[Ix] = newItem;

                        message = INCEventArgs.CollectionReplace<TType>(
                            AdvanceContentVersion(),
                            indexOffset + Ix,
                            newItem,
                            replaceArgs.OldItem
                        );
                    }
                    break;
                //case NotifyCollectionChangedAction.Reset :
                default:
                    //Exception to the rule. Get Source collection immediately upon receiving a Reset action
                    //Check if there is an actualy changing range. 
                    {
                        var newRange = new List<TType>();

                        using (var enm = bufferNode._Collection.GetEnumerator())
                        {
                            bufferNode._ContentVersion = enm.ContentVersion;

                            while (enm.MoveNext())
                                newRange.Add(enm.Current);
                        }

                        var bufferNode_Count = sb.Count;
                        var newRange_Count = newRange.Count;
                        var comparer = ObticsEqualityComparer<TType>.Default;

                        var upE = 0;
                        var minCount = Math.Min(bufferNode_Count, newRange_Count);

                        for (; upE < minCount && comparer.Equals(sb[upE], newRange[upE]); ++upE) ;

                        var downE = 0;
                        minCount -= upE;

                        for (; downE < minCount && comparer.Equals(sb[bufferNode_Count - downE - 1], newRange[newRange_Count - downE - 1]); ++downE) ;

                        var countN = newRange_Count - downE - upE;
                        var countO = bufferNode_Count - downE - upE;
                        var countM = Math.Min(countO, countN);

                        messages = new INCEventArgs[countN + countO - countM];

                        indexOffset += upE;

                        int i = 0;

                        for (; i < countM; ++i)
                            messages[i] = INCEventArgs.CollectionReplace<TType>(
                                AdvanceContentVersion(),
                                indexOffset++,
                                newRange[upE + i],
                                sb[upE + i]
                            );

                        for (; i < countO; ++i)
                            messages[i] = INCEventArgs.CollectionRemove<TType>(
                                AdvanceContentVersion(),
                                indexOffset,
                                sb[upE + i]
                            );

                        for (; i < countN; ++i)
                            messages[i] = INCEventArgs.CollectionAdd<TType>(
                                AdvanceContentVersion(),
                                indexOffset++,
                                newRange[upE + i]
                            );

                        message = null;

                        sb.RemoveRange(upE, countO);
                        sb.InsertRange(upE, newRange.GetRange(upE, countN));
                        _Buffer.SetWeightAt(indexAndWeight._Index, sb.Count);
                    }
                    break;
            }

            return Change.Controled;
        }

        protected override IEnumerator<TType> GetEnumeratorDirect()
        {
            return
                SL.Enumerable.ToList(
                    SL.Enumerable.SelectMany(
                        _Prms, 
                        i => (IEnumerable<TType>)i ?? SL.Enumerable.Empty<TType>()
                    )
                )
                .GetEnumerator()
            ;
        }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        {
            return
                SL.Enumerable.SelectMany(_Buffer, n => n._SubBuffer).GetEnumerator()
            ;
        }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        {
            return
                SL.Enumerable.ToArray(SL.Enumerable.SelectMany(_Buffer, n => n._SubBuffer))
            ;
        }
    }

}
