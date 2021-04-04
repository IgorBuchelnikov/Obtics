using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;

namespace Obtics.Collections
{
    internal delegate void INCEventHandler(object sender, INCEventArgs args) ;

    internal interface IINC
    {
        event INCEventHandler INC ;
    }

    internal enum INCEventArgsTypes
    {
        CollectionAdd,
        CollectionMove,
        CollectionRemove,
        CollectionReplace,
        CollectionReset,
        PropertyChanged
    }

    internal abstract class INCEventArgs : EventArgs
    {
        public abstract INCEventArgsTypes Type { get; }

        public static INCEventArgs PropertyChanged(string propertyName)
        { return new INPropertyChangedEventArgs(propertyName); }

        public static INCEventArgs CollectionReset(SequenceNumber sequenceNumber)
        { return new INCollectionResetEventArgs(sequenceNumber); }


        static bool BuildItems<TType,TContainer,TIterator>(ref TContainer sourceContainer, IFastEnumerator<TType, TContainer, TIterator> sourceEnumerator, out TType item, out TType[] items)
        {
            var sourceList = sourceEnumerator as IFastList<TType, TContainer, TIterator>;

            if (sourceList != null)
            {
                switch( sourceList.GetCount(ref sourceContainer) )
                {
                    case 0 :
                        item = default(TType);
                        items = null;
                        return false;

                    case 1 :
                        item = sourceList.GetItem(ref sourceContainer, 0);
                        items = null;
                        return true;
                
                    default :
                        item = default(TType);
                        items = FastList.ToArray(ref sourceContainer, sourceList);
                        return true;
                }            
            }
            else
            {
                item = default(TType);
                items = FastEnumerator.ToArray(ref sourceContainer, sourceList);
                return items.Length > 0;
            }
        }

        public static INCEventArgs CollectionAdd<TType>(SequenceNumber sequenceNumber, int newIndex, TType item)
        { return new INCollectionAddEventArgs<TType>(sequenceNumber, ref item, null, newIndex); }

        public static INCEventArgs CollectionAdd<TType, TContainer, TPosition>(SequenceNumber sequenceNumber, int newIndex, ref TContainer container, IFastEnumerator<TType, TContainer, TPosition> enumerator)
        {
            TType item;
            TType[] items;

            return BuildItems(ref container, enumerator, out item, out items) ? new INCollectionAddEventArgs<TType>(sequenceNumber, ref item, items, newIndex) : null ;
        }

        public static INCEventArgs CollectionRemove<TType>(SequenceNumber sequenceNumber, int newIndex, TType item)
        { return new INCollectionRemoveEventArgs<TType>(sequenceNumber, ref item, null, newIndex); }

        public static INCEventArgs CollectionRemove<TType, TContainer, TPosition>(SequenceNumber sequenceNumber, int oldIndex, ref TContainer container, IFastEnumerator<TType, TContainer, TPosition> enumerator)
        {
            TType item;
            TType[] items;

            return BuildItems(ref container, enumerator, out item, out items) ? new INCollectionRemoveEventArgs<TType>(sequenceNumber, ref item, items, oldIndex) : null;
        }

        public static INCEventArgs CollectionMove<TType>(SequenceNumber sequenceNumber, int newIndex, int oldIndex, TType item)
        { return new INCollectionMoveEventArgs<TType>(sequenceNumber, ref item, null, newIndex, oldIndex); }

        public static INCEventArgs CollectionMove<TType, TContainer, TPosition>(SequenceNumber sequenceNumber, int newIndex, int oldIndex, ref TContainer container, IFastEnumerator<TType, TContainer, TPosition> enumerator)
        {
            TType item;
            TType[] items;

            return BuildItems(ref container, enumerator, out item, out items) ? new INCollectionMoveEventArgs<TType>(sequenceNumber, ref item, items, newIndex, oldIndex) : null;
        }

        public static INCEventArgs CollectionReplace<TType>(SequenceNumber sequenceNumber, int index, TType newItem, TType oldItem)
        { return new INCollectionReplaceEventArgs<TType>(sequenceNumber, ref newItem, null, ref oldItem, null, index); }

        public static INCEventArgs CollectionReplace<TType, TNewContainer, TNewPosition, TOldContainer, TOldPosition>(SequenceNumber sequenceNumber, int index, ref TNewContainer newContainer, IFastEnumerator<TType, TNewContainer, TNewPosition> newEnumerator, ref TOldContainer oldContainer, IFastEnumerator<TType, TOldContainer, TOldPosition> oldEnumerator)
        {
            TType newItem;
            TType[] newItems;
            var haveNew = BuildItems(ref newContainer, newEnumerator, out newItem, out newItems);


            TType oldItem;
            TType[] oldItems;
            var haveOld = BuildItems(ref oldContainer, oldEnumerator, out oldItem, out oldItems);

            return
                haveNew && haveOld ?    (INCEventArgs)new INCollectionReplaceEventArgs<TType>(sequenceNumber, ref newItem, newItems, ref oldItem, oldItems, index) :
                haveNew ?               (INCEventArgs)new INCollectionAddEventArgs<TType>(sequenceNumber, ref newItem, newItems, index) :
                haveOld ?               (INCEventArgs)new INCollectionRemoveEventArgs<TType>(sequenceNumber, ref oldItem, oldItems, index) :
                                        null;
        }

    }

