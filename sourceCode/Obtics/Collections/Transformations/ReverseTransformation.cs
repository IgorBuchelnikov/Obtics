using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Collections.Specialized;
using System.Collections;

namespace Obtics.Collections.Transformations
{
    internal sealed class ReverseTransformation<TType> : TranslucentTransformationBase<TType, IInternalEnumerable<TType>, IInternalEnumerable<TType>>
    {
        public static ReverseTransformation<TType> Create(IEnumerable<TType> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<ReverseTransformation<TType>, IInternalEnumerable<TType>>(source);
        }

        public override IInternalEnumerable<TType> UnorderedForm
        { get { return _Prms; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        int _Buffer;

        protected override VersionNumber InitializeBuffer(ref ObservableObjectBase<IInternalEnumerable<TType>>.FlagsState flags)
        {
            using (var sourceEnumerable = _Prms.GetEnumerator())
            {
                var ctr = 0;

                while (sourceEnumerable.MoveNext())
                    ++ctr;

                _Buffer = ctr;

                return sourceEnumerable.ContentVersion;
            }
        }

        WeakReference _Cache;

        class CacheClass : List<TType>
        {
            internal VersionNumber _SourceContentVersion;
        }

        CacheClass Cache
        {
            get { return _Cache == null ? null : (CacheClass)_Cache.Target; }
            set
            {
                if (_Cache == null)
                    _Cache = new WeakReference(value);
                else
                    _Cache.Target = value;
            }
        }

        protected override IEnumerator<TType> GetEnumeratorDirect()
        {
            var cache = Cache;

            using (var sourceEnumerator = _Prms.GetEnumerator())
            {
                if (cache == null || cache._SourceContentVersion != sourceEnumerator.ContentVersion)
                {
                    Cache = cache = new CacheClass();
                    CollectionsHelper.Fill(cache, sourceEnumerator);
                    cache._SourceContentVersion = sourceEnumerator.ContentVersion;
                    cache.Reverse(0, cache.Count);
                }
            }

            return cache.GetEnumerator();
        }

        protected override Tuple<bool, IEnumerator<TType>> GetEnumeratorFromBuffer()
        {
            var cache = Cache;
            using (var sourceEnumerator = _Prms.GetEnumerator())
            {
                if (cache == null || cache._SourceContentVersion != sourceEnumerator.ContentVersion)
                {
                    Cache = cache = new CacheClass();
                    CollectionsHelper.Fill(cache, sourceEnumerator);
                    cache._SourceContentVersion = sourceEnumerator.ContentVersion;
                    cache.Reverse(0, cache.Count);
                }

                return Tuple.Create(_SourceContentVersion == sourceEnumerator.ContentVersion, (IEnumerator<TType>)cache.GetEnumerator());
            }
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message, out INCEventArgs[] messages)
        {
            _Cache = null;
            messages = null;

            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //string msg = "HaveBuffer:" + HaveBuffer + " Buffer:" + _Buffer;
            //_Debug.Add(msg);
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    return ProcessSourceCollectionChangeNotification_AddAction((INCollectionAddEventArgs<TType>)args, out message);
                case INCEventArgsTypes.CollectionRemove:
                    return ProcessSourceCollectionChangeNotification_RemoveAction((INCollectionRemoveEventArgs<TType>)args, out message);
                case INCEventArgsTypes.CollectionReplace:
                    return ProcessSourceCollectionChangeNotification_ReplaceAction((INCollectionReplaceEventArgs<TType>)args, out message);
                case INCEventArgsTypes.CollectionReset:
                    message = INCEventArgs.CollectionReset(AdvanceContentVersion());
                    return Change.Destructive;
                default:
                    throw new Exception("Unexpected event type.");
            }
        }

        private Change ProcessSourceCollectionChangeNotification_ReplaceAction(INCollectionReplaceEventArgs<TType> args, out INCEventArgs message)
        {
            var index = _Buffer - args.Index - 1;

            message =
                INCEventArgs.CollectionReplace<TType>(
                    AdvanceContentVersion(),
                    index,
                    args.NewItem,
                    args.OldItem
                );

            return Change.Controled;
        }

        private Change ProcessSourceCollectionChangeNotification_RemoveAction(INCollectionRemoveEventArgs<TType> args, out INCEventArgs message)
        {
            var index = --_Buffer - args.Index;

            message =
                INCEventArgs.CollectionRemove<TType>(
                    AdvanceContentVersion(),
                    index,
                    args.Item
                );

            return Change.Controled;
        }

        private Change ProcessSourceCollectionChangeNotification_AddAction(INCollectionAddEventArgs<TType> args, out INCEventArgs message)
        {
            var index = _Buffer++ - args.Index;

            message = 
                INCEventArgs.CollectionAdd<TType>(
                    AdvanceContentVersion(),
                    index,
                    args.Item
                );

            return Change.Controled;
        }
    }

}
