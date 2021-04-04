using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class IsNotEmptyAggregate<TType> : CountAggregateBase<TType, bool>
    {
        public static IsNotEmptyAggregate<TType> Create(IEnumerable<TType> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<IsNotEmptyAggregate<TType>, IInternalEnumerable<TType>>(source);
        }

        protected override bool GetValueFromBuffer(ref FlagsState flags)
        { return _Buffer != 0L; }

        protected override bool GetValueDirect()
        { return System.Linq.Enumerable.Any(_Prms); }
    }
}
