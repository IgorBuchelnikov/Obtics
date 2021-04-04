using System;
using System.Collections.Generic;
using SL = System.Linq;
using SLE = System.Linq.Enumerable;
using System.Text;
using TvdP.Collections;

namespace Obtics.Collections.Transformations
{
    internal class UnorderedBoundGroupFilterDispenser<TType, TKey> : ObservableObjectBase<Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>>, IReceiveChangeNotification
    {
        protected enum State
        {
            Initial = 0,
            Clients = 1,
            Visible = 2,
            Hidden = 3,
            Excepted = 4
        }

        #region Bitflags

        const Int32 StateMask = 7 << ObservableObjectBase<Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>>.BitFlagIndexEnd;
        const Int32 StateOffset = ObservableObjectBase<Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>>.BitFlagIndexEnd;

#if PARALLEL
        const Int32 ParallelizationForbiddenMask = 1 << (ObservableObjectBase<Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>>.BitFlagIndexEnd + 3);

        protected new const Int32 BitFlagIndexEnd = ObservableObjectBase<Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>>.BitFlagIndexEnd + 4;
#else

        protected new const Int32 BitFlagIndexEnd = ObservableObjectBase<Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>>.BitFlagIndexEnd + 3;
#endif
        #endregion

        protected static State GetState(ref FlagsState flagsState)
        { return (State)flagsState.GetNum(StateMask, StateOffset); }

        protected static State SetState(ref FlagsState flagsState, State state)
        { return (State)flagsState.SetNum(StateMask, StateOffset, (int)state); }

        static int GetKeyHash(TKey key, IEqualityComparer<TKey> equalityComparer )
        { return _KeyIsNullable && key == null ? 0 : equalityComparer.GetHashCode(key); }

        class GroupHead : SortedSkiplist<GroupHead,int>.Node, IInternalEnumerable<TType>, SL.IGrouping<TKey, TType>, INotifyChanged
        {
            public VersionNumber _ContentVersion;
            public Tuple<UnorderedBoundGroupFilterDispenser<TType, TKey>, TKey> _Prms;
            public int _KeyHash;

            public GroupHead(Tuple<UnorderedBoundGroupFilterDispenser<TType, TKey>, TKey> prms)
            {
                _Prms = prms;
                _KeyHash = GetKeyHash(_Prms.Second, prms.First._Prms.Second);
            }

            //should always have only 1 receiver.
            public IReceiveChangeNotification _Receiver;

            #region IVersionedEnumerable<TType> Members

            IEnumerator<TType> GetEnumerationSegment(ItemSkipList buffer)
            {
                var key = _Prms.Second;
                var comparer = _Prms.First._Prms.Second;

                using (var enm = buffer.FindAll(GetKeyHash(key, comparer)))
                {
                    while (enm.MoveNext())
                    {
                        var current = enm.Current;
                        if (comparer.Equals(key, current._Key))
                            yield return current._Value;
                    }
                }
            }

            public IVersionedEnumerator<TType> GetEnumerator()
            {
                var owner = _Prms.First;

                FlagsState flags;

                while (!owner.GetAndLockFlags(out flags));

                try
                {
                    switch (GetState(ref flags))
                    {
                        case State.Excepted:
                            //return
                            //    VersionedEnumerator.WithContentVersion(
                            //        SLE.Empty<TType>(),
                            //        _ContentVersion
                            //    )
                            //;

                        case State.Initial:
                            return
                                VersionedEnumerator.WithContentVersion(
                                    SLE.Select(SLE.Where(owner._Prms.First, t => owner._Prms.Second.Equals(t.Second, _Prms.Second)), t => t.First),
                                    _ContentVersion
                                );

                        case State.Clients :
                            owner._Prms.First.SubscribeINC(owner);
                            foreach (var head in owner._HeadList)
                                head._ContentVersion = head._ContentVersion.Next;
                            goto case State.Hidden ;

                        case State.Hidden:
                            owner._Buffer = new ItemSkipList { _KeyEqualityComparer = owner._Prms.Second };

                            using (var sourceEnumerator = owner._Prms.First.GetEnumerator())
                            {
                                owner._SourceContentVersion = sourceEnumerator.ContentVersion;

                                while (sourceEnumerator.MoveNext())
                                    owner._Buffer.Add(sourceEnumerator.Current);
                            }

                            SetState(ref flags, State.Visible);
                            goto case State.Visible ;

                        case State.Visible:
                            return
                                XLazySnapshotEnumerator.Create(
                                    this,
                                    _ContentVersion,
                                    GetEnumerationSegment(owner._Buffer)
                                )
                            ;                        

                        default:
                            throw new Exception("Unexpected enum value");
                    }                    
                }
                catch
                {
                    switch (GetState(ref flags))
                    {
                        case State.Visible:
                            owner._Buffer = null;
                            goto case State.Hidden;

                        case State.Hidden:
                            SetState(ref flags, State.Excepted);
                            break;
                    }

                    throw;
                }
                finally
                {
                    owner.Commit(ref flags);
                }
            }

