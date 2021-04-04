using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics;
using Obtics.Values;

namespace ObticsUnitTest.Helpers
{
    internal class ValueProviderClient<TType>
    {
        IValueProvider<TType> _Source;

        public IValueProvider<TType> Source
        {
            get
            {
                lock (this)
                    return ProtectedSource;
            }

            set
            {
                lock (this)
                    ProtectedSource = value;
            }
        }

        protected virtual IValueProvider<TType> ProtectedSource
        {
            get { return _Source; }
            set
            {
                if (_Source != value)
                {
                    UpdateSource(true);
                    _Source = value;
                    UpdateSource(false);
                }
            }
        }

        protected virtual void UpdateSource(bool before)
        {
            if (!before)
                Reset();
        }

        TType _Buffer ;

        public TType Buffer
        { get { lock(this) return _Buffer; } }

        public void Reset()
        {
            lock (this)
                ProtectedReset();
        }

        public virtual void ProtectedReset()
        {

            if (_Source != null)
                _Buffer = _Source.Value;
            else
                _Buffer = default(TType);
        }
    }
}
