using System;
using System.Collections.Generic;
using System.ComponentModel;
using SL = System.Linq;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class DictionaryTransformation<TKey, TOut> : DictionaryTransformationBase<TKey,TOut,NotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>>
    {
        public static DictionaryTransformation<TKey, TOut> Create(IEnumerable<Tuple<TOut, TKey>> s, IEqualityComparer<TKey> comparer)
        {
            var source = s.Patched();

            if (source == null || comparer == null)
                return null;

            return
                Carrousel.Get<DictionaryTransformation<TKey, TOut>, NotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>>(
                    CreateSource(comparer,source)
                );
        }

        public override IInternalEnumerable<KeyValuePair<TKey, TOut>> UnorderedForm
        {
            get { return UnorderedDictionaryTransformation<TKey, TOut>.Create((UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>)_Prms.UnorderedForm); }
        }

        protected internal override NotifyVpcTransformation<System.Linq.IGrouping<TKey, TOut>, TOut> Source
        { get { return _Prms; } }
    }
}
