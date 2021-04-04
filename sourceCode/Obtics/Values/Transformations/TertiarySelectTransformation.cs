using System;

#if PARALLEL
using System.Threading.Tasks;
#endif

namespace Obtics.Values.Transformations
{
    internal sealed class TertiarySelectTransformation<TOut, TIn1, TIn2, TIn3> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, Func<TIn1, TIn2, TIn3, TOut>>>
    {
        public static TertiarySelectTransformation<TOut, TIn1, TIn2, TIn3> Create(IInternalValueProvider<TIn1> first, IInternalValueProvider<TIn2> second, IInternalValueProvider<TIn3> third, Func<TIn1, TIn2, TIn3, TOut> converter)
        {
            if (first == null || second == null || third == null || converter == null)
                return null;

            return Carrousel.Get<TertiarySelectTransformation<TOut, TIn1, TIn2, TIn3>, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, Func<TIn1, TIn2, TIn3, TOut>>>(Tuple.Create(first, second, third, converter));
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

#if PARALLEL
        protected override TOut GetValue()
        {
            if (Tasks.SuggestParallelization)
            {
                var fSecond = Tasks.CreateFuture(() => _Prms.Second.Value);
                var fThird = Tasks.CreateFuture(() => _Prms.Third.Value);

                var v1 = _Prms.First.Value;

                TIn2 v2 = Tasks.GetResult(fSecond);
                TIn3 v3 = Tasks.GetResult(fThird);

                return _Prms.Fourth(v1,v2,v3);
            }
            else
                return _Prms.Fourth(_Prms.First.Value, _Prms.Second.Value, _Prms.Third.Value);
        }
#else
        protected override TOut GetValue()
        { return _Prms.Fourth(_Prms.First.Value, _Prms.Second.Value, _Prms.Third.Value); }
#endif

    }
}
