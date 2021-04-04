using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Collections.Transformations
{
    internal sealed class CacheTransformation<TOut> : OpaqueTransformationBase<TOut, IInternalEnumerable<TOut>, IInternalEnumerable<TOut>>
    {
        public static CacheTransformation<TOut> Create(IEnumerable<TOut> source)
        {
            var s = source.Patched();

            if (s == null)
                return null;

            return Carrousel.Get<CacheTransformation<TOut>, IInternalEnumerable<TOut>>(s);
        }

        public override IInternalEnumerable<TOut> UnorderedForm
        { get { return UnorderedCacheTransformation<TOut>.Create(_Prms); } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        ValueHybridList<TOut> _Buffer;

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            _Buffer = new ValueHybridList<TOut>();

            using (var enm = _Prms.GetEnumerator())
            {
                CollectionsHelper.Fill(_Buffer, enm);
                return enm.ContentVersion;
            }
        }

        protected override void ClearBuffer(ref FlagsState flags)
        { _Buffer = null; }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs collectionEvent, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            message2 = null;

            switch (collectionEvent.Type)
            {
                case INCEventArgsTypes.CollectionAdd :
                    var addArgs = (INCollectionAddEventArgs<TOut>)collectionEvent;
                    _Buffer.Insert(addArgs.Index, addArgs.Item);
                    message1 = INCEventArgs.CollectionAdd(AdvanceContentVersion(), addArgs.Index, addArgs.Item);
                    break;

                case INCEventArgsTypes.CollectionRemove :
                    var removeArgs = (INCollectionRemoveEventArgs<TOut>)collectionEvent;
                    _Buffer.RemoveAt(removeArgs.Index);
                    message1 = INCEventArgs.CollectionRemove(AdvanceContentVersion(), removeArgs.Index, removeArgs.Item);
                    break;

                case INCEventArgsTypes.CollectionReplace :
                    var replaceArgs = (INCollectionReplaceEventArgs<TOut>)collectionEvent;
                    _Buffer[replaceArgs.Index] = replaceArgs.NewItem;
                    message1 = INCEventArgs.CollectionReplace(AdvanceContentVersion(), replaceArgs.Index, replaceArgs.NewItem, replaceArgs.OldItem);
                    break;

                case INCEventArgsTypes.CollectionReset :
                    message1 = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    return Change.Destructive;

                default:
                    throw new Exception("Unexpected event type.");
            }

            return Change.Controled;
        }

        protected override IEnumerator<TOut> GetEnumeratorDirect()
        {
            //Create a copy before returning the enumerator because we promised not
            //to raise exceptions while enumerating once the enumerator was aquired.
            return System.Linq.Enumerable.ToList(_Prms).GetEnumerator(); 
        }

        protected override IEnumerator<TOut> GetEnumeratorFromBuffer()
        { return _Buffer.GetEnumerator(); }

        protected override IEnumerable<TOut> GetSnapshotEnumerable()
        { return System.Linq.Enumerable.ToArray( _Buffer ); }
    }
}
