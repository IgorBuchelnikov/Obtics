using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class FindFirstAggregate<TType> : FirstAggregateBase<TType, TType, Tuple<IInternalEnumerable<TType>, Func<TType, bool>>>
    {
        public static FindFirstAggregate<TType> Create(IEnumerable<TType> s, Func<TType, bool> predicate)
        {
            var source = s.Patched();

            if( source == null || predicate == null )
                return null;

            return Carrousel.Get<FindFirstAggregate<TType>, IInternalEnumerable<TType>, Func<TType, bool>>(source, predicate);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        protected override bool TestItem(TType item)
        { return _Prms.Second(item); }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Index == -1)
                throw new InvalidOperationException(); //TODO:Text

            return _Item;
        }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.First(_Prms.First, _Prms.Second); }
    }

    internal sealed class FindFirstOrDefaultAggregate<TType> : FirstAggregateBase<TType, TType, Tuple<IInternalEnumerable<TType>, Func<TType, bool>>>
    {
        public static FindFirstOrDefaultAggregate<TType> Create(IEnumerable<TType> s, Func<TType, bool> predicate)
        {
            var source = s.Patched();

            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<FindFirstOrDefaultAggregate<TType>, IInternalEnumerable<TType>, Func<TType, bool>>(source, predicate);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        protected override bool TestItem(TType item)
        { return _Prms.Second(item); }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        { return _Index == -1 ? default(TType) : _Item; }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.FirstOrDefault(_Prms.First, _Prms.Second); }
    }
}
