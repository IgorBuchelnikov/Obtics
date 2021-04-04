using System;
using System.Collections.Generic;

namespace Obtics.Collections.Transformations
{
    internal sealed class FilterTransformation<TType> : FilterTransformationBase<TType, IInternalEnumerable<TType>, Tuple<IInternalEnumerable<TType>, Func<TType, bool>>>
    {
        public static FilterTransformation<TType> Create(IEnumerable<TType> s, Func<TType, bool> predicate)
        {
            var source = s.Patched();

            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<FilterTransformation<TType>, IInternalEnumerable<TType>, Func<TType, bool>>(source, predicate);
        }

        public override IInternalEnumerable<TType> UnorderedForm
        { get { return UnorderedFilterTransformation<TType>.Create( _Prms.First, _Prms.Second ); } }

        internal protected override IInternalEnumerable<TType> Source
        { get { return _Prms.First; } }

        protected override bool SelectMember(TType item)
        { return _Prms.Second(item); }
    }
}
