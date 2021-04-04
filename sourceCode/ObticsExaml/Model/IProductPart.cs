using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;

namespace ObticsExaml.Model
{
    public interface IProductPart : IContextElement
    {
        IValueProvider<IProduct> Product { get; }
        IValueProvider<IPart> Part { get; }
        IValueProvider<uint> PartCount { get; }
        void Delete();
    }
}
