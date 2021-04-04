using System.Collections.Generic;
using SL = System.Linq;
using System.ComponentModel;
using Obtics.Values;
using System;

namespace Obtics.Collections.Transformations
{
    internal sealed class UnorderedLookupTransformation<TKey, TOut> : UnorderedDistinctTransformationBase<SL.IGrouping<TKey, TOut>, GroupFilterConverterTransformation<TOut, TKey>, GroupFilterConverterTransformation<TOut, TKey>>, IObservableLookup<TKey, TOut>
    {
        public static UnorderedLookupTransformation<TKey, TOut> Create(GroupFilterConverterTransformation<TOut, TKey> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<UnorderedLookupTransformation<TKey, TOut>, GroupFilterConverterTransformation<TOut, TKey>>(
                source
            );
        }

        public override bool IsMostUnordered
        { get { return true; } }

        internal protected override GroupFilterConverterTransformation<TOut, TKey> Source
        { get { return _Prms; } }

        #region IObservableLookup<TKey,TOut> Members

        //GroupFilterConverter truly static. no lock
        public IEnumerable<TOut> this[TKey key]
        { get { return _Prms.GetGroup(key); } }

        public IEnumerable<TOut> this[IValueProvider<TKey> key]
        {
            get
            {
                //GroupFilterConverter truly static. no lock
                return
                    Obtics.Values.Transformations.UnarySelectTransformation<TKey, IVersionedEnumerable<TOut>>.Create(
                        key.Patched(),
                        k => (IVersionedEnumerable<TOut>)_Prms.GetGroup(k)
                    )._Cascade();
            }
        }

        public IValueProvider<bool> Contains(IValueProvider<TKey> key)
        {
            //GroupFilterConverter truly static. no lock
            return
                Obtics.Values.Transformations.CascadeTransformation<bool, IValueProvider<bool>>.GeneralCreate(
                    Obtics.Values.Transformations.UnarySelectTransformation<TKey, IValueProvider<bool>>.Create(
                        key.Patched(),
                        kv =>
                            IsNotEmptyAggregate<TOut>.Create((IVersionedEnumerable<TOut>)_Prms.GetGroup(kv))
                    )
                );
        }

        #endregion

        #region ILookup<TKey,TIn> Members

        //GroupFilterConverter truly static. no lock
        public bool Contains(TKey key)
        {
            return SL.Enumerable.Any(_Prms.GetGroup(key));
            //return GroupFilterConverter.BoundGroupFilterDispenser.Cache.ContainsKey(key); 
        }

        public const string CountPropertyName = SICollection.CountPropertyName;

        public int Count
        { get { return SL.Enumerable.Count<SL.IGrouping<TKey, TOut>>(this); } }

        #endregion
    }

}
