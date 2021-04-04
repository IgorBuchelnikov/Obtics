using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Obtics.Configuration
{
#if !SILVERLIGHT
    /// <summary>
    /// Internaly used TypeConverter that converts a string to a <see cref="Type"/> object.
    /// </summary>
    class TypeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        { return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType); }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var valueAsString = value as string;
            return valueAsString != null ? (object)Type.GetType(valueAsString) : base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        { return destinationType == typeof(string) || base.CanConvertTo(context, destinationType); }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        { return destinationType == typeof(string) ? (object)((Type)value).AssemblyQualifiedName : base.ConvertTo(context, culture, value, destinationType); }
    }
#endif
}
