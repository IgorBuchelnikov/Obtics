using System.Collections.Generic;
using System.Collections.Specialized;
using Obtics.Values;
using System;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Returns true if all booleans in the source have value true. Also returns true if the source is empty.
    /// </summary>
    internal sealed class AllAggregate : PredictingAggregateBase<bool, IInternalEnumerable<bool>>
    {
        public static AllAggregate Create(IEnumerable<bool> s)
        {
            var source = s.PatchedUnordered() ;

            if (source == null)
                return null;

            return Carrousel.Get<AllAggregate, IInternalEnumerable<bool>>(source);
        }

        long _Buffer;

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using (var sourceEnumerator = _Prms.GetEnumerator())
            {
                long ctr = 0L;

                while (sourceEnumerator.MoveNext())
                    if (!sourceEnumerator.Current)
                        ++ctr;

                _Buffer = ctr;

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        protected override bool GetValueFromBuffer(ref FlagsState flags)
        { return _Buffer == 0L; }

        protected override bool GetValueDirect()
        { return System.Linq.Enumerable.All(_Prms, b => b); }

        protected override Change ProcessSourceCollectionChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args)
        {
            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    if (
                        !((INCollectionAddEventArgs<bool>)args).Item
                        && ++_Buffer == 1L
                    )
                        return Change.Controled;

                    break;
                case INCEventArgsTypes.CollectionRemove:
                    if (
                        !((INCollectionRemoveEventArgs<bool>)args).Item
                        && --_Buffer == 0L
                    )
                        return Change.Controled;

                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<bool>)args;
                        var newItem = replaceArgs.NewItem;

                        if (
                            newItem != replaceArgs.OldItem
                            && (
                                !newItem && ++_Buffer == 1L
                                || newItem && --_Buffer == 0L
                            )
                        )
                            return Change.Controled;
                    }
                    break;
                case INCEventArgsTypes.CollectionReset:
                    return Change.Destructive;
                default:
                    throw new InvalidOperationException("Unknown event type");
            }

            return Change.None;
        }
    }
}