    internal sealed class INPropertyChangedEventArgs : INCEventArgs
    {
        internal INPropertyChangedEventArgs(string propertyName)
        { _PropertyName = propertyName; }

        readonly string _PropertyName;

        public string PropertyName { get { return _PropertyName; } }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.PropertyChanged; } }
    }

    internal abstract class INCollectionChangedEventArgs : INCEventArgs
    {
        protected INCollectionChangedEventArgs(SequenceNumber sequenceNumber)
        { _SequenceNumber = sequenceNumber; }

        readonly SequenceNumber _SequenceNumber;

        public SequenceNumber SequenceNumber { get { return _SequenceNumber; } }
    }

    internal sealed class INCollectionResetEventArgs : INCollectionChangedEventArgs
    {
        internal INCollectionResetEventArgs(SequenceNumber sequenceNumber)
            : base( sequenceNumber )
        {}

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionReset; } }
    }

    internal abstract class SingleRangeINCollectionEventArgs<TType> : INCollectionChangedEventArgs
    {
        protected SingleRangeINCollectionEventArgs(SequenceNumber sequenceNumber, ref TType item, TType[] items)
            : base(sequenceNumber)
        {
            _Item = item;
            _Items = items;
        }

        TType _Item;
        TType[] _Items;

        public int ItemsCount
        { get { return _Items == null ? 1 : _Items.Length; } }

        public void CopyItemsToArray(TType[] array, int startIndex)
        {
            if (_Items == null)
                array[startIndex] = _Item;
            else
                for (int i = 0, end = _Items.Length; i != end; ++i)
                    array[startIndex + i] = _Items[i];
        }

        public void CopyItemsToList(IList<TType> list, int startIndex)
        {
            if (_Items == null)
                list[startIndex] = _Item;
            else
                for (int i = 0, end = _Items.Length; i != end; ++i)
                    list[startIndex + i] = _Items[i];
        }

        public struct FastIterator
        {
            internal SingleRangeINCollectionEventArgs<TType> _Container;
            internal int _Index;
        }

        class FastEnumeratorClass : IFastEnumerator<TType, SingleRangeINCollectionEventArgs<TType>, FastIterator>
        {
            #region IFastEnumerator<TType,SingleRangeINCollectionEventArgs<TType>,int> Members

            public FastIterator GetBegin(ref SingleRangeINCollectionEventArgs<TType> container)
            { return new FastIterator { _Container = container, _Index = -1 }; }

            public bool MoveNext(ref FastIterator iterator)
            {
                if (iterator._Index < -1)
                    return false;
                else if (iterator._Container._Items == null)
                    return ++iterator._Index == 0;
                else
                    return ++iterator._Index < iterator._Container._Items.Length;
            }

            public TType GetCurrent(ref FastIterator iterator)
            {
                if (iterator._Container._Items == null)
                {
                    if (iterator._Index != 0)
                        throw new ArgumentOutOfRangeException("index");

                    return iterator._Container._Item;
                }
                else
                    return iterator._Container._Items[iterator._Index];
            }

            #endregion
        }

        static readonly IFastEnumerator<TType, SingleRangeINCollectionEventArgs<TType>, FastIterator> _FastEnumerator = new FastEnumeratorClass();
        public static IFastEnumerator<TType, SingleRangeINCollectionEventArgs<TType>, FastIterator> FastEnumerator { get { return _FastEnumerator; } }

        public IEnumerable<TType> Items 
        { get { return Obtics.Collections.FastEnumerator.ClassicEnumerable(this, FastEnumerator); } }

        public int Count { get { return _Items == null ? 1 : _Items.Length; } }
    }

    internal sealed class INCollectionAddEventArgs<TType> : SingleRangeINCollectionEventArgs<TType>
    {
        internal INCollectionAddEventArgs(SequenceNumber sequenceNumber, ref TType item, TType[] items, int index)
            : base(sequenceNumber, ref item, items)
        { _NewIndex = index; }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionAdd; } }

        readonly int _NewIndex;

        public int NewIndex { get { return _NewIndex; } }
    }

    internal sealed class INCollectionRemoveEventArgs<TType> : SingleRangeINCollectionEventArgs<TType>
    {
        internal INCollectionRemoveEventArgs(SequenceNumber sequenceNumber, ref TType item, TType[] items, int index)
            : base(sequenceNumber, ref item, items)
        { _OldIndex = index; }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionRemove; } }

        readonly int _OldIndex;

        public int OldIndex { get { return _OldIndex; } }
    }

    internal sealed class INCollectionMoveEventArgs<TType> : SingleRangeINCollectionEventArgs<TType>
    {
        internal INCollectionMoveEventArgs(SequenceNumber sequenceNumber, ref TType item, TType[] items, int newIndex, int oldIndex)
            : base(sequenceNumber, ref item, items)
        {
            _NewIndex = newIndex;
            _OldIndex = oldIndex; 
        }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionMove; } }

        readonly int _NewIndex;

        public int NewIndex { get { return _NewIndex; } }

        readonly int _OldIndex;

        public int OldIndex { get { return _OldIndex; } }
    }


    internal abstract class DoubleRangeINCollectionEventArgs<TType> : INCollectionChangedEventArgs
    {
        protected DoubleRangeINCollectionEventArgs(SequenceNumber sequenceNumber, ref TType newItem, TType[] newItems, ref TType oldItem, TType[] oldItems)
            : base(sequenceNumber)
        {
            _NewItem = newItem;
            _NewItems = newItems;
            _OldItem = oldItem;
            _OldItems = oldItems;
        }

        readonly TType _NewItem;
        readonly TType[] _NewItems;

        public struct FastIterator
        {
            internal DoubleRangeINCollectionEventArgs<TType> _Container;
            internal int _Index;
        }

        class NewFastEnumeratorClass : IFastEnumerator<TType, DoubleRangeINCollectionEventArgs<TType>, FastIterator>
        {
            #region IFastEnumerator<TType,SingleRangeINCollectionEventArgs<TType>,int> Members

            public FastIterator GetBegin(ref DoubleRangeINCollectionEventArgs<TType> container)
            { return new FastIterator{ _Container = container, _Index = -1 }; }

            public bool MoveNext(ref FastIterator iterator)
            {
                if (iterator._Index < -1)
                    return false;
                else if (iterator._Container._NewItems == null)
                    return ++iterator._Index == 0;
                else
                    return ++iterator._Index < iterator._Container._NewItems.Length;
            }

            public TType GetCurrent(ref FastIterator iterator)
            {
                if (iterator._Container._NewItems == null)
                {
                    if (iterator._Index != 0)
                        throw new ArgumentOutOfRangeException("index");

                    return iterator._Container._NewItem;
                }
                else
                    return iterator._Container._NewItems[iterator._Index];
            }

            #endregion
        }

        static readonly IFastEnumerator<TType, DoubleRangeINCollectionEventArgs<TType>, FastIterator> _NewFastEnumerator = new NewFastEnumeratorClass();
        public static IFastEnumerator<TType, DoubleRangeINCollectionEventArgs<TType>, FastIterator> NewItemsFastEnumerator { get { return _NewFastEnumerator; } }

        public IEnumerable<TType> NewItems 
        { get { return FastEnumerator.ClassicEnumerable(this, NewItemsFastEnumerator); } }

        public int NewItemsCount { get { return _NewItems == null ? 1 : _NewItems.Length; } }


        readonly TType _OldItem;
        readonly TType[] _OldItems;

        class OldFastEnumeratorClass : IFastEnumerator<TType, DoubleRangeINCollectionEventArgs<TType>, FastIterator>
        {
            #region IFastEnumerator<TType,SingleRangeINCollectionEventArgs<TType>,int> Members

            public FastIterator GetBegin(ref DoubleRangeINCollectionEventArgs<TType> container)
            { return new FastIterator { _Container = container, _Index = -1 }; }

            public bool MoveNext(ref FastIterator iterator)
            {
                if (iterator._Index < -1)
                    return false;
                else if (iterator._Container._OldItems == null)
                    return ++iterator._Index == 0;
                else
                    return ++iterator._Index < iterator._Container._OldItems.Length;
            }

            public TType GetCurrent(ref FastIterator iterator)
            {
                if (iterator._Container._OldItems == null)
                {
                    if (iterator._Index != 0)
                        throw new ArgumentOutOfRangeException("index");

                    return iterator._Container._OldItem;
                }
                else
                    return iterator._Container._OldItems[iterator._Index];
            }

            #endregion
        }

        static readonly IFastEnumerator<TType, DoubleRangeINCollectionEventArgs<TType>, FastIterator> _OldFastEnumerator = new OldFastEnumeratorClass();
        public static IFastEnumerator<TType, DoubleRangeINCollectionEventArgs<TType>, FastIterator> OldItemsFastEnumerator { get { return _OldFastEnumerator; } }

        public IEnumerable<TType> OldItems
        { get { return FastEnumerator.ClassicEnumerable(this, OldItemsFastEnumerator); } }

        public int OldItemsCount { get { return _OldItems == null ? 1 : _OldItems.Length; } }
    }

    internal sealed class INCollectionReplaceEventArgs<TType> : DoubleRangeINCollectionEventArgs<TType>
    {
        internal INCollectionReplaceEventArgs(SequenceNumber sequenceNumber, ref TType newItem, TType[] newItems, ref TType oldItem, TType[] oldItems, int index)
            : base(sequenceNumber, ref newItem, newItems, ref oldItem, oldItems)
        {
            _Index = index;
        }

        readonly int _Index;

        public int Index { get { return _Index; } }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionReplace; } }
    }

}
