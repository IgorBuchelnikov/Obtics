using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Skips an X number of elements from the source to return the remaining elements.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    internal sealed class SkipTransformation<TType> : OpaqueTransformationBase<TType, IInternalEnumerable<TType>, Tuple<IInternalEnumerable<TType>, int>>
    {
        public static SkipTransformation<TType> Create(IEnumerable<TType> s, int count)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<SkipTransformation<TType>, IInternalEnumerable<TType>, int>(source, count);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        IInternalEnumerable<TType> _Source
        { get { return _Prms.First; } }

        int _Count { get { return _Prms.Second; } }

        List<TType> _Buffer;

        protected override void ClearBuffer(ref FlagsState flags)
        { _Buffer = null; }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
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
            return SL.Enumerable.ToList( SL.Enumerable.Skip(_Prms.First,_Prms.Second) ).GetEnumerator();
        }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        {
            var buffer = _Buffer;
            for (int i = _Count, end = buffer.Count; i < end; ++i)
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
            messages = null;
            message = null;
            var ret = Change.None;
            var buffer = _Buffer;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;

                        int begin = addArgs.Index;
                        int count = _Count;

                        buffer.Insert(begin, addArgs.Item);

                        var bufferCount = buffer.Count;

                        if (bufferCount > count)
                        {
                            var bcMax = Math.Max(begin, count);

                            message = INCEventArgs.CollectionAdd<TType>(
                                AdvanceContentVersion(),
                                bcMax - count,
                                buffer[bcMax]
                            );

                            ret = Change.Controled;
                        }
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;

                        int begin = removeArgs.Index;
                        var count = _Count;
                        var bufferCount = buffer.Count;

                        if (bufferCount > count)
                        {
                            var bcMax = Math.Max(count, begin);

                            message = INCEventArgs.CollectionRemove<TType>(
                                AdvanceContentVersion(),
                                bcMax - count,
                                buffer[bcMax]
                            );

                            ret = Change.Controled;
                        }

                        buffer.RemoveAt(begin);
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;

                        int ix = replaceArgs.Index;
                        var count = _Count;
                        var newItem = buffer[ix] = replaceArgs.NewItem;

                        if (ix >= count)
                        {
                            message = INCEventArgs.CollectionReplace<TType>(
                                AdvanceContentVersion(),
                                ix - count,
                                newItem,
                                replaceArgs.OldItem
                            );

                            ret = Change.Controled;
                        }
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
