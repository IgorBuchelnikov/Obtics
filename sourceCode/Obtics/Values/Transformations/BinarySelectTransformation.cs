using System;

#if PARALLEL
using System.Threading.Tasks;
#endif

namespace Obtics.Values.Transformations
{
    internal sealed class BinarySelectTransformation<TOut, TIn1, TIn2> : ConvertTransformationBase<TOut, Tuple<IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, Func<TIn1, TIn2, TOut>>>
    {
        public static BinarySelectTransformation<TOut, TIn1, TIn2> Create(IInternalValueProvider<TIn1> first, IInternalValueProvider<TIn2> second, Func<TIn1, TIn2, TOut> func)
        {
            if (first == null || second == null || func == null)
                return null;

            return Carrousel.Get<BinarySelectTransformation<TOut, TIn1, TIn2>, IInternalValueProvider<TIn1>, IInternalValueProvider<TIn2>, Func<TIn1, TIn2, TOut>>(first, second, func);
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

#if PARALLEL
        protected override TOut GetValue()
        {
            if (Tasks.SuggestParallelization)
            {
                var fSecond = Tasks.CreateFuture(() => _Prms.Second.Value);
                var v1 = _Prms.First.Value;

                TIn2 v2 = Tasks.GetResult(fSecond);
                                   
                return _Prms.Third(v1, v2);
            }
            else
                return _Prms.Third(_Prms.First.Value, _Prms.Second.Value);
        }
#else
        protected override TOut GetValue()
        { return _Prms.Third(_Prms.First.Value, _Prms.Second.Value); }
#endif
    }
}
