using System.Collections.Generic;
using System.Collections.Specialized;
using SL = System.Linq;
using System;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Base class for Distinct transformations. Given an EqualityComparer the result will contain only the 
    /// first occuring element of a certain equality set.
    /// </summary>
    /// <typeparam name="TType">Type of the elements</typeparam>
    /// <typeparam name="TSource">Type of source. Must implement <see cref="IVersionedEnumerable{TType}"/>.</typeparam>
    /// <typeparam name="TPrms">Type of the parameter struct.</typeparam>
    internal abstract class DistinctTransformationBase<TType, TSource, TPrms> : OpaqueTransformationBase<TType, TSource, TPrms>
        where TSource : IInternalEnumerable<TType>
    {
        protected internal abstract TSource Source { get; }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        class FirstNode : Skiplist<FirstNode>.Node
        {
            public TType _item;
        }

        class SecondNode : SortedSkiplist<SecondNode, KeyValuePair<FirstNode, int>>.Node
        {
            public FirstNode _firstNode;
            public int _hash;
        }

        class ThirdNode : SortedSkiplist<ThirdNode, int>.Node
        {
            public FirstNode _firstNode;
        }

        class ItemsByKeyHashAndIndex : SortedSkiplist<SecondNode, KeyValuePair<FirstNode, int>>
        {
            public Skiplist<FirstNode> _indexList ;

            protected override int Compare(KeyValuePair<FirstNode, int> first, KeyValuePair<FirstNode, int> second)
            {
                if (first.Value < second.Value)
                    return -1;

                if (first.Value > second.Value)
                    return 1;

                int firstIndex = _indexList.IndexOf(first.Key);
                int secondIndex = _indexList.IndexOf(second.Key);

                return
                    firstIndex < secondIndex ? -1 :
                    firstIndex > secondIndex ? 1 :
                    0
                ;
            }

            int Index(SecondNode node, ref int? index)
            {
                if (!index.HasValue)
                    index = _indexList.IndexOf(node._firstNode);

                return index.Value;
            }

            int Compare(SecondNode first, SecondNode second, ref int? secondIndexStore)
            {
                if (first._hash < second._hash)
                    return -1;

                if (first._hash > second._hash)
                    return 1;

                int firstIndex = _indexList.IndexOf(first._firstNode);
                int secondIndex = Index(second, ref secondIndexStore);

                return
                    firstIndex < secondIndex ? -1 :
                    firstIndex > secondIndex ? 1 :
                    0
                ;
            }

            int FindFirstInGroupHash(int hash, Noderef[] currentNeighbours, int currentHeight, out SecondNode node)
            {
                var testedRef = currentNeighbours[currentHeight];

                int comp = testedRef._Node == null ? 1 : Comparer<int>.Default.Compare(testedRef._Node._hash, hash);

                if (comp < 0)
                    return FindFirstInGroupHash(hash, testedRef._Node._Neighbours, currentHeight, out node) + testedRef._Distance;
                else if (testedRef._Distance == 1)
                {
                    node = comp == 0 ? testedRef._Node : null;
                    return 1;
                }
                else
                    return FindFirstInGroupHash(hash, currentNeighbours, currentHeight - 1, out node);
            }

            public int FindFirstInGroup(KeyValuePair<FirstNode, int> key, out SecondNode node, IEqualityComparer<TType> equalityComparer)
            {
                if (_MaxHeight == 0)
                {
                    node = null;
                    return -1;
                }

                var ix = FindFirstInGroupHash(key.Value, _First, _MaxHeight - 1, out node);

                while (node != null && !equalityComparer.Equals(key.Key._item, node._firstNode._item))
                {
                    if(node._hash != key.Value)
                    {
                        node = null;
                        return -1;
                    }

                    node = node._Neighbours[0]._Node;
                    ++ix;
                }

                return node == null ? -1 : ix - 1;
            }

            protected override KeyValuePair<FirstNode, int> SelectKey(SecondNode node)
            { return new KeyValuePair<FirstNode,int>(node._firstNode, node._hash); }
        }

        class GroupHeadsByIndex : SortedSkiplist<ThirdNode, int>
        {
            public Skiplist<FirstNode> _indexList ;

            protected override int Compare(int first, int second)
            {
                return
                    first < second ? -1 :
                    first > second ? 1 :
                    0
                ;
            }

            protected override int SelectKey(ThirdNode node)
            {
                return _indexList.IndexOf(node._firstNode);
            }
        }

        ItemsByKeyHashAndIndex _Buffer2;
        GroupHeadsByIndex _Buffer3;

        protected override void ClearBuffer(ref FlagsState flags)
        { 
            _Buffer2 = null;
            _Buffer3 = null;
        }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            var buffer1 = new Skiplist<FirstNode>();
            var buffer2 = new ItemsByKeyHashAndIndex() { _indexList = buffer1 };
            var buffer3 = new GroupHeadsByIndex() { _indexList = buffer1 };
            var groupset = new HashSet<TType>(Comparer);
            var comparer = Comparer;

            using (var sourceEnumerator = Source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    var firstNode = new FirstNode { _item = sourceEnumerator.Current };
                    buffer1.Add(firstNode);
                    buffer2.Add(new SecondNode { _firstNode = firstNode, _hash = Comparer.GetHashCode(firstNode._item) });

                    if (!groupset.Contains(firstNode._item))
                    {
                        buffer3.Add(new ThirdNode { _firstNode = firstNode });
                        groupset.Add(firstNode._item);
                    }
                }

                _Buffer2 = buffer2;
                _Buffer3 = buffer3;

                return sourceEnumerator.ContentVersion ;
            }
        }

        protected override IEnumerator<TType> GetEnumeratorDirect()
        { return SL.Enumerable.ToList(SL.Enumerable.Distinct(Source, Comparer)).GetEnumerator(); }

        IEnumerable<TType> BufferEnumerable()
        { return SL.Enumerable.Select(_Buffer3, tn => tn._firstNode._item); }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        { return BufferEnumerable().GetEnumerator(); }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        { return SL.Enumerable.ToArray( BufferEnumerable() ); }

        protected virtual IEqualityComparer<TType> Comparer
        { get { return ObticsEqualityComparer<TType>.Default; } }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        INCEventArgs tmp1, tmp2;
                        var addArgs = (INCollectionAddEventArgs<TType>)args;
                        var ret = AddAction(addArgs.Index, addArgs.Item, out tmp1, out tmp2);
                        Merge2(tmp1, tmp2, out message1, out message2);
                        return ret;
                    }
                case INCEventArgsTypes.CollectionRemove:
                    {
                        INCEventArgs tmp1, tmp2;
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;
                        var ret = RemoveAction(removeArgs.Index, removeArgs.Item, out tmp1, out tmp2);
                        Merge2(tmp1, tmp2, out message1, out message2);
                        return ret;
                    }
                case INCEventArgsTypes.CollectionReplace:
                    {
                        INCEventArgs tmp1, tmp2, tmp3, tmp4;
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;

                        var c1 = RemoveAction(replaceArgs.Index, replaceArgs.OldItem, out tmp1, out tmp2);
                        var c2 = AddAction(replaceArgs.Index, replaceArgs.NewItem, out tmp3, out tmp4);

                        if (c1 == Change.None)
                        {
                            Merge2(tmp3, tmp4, out message1, out message2);
                            return c2;
                        }
                        else if (c2 == Change.None)
                        {
                            Merge2(tmp1, tmp2, out message1, out message2);
                            return c1;
                        }
                        else
                        {
                            int tmpCt =
                                (tmp1 != null ? 1 : 0)
                                + (tmp2 != null ? 1 : 0)
                                + (tmp3 != null ? 1 : 0)
                                + (tmp4 != null ? 1 : 0)
                            ;

                            message2 = new INCEventArgs[tmpCt];

                            int ix = 0;

                            if (tmp1 != null)
                                message2[ix++] = tmp1;

                            if (tmp2 != null)
                                message2[ix++] = tmp2;

                            if (tmp3 != null)
                                message2[ix++] = tmp3;

                            if (tmp4 != null)
                                message2[ix++] = tmp4;

                            message1 = null;

                            return Change.Controled;
                        }
                    }

                default:
                    message1 = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    message2 = null;
                    return Change.Destructive;
            }
        }

        private static void Merge2(INCEventArgs tmp1, INCEventArgs tmp2, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            if (tmp1 != null && tmp2 != null)
            {
                message2 = new INCEventArgs[] { tmp1, tmp2 };
                message1 = null;
            }
            else
            {
                message2 = null;
                message1 = tmp1 ?? tmp2;
            }
        }

        static SecondNode NextInGroup(SecondNode node, IEqualityComparer<TType> comparer)
        {
            SecondNode nextNode = node;

            while(true)
            {
                nextNode = nextNode._Neighbours[0]._Node;

                if(nextNode == null || node._hash != nextNode._hash)
                    return null;

                if(comparer.Equals(nextNode._firstNode._item,node._firstNode._item))
                    return nextNode;
            }
        }

        private Change RemoveAction(int iN, TType n, out INCEventArgs message1, out INCEventArgs message2)
        {
            message1 = null;
            message2 = null;
            var ret = Change.None;

            IEqualityComparer<TType> comparer = Comparer;

            var secondKey = new KeyValuePair<FirstNode, int>(_Buffer2._indexList[iN], comparer.GetHashCode(n));

            SecondNode secondNode;

            _Buffer2.FindFirstInGroup(secondKey, out secondNode, comparer);

            if (object.ReferenceEquals(secondNode._firstNode, secondKey.Key))
            {
                ret = Change.Controled;

                //group moves or removes
                var nextInGroup = NextInGroup(secondNode, comparer);

                //remove
                {
                    ThirdNode thirdNode;
                    int thirdIndex = 
                        _Buffer3.FindFirst(
                            iN, 
                            out thirdNode
                        )
                    ;
                    message1 = INCEventArgs.CollectionRemove(AdvanceContentVersion(), thirdIndex, n);
                    _Buffer3.Remove(thirdNode);
                }

                if (nextInGroup != null)
                {
                    //(re)insert
                    int thirdIndex = _Buffer3.AddWithIndex(new ThirdNode { _firstNode = nextInGroup._firstNode });
                    message2 = INCEventArgs.CollectionAdd(AdvanceContentVersion(), thirdIndex, nextInGroup._firstNode._item);
                }
            }

            _Buffer2.RemoveAt(_Buffer2.FindFirst(secondKey, out secondNode));
            _Buffer2._indexList.RemoveAt(iN);

            return ret;
        }

        private Change AddAction(int iN, TType n, out INCEventArgs message1, out INCEventArgs message2)
        {
            message1 = null;
            message2 = null;
            var ret = Change.None;

            IEqualityComparer<TType> comparer = Comparer;

            var firstNode = new FirstNode { _item = n };

            _Buffer2._indexList.Insert(iN, firstNode);

            var nHash = comparer.GetHashCode(n);

            var secondIndex = _Buffer2.AddWithIndex(new SecondNode { _firstNode = firstNode, _hash = nHash });

            var secondKey = new KeyValuePair<FirstNode, int>(firstNode, nHash);

            SecondNode secondNode;

            var groupHeadIndex = _Buffer2.FindFirstInGroup(secondKey, out secondNode, comparer);

            if (object.ReferenceEquals(secondNode._firstNode, firstNode))
            {
                //new head.
                ret = Change.Controled;

                var oldHead = NextInGroup(secondNode, comparer);

                if (oldHead != null)
                {
                    //remove
                    ThirdNode thirdNode;
                    int thirdIndex =
                        _Buffer3.FindFirst(
                            _Buffer2._indexList.IndexOf(oldHead._firstNode),
                            out thirdNode
                        )
                    ;
                    message1 = INCEventArgs.CollectionRemove(AdvanceContentVersion(), thirdIndex, n);
                    _Buffer3.Remove(thirdNode);
                }

                {
                    //insert new head
                    int thirdIndex = _Buffer3.AddWithIndex(new ThirdNode { _firstNode = firstNode });
                    message2 = INCEventArgs.CollectionAdd(AdvanceContentVersion(), thirdIndex, n);
                }
            }

            return ret;
        }
    }
}
