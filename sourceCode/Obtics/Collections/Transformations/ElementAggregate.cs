using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal abstract class ElementAggregateBase<TType> : PredictingAggregateBase<TType, Tuple<IInternalEnumerable<TType>, int>>
    {
        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        TType _Element;

        protected override void ClearBuffer(ref FlagsState flags)
        { _Element = default(TType); }

        protected abstract TType GetFallback();

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using (var sourceEnumerator = _Prms.First.GetEnumerator())
            {
                int index = _Prms.Second;

                while (sourceEnumerator.MoveNext())
                    if (index-- == 0)
                    {
                        _Element = sourceEnumerator.Current;
                        goto found;
                    }

                _Element = GetFallback();

            found:

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        { return _Element; }

        protected override Change ProcessSourceCollectionChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args)
        {
            int index = _Prms.Second;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;
                        var newStart = addArgs.Index;

                        if (newStart < index)
                            return Change.Destructive;
                        else if (
                            newStart == index
                            && !ObticsEqualityComparer<TType>.Default.Equals(_Element, addArgs.Item)
                        )
                        {
                            _Element = addArgs.Item;
                            return Change.Controled;
                        }
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;
                        var oldStart = removeArgs.Index;

                        if (oldStart <= index)
                            return Change.Destructive;
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;

                        if (
                            replaceArgs.Index == index
                            && !ObticsEqualityComparer<TType>.Default.Equals(_Element, replaceArgs.NewItem)
                            )
                        {
                            _Element = replaceArgs.NewItem;
                            return Change.Controled;
                        }
                    }
                    break;
                case INCEventArgsTypes.CollectionReset:
                    return Change.Destructive;
                default:
                    throw new InvalidOperationException("Unknown event type.");
            }

            return Change.None;
        }
    }

    internal sealed class ElementAggregate<TType> : ElementAggregateBase<TType>
    {
        public static ElementAggregate<TType> Create(IEnumerable<TType> s, int index)
        {
            var source = s.Patched();

            if (index < 0 || source == null)
                return null;

            return Carrousel.Get<ElementAggregate<TType>, IInternalEnumerable<TType>, int>(source, index);
        }

        protected override TType GetFallback()
        { throw new ArgumentOutOfRangeException(); } //TODO: Text
        
        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.ElementAt(_Prms.First, _Prms.Second); }
    }

    internal sealed class ElementAggregateWithDefault<TType> : ElementAggregateBase<TType>
    {
        public static ElementAggregateWithDefault<TType> Create(IEnumerable<TType> s, int index)
        {
            var source = s.Patched();

            if (index < 0 || source == null)
                return null;

            return Carrousel.Get<ElementAggregateWithDefault<TType>, IInternalEnumerable<TType>, int>(source, index);
        }

        protected override TType GetFallback()
        { return default(TType); }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.ElementAtOrDefault(_Prms.First, _Prms.Second); }
    }
}
