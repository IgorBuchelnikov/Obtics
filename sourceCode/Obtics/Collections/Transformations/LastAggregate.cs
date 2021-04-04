using System.Collections.Generic;
using Obtics.Values;
using System;

namespace Obtics.Collections.Transformations
{
    internal sealed class LastAggregate<TType> : LastAggregateBase<TType, TType, IInternalEnumerable<TType>>
    {
        public static LastAggregate<TType> Create(IEnumerable<TType> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<LastAggregate<TType>, IInternalEnumerable<TType>>(source);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms; } }

        protected override bool TestItem(TType item)
        { return true; }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Index == -1)
                throw new InvalidOperationException(); //TODO: text?

            return _Item;
        }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.Last(Source); }
    }

    internal sealed class LastOrDefaultAggregate<TType> : LastAggregateBase<TType, TType, IInternalEnumerable<TType>>
    {
        public static LastOrDefaultAggregate<TType> Create(IEnumerable<TType> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<LastOrDefaultAggregate<TType>, IInternalEnumerable<TType>>(source);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms; } }

        protected override bool TestItem(TType item)
        { return true; }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        { return _Index == -1 ? default(TType) : _Item; }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.LastOrDefault(Source); }
    }

}
