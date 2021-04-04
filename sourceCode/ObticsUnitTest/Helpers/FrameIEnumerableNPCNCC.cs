using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Obtics.Collections;
using System.Collections;
using System.Threading;

namespace ObticsUnitTest.Helpers
{
    internal class FrameIEnumerableNPCNCC<TType> : FrameIEnumerableNPC<TType>, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        event NotifyCollectionChangedEventHandler _CollectionChanged;

        /// <summary>
        /// PropertyChanged
        /// </summary>
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock (FrameBuffer)
                {
                    _CollectionChanged += value;
                    OnCollectionChangedListenerAdded();
                }
            }

            remove
            {
                lock (FrameBuffer)
                {
                    _CollectionChanged -= value;
                    OnCollectionChangedListenerRemoved();
                }
            }
        }


        public event EventHandler CollectionChangedListenerAdded;

        protected virtual void OnCollectionChangedListenerAdded()
        {
            var ccla = CollectionChangedListenerAdded;
            if (ccla != null)
            {
                Monitor.Exit(FrameBuffer);

                try
                {
                    ccla(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(FrameBuffer);
                }
            }
        }

        public event EventHandler CollectionChangedListenerRemoved;

        protected virtual void OnCollectionChangedListenerRemoved()
        {
            var cclr = CollectionChangedListenerRemoved;

            if (cclr != null)
            {
                Monitor.Exit(FrameBuffer);

                try
                {
                    cclr(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(FrameBuffer);
                }
            }
        }


        public virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var cc = _CollectionChanged;
            if (cc != null)
            {
                Monitor.Exit(FrameBuffer);

                try
                {
                    cc(this, args);
                }
                finally
                {
                    Monitor.Enter(FrameBuffer);
                }
            }
        }

        public int CollectionChangedClientsCount
        { get { lock(FrameBuffer) return _CollectionChanged == null ? 0 : _CollectionChanged.GetInvocationList().Length; } }

        #endregion

        protected override VersionNumber ProtectedAddItems(IList<TType> newItems, int index)
        {
#if SILVERLIGHT
            VersionNumber res = ContentVersion;

            for (int i = 0, end = newItems.Count; i < end; ++i)
            {
                res = base.ProtectedAddItems(new List<TType>(new TType[] { newItems[i] }), index + i);
                OnCollectionChanged(
                    SIOrderedNotifyCollectionChanged.Create(
                        res, 
                        NotifyCollectionChangedAction.Add, 
                        newItems[i], 
                        index + i
                    )
                );
            }
#else
            var res  = base.ProtectedAddItems(newItems, index);
            OnCollectionChanged(
                SIOrderedNotifyCollectionChanged.Create(
                    res, 
                    NotifyCollectionChangedAction.Add, 
                    (IList)newItems, 
                    index
                )
            );
#endif
            return res;
        }

        protected override VersionNumber ProtectedMoveItems(int count, int newIndex, int oldIndex)
        {
            List<TType> range = FrameBuffer.GetRange(oldIndex, count);

#if SILVERLIGHT
            var res = ProtectedRemoveItems(count, oldIndex);
            res = ProtectedAddItems(range, newIndex);
#else
            var res = base.ProtectedMoveItems(count, newIndex, oldIndex);
            OnCollectionChanged(
                SIOrderedNotifyCollectionChanged.Create(
                    res,
                    NotifyCollectionChangedAction.Move,
                    (IList)range,
                    newIndex,
                    oldIndex
                )
            );
#endif
            return res;
        }

        protected override VersionNumber ProtectedRemoveItems(int count, int index)
        {
            List<TType> range = FrameBuffer.GetRange(index, count);
#if SILVERLIGHT
            VersionNumber res = ContentVersion;

            for (int i = 0; i < count; ++i)
            {
                res = base.ProtectedRemoveItems(1, index);
                OnCollectionChanged(
                    SIOrderedNotifyCollectionChanged.Create(
                        res,
                        NotifyCollectionChangedAction.Remove,
                        range[i],
                        index
                    )
                );
            }
#else
            var res  = base.ProtectedRemoveItems(count, index);
            OnCollectionChanged(
                SIOrderedNotifyCollectionChanged.Create(
                    res,
                    NotifyCollectionChangedAction.Remove,
                    (IList)range,
                    index
                )
            );
#endif

            return res;
        }

        protected override VersionNumber ProtectedReplaceItems(IList<TType> newItems, int oldItemsCount, int index)
        {
            List<TType> oldRange = FrameBuffer.GetRange(index, oldItemsCount);
#if SILVERLIGHT
            VersionNumber res = ContentVersion;

            var overlapCount = Math.Min(newItems.Count, oldItemsCount);
            int i = 0;

            for (; i < overlapCount; ++i)
            {
                res = base.ProtectedReplaceItems(new List<TType>(new TType[] { newItems[i] }), 1, index + i);
                OnCollectionChanged(
                    SIOrderedNotifyCollectionChanged.Create(
                        res,
                        NotifyCollectionChangedAction.Replace,
                        newItems[i],
                        oldRange[i],
                        index + i
                    )
                );
            }

            if (overlapCount < oldItemsCount)
                res = ProtectedRemoveItems(oldItemsCount - overlapCount, index + i);

            if (overlapCount < newItems.Count)
                res = ProtectedAddItems(newItems.Skip(i).ToList(), index + i);
#else
            var res = base.ProtectedReplaceItems(newItems, oldItemsCount, index);
            OnCollectionChanged(
                SIOrderedNotifyCollectionChanged.Create(
                    res,
                    NotifyCollectionChangedAction.Replace,
                    (IList)newItems,
                    (IList)oldRange,
                    index
                )
            );
#endif
            return res;
        }

        protected override VersionNumber ProtectedResetItems(IList<TType> newItems)
        {
            var res = base.ProtectedResetItems(newItems);
            OnCollectionChanged(
                SIOrderedNotifyCollectionChanged.Create(
                    res,
                    NotifyCollectionChangedAction.Reset
                )
            );
            return res;
        }
    }
}
