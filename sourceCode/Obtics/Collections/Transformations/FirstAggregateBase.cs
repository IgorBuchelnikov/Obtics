using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal abstract class FirstAggregateBase<TIn, TOut, TPrms> : PredictingAggregateBase<TOut, TPrms>
    {
        protected virtual bool TestItem(TIn item)
        { return true; }

        protected internal abstract IInternalEnumerable<TIn> Source { get; }

        protected sealed override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected sealed override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        protected sealed override void ClearBuffer(ref FlagsState flags)
        { _Item = default(TIn); }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using (var sourceEnumerator = Source.GetEnumerator())
            {
                var version = sourceEnumerator.ContentVersion;

                int index = 0;

                while (sourceEnumerator.MoveNext())
                {
                    var item = sourceEnumerator.Current;

                    if (TestItem(item))
                    {
                        _Index = index;
                        _Item = item;
                        goto found;
                    }

                    ++index;
                }

                _Index = -1;
                _Item = default(TIn);

            found: ;

                return version;
            }
        }

        protected int _Index;
        protected TIn _Item;

        #region INotifyCollectionChanged Members

        protected override Change ProcessSourceCollectionChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args)
        {
            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    return SourceCollectionChanged_AddAction(ref flags, (INCollectionAddEventArgs<TIn>)args);
                case INCEventArgsTypes.CollectionRemove:
                    return SourceCollectionChanged_RemoveAction(ref flags, (INCollectionRemoveEventArgs<TIn>)args);
                case INCEventArgsTypes.CollectionReplace:
                    return SourceCollectionChanged_ReplaceAction(ref flags, (INCollectionReplaceEventArgs<TIn>)args);
                case INCEventArgsTypes.CollectionReset:
                    return Change.Destructive;
                default:
                    throw new InvalidOperationException("Unknown event type.");
            }
        }

        private Change SourceCollectionChanged_ReplaceAction(ref FlagsState flags, INCollectionReplaceEventArgs<TIn> args)
        {
            int start = args.Index;

            var newItem = args.NewItem;

            if (
                (_Index == -1L || _Index >= start)
                && TestItem(newItem)
            )
                return ReplaceBuffer(ref flags, start, newItem);
            else if (_Index == start)
                return Change.Destructive;

            return Change.None;
        }

        private Change SourceCollectionChanged_RemoveAction(ref FlagsState flags, INCollectionRemoveEventArgs<TIn> args)
        {
            int start = args.Index;

            if (_Index >= 0)
            {
                if (start < _Index)
                    //Keep same item
                    return ReplaceBuffer(ref flags, _Index - 1, _Item);
                else if (start == _Index)
                    return Change.Destructive;
            }

            return Change.None;
        }

        private Change SourceCollectionChanged_AddAction(ref FlagsState flags, INCollectionAddEventArgs<TIn> args)
        {
            int start = args.Index;

            if (start <= _Index || _Index == -1)
            {
                var item = args.Item;

                if (TestItem(item))
                    return ReplaceBuffer(ref flags, start, item);
                else if (_Index != -1)
                    return ReplaceBuffer(ref flags, _Index + 1, _Item); 
            }

            return Change.None;
        }

        private Change ReplaceBuffer(ref FlagsState flags, int index, TIn item)
        {
            TOut oldValue = GetValueFromBuffer(ref flags);

            _Index = index;
            _Item = item;

            return ObticsEqualityComparer<TOut>.Default.Equals(GetValueFromBuffer(ref flags), oldValue) ? Change.None : Change.Controled;
        }

        #endregion
    }
}
