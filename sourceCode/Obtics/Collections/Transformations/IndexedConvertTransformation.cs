using System;
using SL = System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    internal sealed class IndexedConvertTransformation<TIn, TOut> : OpaqueTransformationBase<TOut, IInternalEnumerable<TIn>, Tuple<IInternalEnumerable<TIn>, Func<TIn, int, TOut>>>
    {
        public static IndexedConvertTransformation<TIn, TOut> Create(IEnumerable<TIn> s, Func<TIn, int, TOut> converter)
        {
            var source = s.Patched();

            if (source == null || converter == null)
                return null;

            return Carrousel.Get<IndexedConvertTransformation<TIn, TOut>, IInternalEnumerable<TIn>, Func<TIn, int, TOut>>(source, converter);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        IndexedTranslator<TIn> _Buffer;

        protected override void ClearBuffer(ref FlagsState flags)
        { _Buffer = null; }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using (var sourceEnumerator = _Prms.First.GetEnumerator())
            {
                _Buffer = new IndexedTranslator<TIn>();
                CollectionsHelper.Fill(_Buffer, sourceEnumerator);

                return sourceEnumerator.ContentVersion;
            }
        }

        #region IEnumerable<TOut> Members

        protected override IEnumerator<TOut> GetEnumeratorDirect()
        { return SL.Enumerable.Select(_Prms.First, _Prms.Second).GetEnumerator(); }

        protected override IEnumerator<TOut> GetEnumeratorFromBuffer()
        { return SL.Enumerable.Select(_Buffer, _Prms.Second).GetEnumerator(); }

        protected override IEnumerable<TOut> GetSnapshotEnumerable()
        { return SL.Enumerable.ToArray(SL.Enumerable.Select(_Buffer, _Prms.Second)); }

        #endregion

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            message1 = null;
            message2 = _Buffer.Translate(args);

            for (int i = 0, end = message2.Length; i < end; ++i)
            {
                var currentArgs = message2[i];
                var ccn = AdvanceContentVersion();
                var cvt = _Prms.Second;

                switch (currentArgs.Type)
                {
                    case INCEventArgsTypes.CollectionAdd:
                        {
                            var addArgs = (INCollectionAddEventArgs<TIn>)currentArgs;
                            var ix = addArgs.Index;

                            currentArgs =
                                INCEventArgs.CollectionAdd(
                                    ccn,
                                    ix,
                                    cvt(addArgs.Item, ix)
                                );
                        }
                        break;
                    case INCEventArgsTypes.CollectionReplace:
                        {
                            var replaceArgs = (INCollectionReplaceEventArgs<TIn>)currentArgs;
                            var ix = replaceArgs.Index;

                            currentArgs =
                                INCEventArgs.CollectionReplace(
                                    ccn,
                                    ix,
                                    cvt(replaceArgs.NewItem, ix),
                                    cvt(replaceArgs.OldItem, ix)
                                );
                        }
                        break;
                    case INCEventArgsTypes.CollectionRemove:
                        {
                            var removeArgs = (INCollectionRemoveEventArgs<TIn>)currentArgs;
                            var ix = removeArgs.Index;

                            currentArgs =
                                INCEventArgs.CollectionRemove(
                                    ccn,
                                    ix,
                                    cvt(removeArgs.Item, ix)
                                );
                        }
                        break;
                    //case NotifyCollectionChangedAction.Reset:
                    default:
                        message2 = null;
                        message1 = INCEventArgs.CollectionReset(ccn);
                        return Change.Destructive;
                }

                message2[i] = currentArgs;
            }

            return Change.Controled;
        }
    }
}
