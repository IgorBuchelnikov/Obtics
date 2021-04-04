using System.Collections.Generic;
using System.Collections.Specialized;
using SL = System.Linq;
using System;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal abstract class CountAggregateBase<TIn, TOut> : PredictingAggregateBase<TOut, IInternalEnumerable<TIn>>
    {
        protected long _Buffer;

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using(var enm = _Prms.GetEnumerator())
            {
                var version = enm.ContentVersion;
                long ctr = 0;
                while(enm.MoveNext())
                    ++ctr;
                _Buffer = ctr;
                return version;
            }
        }

        protected override Change ProcessSourceCollectionChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args)
        {
            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    ++_Buffer;
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    --_Buffer;
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    return Change.None;
                case INCEventArgsTypes.CollectionReset:
                    return Change.Destructive;
                default:
                    throw new InvalidOperationException("Unexpected event type.");
            }

            return Change.Controled;
        }
    }

}
