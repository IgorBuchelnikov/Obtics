using System;

namespace Obtics.Values.Transformations
{
    internal sealed class QuartaryTransformation<TOut, TIn1, TIn2, TIn3, TIn4> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, IInternalValueProvider<TIn4>, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, IValueProvider<TIn3>, IValueProvider<TIn4>, TOut>>>
    {
        public static QuartaryTransformation<TOut, TIn1, TIn2, TIn3, TIn4> Create(IInternalValueProvider<TIn1> first, IInternalValueProvider<TIn2> second, IInternalValueProvider<TIn3> third, IInternalValueProvider<TIn4> fourth, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, IValueProvider<TIn3>, IValueProvider<TIn4>, TOut> converter)
        {
            if (first == null || second == null || third == null || fourth == null || converter == null)
                return null;

            return Carrousel.Get<QuartaryTransformation<TOut, TIn1, TIn2, TIn3, TIn4>, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, IInternalValueProvider<TIn4>, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, IValueProvider<TIn3>, IValueProvider<TIn4>, TOut>>>(Tuple.Create(first, second, third, fourth, converter)); 
        }

        protected override void SubscribeOnSources()
        {
            _Prms.First.SubscribeINC(this);
            _Prms.Second.SubscribeINC(this);
            _Prms.Third.SubscribeINC(this);
            _Prms.Fourth.SubscribeINC(this);
        }

        protected override void UnsubscribeFromSources()
        {
            _Prms.First.UnsubscribeINC(this);
            _Prms.Second.UnsubscribeINC(this);
            _Prms.Third.UnsubscribeINC(this);
            _Prms.Fourth.UnsubscribeINC(this);
        }

        protected override TOut GetValue()
        { return _Prms.Fifth(_Prms.First, _Prms.Second, _Prms.Third, _Prms.Fourth); }
    }
}