            #endregion

            #region IVersionedEnumerable Members

            public VersionNumber ContentVersion
            {
                get
                {
                    var owner = _Prms.First;
                    FlagsState flags;

                    while (!owner.GetAndLockFlags(out flags)) ;

                    try
                    {
                        return _ContentVersion;
                    }
                    finally
                    {
                        owner.Commit(ref flags);
                    }
                }
            }

            IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
            { return this.GetEnumerator(); }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            { return this.GetEnumerator(); }

            #endregion

            #region IEnumerable<TType> Members

            IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
            { return this.GetEnumerator(); }

            #endregion

            internal void TakeSnapshot()
            {
                XLazySnapshotEnumerator.TakeSnapshot(
                    this,
                    _ContentVersion,
                    () => SLE.ToArray(CollectionsHelper.EnumerableFromEnumerator(GetEnumerationSegment(_Prms.First._Buffer)))
                );
            }

            #region INotifyChanged Members


            public void SubscribeINC(IReceiveChangeNotification receiver)
            {
                var owner = _Prms.First;

                FlagsState flags;

                while (!owner.GetAndLockFlags(out flags)) ;

                try
                {
                    _Receiver = receiver;
                    owner._HeadList.Add(this);

                    if (owner._HeadList.Count == 1)
                    {
                        switch (GetState(ref flags))
                        {
                            case State.Initial:
                                SetState(ref flags, State.Clients);
                                break;

                            case State.Excepted:
                            case State.Hidden:
                            case State.Visible:
                                _ContentVersion = _ContentVersion.Next;
                                break;
                        }
                    }
                }
                finally
                {
                    owner.Commit(ref flags);
                }
            }

            public void UnsubscribeINC(IReceiveChangeNotification receiver)
            {
                var owner = _Prms.First;

                FlagsState flags;

                while (!owner.GetAndLockFlags(out flags)) ;

                try
                {
                    _Receiver = null;
                    TakeSnapshot();

                    owner._HeadList.Remove(this);

                    if (owner._HeadList.Count == 0)
                    {
                        switch (GetState(ref flags))
                        {
                            case State.Visible:
                                owner._Buffer = null;
                                goto case State.Hidden;

                            case State.Excepted:
                            case State.Hidden:
                                owner._Prms.First.UnsubscribeINC(owner);
                                goto case State.Clients;

                            case State.Clients:
                                SetState(ref flags, State.Initial);
                                break;
                        }
                    }

                }
                finally
                {
                    owner.Commit(ref flags);
                }
            }

            #endregion

            #region IGrouping<TKey,TType> Members

            public TKey Key
            { get { return _Prms.Second; } }

            #endregion

            #region IInternalEnumerable<TType> Members

            public IInternalEnumerable<TType> UnorderedForm
            { get { return this; } }

            #endregion

            #region IInternalEnumerable Members

            IInternalEnumerable IInternalEnumerable.UnorderedForm
            { get { return this; } }

            public bool IsMostUnordered
            { get { return true; } }

            public bool HasSafeEnumerator
            { get { return false; } }

            #endregion
        }

        static bool _KeyIsNullable = typeof(TKey).IsByRef;

        class GroupHeadSkipList : SortedSkiplist<GroupHead, int>
        {
            protected override int Compare(int a, int b)
            {
                return
                    a < b ? -1 :
                    a == b ? 0 :
                    1
                ;
            }

            protected override int SelectKey(GroupHead node)
            { return node._KeyHash; }
        }

        class ItemNode : SortedSkiplist<ItemNode, int>.Node
        {
            public TType _Value;
            public TKey _Key;
            public int _KeyHash;
        }

        static int GetItemHash(Tuple<TType, TKey> item, IEqualityComparer<TKey> equalitComparer)
        { return GetKeyHash(item.Second, equalitComparer); }

        class ItemSkipList : SortedSkiplist<ItemNode, int>
        {
            public IEqualityComparer<TKey> _KeyEqualityComparer;

