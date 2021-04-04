using System;

#if PARALLEL
using System.Threading.Tasks;
#endif

namespace Obtics.Values.Transformations
{
    internal sealed class QuartarySelectTransformation<TOut, TIn1, TIn2, TIn3, TIn4> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, IInternalValueProvider<TIn4>, Func<TIn1, TIn2, TIn3, TIn4, TOut>>>
    {
        public static QuartarySelectTransformation<TOut, TIn1, TIn2, TIn3, TIn4> Create(IInternalValueProvider<TIn1> first, IInternalValueProvider<TIn2> second, IInternalValueProvider<TIn3> third, IInternalValueProvider<TIn4> fourth, Func<TIn1, TIn2, TIn3, TIn4, TOut> converter)
        {
            if (first == null || second == null || third == null || fourth == null || converter == null)
                return null;

            return Carrousel.Get<QuartarySelectTransformation<TOut, TIn1, TIn2, TIn3, TIn4>, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, IInternalValueProvider<TIn3>, IInternalValueProvider<TIn4>, Func<TIn1, TIn2, TIn3, TIn4, TOut>>>(Tuple.Create(first, second, third, fourth, converter)); 
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

#if PARALLEL
        protected override TOut GetValue()
        {
            if (Tasks.SuggestParallelization)
            {
                var fSecond = Tasks.CreateFuture(() => _Prms.Second.Value);
                var fThird = Tasks.CreateFuture(() => _Prms.Third.Value);
                var fFourth = Tasks.CreateFuture(() => _Prms.Fourth.Value);

                var v1 = _Prms.First.Value;

                TIn2 v2 = Tasks.GetResult(fSecond);
                TIn3 v3 = Tasks.GetResult(fThird);
                TIn4 v4 = Tasks.GetResult(fFourth);

                return _Prms.Fifth(v1,v2,v3,v4);
            }
            else
                return _Prms.Fifth(_Prms.First.Value, _Prms.Second.Value, _Prms.Third.Value, _Prms.Fourth.Value);
        }
#else
        protected override TOut GetValue()
        { return _Prms.Fifth(_Prms.First.Value, _Prms.Second.Value, _Prms.Third.Value, _Prms.Fourth.Value); }
#endif
    }
}
