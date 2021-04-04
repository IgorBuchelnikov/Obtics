using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;

namespace ObticsExaml.Model
{
    public enum StockStatusEnum
    {
        Plenty,
        Low,
        Insufficient,
        Empty
    }

    public interface IPart : IContextElement
    {
        IValueProvider<string> Name { get; }
        IEnumerable<IProductPart> ProductParts { get; }
        IProductPart AddProductPart(IProduct product, uint partCount);
        IValueProvider<StockStatusEnum> StockStatus { get; }
        IValueProvider<uint> InStock { get; }
        IValueProvider<bool> CanDelete { get; }
        IValueProvider<string> WhyCanNotDelete { get; }
        void Delete();
    }
}
