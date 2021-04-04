using System;


namespace Obtics.Values.Transformations
{
    internal sealed class MultiTransformation<TType> : ConvertTransformationBase<TType, Tuple<Func<IValueProvider[], TType>, ArrayStructuredEqualityWrapper<IInternalValueProvider>>>
    {
        public static MultiTransformation<TType> Create(Func<IValueProvider[], TType> function, params IInternalValueProvider[] sources)
        {
            if (sources == null || function == null || Array.IndexOf(sources, null) != -1)
                return null;

            return Carrousel.Get<MultiTransformation<TType>, Func<IValueProvider[], TType>, ArrayStructuredEqualityWrapper<IInternalValueProvider>>(function, new ArrayStructuredEqualityWrapper<IInternalValueProvider>(sources));
        }

        protected override void SubscribeOnSources()
        {
            foreach (var s in _Prms.Second.Array)
                s.SubscribeINC(this);
        }

        protected override void UnsubscribeFromSources()
        {
            foreach (var s in _Prms.Second.Array)
                s.UnsubscribeINC(this);
        }

        protected override TType GetValue()
        { return _Prms.First(_Prms.Second.Array); }
    }
}
