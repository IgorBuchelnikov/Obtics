using System.Collections.Generic;

namespace Obtics.Collections.Transformations
{
    internal sealed class DistinctTransformation<TType> : DistinctTransformationBase<TType, IInternalEnumerable<TType>, Tuple<IInternalEnumerable<TType>, IEqualityComparer<TType>>>
    {
        public static DistinctTransformation<TType> Create(IEnumerable<TType> s, IEqualityComparer<TType> comparer)
        {
            var source = s.Patched();

            if (source == null || comparer == null)
                return null;

            return Carrousel.Get<DistinctTransformation<TType>, IInternalEnumerable<TType>, IEqualityComparer<TType>>(source, comparer);
        }

        public override IInternalEnumerable<TType> UnorderedForm
        {
            get { return UnorderedDistinctTransformation<TType>.Create(_Prms.First, _Prms.Second); }
        }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        protected override IEqualityComparer<TType> Comparer
        { get { return _Prms.Second; } }
    }

}
