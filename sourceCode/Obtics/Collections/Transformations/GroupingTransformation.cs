using System.Collections.Generic;
using SL = System.Linq;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// GroupingTransformation takes a source and a key value and forms a Grouping literaly out of the
    /// given parameters.
    /// </summary>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TElement">Type of the elements</typeparam>
    internal sealed class GroupingTransformation<TKey, TElement> : ConvertTransformationBase<TElement,TElement,IInternalEnumerable<TElement>,Tuple<IInternalEnumerable<TElement>, TKey>>, SL.IGrouping<TKey, TElement>
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="source">The source of the elements</param>
        /// <param name="key">The key value</param>
        public static GroupingTransformation<TKey, TElement> Create(IEnumerable<TElement> source, TKey key)
        {
            var versionedSource = source.Patched();

            if (versionedSource == null)
                return null;

            return Carrousel.Get<GroupingTransformation<TKey, TElement>, IInternalEnumerable<TElement>, TKey>(versionedSource, key);
        }

        public override bool IsMostUnordered
        { get { return _Prms.First.IsMostUnordered; } }

        public override bool HasSafeEnumerator
        { get { return _Prms.First.HasSafeEnumerator; } }

        public override IInternalEnumerable<TElement> UnorderedForm
        { get { return IsMostUnordered ? this : Create( _Prms.First.UnorderedForm, _Prms.Second ); } }

        //truly static.. no locking
        public TKey Key
        { get { return _Prms.Second; } }

        internal protected override IInternalEnumerable<TElement> Source
        { get { return _Prms.First; } }

        protected override TElement ConvertValue(TElement value)
        { return value; }
    }
}
