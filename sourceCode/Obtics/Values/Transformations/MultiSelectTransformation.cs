using System;

#if PARALLEL
using System.Threading.Tasks;
#endif


namespace Obtics.Values.Transformations
{
    internal sealed class MultiSelectTransformation<TType> : ConvertTransformationBase<TType, Tuple<Func<object[], TType>, ArrayStructuredEqualityWrapper<IInternalValueProvider>>>
    {
        public static MultiSelectTransformation<TType> Create(Func<object[], TType> function, params IInternalValueProvider[] sources)
        {
            if (sources == null || function == null || Array.IndexOf(sources, null) != -1)
                return null;

            return Carrousel.Get<MultiSelectTransformation<TType>, Func<object[], TType>, ArrayStructuredEqualityWrapper<IInternalValueProvider>>(function, new ArrayStructuredEqualityWrapper<IInternalValueProvider>(sources));
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

#if PARALLEL
        protected override TType GetValue()
        {
            var vpArray = _Prms.Second.Array;
            var vpaLength = vpArray.Length;
            var values = new object[vpaLength];
            var j = vpaLength;
            var i = 0;

            while ( i < j )
            {
                if (Tasks.SuggestParallelization)
                {
                    --j;
                    var valueProvider = vpArray[j];
                    values[j] = Tasks.CreateFuture(() => valueProvider.Value);
                }
                else
                {
                    values[i] = vpArray[i].Value;
                    ++i;
                }
            }

            while (i < vpaLength)
            {
                var future = (Tasks.Future<object>)values[i];

                values[i] = Tasks.GetResult(future);

                ++i;
            }

            return _Prms.First(values);
        }
#else
        protected override TType GetValue()
        {
            var vpArray = _Prms.Second.Array;
            var vpaLength = vpArray.Length;

            var values = new object[vpaLength];

            for (int i = 0; i < vpaLength; ++i)
                values[i] = vpArray[i].Value;

            return _Prms.First(values);
        }
#endif

    }
}
