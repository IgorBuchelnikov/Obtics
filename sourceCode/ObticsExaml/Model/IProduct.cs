using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;

namespace ObticsExaml.Model
{
    public enum SellingStatusEnum
    {
        Free,
        Limited,
        Blocked
    };

    public interface IProduct : IContextElement
    {
        IValueProvider<string> Name { get; }
        IEnumerable<IProductPart> ProductParts { get; }
        IProductPart AddProductPart(IPart part, uint partCount);
        IValueProvider<bool> SaleProhibited { get; }
        IValueProvider<SellingStatusEnum> SellingStatus { get; }
        IValueProvider<bool> CanSell { get; }
        IValueProvider<string> WhyCanNotSell { get; }
        IValueProvider<bool> CanDelete { get; }
        IValueProvider<string> WhyCanNotDelete { get; }
        void Delete();

        void Sell();
    }
}
