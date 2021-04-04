using System.Collections.Generic;

namespace Obtics.Collections.Transformations
{
    internal sealed class UnorderedDistinctTransformation<TType> : UnorderedDistinctTransformationBase<TType, IInternalEnumerable<TType>, Tuple<IInternalEnumerable<TType>, IEqualityComparer<TType>>>
    {
        public static UnorderedDistinctTransformation<TType> Create(IVersionedEnumerable<TType> source, IEqualityComparer<TType> comparer)
        {
            var s = source.PatchedUnordered();

            if (s == null || comparer == null)
                return null;

            return Carrousel.Get<UnorderedDistinctTransformation<TType>, IInternalEnumerable<TType>, IEqualityComparer<TType>>(s, comparer);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        protected override IEqualityComparer<TType> Comparer
        { get { return _Prms.Second; } }
    }

}
