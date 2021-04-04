using System;
using SL = System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    internal sealed class UnorderedCascadeTransformation<TType, TSeq> : OpaqueTransformationBase<TType, IInternalEnumerable<TSeq>, IInternalEnumerable<TSeq>>
        where TSeq : IEnumerable<TType>
    {
        public static UnorderedCascadeTransformation<TType,TSeq> Create(IEnumerable<TSeq> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<UnorderedCascadeTransformation<TType, TSeq>, IInternalEnumerable<TSeq>>(source);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        class BufferNodeCC : Dictionary<Tuple<TType>,int>, IDisposable, IReceiveChangeNotification
        {
            public IInternalEnumerable<TType> _Collection;
            public readonly UnorderedCascadeTransformation<TType,TSeq> _Owner;
            public VersionNumber? _ContentVersion;
            public int _Count;

            public BufferNodeCC(TSeq rawCollection, UnorderedCascadeTransformation<TType,TSeq> owner)
            {
                var collection = rawCollection.PatchedUnordered();
                _Collection = collection;
                _Owner = owner;
                _Count = 1;

                if (collection != null)
                {
                    collection.SubscribeINC(this);

                    using (IVersionedEnumerator<TType> enm = collection.GetEnumerator())
                    {
                        _ContentVersion = enm.ContentVersion;

                        while (enm.MoveNext())
                        {
                            var current = Tuple.Create(enm.Current);

                            if (!ContainsKey(current))
                                Add(current, 1);
                            else
                                this[current] += 1;
                        }
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
                _Count = 0;

                GC.SuppressFinalize(this);
            }

            #endregion

            #region IReceiveChangeNotification Members

            public void NotifyChanged(object sender, INCEventArgs changeArgs)
            {
                if(changeArgs.IsCollectionEvent || changeArgs.IsExeptionEvent)
                    _Owner.ChildChangedEvent(this, changeArgs);
            }

            #endregion
        }

        Dictionary<TSeq,BufferNodeCC> _Buffer;

        protected override void ClearBuffer(ref FlagsState flags)
        {
            foreach (BufferNodeCC bncc in _Buffer.Values)
                bncc.Dispose();

            _Buffer = null;
        }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            _Buffer = new Dictionary<TSeq, BufferNodeCC>();

            using (var sourceEnumerator = _Prms.GetEnumerator())
            {
                var bnccList = _Buffer;

                while (sourceEnumerator.MoveNext())
                {
                    var current = sourceEnumerator.Current;
                    BufferNodeCC bncc;

                    if (!bnccList.TryGetValue(current, out bncc))
                        bnccList.Add(current, new BufferNodeCC(current, this));
                    else
                        ++bncc._Count;
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

        private static void RemoveSequence(Dictionary<TSeq, BufferNodeCC> buffer, TSeq child, List<TType> oldItemsRange)
        {
            BufferNodeCC bncc = buffer[child];

            foreach (var kvp in bncc)
                for (int j = 0, jEnd = kvp.Value; j < jEnd; ++j)
                    oldItemsRange.Add(kvp.Key.First);

            if (--bncc._Count == 0)
            {
                buffer.Remove(child);
                bncc.Dispose();
            }
        }

        private void AddSequence(Dictionary<TSeq, BufferNodeCC> buffer, TSeq child, List<TType> newItemsRange)
        {
            BufferNodeCC bncc;

            if (!buffer.TryGetValue(child, out bncc))
                buffer.Add(child, bncc = new BufferNodeCC(child, this));
            else
                ++bncc._Count;

            foreach (var kvp in bncc)
                for (int j = 0, jEnd = kvp.Value; j < jEnd; ++j)
                    newItemsRange.Add(kvp.Key.First);
        }


        private Change ReplaceSourceAction(INCollectionReplaceEventArgs<TSeq> args, out INCEventArgs[] res)
        {
            Dictionary<TSeq, BufferNodeCC> buffer = _Buffer;

            var oldItemsRange = new List<TType>();
            var newItemsRange = new List<TType>();

            RemoveSequence(buffer, args.OldItem, oldItemsRange);
            AddSequence(buffer, args.NewItem, newItemsRange);

            var countO = oldItemsRange.Count;
            var countN = newItemsRange.Count;
            var countR = Math.Min(countN, countO);
            var totalCount = countO + countN - countR;

            if (totalCount != 0)
            {
                res = new INCEventArgs[totalCount];

                int i = 0;

                for (; i < countR; ++i)
                    res[i] = INCEventArgs.CollectionReplace(
                        AdvanceContentVersion(),
                        -1,
                        newItemsRange[i],
                        oldItemsRange[i]
                    );

                for (; i < countO; ++i)
                    res[i] = INCEventArgs.CollectionRemove(
                        AdvanceContentVersion(),
                        -1,
                        oldItemsRange[i]
                    );

                for (; i < countN; ++i)
                    res[i] = INCEventArgs.CollectionAdd(
                        AdvanceContentVersion(),
                        -1,
                        newItemsRange[i]
                    );

                return Change.Controled;
            }

            res = null;

            return Change.None;
        }

        private Change RemoveSourceAction(INCollectionRemoveEventArgs<TSeq> args, out INCEventArgs[] res)
        {
            Dictionary<TSeq, BufferNodeCC> buffer = _Buffer;

            var oldItemsRange = new List<TType>();

            RemoveSequence(buffer, args.Item, oldItemsRange);

            if (oldItemsRange.Count > 0)
            {
                res = new INCEventArgs[oldItemsRange.Count];

                for (int i = 0, end = oldItemsRange.Count; i < end; ++i)
                    res[i] =
                        INCEventArgs.CollectionRemove(
                            AdvanceContentVersion(),
                            -1,
                            oldItemsRange[i]
                        );

                return Change.Controled;
            }

            res = null;

            return Change.None;
        }

        private Change AddSourceAction(INCollectionAddEventArgs<TSeq> args, out INCEventArgs[] res)
        {
            Dictionary<TSeq, BufferNodeCC> buffer = _Buffer;

            var newItemsRange = new List<TType>();

            AddSequence(buffer, args.Item, newItemsRange);

            if (newItemsRange.Count > 0)
            {
                res = new INCEventArgs[newItemsRange.Count];

                for (int i = 0, end = newItemsRange.Count; i < end; ++i)
                    res[i] =
                        INCEventArgs.CollectionAdd(
                            AdvanceContentVersion(),
                            -1,
                            newItemsRange[i]
                        );

                return Change.Controled;
            }

            res = null;

            return Change.None;
        }

        void ChildChangedEvent(BufferNodeCC bufferNodeCC, INCEventArgs changeArgs)
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
                if (changeArgs.Type == INCEventArgsTypes.Exception)
                {
                    switch (state)
                    {
                        case State.Snapshot:
                            TakeSnapshot();
                            goto preVisible;

                        case State.Cached:
                            ClearExternalCache(ref flags);

                        preVisible:
                            SetState(ref flags, State.Visible);
                            goto case State.Visible;

                        case State.Visible:
                            AdvanceContentVersion();
                            //not changing state
                            goto case State.Excepted;

                        case State.Excepted:
                            message = changeArgs;
                            break;
                    }
                }
                else if (bufferNodeCC._Collection != null)
                {
                    var buffer = _Buffer;

                    if (bufferNodeCC._Count == 0)
                        return;

                    var ve = (INCollectionChangedEventArgs)changeArgs;

                    var historicRelation = ve.VersionNumber.IsInRelationTo(bufferNodeCC._ContentVersion);

                    if (historicRelation == VersionRelation.Past)
                        return;

                    bufferNodeCC._ContentVersion = ve.VersionNumber;

                    switch (state)
                    {
                        case State.Cached:
                            ClearExternalCache(ref flags);
                            SetState(ref flags, state = State.Visible);
                            break;

                        case State.Snapshot:
                            TakeSnapshot();
                            SetState(ref flags, state = State.Visible);
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

                        SetState(ref flags, state = State.Hidden);
                        message = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    }
                    else
                    {
                        try
                        {
                            ProcessIncrementalChildChangeEvent(bufferNodeCC, ve, ref message, ref messages);                            
                        }
                        catch (Exception ex)
                        {
                            //buffer may be corrupted.. clear it. client needs to read it again
                            //to have it built up.
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
                foreach (var m in messages)
                    SendMessage(ref flags, m);
        }

        void ProcessIncrementalChildChangeEvent(BufferNodeCC bufferNode, INCollectionChangedEventArgs args, ref INCEventArgs message, ref INCEventArgs[] messages)
        {
            var buffer = _Buffer;

            var bufferNode_Count = bufferNode._Count;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;

                        TType item = addArgs.Item;
                        var key = Tuple.Create(item);

                        if (!bufferNode.ContainsKey(key))
                            bufferNode.Add(key, 1);
                        else
                            ++bufferNode[key];

                        if (bufferNode_Count > 1)
                        {
                            messages = new INCEventArgs[bufferNode_Count];

                            for (int i = 0; i < bufferNode_Count; ++i)
                                messages[i] =
                                    INCEventArgs.CollectionAdd(
                                        AdvanceContentVersion(),
                                        -1,
                                        item
                                    );
                        }
                        else
                            message = addArgs.Clone(AdvanceContentVersion());
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;

                        TType item = removeArgs.Item;
                        var key = Tuple.Create(item);

                        if (--bufferNode[key] == 0)
                            bufferNode.Remove(key);

                        if (bufferNode_Count > 1)
                        {
                            messages = new INCEventArgs[bufferNode_Count];

                            for (int i = 0; i < bufferNode_Count; ++i)
                                messages[i] =
                                    INCEventArgs.CollectionRemove(
                                        AdvanceContentVersion(),
                                        -1,
                                        item
                                    );
                        }
                        else
                            message = removeArgs.Clone(AdvanceContentVersion());
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;

                        TType newItem = replaceArgs.NewItem;
                        TType oldItem = replaceArgs.OldItem;

                        var newKey = Tuple.Create(newItem);
                        var oldKey = Tuple.Create(oldItem);

                        if (--bufferNode[oldKey] == 0)
                            bufferNode.Remove(oldKey);

                        if (!bufferNode.ContainsKey(newKey))
                            bufferNode.Add(newKey, 1);
                        else
                            ++bufferNode[newKey];

                        if (bufferNode_Count > 1)
                        {
                            messages = new INCEventArgs[bufferNode_Count];

                            for (int i = 0; i < bufferNode_Count; ++i)
                                messages[i] =
                                    INCEventArgs.CollectionReplace(
                                        AdvanceContentVersion(),
                                        -1,
                                        newItem,
                                        oldItem
                                    );
                        }
                        else
                            message = replaceArgs.Clone(AdvanceContentVersion());
                    }
                    break;
                //case NotifyCollectionChangedAction.Reset :
                default:
                    //Exception to the rule. Get Source collection immediately upon receiving a Reset action
                    //Check if there is an actualy changing range. 
                    {
                        var testRange = new List<KeyValuePair<Tuple<TType>, int>>(bufferNode);

                        using (var enm = bufferNode._Collection.GetEnumerator())
                        {
                            bufferNode._ContentVersion = enm.ContentVersion;

                            while (enm.MoveNext())
                            {
                                var current = Tuple.Create(enm.Current);

                                if (!bufferNode.ContainsKey(current))
                                {
                                    bufferNode.Add(current, 1);
                                    testRange.Add(new KeyValuePair<Tuple<TType>, int>(current, 0));
                                }
                                else
                                    ++bufferNode[current];
                            }
                        }

                        var newRange = new List<TType>();
                        var oldRange = new List<TType>();

                        foreach (var kvp in testRange)
                        {
                            var newCount = bufferNode[kvp.Key] - kvp.Value;

                            if (newCount == 0)
                                bufferNode.Remove(kvp.Key);
                            else
                                bufferNode[kvp.Key] = newCount;

                            if (newCount > kvp.Value)
                                for (int i = 0, end = newCount - kvp.Value; i < end; ++i)
                                    newRange.Add(kvp.Key.First);
                            else
                                for (int i = 0, end = kvp.Value - newCount; i < end; ++i)
                                    oldRange.Add(kvp.Key.First);
                        }

                        var countN = newRange.Count;
                        var countO = oldRange.Count;

                        if (countN + countO != 0)
                        {

                            messages = new INCEventArgs[(countO + countN) * bufferNode_Count];
                            var w = 0;

                            for (int i = 0; i < countO; ++i)
                            {
                                var item = oldRange[i];

                                for (int j = 0; j < bufferNode_Count; ++j)
                                    messages[w++] =
                                        INCEventArgs.CollectionRemove(
                                            AdvanceContentVersion(),
                                            -1,
                                            item
                                        );
                            }

                            for (int i = 0; i < countN; ++i)
                            {
                                var item = newRange[i];

                                for (int j = 0; j < bufferNode_Count; ++j)
                                    messages[w++] =
                                        INCEventArgs.CollectionAdd(
                                            AdvanceContentVersion(),
                                            -1,
                                            item
                                        );
                            }

                        }
                    }
                    break;
            }
        }

        private static IEnumerable<TType> RepeatedRange(int count, IEnumerable<TType> items)
        { return SL.Enumerable.SelectMany(SL.Enumerable.Repeat(items, count), m => m); }

        IEnumerable<TType> BufferEnumerable()
        {
            foreach (var kvp in _Buffer)
                foreach (var ikvp in kvp.Value)
                    for (int j = 0, jEnd = ikvp.Value * kvp.Value._Count; j < jEnd; ++j )
                        yield return ikvp.Key.First;
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
        { return BufferEnumerable().GetEnumerator(); }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        { return SL.Enumerable.ToArray(BufferEnumerable()) ; }
    }
}
