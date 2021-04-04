using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class CountAggregate<TType> : CountAggregateBase<TType, int>
    {
        public static CountAggregate<TType> Create(IEnumerable<TType> source)
        {
            var s = source.PatchedUnordered();
            return s == null ? null : Carrousel.Get<CountAggregate<TType>, IInternalEnumerable<TType>>(s);
        }

        protected override int GetValueFromBuffer(ref FlagsState flags)
        { return (int)_Buffer; }

        protected override int GetValueDirect()
        { return System.Linq.Enumerable.Count( _Prms ); }
    }

    internal sealed class LongCountAggregate<TType> : CountAggregateBase<TType, long>
    {
        public static LongCountAggregate<TType> Create(IEnumerable<TType> source)
        {
            var s = source.PatchedUnordered();
            return s == null ? null : Carrousel.Get<LongCountAggregate<TType>, IInternalEnumerable<TType>>(s);
        }

        protected override long GetValueFromBuffer(ref FlagsState flags)
        { return _Buffer; }

        protected override long GetValueDirect()
        { return System.Linq.Enumerable.LongCount(_Prms); }
    }
}
