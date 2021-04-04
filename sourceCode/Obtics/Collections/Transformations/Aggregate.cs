using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class Aggregate<TIn, TOut> : AggregateBase<TOut, Tuple<IInternalEnumerable<TIn>, Func<IEnumerable<TIn>, TOut>>>
    {
        public static Aggregate<TIn, TOut> Create(IEnumerable<TIn> source, Func<IEnumerable<TIn>, TOut> func)
        {
            var s = source.Patched();

            if (s == null || func == null)
                return null;

            return Carrousel.Get<Aggregate<TIn, TOut>, IInternalEnumerable<TIn>, Func<IEnumerable<TIn>, TOut>>(s, func); 
        }

        protected override TOut GetValueDirect()
        { return _Prms.Second(_Prms.First); }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }
    }
}
