using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics;
using Obtics.Values;
using System.Threading;

namespace ObticsUnitTest.Helpers
{
    internal class ValueChangedEventArgs<TType> : EventArgs
    {
        TType _OldValue;

        public TType OldValue
        { get{ return _OldValue; } }

        TType _NewValue;

        public TType NewValue
        { get{ return _NewValue; } }

        public ValueChangedEventArgs( TType newValue, TType oldValue )
        {
            _OldValue = oldValue;
            _NewValue = newValue;
        }

    }
    internal class FrameIValueProvider<TType> : IValueProvider<TType>
    {
        #region IValueProvider<TType> Members

        protected TType _Value;
        protected Exception _ExceptionValue;

        public const string ValuePropertyName = "Value";

        public TType Value
        {
            get
            {
                lock (this)
                {
                    if (_ExceptionValue != null)
                        throw _ExceptionValue;

                    var res = ProtectedValue;
                    OnGetValueCalled();
                    return res;
                }
            }
            set
            {
                lock (this)
                {
                    var oldValue = ProtectedValue;
                    ProtectedValue = value;
                    OnSetValueCalled(value, oldValue);
                }
            }
        }

        public void SetExceptionValue(Exception ex)
        {
            lock (this)
            {
                ProtectedSetExceptionValue(ex);
            }
        }

        protected virtual void ProtectedSetExceptionValue(Exception ex)
        {
            _ExceptionValue = ex;
        }

        protected virtual TType ProtectedValue
        {
            get
            { return _Value; }
            set
            {
                if (_IsReadOnly)
                    throw new NotSupportedException();

                _Value = value; 
            }
        }


        internal void SetValue(TType value)
        { lock (this) ProtectedSetValue(value); }

        protected virtual void ProtectedSetValue(TType value)
        {
            _ExceptionValue = null;
            _Value = value; 
        }


        public event EventHandler GetValueCalled;

        protected virtual void OnGetValueCalled()
        {
            var gvc = GetValueCalled;


            if (gvc != null)
            {
                Monitor.Exit(this);

                try
                {
                    gvc(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
        }

        public event EventHandler<ValueChangedEventArgs<TType>> SetValueCalled;

        protected virtual void OnSetValueCalled(TType newValue, TType oldValue)
        {
            var svc = SetValueCalled;

            if (svc != null)
            {
                Monitor.Exit(this);

                try
                {
                    svc(this, new ValueChangedEventArgs<TType>(newValue, oldValue));
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
        }


        #endregion

        #region IValueProvider Members

        object IValueProvider.Value
        {
            get{ return this.Value; }
            set{ this.Value = (TType)value; }
        }

        protected bool _IsReadOnly;

        public const string IsReadOnlyPropertyName = "IsReadOnly";

        public bool IsReadOnly
        {
            get
            {
                lock (this)
                {
                    var res = _IsReadOnly;
                    OnGetIsReadOnlyCalled();
                    return res;
                }
            }

            internal set
            {
                lock (this)
                    ProtectedIsReadOnly = value;
            }
        }

        protected virtual bool ProtectedIsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            set
            { _IsReadOnly = value; }
        }

        public event EventHandler GetIsReadOnlyCalled;

        protected virtual void OnGetIsReadOnlyCalled()
        {
            var giroc = GetIsReadOnlyCalled;

            if (giroc != null)
            {
                Monitor.Exit(this);

                try
                {
                    giroc(this, EventArgs.Empty);
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
        }

        #endregion
    }
}
