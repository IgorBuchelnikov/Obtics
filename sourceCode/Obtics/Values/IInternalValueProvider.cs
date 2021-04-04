using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Values
{
    internal interface IInternalValueProvider : IValueProvider, INotifyChanged
    { }

    internal interface IInternalValueProvider<T> : IInternalValueProvider, IValueProvider<T>
    { }
}
