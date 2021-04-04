using SL = System.Linq;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Specific purpose transformation (should be internal?)
    /// Transform a grouping of Pairs of items en keys to a grouping of items.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TItem">Type of the item.</typeparam>
    /// <typeparam name="TSource">Type of  the source sequence.</typeparam>
    /// <remarks>
    /// The first Property of the Pair is the Item and the Second property is the Key. This key should be
    /// the same as the Key of the group, though it wouldn't matter to this transformation really.
    /// </remarks>
    internal sealed class ConvertPairsGroupingTransformation<TKey, TItem, TSource> : ConvertTransformationBase<Tuple<TItem, TKey>, TItem, TSource, TSource>, SL.IGrouping<TKey, TItem>
        where TSource : class, SL.IGrouping<TKey, Tuple<TItem, TKey>>, IInternalEnumerable<Tuple<TItem, TKey>>, INotifyChanged
    {
        public static ConvertPairsGroupingTransformation<TKey, TItem, TSource> Create(TSource source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<ConvertPairsGroupingTransformation<TKey, TItem, TSource>, TSource>(source);
        }

        public override bool IsMostUnordered
        { get { return _Prms.IsMostUnordered(); } }

        public override bool HasSafeEnumerator
        { get { return _Prms.HasSafeEnumerator; } }

        public override IInternalEnumerable<TItem> UnorderedForm
        {
            get 
            {
                if (IsMostUnordered)
                    return this;

                var orderedSource = _Prms as BoundGroupFilterTransformation<TItem, TKey>;

                if (orderedSource != null)
                    return (IInternalEnumerable<TItem>)orderedSource.Source.UnorderedFormX.GetGroup(Key);

                throw new System.Exception("Unexpected usage of ConvertPairsGroupingTransformation");
            }
        }

        internal protected override TSource Source
        { get { return _Prms; } }

        protected override TItem ConvertValue(Tuple<TItem, TKey> value)
        { return value.First; }  

        #region SL.IGrouping<TKey,TItem> Members

        //public but only accessing truly static source.. no locking
        public TKey Key
        { get { return Source.Key; } }

        #endregion
    }
}
