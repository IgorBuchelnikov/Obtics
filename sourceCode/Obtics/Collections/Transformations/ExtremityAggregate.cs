using System.Collections.Generic;
using Obtics.Values;
using System;

namespace Obtics.Collections.Transformations
{
    internal sealed class ExtremityAggregateNullableValue<TOut> : ExtremityAggregateBase<TOut?, TOut?, IInternalEnumerable<TOut?>>
        where TOut : struct
    {
        public static ExtremityAggregateNullableValue<TOut> Create(IEnumerable<TOut?> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<ExtremityAggregateNullableValue<TOut>, IInternalEnumerable<TOut?>>(source);
        }

        internal protected override IInternalEnumerable<TOut?> Source
        { get { return _Prms; } }

        protected sealed override int CompareExtremes(TOut? first, TOut? second)
        {
            return
                !first.HasValue ? -1 :
                !second.HasValue ? 1 :
                Comparer<TOut>.Default.Compare(first.Value, second.Value);
        }

        protected sealed override TOut? GetValueFromBuffer(ref FlagsState flags)
        { return GetHaveBuffer(ref flags) ? _Buffer : default(TOut?); }

        protected sealed override TOut? GetValueDirect()
        {
            TOut? res = default(TOut?);

            foreach (var item in Source)
                if (CompareExtremes(item, res) > 0)
                    res = item;

            return res;
        }
    }

    internal sealed class ExtremityAggregateNullableValueDescending<TOut> : ExtremityAggregateBase<TOut?, TOut?, IInternalEnumerable<TOut?>>
        where TOut : struct
    {
        public static ExtremityAggregateNullableValueDescending<TOut> Create(IEnumerable<TOut?> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<ExtremityAggregateNullableValueDescending<TOut>, IInternalEnumerable<TOut?>>(source);
        }

        internal protected override IInternalEnumerable<TOut?> Source
        { get { return _Prms; } }

        protected override int CompareExtremes(TOut? first, TOut? second)
        {
            return
                !first.HasValue ? -1 :
                !second.HasValue ? 1 :
                Comparer<TOut>.Default.Compare(second.Value, first.Value)
            ;
        }

        protected override TOut? GetValueFromBuffer(ref FlagsState flags)
        { return GetHaveBuffer(ref flags) ? _Buffer : default(TOut?); }

        protected sealed override TOut? GetValueDirect()
        {
            TOut? res = default(TOut?);

            foreach (var item in Source)
                if (CompareExtremes(item, res) > 0)
                    res = item;

            return res;
        }
    }

    internal sealed class ExtremityAggregateValue<TOut> : ExtremityAggregateBase<TOut, TOut, IInternalEnumerable<TOut>>
        where TOut : struct
    {
        public static ExtremityAggregateValue<TOut> Create(IEnumerable<TOut> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<ExtremityAggregateValue<TOut>, IInternalEnumerable<TOut>>(source);
        }

        internal protected override IInternalEnumerable<TOut> Source
        { get { return _Prms; } }

        protected sealed override int CompareExtremes(TOut first, TOut second)
        { return Comparer<TOut>.Default.Compare(first, second); }

        protected sealed override TOut GetValueFromBuffer(ref FlagsState flags)
        {
            if (!GetHaveBuffer(ref flags))
                throw new InvalidOperationException(); //TODO:Text?

            return _Buffer;
        }

        protected sealed override TOut GetValueDirect()
        { return System.Linq.Enumerable.Max(Source); }
    }

    internal sealed class ExtremityAggregateValueDescending<TOut> : ExtremityAggregateBase<TOut, TOut, IInternalEnumerable<TOut>>
        where TOut : struct
    {
        public static ExtremityAggregateValueDescending<TOut> Create(IEnumerable<TOut> s)
        {
            var source = s.Patched();

            if (source == null)
                return null;

            return Carrousel.Get<ExtremityAggregateValueDescending<TOut>, IInternalEnumerable<TOut>>(source);
        }

        internal protected override IInternalEnumerable<TOut> Source
        { get { return _Prms; } }

        protected sealed override int CompareExtremes(TOut first, TOut second)
        { return Comparer<TOut>.Default.Compare(second, first); }

        protected sealed override TOut GetValueFromBuffer(ref FlagsState flags)
        {
            if (!GetHaveBuffer(ref flags))
                throw new InvalidOperationException(); //TODO:Text?

            return _Buffer;
        }

        protected sealed override TOut GetValueDirect()
        { return System.Linq.Enumerable.Min(Source); }
    }

    internal sealed class ExtremityAggregate<TOut> : ExtremityAggregateBase<TOut, TOut, Tuple<IInternalEnumerable<TOut>, IComparer<TOut>>>
    {
        public static ExtremityAggregate<TOut> Create(IEnumerable<TOut> s, IComparer<TOut> comparer)
        {
            var source = s.Patched();

            if (source == null || comparer == null)
                return null;

            return Carrousel.Get<ExtremityAggregate<TOut>, IInternalEnumerable<TOut>, IComparer<TOut>>(source, comparer);
        }

        internal protected override IInternalEnumerable<TOut> Source
        { get { return _Prms.First; } }

        protected override int CompareExtremes(TOut first, TOut second)
        { return _Prms.Second.Compare(first, second); }

        protected override TOut GetValueFromBuffer(ref FlagsState flags)
        {
            if (!GetHaveBuffer(ref flags))
                throw new InvalidOperationException(); //TODO:Text?

            return _Buffer;
        }

        protected override TOut GetValueDirect()
        {
            using (var enm = Source.GetEnumerator())
            {
                if (!enm.MoveNext())
                    throw new InvalidOperationException(); //TODO:Text

                TOut res = enm.Current;

                while (enm.MoveNext())
                {
                    var current = enm.Current;

                    if (_Prms.Second.Compare(current, res) > 0)
                        res = current;
                }

                return res;
            }
        }
    }
}
