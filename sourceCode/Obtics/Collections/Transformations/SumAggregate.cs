using System;
using System.Collections.Generic;
using SL = System.Linq;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Helper to deal with sum overflow issues. Note: transformations can not raise exceptions
    /// as a consequence of an event (like adding an item that causes an overflow in the Sum).
    /// </summary>
    internal static class SumHelper
    {
        static int Direction(Int64 v)
        {
            return
                v > 0L ? 1 :
                v < 0L ? -1 :
                0
            ;
        }

        public static Tuple<Int64, Int32> Calc(Int64 item, Tuple<Int64, Int32> tv, bool add)
        {
            var directionItem = Direction(item);

            switch ( (add ? directionItem : -directionItem)  + Direction(tv.First))
            {
                case 0:
                case 1:
                case -1:
                    return Tuple.Create(add ? tv.First + item : tv.First - item, tv.Second);
                case 2:
                    {
                        Int64 s = add ? tv.First - Int64.MaxValue + item : tv.First - Int64.MaxValue - item;

                        return
                            s > 0L ? Tuple.Create(s + Int64.MinValue, tv.Second + 1) :
                            Tuple.Create(s + Int64.MaxValue, tv.Second);
                    }
                case -2:
                    {
                        Int64 s = add ? tv.First - Int64.MinValue + item : tv.First - Int64.MinValue - item;

                        return
                            s < 0L ? Tuple.Create(s + Int64.MaxValue, tv.Second - 1) :
                            Tuple.Create(s + Int64.MinValue, tv.Second);
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        public static Int64? GetResult(Tuple<Int64, Int32> total)
        {
            return
                total.Second != 0 ? (Int64?)null :
                total.First;
        }

    }

    internal sealed class SumAggregateNullableDecimal : CommunicativeAggregateBase<Decimal?, Decimal?, Decimal, IInternalEnumerable<Decimal?>>
    {
        public static SumAggregateNullableDecimal Create(IEnumerable<Decimal?> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<SumAggregateNullableDecimal, IInternalEnumerable<Decimal?>>(source);
        }

        internal protected override IInternalEnumerable<decimal?> Source
        { get { return _Prms; } }

        protected override Change AddItem(Decimal? item)
        {
            if (!item.HasValue || item.Value == 0m)
                return Change.None;

            _Acc += item.Value;

            return Change.Controled;
        }

        protected override Change RemoveItem(Decimal? item)
        {
            if (!item.HasValue || item.Value == 0m)
                return Change.None;

            _Acc -= item.Value;

            return Change.Controled;
        }

        protected override decimal? GetValueFromBuffer(ref FlagsState flags)
        { return _Acc; }

        protected override decimal? GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }
    }

    internal sealed class SumAggregateDecimal : CommunicativeAggregateBase<Decimal, Decimal, Decimal, IInternalEnumerable<Decimal>>
    {
        public static SumAggregateDecimal Create(IEnumerable<Decimal> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<SumAggregateDecimal, IInternalEnumerable<Decimal>>(source);
        }

        internal protected override IInternalEnumerable<decimal> Source
        { get { return _Prms; } }

        protected override Change AddItem(Decimal item)
        {
            if (item == 0m)
                return Change.None;

            _Acc += item;

            return Change.Controled;
        }

        protected override Change RemoveItem(Decimal item)
        {
            if (item == 0m)
                return Change.None;

            _Acc -= item;

            return Change.Controled;
        }

        protected override decimal GetValueFromBuffer(ref FlagsState flags)
        { return _Acc; }

        protected override decimal GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }
    }

    internal sealed class SumAggregateNullableDouble : AggregateBase<Double?, IInternalEnumerable<Double?>>
    {
        public static SumAggregateNullableDouble Create(IEnumerable<Double?> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<SumAggregateNullableDouble, IInternalEnumerable<Double?>>(s);
        }

        protected override Double? GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }


    //TODO: more efficient. Only completely new sum when order of sum decreases by factor 10
    internal sealed class SumAggregateDouble : AggregateBase<Double, IInternalEnumerable<Double>>
    {
        public static SumAggregateDouble Create(IEnumerable<Double> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<SumAggregateDouble, IInternalEnumerable<Double>>(s);
        }

        protected override Double GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }

    internal sealed class SumAggregateNullableSingle : AggregateBase<Single?, IInternalEnumerable<Single?>>
    {
        public static SumAggregateNullableSingle Create(IEnumerable<Single?> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<SumAggregateNullableSingle, IInternalEnumerable<Single?>>(s);
        }

        protected override Single? GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }

    internal sealed class SumAggregateSingle : AggregateBase<Single, IInternalEnumerable<Single>>
    {
        public static SumAggregateSingle Create(IEnumerable<Single> source)
        {
            var s = source.PatchedUnordered();

            if (s == null)
                return null;

            return Carrousel.Get<SumAggregateSingle, IInternalEnumerable<Single>>(s);
        }

        protected override Single GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }
    }

    internal sealed class SumAggregateNullableInt32 : CommunicativeAggregateBase<Int32?, Int32?, Int64, IInternalEnumerable<Int32?>>
    {
        public static SumAggregateNullableInt32 Create(IEnumerable<Int32?> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<SumAggregateNullableInt32, IInternalEnumerable<Int32?>>(source);
        }

        internal protected override IInternalEnumerable<Int32?> Source
        { get { return _Prms; } }

        protected override Change AddItem(Int32? item)
        {
            if (!item.HasValue || item.Value == 0)
                return Change.None;

            _Acc += item.Value;

            return Change.Controled;
        }

        protected override Change RemoveItem(Int32? item)
        {
            if (!item.HasValue || item.Value == 0)
                return Change.None;

            _Acc -= item.Value;

            return Change.Controled;
        }

        protected override Int32? GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Acc < (long)Int32.MinValue || _Acc > (long)Int32.MaxValue)
                throw new OverflowException(); //TODO:Test?

            return (Int32)_Acc;
        }

        protected override Int32? GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }
    }

    internal sealed class SumAggregateInt32 : CommunicativeAggregateBase<Int32, Int32, Int64, IInternalEnumerable<Int32>>
    {
        public static SumAggregateInt32 Create(IEnumerable<Int32> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<SumAggregateInt32, IInternalEnumerable<Int32>>(source);
        }

        internal protected override IInternalEnumerable<Int32> Source
        { get { return _Prms; } }

        protected override Change AddItem(Int32 item)
        {
            if (item == 0)
                return Change.None;

            _Acc += item;

            return Change.Controled;
        }

        protected override Change RemoveItem(Int32 item)
        {
            if (item == 0)
                return Change.None;

            _Acc -= item;

            return Change.Controled;
        }

        protected override Int32 GetValueFromBuffer(ref FlagsState flags)
        {
            if (_Acc < (long)Int32.MinValue || _Acc > (long)Int32.MaxValue)
                throw new OverflowException(); //TODO:Test?

            return  (Int32)_Acc; 
        }

        protected override Int32 GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }
    }

    internal sealed class SumAggregateNullableInt64 : CommunicativeAggregateBase<Int64?, Int64?, Tuple<Int64, Int32>, IInternalEnumerable<Int64?>>
    {
        public static SumAggregateNullableInt64 Create(IEnumerable<Int64?> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<SumAggregateNullableInt64, IInternalEnumerable<Int64?>>(source);
        }

        internal protected override IInternalEnumerable<Int64?> Source
        { get { return _Prms; } }

        protected override Tuple<long, int> GetInititialTotalValue()
        { return Tuple.Create(0L, 0); }

        protected override Change AddItem(Int64? item)
        {
            if (!item.HasValue || item.Value == 0L)
                return Change.None;

            _Acc = SumHelper.Calc(item.Value, _Acc, true);

            return Change.Controled; 
        }

        protected override Change RemoveItem(Int64? item)
        {
            if (!item.HasValue || item.Value == 0L)
                return Change.None;

            _Acc = SumHelper.Calc(item.Value, _Acc, false);

            return Change.Controled;
        }

        protected override long? GetValueFromBuffer(ref FlagsState flags)
        {
            var res = SumHelper.GetResult(_Acc);

            if (!res.HasValue)
                throw new OverflowException(); //TODO:Text

            return res.Value;
        }

        protected override long? GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }
    }

    internal sealed class SumAggregateInt64 : CommunicativeAggregateBase<Int64, Int64, Tuple<Int64, Int32>, IInternalEnumerable<Int64>>
    {
        public static SumAggregateInt64 Create(IEnumerable<Int64> s)
        {
            var source = s.PatchedUnordered();

            if (source == null)
                return null;

            return Carrousel.Get<SumAggregateInt64, IInternalEnumerable<Int64>>(source);
        }


        internal protected override IInternalEnumerable<Int64> Source
        { get { return _Prms; } }

        protected override Tuple<long, int> GetInititialTotalValue()
        { return Tuple.Create(0L, 0); }

        protected override Change AddItem(Int64 item)
        {
            if (item == 0L)
                return Change.None;

            _Acc = SumHelper.Calc(item, _Acc, true);

            return Change.Controled;
        }

        protected override Change RemoveItem(Int64 item)
        {
            if (item == 0L)
                return Change.None;

            _Acc = SumHelper.Calc(item, _Acc, false);

            return Change.Controled;
        }

        protected override long GetValueFromBuffer(ref FlagsState flags)
        {
            var res = SumHelper.GetResult(_Acc);

            if (!res.HasValue)
                throw new OverflowException(); //TODO:Text

            return res.Value; 
        }

        protected override long GetValueDirect()
        { return System.Linq.Enumerable.Sum(_Prms); }
    }
}
