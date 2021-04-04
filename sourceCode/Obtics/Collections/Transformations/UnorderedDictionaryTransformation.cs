using System;
using System.Collections.Generic;
using System.ComponentModel;
using SL = System.Linq;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class UnorderedDictionaryTransformation<TKey, TOut> : UnorderedDictionaryTransformationBase<TKey,TOut, UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>>
    {
        public static UnorderedDictionaryTransformation<TKey, TOut> Create(UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut> source)
        {
            if (source == null)
                return null;

            return
                Carrousel.Get<UnorderedDictionaryTransformation<TKey, TOut>, UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>>(source);
        }

        protected internal override UnorderedNotifyVpcTransformation<System.Linq.IGrouping<TKey, TOut>, TOut> Source
        { get { return _Prms; } }
    }
}
