using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Obtics;
using System.Threading;
using Obtics.Collections;

namespace ObticsUnitTest.Helpers
{
    public class FrameIEnumerableNPC<TType> : FrameIEnumerable<TType>, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        event PropertyChangedEventHandler _PropertyChanged;

        /// <summary>
        /// PropertyChanged
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (FrameBuffer)
                {
                    _PropertyChanged += value;
                    OnPropertyChangedListenerAdded();
                }
            }

            remove
            {
                lock (FrameBuffer)
                {
                    _PropertyChanged -= value;
                    OnPropertyChangedListenerRemoved();
                }
            }
        }

        public event EventHandler PropertyChangedListenerAdded;

        protected virtual void OnPropertyChangedListenerAdded()
        {
            var pcla = PropertyChangedListenerAdded;
            if (pcla != null)
            {
                Monitor.Exit(FrameBuffer);

                try
                {
                    pcla(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(FrameBuffer);
                }
            }
        }

        public event EventHandler PropertyChangedListenerRemoved;

        protected virtual void OnPropertyChangedListenerRemoved()
        {
            var pclr = PropertyChangedListenerRemoved;
            if (pclr != null)
            {
                Monitor.Exit(FrameBuffer);

                try
                {
                    pclr(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(FrameBuffer);
                }
            }
        }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var pc = _PropertyChanged;

            if (pc != null)
            {
                Monitor.Exit(FrameBuffer);

                try
                {
                    pc(this, args);
                }
                finally
                {
                    Monitor.Enter(FrameBuffer);
                }
            }
        }

        protected virtual void OnPropertyChanged(string propname)
        { this.OnPropertyChanged(new PropertyChangedEventArgs(propname)); }

        public int PropertyChangedClientsCount
        { get { lock(FrameBuffer) return _PropertyChanged == null ? 0 : _PropertyChanged.GetInvocationList().Length; } }

        #endregion

        protected override VersionNumber ProtectedAddItems(IList<TType> newItems, int index)
        {
            var res = base.ProtectedAddItems(newItems, index);
            OnPropertyChanged(SIList.ItemsIndexerPropertyName);
            return res;
        }

        protected override VersionNumber ProtectedMoveItems(int count, int newIndex, int oldIndex)
        {
            var res = base.ProtectedMoveItems(count, newIndex, oldIndex);
            OnPropertyChanged(SIList.ItemsIndexerPropertyName);
            return res;
        }

        protected override VersionNumber ProtectedRemoveItems(int count, int index)
        {
            var res = base.ProtectedRemoveItems(count, index);
            OnPropertyChanged(SIList.ItemsIndexerPropertyName);
            return res;
        }

        protected override VersionNumber ProtectedReplaceItems(IList<TType> newItems, int oldItemsCount, int index)
        {
            var res = base.ProtectedReplaceItems(newItems, oldItemsCount, index);
            OnPropertyChanged(SIList.ItemsIndexerPropertyName);
            return res;
        }

        protected override VersionNumber ProtectedResetItems(IList<TType> newItems)
        {
            var res = base.ProtectedResetItems(newItems);
            OnPropertyChanged(SIList.ItemsIndexerPropertyName);
            return res;
        }
    }
}
