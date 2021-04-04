using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace ObticsUnitTest.Helpers
{
#if !SILVERLIGHT
    class BindingListFrame<TElement> : IFrameIEnumerable<TElement>
    {
        public BindingList<TElement> _BindingList;

        public BindingListFrame(BindingList<TElement> bl)
        { _BindingList = bl; }

        #region IFrameIEnumerable<TElement> Members

        public void AddItems(IList<TElement> newItems, int index)
        {
            lock (((ICollection)_BindingList).SyncRoot)
            foreach (var item in newItems)
            {                
                    _BindingList.Insert(index++, item);
            }
        }

        public void MoveItems(int count, int newIndex, int oldIndex)
        {
            lock (((ICollection)_BindingList).SyncRoot)
            if (newIndex > oldIndex)
            {
                for (int i = 0; i < count; ++i)
                {
                    var item = _BindingList[oldIndex];
                    _BindingList.RemoveAt(oldIndex);
                    _BindingList.Insert(newIndex + count - 1, item);
                }
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    var item = _BindingList[oldIndex + count - 1];
                    _BindingList.RemoveAt(oldIndex + count - 1);
                    _BindingList.Insert(newIndex, item);
                }
            }
        }

        public void RemoveItems(int count, int index)
        {
            lock (((ICollection)_BindingList).SyncRoot)
            for(int i = 0; i < count; ++i)
                _BindingList.RemoveAt(index);
        }

        public void ReplaceItems(IList<TElement> newItems, int oldItemsCount, int index)
        {
            lock (((ICollection)_BindingList).SyncRoot)
            {
                var iterator = newItems.GetEnumerator();
                var i = 0;

                for (; i < oldItemsCount && iterator.MoveNext(); ++i)
                    _BindingList[index + i] = iterator.Current;

                if (i != oldItemsCount)
                    for (int j = i; j < oldItemsCount; ++j)
                        _BindingList.RemoveAt(index + i);
                else
                    while (iterator.MoveNext())
                        _BindingList.Insert(index + i++, iterator.Current);
            }
        }

        public void ResetItems(IList<TElement> newItems)
        {
            lock (((ICollection)_BindingList).SyncRoot)
                _BindingList.Clear();

            AddItems(newItems, 0);
        }

        #endregion
    }
#endif
}
