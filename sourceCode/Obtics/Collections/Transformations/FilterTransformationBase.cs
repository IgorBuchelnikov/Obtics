using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using SL = System.Linq;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    internal struct ItemIndexPair<TType>
    {
        public readonly TType Item;
        public readonly int Index;

        public ItemIndexPair<TType> MoveIndex(int ammount)
        { return new ItemIndexPair<TType>(Item,Index + ammount); }

        public ItemIndexPair(TType item, int index)
        {
            Item = item;
            Index = index;
        }
    }

    internal abstract class FilterTransformationBase<TType, TSource, TPrms> : OpaqueTransformationBase<TType, TSource, TPrms>
        where TSource : IInternalEnumerable<TType>
    {
        protected internal abstract TSource Source { get; }
        protected abstract bool SelectMember(TType item);

        protected override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected class Node : WeightedSkiplist<Node>.Node
        {
            public Node(TType item)
            { Item = item; }

            public TType Item;
        }


        protected WeightedSkiplist<Node> _Buffer;

        protected override void ClearBuffer(ref FlagsState flags)
        { _Buffer = null;}

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            var buffer = _Buffer = new WeightedSkiplist<Node>();

            using (var sourceEnumerator = Source.GetEnumerator())
            {
                int collectedWeight = 0;

                while (sourceEnumerator.MoveNext())
                {
                    var item = sourceEnumerator.Current;

                    if (SelectMember(item))
                    {
                        buffer.Add(new Node(item), collectedWeight + 1);
                        collectedWeight = 0;
                    }
                    else
                        collectedWeight += 1;
                }

                buffer.SetWeightAt(buffer.Count, collectedWeight);

                return sourceEnumerator.ContentVersion;
            }
        }

        #region Process SCCN helpers

        int? RemoveAtSourceIndex(int index)
        {
            var info = _Buffer.AtWeightOffset(index+1);

            if (info.WeightOffset == index+1 && info.Node != null)
            {
                //exactly on node
                if (info.Weight != 1)
                {
                    //add remaining weight to next node
                    _Buffer.IncreaseWeightAt(info.NodeIndex + 1, info.Weight - 1);
                }

                _Buffer.RemoveAt(info.NodeIndex);

                return (int?)info.NodeIndex;
            }
            else
            {
                //not on node
                _Buffer.IncreaseWeightAt(info.NodeIndex, -1);
                return (int?)null;
            }
        }

        void InsertAtSourceIndex(int index)
        {
            var info = _Buffer.AtWeightOffset(index);
            _Buffer.IncreaseWeightAt(info.NodeIndex + (info.WeightOffset == index && info.Node != null ? 1 : 0), 1);
        }

        int InsertAtSourceIndex(int index, Node node)
        {
            var info = _Buffer.AtWeightOffset(index);

            if(info.WeightOffset == index && info.Node != null)
            {
                //on 'real node' -> position right after
                _Buffer.Insert(info.NodeIndex + 1, node, 1);
                return info.NodeIndex + 1;
            }
            else
            {
                //before 'node' -> position before
                var existingNodeWeight = info.WeightOffset - index ;
                var newNodeWeight = info.Weight - existingNodeWeight + 1;

                _Buffer.SetWeightAt(info.NodeIndex, existingNodeWeight);
                _Buffer.Insert(info.NodeIndex, node, newNodeWeight);
                return info.NodeIndex;
            }

        }

        #endregion

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs collectionEvent, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            message2 = null;

            switch (collectionEvent.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    return AddAction((INCollectionAddEventArgs<TType>)collectionEvent, out message1);
                case INCEventArgsTypes.CollectionRemove:
                    return RemoveAction((INCollectionRemoveEventArgs<TType>)collectionEvent, out message1);
                case INCEventArgsTypes.CollectionReplace:
                    return ReplaceAction((INCollectionReplaceEventArgs<TType>)collectionEvent, out message1);
                default:
                    message1 = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    return Change.Destructive;
            }
        }

        private Change ReplaceAction(INCollectionReplaceEventArgs<TType> args, out INCEventArgs translatedArgs)
        {
            var argsIndex = args.Index;
            var buffer = _Buffer;
            var newItem = args.NewItem;
            var oldItem = args.OldItem;

            bool remove = SelectMember(oldItem);
            bool add = SelectMember(newItem);

            if (remove)
            {
                if (add)
                {
                    var info = buffer.AtWeightOffset(argsIndex + 1);
                    info.Node.Item = newItem; //dirty?

                    translatedArgs = INCEventArgs.CollectionReplace(
                        AdvanceContentVersion(),
                        info.NodeIndex,
                        newItem,
                        oldItem
                    );
                }
                else
                {
                    var index = RemoveAtSourceIndex(argsIndex);
                    InsertAtSourceIndex(argsIndex);

                    translatedArgs = INCEventArgs.CollectionRemove(
                        AdvanceContentVersion(),
                        index.Value,
                        oldItem
                    );
                }

                return Change.Controled;
            }
            else if (add)
            {
                RemoveAtSourceIndex(argsIndex);
                var index = InsertAtSourceIndex(argsIndex, new Node(newItem));

                translatedArgs = INCEventArgs.CollectionAdd(
                    AdvanceContentVersion(),
                    index,
                    newItem
                );

                return Change.Controled;
            }

            translatedArgs = null;
            return Change.None;
        }

        private Change RemoveAction(INCollectionRemoveEventArgs<TType> args, out INCEventArgs translatedArgs)
        {
            var index = RemoveAtSourceIndex(args.Index);

            if (index.HasValue)
            {
                translatedArgs = INCEventArgs.CollectionRemove(
                    AdvanceContentVersion(),
                    index.Value,
                    args.Item
                );

                return Change.Controled;
            }
            else
            {
                translatedArgs = null;
                return Change.None;
            }
        }

        private Change AddAction(INCollectionAddEventArgs<TType> args, out INCEventArgs translatedArgs)
        {
            int newIx = args.Index;
            var item = args.Item;

            if (SelectMember(item))
            {
                var index = InsertAtSourceIndex(newIx, new Node(item));

                translatedArgs = INCEventArgs.CollectionAdd(
                    AdvanceContentVersion(),
                    index,
                    item
                );

                return Change.Controled;
            }
            else
            {
                InsertAtSourceIndex(newIx);
                translatedArgs = null;
                return Change.None;
            }
        }

        #region IEnumerable<TType> Members

        protected override IEnumerator<TType> GetEnumeratorDirect()
        {
            return SL.Enumerable.ToList(SL.Enumerable.Where(Source, (Func<TType,bool>)SelectMember)).GetEnumerator();
        }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        {
            return SL.Enumerable.ToArray( SL.Enumerable.Select(_Buffer, iip => iip.Item) );
        }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        {
            return SL.Enumerable.Select(_Buffer, iip => iip.Item).GetEnumerator();
        }

        #endregion
    }
}
