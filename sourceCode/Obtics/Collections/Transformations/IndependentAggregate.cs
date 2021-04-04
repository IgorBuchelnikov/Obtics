using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class IndependentAggregate<TOut> : AggregateBase<TOut, Tuple<IInternalEnumerable, Func<TOut>>>
    {
        public static IndependentAggregate<TOut> Create(IEnumerable source, Func<TOut> func)
        {
            var s = source.Patched();

            if (s != null)
                s = s.UnorderedForm;

            if (s == null || func == null)
                return null;

            return Carrousel.Get<IndependentAggregate<TOut>, IInternalEnumerable, Func<TOut>>(s, func);
        }

        protected override TOut GetValueDirect()
        { return _Prms.Second(); }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }
    }
}
