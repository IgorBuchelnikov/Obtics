using System;
using System.Collections.Generic;

namespace Obtics.Collections.Transformations
{
    internal sealed class ConvertToPairsTransformation<TValue, TKey> : ConvertTransformationBase<TValue, Tuple<TValue, TKey>, IInternalEnumerable<TValue>, Tuple<IInternalEnumerable<TValue>, Func<TValue, TKey>>>
    {
        public static ConvertToPairsTransformation<TValue, TKey> Create(IEnumerable<TValue> s, Func<TValue, TKey> converter)
        {
            var source = s.Patched();

            if (source == null || converter == null)
                return null;

            return Carrousel.Get<ConvertToPairsTransformation<TValue, TKey>, IInternalEnumerable<TValue>, Func<TValue, TKey>>(source, converter);
        }

        public override bool IsMostUnordered
        { get { return _Prms.First.IsMostUnordered; } }

        public override IInternalEnumerable<Tuple<TValue, TKey>> UnorderedForm
        { get { return IsMostUnordered ? this : Create( _Prms.First.UnorderedForm, _Prms.Second ); } }

        /// <summary>
        /// Publication of COnverter specially for SortTransformation so that it doesn't need to
        /// maintain it's own copy.
        /// </summary>
        internal Func<TValue, TKey> Converter
        { get { return _Prms.Second; } }

        internal protected override IInternalEnumerable<TValue> Source
        { get { return _Prms.First; } }

        protected override Tuple<TValue, TKey> ConvertValue(TValue item)
        { return Tuple.Create(item, _Prms.Second(item)); }
    }
}
