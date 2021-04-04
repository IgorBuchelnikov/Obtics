using System.Collections.Generic;
using SL = System.Linq;
using System;

namespace Obtics.Collections.Transformations
{
    internal sealed class GroupFilterConverterTransformation<TType, TKey> : ConvertTransformationBase<Tuple<TType, TKey>, SL.IGrouping<TKey, TType>, IInternalEnumerable<Tuple<TType, TKey>>, Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>, BoundGroupFilterDispenser<TType, TKey>>>
    {
        public static GroupFilterConverterTransformation<TType, TKey> Create(IEnumerable<Tuple<TType, TKey>> s, IEqualityComparer<TKey> comparer)
        {
            var source = s.Patched();

            if (source == null || comparer == null)
                return null;

            return Carrousel.Get<GroupFilterConverterTransformation<TType, TKey>, IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>, BoundGroupFilterDispenser<TType, TKey>>(source, comparer, BoundGroupFilterDispenser<TType, TKey>.Create(source, comparer) );
        }

        public override bool IsMostUnordered
        { get { return _Prms.First.IsMostUnordered; } }

        public override IInternalEnumerable<System.Linq.IGrouping<TKey, TType>> UnorderedForm
        { 
            get 
            {
                return 
                    IsMostUnordered ? this : 
                    Carrousel.Get<GroupFilterConverterTransformation<TType, TKey>, IInternalEnumerable<Tuple<TType, TKey>>, IEqualityComparer<TKey>, BoundGroupFilterDispenser<TType, TKey>>(_Prms.First.UnorderedForm, Comparer, _BoundGroupFilterDispenser); 
            } 
        }

        internal protected override IInternalEnumerable<Tuple<TType, TKey>> Source
        { get { return _Prms.First; } }

        internal IEqualityComparer<TKey> Comparer
        { get { return _Prms.Second; } }

        #region Group collections cache

        BoundGroupFilterDispenser<TType, TKey> _BoundGroupFilterDispenser
        { get { return _Prms.Third; } }

        //_BoundGroupFilterDispenser is truly static.. no locking
        internal BoundGroupFilterDispenser<TType, TKey> BoundGroupFilterDispenser
        { get { return _BoundGroupFilterDispenser; } }

        /// <summary>
        /// Returns a group collection by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SL.IGrouping<TKey, TType> GetGroup(TKey key)
        {
            //only accessing truly static field.. no locking
            return _BoundGroupFilterDispenser.GetGroup(key); 
        }

        #endregion

        protected override SL.IGrouping<TKey, TType> ConvertValue(Tuple<TType, TKey> value)
        { return GetGroup(value.Second); }
    }
}
