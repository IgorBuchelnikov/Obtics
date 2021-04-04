using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using SL = System.Linq;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Returns the first X number of elements from the source and ommits the remaining elements.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    internal sealed class TakeTransformation<TType> : OpaqueTransformationBase<TType, IInternalEnumerable<TType>, Tuple<IInternalEnumerable<TType>, int>>
    {
        public static TakeTransformation<TType> Create(IEnumerable<TType> s, int count)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<TakeTransformation<TType>, IInternalEnumerable<TType>, int>(source, count);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        int _Count{ get{return _Prms.Second;}}

        IInternalEnumerable<TType> _Source
        { get { return _Prms.First; } }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        List<TType> _Buffer;

        protected override void ClearBuffer(ref ObservableObjectBase<Tuple<IInternalEnumerable<TType>, int>>.FlagsState flags)
        { _Buffer = null; }

        protected override VersionNumber InitializeBuffer(ref ObservableObjectBase<Tuple<IInternalEnumerable<TType>, int>>.FlagsState flags)
        {
            using (var sourceEnumerator = _Source.GetEnumerator())
            {
                _Buffer = new List<TType>();
                CollectionsHelper.Fill(_Buffer, sourceEnumerator);

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override IEnumerator<TType> GetEnumeratorDirect()
        {
            return 
                SL.Enumerable.ToList(SL.Enumerable.Take(_Source, _Count)).GetEnumerator();
            ;
        }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        {
            var buffer = _Buffer;
            for (int i = 0, end = Math.Min(_Count, buffer.Count); i < end; ++i)
                yield return buffer[i];
        }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        {
            var list = new List<TType>();
            CollectionsHelper.Fill(list, GetEnumeratorFromBuffer());
            return list;
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message, out INCEventArgs[] messages)
        {
            message = null;
            messages = null;
            var ret = Change.None;

            var buffer = _Buffer;

            var count = _Count;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;

                        var item = addArgs.Item;
                        int begin = addArgs.Index;
                        int countM1 = count - 1;

                        if (begin < count)
                        {
                            if (buffer.Count >= count)
                            {
                                if (begin == countM1)
                                    message =
                                       INCEventArgs.CollectionReplace(
                                           AdvanceContentVersion(),
                                           countM1,
                                           item,
                                           buffer[countM1]
                                       );
                                else
                                    messages =
                                        new INCEventArgs[] {
                                                INCEventArgs.CollectionRemove(
                                                    AdvanceContentVersion(),
                                                    countM1,
                                                    buffer[countM1]
                                                ),
                                                INCEventArgs.CollectionAdd(
                                                    AdvanceContentVersion(),
                                                    begin,
                                                    item
                                                )
                                            };
                            }
                            else
                                message =
                                    INCEventArgs.CollectionAdd(
                                        AdvanceContentVersion(),
                                        begin,
                                        item
                                    );

                            ret = Change.Controled;
                        }

                        buffer.Insert(begin, item);
                    }
                    break;

                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;

                        var item = removeArgs.Item;
                        int begin = removeArgs.Index;
                        int countM1 = count - 1;

                        if (begin < count)
                        {
                            if (buffer.Count > count)
                            {
                                if (begin == countM1)
                                    message =
                                        INCEventArgs.CollectionReplace(
                                            AdvanceContentVersion(),
                                            countM1,
                                            buffer[count],
                                            item
                                        );
                                else
                                    messages =
                                        new INCEventArgs[] {
                                                INCEventArgs.CollectionRemove(
                                                    AdvanceContentVersion(),
                                                    begin,
                                                    item
                                                ),
                                                INCEventArgs.CollectionAdd(
                                                    AdvanceContentVersion(),
                                                    countM1,
                                                    buffer[count]
                                                )
                                            };
                            }
                            else
                                message =
                                    INCEventArgs.CollectionRemove(
                                        AdvanceContentVersion(),
                                        begin,
                                        item
                                    );

                            ret = Change.Controled;
                        }

                        buffer.RemoveAt(begin);
                    }
                    break;

                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;
                        var newItem = replaceArgs.NewItem;
                        var ix = replaceArgs.Index;

                        if (ix < count)
                        {
                            message = INCEventArgs.CollectionReplace(AdvanceContentVersion(), ix, newItem, replaceArgs.OldItem);
                            ret = Change.Controled;
                        }

                        buffer[ix] = newItem;
                    }
                    break;

                default:
                    message = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    ret = Change.Destructive;
                    break;
            }

            return ret;
        }
    }
}
