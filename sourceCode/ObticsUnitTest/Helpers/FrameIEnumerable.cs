using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Obtics.Collections;
using System.Threading;

namespace ObticsUnitTest.Helpers
{
    public interface IFrameIEnumerable<TType>
    {
        void AddItems(IList<TType> newItems, int index);
        void MoveItems(int count, int newIndex, int oldIndex);
        void RemoveItems(int count, int index);
        void ReplaceItems(IList<TType> newItems, int oldItemsCount, int index);
        void ResetItems(IList<TType> newItems);
    }

    public class FrameIEnumerable<TType> : IFrameIEnumerable<TType>, IVersionedEnumerable<TType>, ICollection
    {
        List<TType> _FrameBuffer = new List<TType>();

        public List<TType> FrameBuffer
        {
            get { return _FrameBuffer; }
            set { _FrameBuffer = value; }
        }

        public event EventHandler GetEnumeratorCalled;

        protected void OnGetEnumeratorCalled()
        {
            var gec = GetEnumeratorCalled;

            if (gec != null)
            {
                Monitor.Exit(FrameBuffer);

                try
                {
                    gec(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(FrameBuffer);
                }
            }
        }

        #region IEnumerable<TType> Members

        IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        protected VersionNumber GetNextContentVersion()
        { return (ContentVersion = ContentVersion.Next); }

        public void AddItems(IList<TType> newItems, int index)
        {
            lock (FrameBuffer)
                ProtectedAddItems(newItems, index);
        }

        protected virtual VersionNumber ProtectedAddItems(IList<TType> newItems, int index)
        {
            FrameBuffer.InsertRange(index, newItems);
            return GetNextContentVersion();
        }

        public void MoveItems(int count, int newIndex, int oldIndex)
        {
            lock (FrameBuffer)
                ProtectedMoveItems(count, newIndex, oldIndex);
        }

        protected virtual VersionNumber ProtectedMoveItems(int count, int newIndex, int oldIndex)
        {
            List<TType> range = FrameBuffer.GetRange(oldIndex, count);
            FrameBuffer.RemoveRange(oldIndex, count);
            FrameBuffer.InsertRange(newIndex, range);
            return GetNextContentVersion();
        }

        public void RemoveItems(int count, int index)
        {
            lock (FrameBuffer)
                ProtectedRemoveItems(count, index);
        }

        protected virtual VersionNumber ProtectedRemoveItems(int count, int index)
        {
            FrameBuffer.RemoveRange(index, count);
            return GetNextContentVersion();
        }

        public void ReplaceItems(IList<TType> newItems, int oldItemsCount, int index)
        {
            lock (FrameBuffer)
                ProtectedReplaceItems(newItems, oldItemsCount, index);
        }

        protected virtual VersionNumber ProtectedReplaceItems(IList<TType> newItems, int oldItemsCount, int index)
        {
            FrameBuffer.RemoveRange(index, oldItemsCount);
            FrameBuffer.InsertRange(index, newItems);
            return GetNextContentVersion();
        }

        public void ResetItems(IList<TType> newItems)
        {
            lock (FrameBuffer)
                ProtectedResetItems(newItems);
        }

        protected virtual VersionNumber ProtectedResetItems(IList<TType> newItems)
        {
            FrameBuffer.Clear();
            FrameBuffer.AddRange(newItems);
            return GetNextContentVersion();
        }

        #region IVersionedEnumerable<TType> Members

        public IVersionedEnumerator<TType> GetEnumerator()
        {
            lock (FrameBuffer)
            {
                var res = VersionedEnumerator.WithContentVersion(((IEnumerable<TType>)_FrameBuffer.ToArray()).GetEnumerator(), ContentVersion);
                OnGetEnumeratorCalled();
                return res;
            }
        }

        #endregion

        #region IVersionedEnumerable Members

        VersionNumber _VersionNumber;

        public VersionNumber ContentVersion
        {
            get { lock(FrameBuffer) return _VersionNumber; }
            set { lock(FrameBuffer) _VersionNumber = value; }
        }

        IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return FrameBuffer; }
        }

        #endregion
    }
}
