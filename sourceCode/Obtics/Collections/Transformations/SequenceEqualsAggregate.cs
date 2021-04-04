using System.Collections.Generic;
using Obtics.Values.Transformations;
using Obtics.Values;
using System;

namespace Obtics.Collections.Transformations
{
    internal sealed class SequenceEqualsAggregate<TType> : AggregateBase<bool,Tuple<IInternalEnumerable<TType>,IInternalEnumerable<TType>,IEqualityComparer<TType>>>
    {
        public static SequenceEqualsAggregate<TType> Create(IEnumerable<TType> first, IEnumerable<TType> second, IEqualityComparer<TType> comparer)
        {
            var f = first.Patched();
            var s = second.Patched();

            if (f == null || s == null || comparer == null)
                return null;

            return Carrousel.Get<SequenceEqualsAggregate<TType>, IInternalEnumerable<TType>, IInternalEnumerable<TType>, IEqualityComparer<TType>>(f, s, comparer);
        }

        protected override bool GetValueDirect()
        { return System.Linq.Enumerable.SequenceEqual(_Prms.First, _Prms.Second, _Prms.Third); }

        protected override void SubscribeOnSources()
        {
            _Prms.First.SubscribeINC(this);
            _Prms.Second.SubscribeINC(this);
        }

        protected override void UnsubscribeFromSources()
        {
            _Prms.First.UnsubscribeINC(this);
            _Prms.Second.UnsubscribeINC(this);
        }
    }
}
