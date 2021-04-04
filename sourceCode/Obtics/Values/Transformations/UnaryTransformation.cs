using System;

namespace Obtics.Values.Transformations
{
    internal sealed class UnaryTransformation<TIn, TOut> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn>, Func<IValueProvider<TIn>, TOut>>>
    {
        public static UnaryTransformation<TIn, TOut> Create(IInternalValueProvider<TIn> source, Func<IValueProvider<TIn>, TOut> converter)
        {
            if (source == null || converter == null)
                return null;

            return Carrousel.Get<UnaryTransformation<TIn, TOut>, IInternalValueProvider<TIn>, Func<IValueProvider<TIn>, TOut>>(source, converter);
        }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }
 
        protected override TOut GetValue()
        { return _Prms.Second(_Prms.First); }
    }
}
