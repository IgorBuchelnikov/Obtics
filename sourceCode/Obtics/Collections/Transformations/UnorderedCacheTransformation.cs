using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Collections.Transformations
{
    internal sealed class UnorderedCacheTransformation<TOut> : OpaqueTransformationBase<TOut, IInternalEnumerable<TOut>, IInternalEnumerable<TOut>>
    {
        public static UnorderedCacheTransformation<TOut> Create(IEnumerable<TOut> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<UnorderedCacheTransformation<TOut>, IInternalEnumerable<TOut>>(s);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        Dictionary<Tuple<TOut>,int> _Buffer;

        protected override VersionNumber InitializeBuffer(ref ObservableObjectBase<IInternalEnumerable<TOut>>.FlagsState flags)
        {
            _Buffer = new Dictionary<Tuple<TOut>,int>();

            using (var enm = _Prms.GetEnumerator())
            {
                while (enm.MoveNext())
                {
                    var current = enm.Current;
                    int count;
                    var key = Tuple.Create(current);

                    _Buffer[key] = _Buffer.TryGetValue(key, out count) ? count + 1 : 1;
                }

                return enm.ContentVersion;
            }
        }

        protected override void ClearBuffer(ref ObservableObjectBase<IInternalEnumerable<TOut>>.FlagsState flags)
        { _Buffer = null; }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs collectionEvent, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            message2 = null;

            switch (collectionEvent.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    var addArgs = (INCollectionAddEventArgs<TOut>)collectionEvent;
                    AddItem(addArgs.Item);                    
                    message1 = INCEventArgs.CollectionAdd(AdvanceContentVersion(), addArgs.Index, addArgs.Item);
                    break;

                case INCEventArgsTypes.CollectionRemove:
                    var removeArgs = (INCollectionRemoveEventArgs<TOut>)collectionEvent;
                    RemoveItem(removeArgs.Item);
                    message1 = INCEventArgs.CollectionRemove(AdvanceContentVersion(), removeArgs.Index, removeArgs.Item);
                    break;

                case INCEventArgsTypes.CollectionReplace:
                    var replaceArgs = (INCollectionReplaceEventArgs<TOut>)collectionEvent;
                    AddItem(replaceArgs.NewItem);
                    RemoveItem(replaceArgs.OldItem);
                    message1 = INCEventArgs.CollectionReplace(AdvanceContentVersion(), replaceArgs.Index, replaceArgs.NewItem, replaceArgs.OldItem);
                    break;

                case INCEventArgsTypes.CollectionReset:
                    message1 = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    return Change.Destructive;

                default:
                    throw new Exception("Unexpected event type.");
            }

            return Change.Controled;
        }

        private void RemoveItem(TOut item)
        {
            var key = Tuple.Create(item);
            var count = _Buffer[key];
            if (count == 1)
                _Buffer.Remove(key);
            else
                _Buffer[key] = count - 1;
        }

        private void AddItem(TOut item)
        {
            var key = Tuple.Create(item);
            int count;
            _Buffer[key] = _Buffer.TryGetValue(key, out count) ? count + 1 : 1;
        }

        protected override IEnumerator<TOut> GetEnumeratorDirect()
        { return (_Prms.HasSafeEnumerator ? (IEnumerable<TOut>)_Prms : System.Linq.Enumerable.ToList(_Prms)).GetEnumerator(); }

        IEnumerable<TOut> BufferEnumerable()
        {
            return
                System.Linq.Enumerable.SelectMany(
                    _Buffer,
                    kvp =>
                        System.Linq.Enumerable.Repeat(
                            kvp.Key.First,
                            kvp.Value
                        )
                )
            ;
        }

        protected override IEnumerator<TOut> GetEnumeratorFromBuffer()
        { 
            return 
                BufferEnumerable()
                .GetEnumerator()
            ; 
        }

        protected override IEnumerable<TOut> GetSnapshotEnumerable()
        { return System.Linq.Enumerable.ToArray(BufferEnumerable()); }
    }
}
