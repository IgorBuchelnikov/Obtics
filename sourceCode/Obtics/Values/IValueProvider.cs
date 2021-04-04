using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Obtics.Values
{
    /// <summary>
    /// untyped value provider interface
    /// </summary>
    /// <remarks>
    /// An IValueProvider implementation typically also implements <see cref="System.ComponentModel.INotifyPropertyChanged"/>, but this is not required.
    /// </remarks>
    public interface IValueProvider
    {
        /// <summary>
        /// the value
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// is it read-write or read only
        /// </summary>
        bool IsReadOnly { get; }
    }

    /// <summary>
    /// value provider interface for a specific type
    /// </summary>
    /// <typeparam name="TType">Type for the <see cref="IValueProvider{TType}.Value"/> property</typeparam>
    public interface IValueProvider<TType> : IValueProvider
    {
        /// <summary>
        /// The typed value. The Typed Value property should always return the same value as the untyped Value property.
        /// </summary>
        new TType Value { get; set; }
    }

    /// <summary>
    /// Static information about the <see cref="IValueProvider"/> interface
    /// </summary>
    internal static class SIValueProvider
    {
        /// <summary>
        /// Property name of the Value property
        /// </summary>
        public const string ValuePropertyName = "Value";

        /// <summary>
        /// ValuePropertyChangedEventArgs
        /// </summary>
        public static readonly PropertyChangedEventArgs ValuePropertyChangedEventArgs = new PropertyChangedEventArgs(ValuePropertyName);

        /// <summary>
        /// Property name of the IsReadOnly property
        /// </summary>
        public const string IsReadOnlyPropertyName = "IsReadOnly";

        /// <summary>
        /// IsReadOnlyPropertyChangedEventArgs
        /// </summary>
        public static readonly PropertyChangedEventArgs IsReadOnlyPropertyChangedEventArgs = new PropertyChangedEventArgs(IsReadOnlyPropertyName);
    }
}
