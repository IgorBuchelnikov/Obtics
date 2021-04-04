using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using SL = System.Linq;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Base for sorting transformations
    /// </summary>
    /// <typeparam name="TType">Type of the result collection elements.</typeparam>
    /// <typeparam name="TKey">Type of the key to sort by.</typeparam>
    /// <typeparam name="TSource">Type of the source collection, which must atleast be an IEnumerable&lt;Pair&lt;TType,TKey&gt;&gt;. </typeparam>
    /// <typeparam name="TPrms">The type of the parameters struct.</typeparam>
    internal abstract class SortTransformationBase<TType, TKey, TSource, TPrms> : OpaqueTransformationBase<TType, TSource, TPrms>
        where TSource : IInternalEnumerable<Tuple<TType,TKey>>
    {
        protected abstract IComparer<TKey> Comparer{ get; }

        protected internal abstract TSource Source { get; }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        protected override IEnumerator<TType> GetEnumeratorDirect()
        {
            return
                SL.Enumerable.Select(
                    SL.Enumerable.OrderBy(Source, i => CreateKey(i), new KeyComparer { _InnerKeyComparer = Comparer }),
                    kvp => kvp.First
                )
                .GetEnumerator()
            ;
        }

        IEnumerable<TType> CreateBufferEnumerable()
        {
            return
                SL.Enumerable.Select(
                    _Buffer,
                    kvp => kvp._Value
                )
            ;
        }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        { return CreateBufferEnumerable().GetEnumerator(); }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        { return SL.Enumerable.ToArray( CreateBufferEnumerable() ); }

        struct Key
        {
            public TKey _Key;
            public int _ValueHash;
        }

        static Key CreateKey(Tuple<TType, TKey> t)
        { return new Key { _Key = t.Second, _ValueHash = ObticsEqualityComparer<TType>.Default.GetHashCode(t.First) }; }

        class KeyComparer : IComparer<Key>
        {
            public IComparer<TKey> _InnerKeyComparer;

            #region IComparer<Key> Members

            public int Compare(Key x, Key y)
            {
                int res = _InnerKeyComparer.Compare(x._Key, y._Key);
                if (res == 0)
                    res = Comparer<int>.Default.Compare(x._ValueHash, y._ValueHash);
                return res;
            }

            #endregion
        }

        class Node : SortedSkiplist<Node,Key>.Node
        {
            public TType _Value;
            public Key _Key;

            public Node(Tuple<TType, TKey> t)
            {
                _Value = t.First;
                _Key = CreateKey(t);
            }

            public Tuple<TType, TKey> GetTuple()
            { return Tuple.Create(_Value, _Key._Key); }
        }

        class SkipList : SortedSkiplist<Node, Key>
        {
            internal IComparer<Key> _KeyComparer;

            protected override Key SelectKey(SortTransformationBase<TType, TKey, TSource, TPrms>.Node node)
            { return node._Key; }

            protected override int Compare(Key first, Key second)
            { return _KeyComparer.Compare(first, second); }
        }

        SkipList _Buffer;

        protected override void ClearBuffer(ref FlagsState flags)
        { _Buffer = null; }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            var buffer = _Buffer = new SkipList { _KeyComparer = new KeyComparer { _InnerKeyComparer = Comparer } };

            using (var sourceEnumerator = Source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                    buffer.Add(new Node(sourceEnumerator.Current));

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            var buffer = _Buffer;
            message1 = null;
            message2 = null;

            var ccn = AdvanceContentVersion();

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<Tuple<TType, TKey>>)args;
                        var item = addArgs.Item;

                        message1 =
                            INCEventArgs.CollectionAdd<TType>(
                                ccn,
                                buffer.AddWithIndex(new Node(item)),
                                item.First
                            );
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<Tuple<TType, TKey>>)args;
                        var newItem = replaceArgs.NewItem;
                        var oldItem = replaceArgs.OldItem;
                        var ix = replaceArgs.Index;

                        Node dummy;
                        var oldIx = buffer.Find( CreateKey(oldItem), n => ObticsEqualityComparer<TType>.Default.Equals(n._Value, oldItem.First), out dummy);
                        buffer.RemoveAt(oldIx);
                        var newIx = buffer.AddWithIndex(new Node(newItem));

                        if (oldIx == newIx)
                            message1 =
                                INCEventArgs.CollectionReplace(
                                    ccn,
                                    newIx,
                                    newItem.First,
                                    oldItem.First
                                );
                        else
                            message2 =
                                new INCEventArgs[] {
                                        INCEventArgs.CollectionRemove(
                                            ccn,
                                            oldIx,
                                            oldItem.First
                                        ),
                                        INCEventArgs.CollectionAdd(
                                            AdvanceContentVersion(),
                                            newIx,
                                            newItem.First
                                        )
                                    };
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<Tuple<TType, TKey>>)args;
                        var item = removeArgs.Item;
                        Node dummy;

                        var oldIx = buffer.Find(CreateKey(item), n => ObticsEqualityComparer<TType>.Default.Equals(n._Value, item.First), out dummy);
                        buffer.RemoveAt(oldIx);

                        message1 =
                            INCEventArgs.CollectionRemove<TType>(
                                ccn,
                                oldIx,
                                item.First
                            );
                    }
                    break;

                default:
                    message1 = INCEventArgs.CollectionReset(ccn);
                    return Change.Destructive;
            }

            return Change.Controled;
        }
    }
}
