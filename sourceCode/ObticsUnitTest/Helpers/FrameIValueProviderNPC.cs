using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Obtics;
using System.Threading;

namespace ObticsUnitTest.Helpers
{
    internal class FrameIValueProviderNPC<TType> : FrameIValueProvider<TType>, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        event PropertyChangedEventHandler _PropertyChanged;

        /// <summary>
        /// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (this)
                {
                    _PropertyChanged += value;
                    OnPropertyChangedListenerAdded();
                }
            }

            remove
            {
                lock (this)
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
                Monitor.Exit(this);

                try
                {
                    pcla(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
        }

        public event EventHandler PropertyChangedListenerRemoved;

        protected virtual void OnPropertyChangedListenerRemoved()
        {
            var pclr = PropertyChangedListenerRemoved;

            if (pclr != null)
            {
                Monitor.Exit(this);

                try
                {
                    pclr(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
        }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var pc = _PropertyChanged;

            if (pc != null)
            {
                Monitor.Exit(this);

                try
                {
                    pc(this, args);
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
        }

        public virtual void OnPropertyChanged(string propname)
        { this.OnPropertyChanged(new PropertyChangedEventArgs(propname)); }

        public int PropertyChangedClientsCount
        { get { lock(this) return _PropertyChanged == null ? 0 : _PropertyChanged.GetInvocationList().Length; } }

        #endregion


        protected override TType ProtectedValue
        {
            get
            {
                return base.ProtectedValue;
            }
            set
            {
                TType oldValue = base.ProtectedValue;
                base.ProtectedValue = value;
                if ( !ObticsEqualityComparer<TType>.Default.Equals(oldValue,value) )
                    OnPropertyChanged(ValuePropertyName);
            }
        }

        protected override void ProtectedSetExceptionValue(Exception ex)
        {
            base.ProtectedSetExceptionValue(ex);
            OnPropertyChanged(ValuePropertyName);
        }

        protected override void ProtectedSetValue(TType value)
        {
            bool change = false;
            TType oldValue = default(TType) ;

            try 
            {
                oldValue = _Value;
            }
            catch
            {
                change = true;
            }

            if(!change)
                change = !ObticsEqualityComparer<TType>.Default.Equals(value, oldValue);

            if (change)
            {
                base.ProtectedSetValue(value);
                OnPropertyChanged(ValuePropertyName);
            }
        }

        protected override bool ProtectedIsReadOnly
        {
            get
            {
                return base.ProtectedIsReadOnly;
            }
            set
            {
                bool oldValue = _IsReadOnly;
                base.ProtectedIsReadOnly = value;
                if (oldValue != _IsReadOnly)
                    OnPropertyChanged(IsReadOnlyPropertyName);                    
            }
        }
    }
}
