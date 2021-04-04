using System;
using System.Collections.Generic;
using SL = System.Linq;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal struct AverageAcc<TType>
    {
        readonly TType _Sum;

        public TType Sum
        { get { return _Sum; } }

        readonly int _Count;

        public int Count
        { get { return _Count; } }

        public AverageAcc(TType sum, int count)
        { _Sum = sum; _Count = count; }
    }

    /// <summary>
    /// AverageAggregate implementation for a sequence of nullable decimals.
    /// </summary>
    /// <remarks>
    /// Calculates the average value over the sequence. Null values are completely ignored.
    /// If the sequence is empty or contains nothing but null values then the result will be null.
    /// </remarks>
    internal sealed class AverageAggregateNullableDecimal : CommunicativeAggregateBase<Decimal?, Decimal?, AverageAcc<Decimal>, IInternalEnumerable<Decimal?>>
    {
        public static AverageAggregateNullableDecimal Create(IEnumerable<Decimal?> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<AverageAggregateNullableDecimal, IInternalEnumerable<Decimal?>>(source);
        }

        internal protected override IInternalEnumerable<decimal?> Source
        { get { return _Prms; } }

        protected override Change AddItem(Decimal? item)
        {
            if (!item.HasValue)
                return Change.None;

            _Acc = new AverageAcc<Decimal>(_Acc.Sum + item.Value, _Acc.Count + 1);

            return Change.Controled;
        }

        protected override Change RemoveItem(Decimal? item)
        {
            if (!item.HasValue)
                return Change.None;

            _Acc = new AverageAcc<Decimal>(_Acc.Sum - item.Value, _Acc.Count - 1);

            return Change.Controled;
        }

        protected override decimal? GetValueFromBuffer(ref FlagsState flags)
        { return _Acc.Count == 0 ? (Decimal?)null : _Acc.Sum / (Decimal)_Acc.Count; }

        protected override decimal? GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }
    }

    /// <summary>
    /// AverageAggregate implementation for decimals
    /// </summary>
    /// <remarks>
    /// Calculates the average value over the sequence. 
    /// If the sequence is empty then the result will be null.
    /// </remarks>
    internal sealed class AverageAggregateDecimal : CommunicativeAggregateBase<Decimal, Decimal, AverageAcc<Decimal>, IInternalEnumerable<Decimal>>
    {
        public static AverageAggregateDecimal Create(IEnumerable<Decimal> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<AverageAggregateDecimal, IInternalEnumerable<Decimal>>(source);
        }

        internal protected override IInternalEnumerable<decimal> Source
        { get { return _Prms; } }

        protected override Change AddItem(Decimal item)
        {
            _Acc = new AverageAcc<Decimal>(_Acc.Sum + item, _Acc.Count + 1);

            return Change.Controled;
        }

        protected override Change RemoveItem(Decimal item)
        {
            _Acc = new AverageAcc<Decimal>(_Acc.Sum - item, _Acc.Count - 1);

            return Change.Controled;
        }

        protected override decimal GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Acc.Count == 0)
                throw new InvalidOperationException(); //TODO:Text?

            return _Acc.Sum / (Decimal)_Acc.Count; 
        }

        protected override decimal GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }
    }

    internal sealed class AverageAggregateNullableDouble : AggregateBase<Double?, IInternalEnumerable<Double?>>
    {
        public static AverageAggregateNullableDouble Create(IEnumerable<Double?> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<AverageAggregateNullableDouble, IInternalEnumerable<Double?>>(s);
        }

        protected override Double? GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }

    internal sealed class AverageAggregateDouble : AggregateBase<Double, IInternalEnumerable<Double>>
    {
        public static AverageAggregateDouble Create(IEnumerable<Double> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<AverageAggregateDouble, IInternalEnumerable<Double>>(s);
        }

        protected override Double GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }

    internal sealed class AverageAggregateNullableSingle : AggregateBase<Single?, IInternalEnumerable<Single?>>
    {
        public static AverageAggregateNullableSingle Create(IEnumerable<Single?> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<AverageAggregateNullableSingle, IInternalEnumerable<Single?>>(s);
        }

        protected override Single? GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }

    internal sealed class AverageAggregateSingle : AggregateBase<Single, IInternalEnumerable<Single>>
    {
        public static AverageAggregateSingle Create(IEnumerable<Single> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<AverageAggregateSingle, IInternalEnumerable<Single>>(s);
        }

        protected override Single GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }

    internal sealed class AverageAggregateNullableInt32 : CommunicativeAggregateBase<Int32?, Double?, AverageAcc<Int64>, IInternalEnumerable<Int32?>>
    {
        public static AverageAggregateNullableInt32 Create(IEnumerable<Int32?> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<AverageAggregateNullableInt32, IInternalEnumerable<Int32?>>(source);
        }

        internal protected override IInternalEnumerable<Int32?> Source
        { get { return _Prms; } }


        protected override Change AddItem(Int32? item)
        {
            if (!item.HasValue)
                return Change.None;

            _Acc = new AverageAcc<Int64>(_Acc.Sum + item.Value, _Acc.Count + 1);

            return Change.Controled;
        }

        protected override Change RemoveItem(Int32? item)
        {
            if (!item.HasValue)
                return Change.None;

            _Acc = new AverageAcc<Int64>(_Acc.Sum - item.Value, _Acc.Count - 1);

            return Change.Controled;
        }

        protected override double? GetValueFromBuffer(ref FlagsState flags)
        { return _Acc.Count == 0 ? (double?)null : (double?)((double)_Acc.Sum / (double)_Acc.Count); }

        protected override double? GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }
    }

    internal sealed class AverageAggregateInt32 : CommunicativeAggregateBase<Int32, Double, AverageAcc<Int64>, IInternalEnumerable<Int32>>
    {
        public static AverageAggregateInt32 Create(IEnumerable<Int32> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<AverageAggregateInt32, IInternalEnumerable<Int32>>(source);
        }

        internal protected override IInternalEnumerable<Int32> Source
        { get { return _Prms; } }

        protected override Change AddItem(Int32 item)
        {
            _Acc = new AverageAcc<Int64>(_Acc.Sum + item, _Acc.Count + 1);

            return Change.Controled;
        }

        protected override Change RemoveItem(Int32 item)
        {
            _Acc = new AverageAcc<Int64>(_Acc.Sum - item, _Acc.Count - 1);

            return Change.Controled;
        }

        protected override double GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Acc.Count == 0)
                throw new InvalidOperationException(); //TODO:Text?

            return (double)_Acc.Sum / (double)_Acc.Count;
        }

        protected override double GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }
    }

    internal sealed class AverageAggregateNullableInt64 : CommunicativeAggregateBase<Int64?, Double?, AverageAcc<Tuple<Int64, Int32>>, IInternalEnumerable<Int64?>>
    {
        public static AverageAggregateNullableInt64 Create(IEnumerable<Int64?> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<AverageAggregateNullableInt64, IInternalEnumerable<Int64?>>(source);
        }

        internal protected override IInternalEnumerable<Int64?> Source
        { get { return _Prms; } }

        protected override AverageAcc<Tuple<long, int>> GetInititialTotalValue()
        { return new AverageAcc<Tuple<long,int>>(Tuple.Create(0L,0), 0); }

        protected override Change AddItem(Int64? item)
        {
            if (!item.HasValue)
                return Change.None;

            _Acc = new AverageAcc<Tuple<Int64, Int32>>(SumHelper.Calc(item.Value, _Acc.Sum, true), _Acc.Count + 1);

            return Change.Controled;
        }

        protected override Change RemoveItem(Int64? item)
        {
            if (!item.HasValue)
                return Change.None;

            _Acc = new AverageAcc<Tuple<Int64, Int32>>(SumHelper.Calc(item.Value, _Acc.Sum, false), _Acc.Count - 1);

            return Change.Controled;
        }

        protected override double? GetValueFromBuffer(ref FlagsState flags)
        { return _Acc.Count == 0 ? (Double?)null : ((Double)_Acc.Sum.Second * ((Double)Int64.MaxValue - (Double)Int64.MinValue + 1d) + (Double)_Acc.Sum.First) / (Double)_Acc.Count; }

        protected override double? GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }
    }

    internal sealed class AverageAggregateInt64 : CommunicativeAggregateBase<Int64, Double, AverageAcc<Tuple<Int64, Int32>>, IInternalEnumerable<Int64>>
    {
        public static AverageAggregateInt64 Create(IEnumerable<Int64> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<AverageAggregateInt64, IInternalEnumerable<Int64>>(source);
        }

        internal protected override IInternalEnumerable<Int64> Source
        { get { return _Prms; } }

        protected override AverageAcc<Tuple<long, int>> GetInititialTotalValue()
        { return new AverageAcc<Tuple<long, int>>(Tuple.Create(0L, 0), 0); }

        protected override Change AddItem(Int64 item)
        {
            _Acc = new AverageAcc<Tuple<Int64, Int32>>(SumHelper.Calc(item, _Acc.Sum, true), _Acc.Count + 1);

            return Change.Controled;
        }

        protected override Change RemoveItem(Int64 item)
        {
            _Acc = new AverageAcc<Tuple<Int64, Int32>>(SumHelper.Calc(item, _Acc.Sum, false), _Acc.Count - 1);

            return Change.Controled;
        }

        protected override double GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Acc.Count == 0)
                throw new InvalidOperationException(); //TODO:Text?

            return ((Double)_Acc.Sum.Second * ((Double)Int64.MaxValue - (Double)Int64.MinValue + 1d) + (Double)_Acc.Sum.First) / (Double)_Acc.Count;
        }

        protected override double GetValueDirect()
        { return System.Linq.Enumerable.Average(_Prms); }
    }
}
