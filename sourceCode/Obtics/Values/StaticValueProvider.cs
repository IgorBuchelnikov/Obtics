using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;


namespace Obtics.Values
{
    /// <summary>
    /// ValueProvider holding a single never changing value.
    /// </summary>
    /// <typeparam name="TType">The type of the Value property</typeparam>
    /// <remarks>
    /// This ValueProvider doesn't implement INotifyPropertyChanged since none of it's
    /// properties ever change.
    /// </remarks>
    internal sealed class StaticValueProvider<TType> : IInternalValueProvider<TType>, INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor, Initializes Value with the given value
        /// </summary>
        /// <param name="value">The value to assign to the Value property</param>
        StaticValueProvider(TType value)
        { this._Value = value; }

        public static StaticValueProvider<TType> Create(TType value)
        { return new StaticValueProvider<TType>(value); }

        #region IValueProvider<TType> Members

        /// <summary>
        /// Propertyname of the Value property
        /// </summary>
        public const string ValuePropertyName = SIValueProvider.ValuePropertyName;

        readonly TType _Value;

        /// <summary>
        /// Value property. Calling the property setter will raise an exception.
        /// </summary>
        public TType Value
        {
            get { return _Value; }
            set { throw new NotImplementedException(); }
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
        /// IsReadOnly property. Always return true for the StaticValueProvider class
        /// </summary>
        public bool IsReadOnly
        { get { return true; } }

        #endregion

        #region IEquatable

        /// <summary>
        /// Equals override. This object is equal to the parameter if they are of the same type and have the same value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        { return Equals(obj as StaticValueProvider<TType>); }

        public override int GetHashCode()
        { return Hasher.CreateFromRef(typeof(StaticValueProvider<TType>)).Add(_Value, ObticsEqualityComparer<TType>.Default); }

        /// <summary>
        /// This StaticValueProvider&lt;TType&gt; object is equal to another StaticValueProvider&lt;TType&gt; object if they have the same Value.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(StaticValueProvider<TType> other)
        { return other != null && (object.ReferenceEquals(this, other) || ObticsEqualityComparer<TType>.Default.Equals(this._Value, other._Value)); }

        #endregion

        #region INotifyChanged Members

        public void SubscribeINC(IReceiveChangeNotification receiver)
        {}

        public void UnsubscribeINC(IReceiveChangeNotification receiver)
        {}

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {}
            remove 
            {}
        }

        #endregion    
    }
}