            protected override int Compare(int a, int b)
            {
                return
                    a < b ? -1 :
                    a == b ? 0 :
                    1
                ;
            }

            protected override int SelectKey(ItemNode node)
            { return node._KeyHash; }

            public int Find(Tuple<TType, TKey> item)
            {
                ItemNode node;
                return 
                    this.Find(
                        GetItemHash(item, _KeyEqualityComparer), 
                        n => 
                            ObticsEqualityComparer<TType>.Default.Equals(n._Value, item.First) 
                            && ObticsEqualityComparer<TKey>.Default.Equals(n._Key, item.Second), 
                        out node
                    )
                ;
            }

            public int Remove(Tuple<TType, TKey> item)
            {
                int ix = this.Find(item);
                this.RemoveAt(ix);
                return ix;
            }

            public int Add(Tuple<TType, TKey> item)
            {
                return this.AddWithIndex(new ItemNode { _Key = item.Second, _Value = item.First, _KeyHash = GetKeyHash(item.Second, _KeyEqualityComparer) });
            }
        }

        internal override void Initialize(Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>> prms)
        {
            base.Initialize(prms);
            _HeadList = new GroupHeadSkipList();
        }

        GroupHeadSkipList _HeadList;
        ItemSkipList _Buffer;
        VersionNumber _SourceContentVersion;

        public static UnorderedBoundGroupFilterDispenser<TType, TKey> Create(IEnumerable<Tuple<TType,TKey>> s, IEqualityComparer<TKey> equalityComparer)
        {
            var source = s.PatchedUnordered();

            if (source == null || equalityComparer == null)
                return null;

            return Carrousel.Get<UnorderedBoundGroupFilterDispenser<TType, TKey>, IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>(source, equalityComparer);
        }

        /// <summary>
        /// Returns a group collection by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SL.IGrouping<TKey, TType> GetGroup(TKey key)
        {
            return
                GroupingTransformation<TKey, TType>.Create(
                    Carrousel.Get(this, key, prms => new GroupHead(prms)),
                    key
                );
        }

