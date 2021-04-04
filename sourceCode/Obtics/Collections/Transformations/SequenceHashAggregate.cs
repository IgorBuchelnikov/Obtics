using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class SequenceHashAggregate<TType> : CommunicativeAggregateBase<TType, int, long, Tuple<IInternalEnumerable<TType>, IEqualityComparer<TType>>>
    {
        public static SequenceHashAggregate<TType> Create(IEnumerable<TType> s, IEqualityComparer<TType> comparer)
        {
            var source = s.Patched();

            if (source == null || comparer == null)
                return null;

            return Carrousel.Get<SequenceHashAggregate<TType>, IInternalEnumerable<TType>, IEqualityComparer<TType>>(source, comparer);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        private long ItemHash(TType item)
        { return (long)(!typeof(TType).IsValueType && (object)item == null ? 0 : _Prms.Second.GetHashCode(item)); }

        private static int EndResult(long h)
        { return (Int32)((h & 0xffffffff) ^ (h >> 32)); }

        protected override Change AddItem(TType item)
        {
            _Acc += ItemHash(item);
            return Change.Controled;
        }

        protected override Change RemoveItem(TType item)
        {
            _Acc -= ItemHash(item);
            return Change.Controled;
        }

        protected override int GetValueFromBuffer(ref FlagsState flags)
        { return EndResult(_Acc); }

        protected override int GetValueDirect()
        {
            var h = 
                System.Linq.Enumerable.Aggregate(
                    _Prms.First,
                    default(long),
                    (acc, v) => acc + ItemHash(v)
                );

            return EndResult(h);
        }
    }
}
