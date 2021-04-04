using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class IndexOfFirstAggregate<TType> : FirstAggregateBase<TType, int?, Tuple<IInternalEnumerable<TType>, Func<TType, bool>>>
    {
        public static IndexOfFirstAggregate<TType> Create(IEnumerable<TType> s, Func<TType, bool> predicate)
        {
            var source = s.Patched();

            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<IndexOfFirstAggregate<TType>, IInternalEnumerable<TType>, Func<TType, bool>>(source, predicate);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        protected override bool TestItem(TType item)
        { return _Prms.Second(item); }

        protected override int? GetValueFromBuffer(ref FlagsState flags)
        { return _Index == -1 ? (int?)null : (int?)_Index; }

        protected override int? GetValueDirect()
        { 
            int ctr = 0;

            foreach(var item in _Prms.First)
                if(_Prms.Second(item))
                    return ctr;
                else
                    ++ctr;

            return null;
        }
    }
}
