using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// AccumulativeAggregate implementation
    /// </summary>
    /// <typeparam name="TIn">Type of the source elements</typeparam>
    /// <typeparam name="TAcc">Type of the accumumated value</typeparam>
    /// <typeparam name="TOut">Type of the returned value</typeparam>
    internal sealed class AccumulativeAggregate<TIn, TAcc, TOut> : AggregateBase<TOut, Tuple<IInternalEnumerable<TIn>, TAcc, Func<TAcc, TIn, TAcc>, Func<TAcc, TOut>>>
    {
        public static AccumulativeAggregate<TIn, TAcc, TOut> Create(IEnumerable<TIn> source, TAcc seed, Func<TAcc, TIn, TAcc> func, Func<TAcc, TOut> resultSelector)
        {
            //Accumulative aggregate, order of sourcesequence is important
            var s = source.Patched();

            if (s == null || resultSelector == null || func == null)
                return null;

            return Carrousel.Get<AccumulativeAggregate<TIn, TAcc, TOut>, IInternalEnumerable<TIn>, TAcc, Func<TAcc, TIn, TAcc>, Func<TAcc, TOut>>(s, seed, func, resultSelector);
        }

        protected override TOut GetValueDirect()
        { return System.Linq.Enumerable.Aggregate(_Prms.First, _Prms.Second, _Prms.Third, _Prms.Fourth); }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }
    }
}