        void IReceiveChangeNotification.NotifyChanged(object sender, INCEventArgs changeArgs)
        {
            if (changeArgs.IsCollectionEvent || changeArgs.IsExeptionEvent)
            {
                FlagsState flags;

            retry:

                while (!GetFlags(out flags)) ;

                List<Tuple<IReceiveChangeNotification,INCEventArgs>> messages = null;

                try
                {
                    var state = GetState(ref flags);

                    if (changeArgs.IsCollectionEvent)
                    {
                        switch (state)
                        {
                            case State.Initial:
                            case State.Clients:
                            case State.Hidden:
                                return;
                        }

                        if (!Lock(ref flags))
                            goto retry;

                        var collectionEvent = (INCollectionChangedEventArgs)changeArgs;
                        var historicRelation = collectionEvent.VersionNumber.IsInRelationTo(_SourceContentVersion);

                        if (historicRelation != VersionRelation.Past)
                        {
                            _SourceContentVersion = collectionEvent.VersionNumber;

                            if (
                                historicRelation == VersionRelation.Future
                                || collectionEvent.Type == INCEventArgsTypes.CollectionReset
                                || state == State.Excepted
                            )
                            {
                                SetState(ref flags, State.Hidden);
                                this._Buffer = null;

                                //reset
                                messages = new List<Tuple<IReceiveChangeNotification,INCEventArgs>>();

                                foreach (var head in _HeadList)
                                {
                                    var cv = head._ContentVersion.Next;
                                    head._ContentVersion = cv;
                                    messages.Add(Tuple.Create(head._Receiver, (INCEventArgs)INCEventArgs.CollectionReset(cv)));
                                }
                            }
                            else
                            {
                                switch (collectionEvent.Type)
                                {
                                    case INCEventArgsTypes.CollectionRemove:
                                        var removeArgs = (INCollectionRemoveEventArgs<Tuple<TType, TKey>>)collectionEvent;
                                        messages = ConstructChangeMessagesAndTakeSnapshots(collectionEvent, removeArgs.Item, null);
                                        _Buffer.Remove(removeArgs.Item);
                                        break;

                                    case INCEventArgsTypes.CollectionAdd:
                                        var addArgs = (INCollectionAddEventArgs<Tuple<TType, TKey>>)collectionEvent;
                                        messages = ConstructChangeMessagesAndTakeSnapshots(collectionEvent, null, addArgs.Item);
                                        _Buffer.Add(addArgs.Item);
                                        break;

                                    case INCEventArgsTypes.CollectionReplace:
                                        var replaceArgs = (INCollectionReplaceEventArgs<Tuple<TType, TKey>>)collectionEvent;
                                        messages = ConstructChangeMessagesAndTakeSnapshots(collectionEvent, replaceArgs.OldItem, replaceArgs.NewItem);
                                        _Buffer.Remove(replaceArgs.OldItem);
                                        _Buffer.Add(replaceArgs.NewItem);
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //exception event
                        switch (state)
                        {
                            case State.Initial:
                            case State.Clients:
                                return;
                        }

                        if (!Lock(ref flags))
                            goto retry;

                        _Buffer = null;

                        SetState(ref flags, State.Excepted);

                        messages = new List<Tuple<IReceiveChangeNotification, INCEventArgs>>();

                        foreach (var head in _HeadList)
                        {
                            head._ContentVersion = head._ContentVersion.Next;
                            messages.Add(Tuple.Create(head._Receiver, changeArgs));
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (!Lock(ref flags))
                        goto retry;

                    switch (GetState(ref flags))
                    {
                        case State.Initial:
                        case State.Clients:
                        case State.Excepted:
                            break;

                        case State.Visible:
                            _Buffer = null;
                            goto case State.Hidden;

                        case State.Hidden:

                            SetState(ref flags, State.Excepted);
                            break;
                    }

                    messages = new List<Tuple<IReceiveChangeNotification,INCEventArgs>>();
                    INCEventArgs msg = INCEventArgs.Exception(ex);

                    foreach (var head in _HeadList)
                        messages.Add(Tuple.Create(head._Receiver, msg));
                }
                finally
                { Commit(ref flags); }

                if (messages != null)
                    foreach (var t in messages)
                        t.First.NotifyChanged(t.First, t.Second);
            }
        }

        private List<Tuple<IReceiveChangeNotification, INCEventArgs>> ConstructChangeMessagesAndTakeSnapshots(INCollectionChangedEventArgs collectionEvent, Tuple<TType, TKey> removedTuple, Tuple<TType, TKey> addedTuple)
        {
            var equalityComparer = _Prms.Second;
            var messages = new List<Tuple<IReceiveChangeNotification, INCEventArgs>>();

            if (removedTuple != null && addedTuple != null && equalityComparer.Equals(removedTuple.Second, addedTuple.Second))
            {
                //added and removed with same key (according to equalityComparer)
                var key = addedTuple.Second;
                using (var enm = _HeadList.FindAll(GetKeyHash(key, equalityComparer)))
                {
                    while (enm.MoveNext())
                    {
                        var head = enm.Current;
                        head.TakeSnapshot();

                        if (equalityComparer.Equals(head._Prms.Second, key))
                        {                            
                            var cv = head._ContentVersion.Next;
                            head._ContentVersion = cv;
                            INCEventArgs msg = INCEventArgs.CollectionReplace(cv, -1, addedTuple.First, removedTuple.First);
                            messages.Add(Tuple.Create(head._Receiver, msg));
                        }
                    }
                }
            }
            else
            {
                if (removedTuple != null)
                {
                    var key = removedTuple.Second;
                    using (var enm = _HeadList.FindAll(GetKeyHash(key, equalityComparer)))
                    {
                        while (enm.MoveNext())
                        {
                            var head = enm.Current;
                            head.TakeSnapshot();

                            if (equalityComparer.Equals(head._Prms.Second, key))
                            {                                
                                var cv = head._ContentVersion.Next;
                                head._ContentVersion = cv;
                                INCEventArgs msg = INCEventArgs.CollectionRemove(cv, -1, removedTuple.First);
                                messages.Add(Tuple.Create(head._Receiver, msg));
                            }
                        }
                    }
                }

                if (addedTuple != null)
                {
                    var key = addedTuple.Second;
                    using (var enm = _HeadList.FindAll(GetKeyHash(key, equalityComparer)))
                    {
                        while (enm.MoveNext())
                        {
                            var head = enm.Current;
                            head.TakeSnapshot();

                            if (equalityComparer.Equals(head._Prms.Second, key))
                            {
                                var cv = head._ContentVersion.Next;
                                head._ContentVersion = cv;
                                INCEventArgs msg = INCEventArgs.CollectionAdd(cv, -1, addedTuple.First);
                                messages.Add(Tuple.Create(head._Receiver, msg));
                            }
                        }
                    }
                }
            }
            return messages;
        }
    }
}