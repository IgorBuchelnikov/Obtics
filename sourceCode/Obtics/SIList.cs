using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Obtics
{
    /// <summary>
    /// SIList, static information about the IList interface
    /// </summary>
    internal static class SIList
    {
        /// <summary>
        /// ItemsIndexerPropertyName
        /// </summary>
        public const string ItemsIndexerPropertyName = "Items[]";

        /// <summary>
        /// ItemsIndexerPropertyChangedEventArgs
        /// </summary>
        public static readonly PropertyChangedEventArgs ItemsIndexerPropertyChangedEventArgs = new PropertyChangedEventArgs(ItemsIndexerPropertyName);
    }
}
