using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Obtics
{
    /// <summary>
    /// SICollection, static information about the ICollection interface
    /// </summary>
    internal static class SICollection
    {
        /// <summary>
        /// CountPropertyName
        /// </summary>
        public const string CountPropertyName = "Count";

        /// <summary>
        /// CountPropertyChangedEventArgs
        /// </summary>
        public static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(CountPropertyName);

        /// <summary>
        /// IsReadOnlyPropertyName
        /// </summary>
        public const string IsReadOnlyPropertyName = "IsReadOnly";

        /// <summary>
        /// IsReadOnlyPropertyChangedEventArgs
        /// </summary>
        public static readonly PropertyChangedEventArgs IsReadOnlyPropertyChangedEventArgs = new PropertyChangedEventArgs(IsReadOnlyPropertyName);
    }
}
