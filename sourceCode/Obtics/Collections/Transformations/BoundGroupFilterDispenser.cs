using System;
using System.Collections.Generic;
using SL = System.Linq;

namespace Obtics.Collections.Transformations
{
    internal sealed class BoundGroupFilterDispenser<TType, TKey> : ConvertTransformationBase<Tuple<TType, TKey>,Tuple<TType, TKey>,IInternalEnumerable<Tuple<TType, TKey>>, Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>>
    {
        public static BoundGroupFilterDispenser<TType, TKey> Create(IEnumerable<Tuple<TType, TKey>> s, IEqualityComparer<TKey> equalityComparer)
        {
            var source = s.Patched();

            if (source == null || equalityComparer == null)
                return null;

            return Carrousel.Get<BoundGroupFilterDispenser<TType, TKey>, IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>>(source, equalityComparer);
        }

        public override bool HasSafeEnumerator
        { get { return _Prms.First.HasSafeEnumerator; } }

        public UnorderedBoundGroupFilterDispenser<TType, TKey> UnorderedFormX
        { get { return UnorderedBoundGroupFilterDispenser<TType, TKey>.Create(_Prms.First, EqualityComparer); } }

        /// <summary>
        /// Returns a group collection by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal SL.IGrouping<TKey, TType> GetGroup(TKey key)
        {
            //public but not accessing any members.. do not lock
            return ConvertPairsGroupingTransformation<TKey, TType, BoundGroupFilterTransformation<TType, TKey>>.Create(BoundGroupFilterTransformation<TType, TKey>.Create(this, key));
        }

        internal protected override IInternalEnumerable<Tuple<TType, TKey>> Source
        { get { return _Prms.First; } }

        protected override Tuple<TType, TKey> ConvertValue(Tuple<TType, TKey> value)
        { return value; }  

        /// <summary>
        /// EqualityComparer publication specially for BoundGroupFilterTransformation so that
        /// it doesn't need its own copy.
        /// </summary>
        internal IEqualityComparer<TKey> EqualityComparer
        { get { return _Prms.Second; } }

        #region Filter data cache

        //filter information shared by all group filters

        WeakReference _Cache;

        internal class CacheClass : System.Collections.Generic.Dictionary<TKey, List<ItemIndexPair<Tuple<TType, TKey>>>>
        {
            public VersionNumber _ContentVersion;

            public CacheClass(IEqualityComparer<TKey> comparer)
                : base(comparer)
            { }

            public int _Count;
        }

        /// <summary>
        /// Cache publication specially for BoundGroupFilterTransformation so that they can share
        /// this information. This should be accessed only when we have listeners for source change notifications
        /// </summary>
        internal CacheClass Cache
        {
            get
            {

                var wr = _Cache;
                var cache = wr == null ? null : _Cache.Target as CacheClass;

                using (var sourceEnumerator = GetEnumerator())
                {
                    if (cache != null && cache._ContentVersion != sourceEnumerator.ContentVersion)
                        cache = null;

                    if (cache == null)
                    {
                        cache = new CacheClass(EqualityComparer);
                        cache._ContentVersion = sourceEnumerator.ContentVersion;

                        int ctr = 0;

                        while (sourceEnumerator.MoveNext())
                        {
                            var kvp = sourceEnumerator.Current;

                            List<ItemIndexPair<Tuple<TType, TKey>>> list;

                            if (!cache.TryGetValue(kvp.Second, out list))
                                cache[kvp.Second] = list = new List<ItemIndexPair<Tuple<TType, TKey>>>();

                            list.Add(new ItemIndexPair<Tuple<TType, TKey>>(kvp, ctr++));
                        }

                        cache._Count = ctr;

                        _Cache = new WeakReference(cache);
                    }
                }

                return cache;
            }
        }

        #endregion
    }
}
