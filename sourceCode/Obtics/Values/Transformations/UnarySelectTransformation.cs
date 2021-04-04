using System;

namespace Obtics.Values.Transformations
{
    internal sealed class UnarySelectTransformation<TIn, TOut> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn>, Func<TIn, TOut>>>
    {
        public static UnarySelectTransformation<TIn, TOut> Create(IInternalValueProvider<TIn> source, Func<TIn, TOut> converter)
        {
            if (source == null || converter == null)
                return null;

            return Carrousel.Get<UnarySelectTransformation<TIn, TOut>, IInternalValueProvider<TIn>, Func<TIn, TOut>>(source, converter);
        }

        protected override TOut GetValue()
        { return _Prms.Second(_Prms.First.Value); }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }
    }
}
