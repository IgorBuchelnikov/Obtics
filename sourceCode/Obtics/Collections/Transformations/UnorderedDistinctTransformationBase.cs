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
    internal abstract class UnorderedDistinctTransformationBase<TType, TSource, TPrms> : OpaqueTransformationBase<TType, TSource, TPrms>
        where TSource : IInternalEnumerable<TType>
    {
        protected internal abstract TSource Source { get; }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        Dictionary<Tuple<TType>,int> _Buffer;

        class TupleComparer : IEqualityComparer<Tuple<TType>>
        {
            public IEqualityComparer<TType> _Comparer;

            #region IEqualityComparer<Tuple<TType>> Members

            public bool Equals(Tuple<TType> x, Tuple<TType> y)
            { return _Comparer.Equals(x.First,y.First); }

            public int GetHashCode(Tuple<TType> obj)
            { return Hasher.Create(obj.First, _Comparer); }

            #endregion
        }

        protected override void ClearBuffer(ref FlagsState flags)
        { _Buffer = null; }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            var buffer = new Dictionary<Tuple<TType>, int>(new TupleComparer { _Comparer = Comparer });
            var comparer = Comparer;

            using (var sourceEnumerator = Source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    var item = Tuple.Create(sourceEnumerator.Current);

                    if (!buffer.ContainsKey(item))
                        buffer.Add(item, 1);
                    else
                        ++buffer[item];
                }

                _Buffer = buffer;

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override IEnumerator<TType> GetEnumeratorDirect()
        { return SL.Enumerable.ToList(SL.Enumerable.Distinct(Source, Comparer)).GetEnumerator(); }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        { return SL.Enumerable.Select(_Buffer, kvp => kvp.Key.First).GetEnumerator(); }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        { return SL.Enumerable.ToArray(SL.Enumerable.Select(_Buffer, kvp => kvp.Key.First)); }

        protected virtual IEqualityComparer<TType> Comparer
        { get { return ObticsEqualityComparer<TType>.Default; } }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            var buffer = _Buffer;

            message1 = null;
            message2 = null;
            var ret = Change.None;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;
                        var item = addArgs.Item;
                        var key = Tuple.Create(item);

                        if (!buffer.ContainsKey(key))
                        {
                            buffer.Add(key, 1);
                            message1 = INCEventArgs.CollectionAdd(
                                AdvanceContentVersion(),
                                -1,
                                item
                            );
                            ret = Change.Controled;
                        }
                        else
                            ++buffer[key];
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;
                        var item = removeArgs.Item;
                        var key = Tuple.Create(item);

                        if (--buffer[key] == 0)
                        {
                            buffer.Remove(key);
                            message1 = INCEventArgs.CollectionRemove(
                                AdvanceContentVersion(),
                                -1,
                                item
                            );
                            ret = Change.Controled;
                        }
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;
                        var newItem = replaceArgs.NewItem;
                        var oldItem = replaceArgs.OldItem;
                        var newKey = Tuple.Create(newItem);
                        var oldKey = Tuple.Create(oldItem);

                        var remove = --buffer[oldKey] == 0;

                        if (remove)
                            buffer.Remove(oldKey);

                        var add = !buffer.ContainsKey(newKey);

                        if (add)
                            buffer.Add(newKey, 1);
                        else
                            ++buffer[newKey];

                        if (add)
                        {
                            message1 =
                                remove
                                    ? replaceArgs.Clone(AdvanceContentVersion())
                                    : INCEventArgs.CollectionAdd(
                                            AdvanceContentVersion(),
                                            -1,
                                            newItem
                                        )
                            ;

                            ret = Change.Controled;
                        }
                        else if (remove)
                        {
                            message1 =
                                INCEventArgs.CollectionRemove(
                                    AdvanceContentVersion(),
                                    -1,
                                    oldItem
                                )
                            ;

                            ret = Change.Controled;
                        }
                    }
                    break;
                default:
                    message1 = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    ret = Change.Destructive;
                    break;
            }

            return ret;
        }
    }
}
