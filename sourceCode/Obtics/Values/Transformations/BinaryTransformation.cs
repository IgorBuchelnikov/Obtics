using System;

namespace Obtics.Values.Transformations
{
    internal sealed class BinaryTransformation<TOut, TIn1, TIn2> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, TOut>>>
    {
        public static BinaryTransformation<TOut, TIn1, TIn2> Create(IInternalValueProvider<TIn1> first, IInternalValueProvider<TIn2> second, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, TOut> func)
        {
            if (first == null || second == null || func == null)
                return null;

            return Carrousel.Get<BinaryTransformation<TOut, TIn1, TIn2>, IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, Func<IValueProvider<TIn1>, IValueProvider<TIn2>, TOut>>(first, second, func);
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
        { return _Prms.Third(_Prms.First, _Prms.Second); }
    }
}
