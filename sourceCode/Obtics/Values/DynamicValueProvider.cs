using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using TvdP.Threading;

namespace Obtics.Values
{
    /// <summary>
    /// ValueProvider object holding a value that can be dynamically updated.
    /// </summary>
    /// <typeparam name="TType">Type of the Value property</typeparam>
    /// <remarks>This is a STATEFULL object in the sense that it doesn't just represent a statically defined view on some source data.
    /// It maintains a reference to the data. </remarks>
    internal sealed class DynamicValueProvider<TType> : IInternalValueProvider<TType>
    {
        /// <summary>
        /// Constructor, taking the initial value for Value as argument
        /// </summary>
        /// <param name="value">The value to initalize the Value property with.</param>
        DynamicValueProvider(TType value)
        { this._Value = value; }

        public static DynamicValueProvider<TType> Create(TType value)
        { return new DynamicValueProvider<TType>(value); }

        #region IValueProvider<TType> Members

        /// <summary>
        /// Propertyname of the Value property
        /// </summary>
        public const string ValuePropertyName = SIValueProvider.ValuePropertyName;

        TType _Value;

        TinyReaderWriterLock _Lock;

        /// <summary>
        /// Value property.
        /// </summary>
        public TType Value
        {
            get 
            {
                _Lock.LockForReading();
                try { return _Value; }
                finally { _Lock.ReleaseForReading(); }
            }
            set
            {
                INCEventArgs pcEventArgs ;
                _Lock.LockForWriting();

                try
                {
                    if (!ObticsEqualityComparer<TType>.Default.Equals(value, _Value))
                    {
                        _Value = value;
                        pcEventArgs = INCEventArgs.PropertyChanged();
                    }
                    else
                        pcEventArgs = null;
                }
                finally { _Lock.ReleaseForWriting(); }

                if (pcEventArgs != null)
#if PARALLEL
                    _ReceiverTable.SendMessage(this, pcEventArgs, false);
#else
                    _ReceiverTable.SendMessage(this, pcEventArgs);
#endif
            }
        }

        #endregion

        #region IValueProvider Members

        object IValueProvider.Value
        {
            get{ return this.Value; }
            set{ this.Value = (TType)value; }
        }

        /// <summary>
        /// Propertyname of the IsReadOnly property
        /// </summary>
        public const string IsReadOnlyPropertyName = SIValueProvider.IsReadOnlyPropertyName ;

        /// <summary>
        /// IsReadOnly property, indicating if the Value property is readonly. In the case of the 
        /// DynamicValueProvider class IsReadOnly always returns false.
        /// </summary>
        public bool IsReadOnly
        { get { return false; } }

        #endregion

        #region IEquatable

        // For statefull objects to be equal the references must be equal (the same instance)
        // this is the default result of object.Equals()

        #endregion

        #region INotifyChanged Members

        NotifyChangedReceiverTable _ReceiverTable;

        public void SubscribeINC(IReceiveChangeNotification consumer)
        { _ReceiverTable.SubscribeINC(consumer); }

        public void UnsubscribeINC(IReceiveChangeNotification consumer)
        { _ReceiverTable.UnsubscribeINC(consumer); }

        #endregion
    }
}
