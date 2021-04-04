using System;

namespace Obtics.Values.Transformations
{
    class Coalesce10Transformation<TOut> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TOut>, TOut>>
    {
        public static Coalesce10Transformation<TOut> Create(IInternalValueProvider<TOut> source, TOut fallback)
        {
            if (source == null)
                return null;

            return Carrousel.Get<Coalesce10Transformation<TOut>, IInternalValueProvider<TOut>, TOut>(source, fallback);
        }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override TOut GetValue()
        {
            var v = _Prms.First.Value;
            return v == null ? _Prms.Second : v;
        }
    }

    class Coalesce11Transformation<TOut> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TOut>, IInternalValueProvider<TOut>>>
    {
        public static Coalesce11Transformation<TOut> Create(IInternalValueProvider<TOut> source, IInternalValueProvider<TOut> fallback)
        {
            if (source == null || fallback == null)
                return null;

            return Carrousel.Get<Coalesce11Transformation<TOut>, IInternalValueProvider<TOut>, IInternalValueProvider<TOut>>(source, fallback);
        }

        protected override void SubscribeOnSources()
        {
            _Prms.First.SubscribeINC(this);
            _Prms.Second.SubscribeINC(this);
        }

        protected override void UnsubscribeFromSources()
        {
            _Prms.First.UnsubscribeINC(this);
            _Prms.Second.UnsubscribeINC(this);
        }

        protected override TOut GetValue()
        {
            var v = _Prms.First.Value;
            return v != null ? v : _Prms.Second.Value;
        }
    }
}

