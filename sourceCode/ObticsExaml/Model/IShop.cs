using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObticsExaml.Model
{
    public interface IShop
    {
        IEnumerable<IProduct> Products { get; }
        IProduct NewProduct(string name);

        IEnumerable<IPart> Parts { get; }
        IPart NewPart(string name, uint inStock);

        IEnumerable<IProductPart> ProductParts { get; }
        IProductPart NewProductPart(IProduct product, IPart part, uint count);
    }
}
