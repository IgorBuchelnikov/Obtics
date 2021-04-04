using System;

namespace Obtics.Values.Transformations
{
    internal sealed class TertiaryTransformation<TOut, TIn1, TIn2, TIn3> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, IValueProvider<TIn3>, TOut>>>
    {
        public static TertiaryTransformation<TOut, TIn1, TIn2, TIn3> Create(IInternalValueProvider<TIn1> first, IInternalValueProvider<TIn2> second, IInternalValueProvider<TIn3> third, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, IValueProvider<TIn3>, TOut> converter)
        {
            if (first == null || second == null || third == null || converter == null)
                return null;

            return Carrousel.Get<TertiaryTransformation<TOut, TIn1, TIn2, TIn3>, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, IValueProvider<TIn3>, TOut>>>(Tuple.Create(first, second, third, converter));
        }

        protected override void SubscribeOnSources()
        {
            _Prms.First.SubscribeINC(this);
            _Prms.Second.SubscribeINC(this);
            _Prms.Third.SubscribeINC(this);
        }

        protected override void UnsubscribeFromSources()
        {
            _Prms.First.UnsubscribeINC(this);
            _Prms.Second.UnsubscribeINC(this);
            _Prms.Third.UnsubscribeINC(this);
        }

        protected override TOut GetValue()
        { return _Prms.Fourth(_Prms.First, _Prms.Second, _Prms.Third); }
    }
}
