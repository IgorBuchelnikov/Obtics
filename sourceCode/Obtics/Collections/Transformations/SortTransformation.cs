using System;
using System.Collections.Generic;
using SL = System.Linq;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class SortTransformation<TType, TKey> : SortTransformationBase<TType, TKey, ConvertToPairsTransformation<TType, TKey>, Tuple<ConvertToPairsTransformation<TType, TKey>, IComparer<TKey>>>, IObservableOrderedEnumerable<TType>
    {
        public static SortTransformation<TType, TKey> Create(IEnumerable<TType> s, Func<TType, TKey> keySelector, IComparer<TKey> comparer)
        {
            var source = s.PatchedUnordered();

            if (source == null || keySelector == null || comparer == null)
                return null;

            return
                Carrousel.Get<SortTransformation<TType, TKey>, ConvertToPairsTransformation<TType, TKey>, IComparer<TKey>>(
                    ConvertToPairsTransformation<TType, TKey>.Create(
                        source,
                        keySelector
                    ),
                    comparer
                );
        }

        public override IInternalEnumerable<TType> UnorderedForm
        { get { return _Prms.First.Source; } }

        internal protected override ConvertToPairsTransformation<TType, TKey> Source
        { get { return _Prms.First; } }

        IComparer<TKey> _Comparer
        { get { return _Prms.Second; } }

        protected override IComparer<TKey> Comparer
        { get { return _Comparer; } }

        Func<TType, TKey> KeySelector
        { get { return _Prms.First.Converter; } }  
    
        #region IObservableOrderedEnumerable<TType> Members

        public IObservableOrderedEnumerable<TType> CreateOrderedEnumerable<TKey2>(Func<TType, TKey2> keySelector, IComparer<TKey2> comparer, bool descending)
        {
            //Not accessing volatile internal state.. No locking
            if (keySelector == null || comparer == null)
                return null;

            var keySelector1 = KeySelector;

            return
                SortTransformation<TType, Tuple<TKey, TKey2>>.Create(
                    this.Source.Source,
                    FuncExtender<TType>.Extend(keySelector1,keySelector,(item, ks1, ks2) => Tuple.Create(ks1(item), ks2(item))),
                    _Comparer.Tupled(descending ? comparer.Inverted() : comparer)
                );
        }


        SL.IOrderedEnumerable<TType> SL.IOrderedEnumerable<TType>.CreateOrderedEnumerable<TKey2>(Func<TType, TKey2> keySelector, IComparer<TKey2> comparer, bool descending)
        { return this.CreateOrderedEnumerable<TKey2>( keySelector, comparer, descending ); }

        public IObservableOrderedEnumerable<TType> CreateOrderedEnumerable<TKey2>(Func<TType, IValueProvider<TKey2>> keySelector, IComparer<TKey2> comparer, bool descending)
        {
            //Not accessing volatile internal state.. No locking
            if (keySelector == null || comparer == null)
                return null;

            var keySelector1 = KeySelector;

            return
                DynamicSortTransformation<TType, Tuple<TKey, TKey2>>.Create(
                    this.Source.Source,
                    FuncExtender<TType>.Extend(
                        keySelector1,
                        keySelector,
                        (item, ks1, ks2) =>
                            ValueProvider._Static(ks1(item))
                                .Convert(
                                    ks2(item),
                                    (key1, key2) => Tuple.Create(key1.Value, key2.Value)
                                )
                    ),
                    _Comparer.Tupled(descending ? comparer.Inverted() : comparer)
                );
        }

        #endregion
    }

    internal sealed class DynamicSortTransformation<TType, TKey> : SortTransformationBase<TType, TKey, IInternalEnumerable<Tuple<TType, TKey>>, Tuple<IInternalEnumerable<Tuple<TType, TKey>>, IComparer<TKey>>>, IObservableOrderedEnumerable<TType>
    {
        public static DynamicSortTransformation<TType, TKey> Create(IEnumerable<TType> s, Func<TType, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer)
        {
            var source = s.PatchedUnordered();

            if (source == null || keySelector == null || comparer == null)
                return null;

            return
                Carrousel.Get<DynamicSortTransformation<TType, TKey>, IInternalEnumerable<Tuple<TType, TKey>>, IComparer<TKey>>(
                    UnorderedNotifyVpcTransformation<TType, TKey>.Create(source, keySelector),
                    comparer
                );
        }

        public override IInternalEnumerable<TType> UnorderedForm
        { get { return ((UnorderedNotifyVpcTransformation<TType, TKey>)_Prms.First)._Source; } }

        internal protected override IInternalEnumerable<Tuple<TType, TKey>> Source
        { get { return _Prms.First; } }

        IComparer<TKey> _Comparer
        { get { return _Prms.Second; } }

        protected override IComparer<TKey> Comparer
        { get { return _Comparer; } }

        Func<TType, IValueProvider<TKey>> KeySelector
        { get { return ((UnorderedNotifyVpcTransformation<TType, TKey>)_Prms.First).Converter; } }

        #region IObservableOrderedEnumerable<TType> Members


        public IObservableOrderedEnumerable<TType> CreateOrderedEnumerable<TKey2>(Func<TType, TKey2> keySelector, IComparer<TKey2> comparer, bool descending)
        {
            //Not accessing volatile internal state.. No locking
            if (keySelector == null || comparer == null)
                return null;

            var keySelector1 = KeySelector;

            return 
                DynamicSortTransformation<TType, Tuple<TKey, TKey2>>.Create(
                    ((UnorderedNotifyVpcTransformation<TType, TKey>)Source)._Source, 
                    FuncExtender<TType>.Extend(
                        keySelector1,
                        keySelector,
                        ( item, ks1, ks2 ) =>
                            ks1(item)
                                .Convert(
                                    ValueProvider._Static(ks2(item)),
                                    (key1, key2) => Tuple.Create(key1.Value, key2.Value)
                                )
                    ),
                    _Comparer.Tupled(descending ? comparer.Inverted() : comparer)
                );
        }


        SL.IOrderedEnumerable<TType> SL.IOrderedEnumerable<TType>.CreateOrderedEnumerable<TKey2>(Func<TType, TKey2> keySelector, IComparer<TKey2> comparer, bool descending)
        { return this.CreateOrderedEnumerable<TKey2>(keySelector, comparer, descending); }

        public IObservableOrderedEnumerable<TType> CreateOrderedEnumerable<TKey2>(Func<TType, IValueProvider<TKey2>> keySelector, IComparer<TKey2> comparer, bool descending)
        {
            //Not accessing volatile internal state.. No locking
            if (keySelector == null || comparer == null)
                return null;

            var keySelector1 = KeySelector;

            return
                DynamicSortTransformation<TType, Tuple<TKey, TKey2>>.Create(
                    ((UnorderedNotifyVpcTransformation<TType, TKey>)Source)._Source,
                    FuncExtender<TType>.Extend(
                        keySelector1,
                        keySelector,
                        ( item, ks1, ks2 )  =>
                            ks1(item)
                                .Convert(
                                    ks2(item),
                                    (key1,key2) => Tuple.Create(key1.Value, key2.Value)
                                )
                    ),
                    _Comparer.Tupled(descending ? comparer.Inverted() : comparer)
                );
        }

        #endregion
    }

}
