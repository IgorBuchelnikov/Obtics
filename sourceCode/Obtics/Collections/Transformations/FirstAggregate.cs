using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class FirstAggregate<TType> : FirstAggregateBase<TType, TType, IInternalEnumerable<TType>>
    {
        public static FirstAggregate<TType> Create(IEnumerable<TType> s)
        {
            var source = s.Patched();

            if( source == null )
                return null;

            return Carrousel.Get<FirstAggregate<TType>, IInternalEnumerable<TType>>(source);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms; } }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Index == -1)
                throw new InvalidOperationException(); //TODO:Text

            return _Item;
        }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.First(_Prms); }
    }

    internal sealed class FirstOrDefaultAggregate<TType> : FirstAggregateBase<TType, TType, IInternalEnumerable<TType>>
    {
        public static FirstOrDefaultAggregate<TType> Create(IEnumerable<TType> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<FirstOrDefaultAggregate<TType>, IInternalEnumerable<TType>>(source);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms; } }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        { return _Index == -1 ? default(TType) : _Item; }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.FirstOrDefault(_Prms); }
    }
}
