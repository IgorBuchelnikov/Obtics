using System;
using System.Collections.Generic;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class FindLastAggregate<TType> : LastAggregateBase<TType, TType, Tuple<IInternalEnumerable<TType>, Func<TType, bool>>>
    {
        public static FindLastAggregate<TType> Create(IEnumerable<TType> s, Func<TType, bool> predicate)
        {
            var source = s.Patched();

            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<FindLastAggregate<TType>, IInternalEnumerable<TType>, Func<TType, bool>>(source, predicate);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        Func<TType, bool> Predicate
        { get { return _Prms.Second; } }

        protected override bool TestItem(TType item)
        { return _Prms.Second(item); }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Index == -1)
                throw new InvalidOperationException(); //TODO: text?

            return _Item; 
        }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.Last(_Prms.First, _Prms.Second); }
    }

    internal sealed class FindLastOrDefaultAggregate<TType> : LastAggregateBase<TType, TType, Tuple<IInternalEnumerable<TType>, Func<TType, bool>>>
    {
        public static FindLastOrDefaultAggregate<TType> Create(IEnumerable<TType> s, Func<TType, bool> predicate)
        {
            var source = s.Patched();

            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<FindLastOrDefaultAggregate<TType>, IInternalEnumerable<TType>, Func<TType, bool>>(source, predicate);
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        Func<TType, bool> Predicate
        { get { return _Prms.Second; } }

        protected override bool TestItem(TType item)
        { return _Prms.Second(item); }

        protected override TType GetValueFromBuffer(ref FlagsState flags)
        { return _Index == -1 ? default(TType) : _Item ; }

        protected override TType GetValueDirect()
        { return System.Linq.Enumerable.LastOrDefault(_Prms.First, _Prms.Second); }
    }

}
