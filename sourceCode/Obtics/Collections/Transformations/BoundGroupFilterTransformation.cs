using System.Collections.Generic;
using SL = System.Linq;
using System;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// BoundGroupFilter, selects those items from a source that yield a specific key via a keyGenerator method
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal sealed class BoundGroupFilterTransformation<TType, TKey> : FilterTransformationBase<Tuple<TType, TKey>, BoundGroupFilterDispenser<TType, TKey>, Tuple<BoundGroupFilterDispenser<TType, TKey>, TKey>>, SL.IGrouping<TKey, Tuple<TType, TKey>>
    {
        internal static BoundGroupFilterTransformation<TType, TKey> Create(BoundGroupFilterDispenser<TType, TKey> source, TKey key)
        {
            if (source == null)
                return null;

            return Carrousel.Get<BoundGroupFilterTransformation<TType, TKey>, BoundGroupFilterDispenser<TType, TKey>,TKey>(source, key);
        }

        internal protected override BoundGroupFilterDispenser<TType, TKey> Source
        { get { return _Prms.First; } }

        //public but just gettingvalue from truly static field so don't lock
        public TKey Key
        { get { return _Prms.Second; } }

        protected override bool SelectMember(Tuple<TType, TKey> item)
        { return _Prms.First.EqualityComparer.Equals(Key, item.Second); }

        protected override VersionNumber InitializeBuffer(ref ObservableObjectBase<Tuple<BoundGroupFilterDispenser<TType, TKey>, TKey>>.FlagsState flags)
        {
            //Only if we have source change listeners can we use Cache
            var buffer = _Buffer = new WeightedSkiplist<Node>();

            List<ItemIndexPair<Tuple<TType, TKey>>> cache;

            var sourceCache = _Prms.First.Cache;
            var lastIndex = -1;

            if (sourceCache.TryGetValue(Key, out cache))
                foreach (var item in cache)
                {
                    buffer.Add(new Node(item.Item), item.Index - lastIndex);
                    lastIndex = item.Index;
                }

            buffer.SetWeightAt(buffer.Count, sourceCache._Count - lastIndex -1);

            return sourceCache._ContentVersion;
        }
    }
}
