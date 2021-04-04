using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Values;
using System.Linq;

namespace ObticsExaml.Model
{
    class Part : ContextElement, IPart
    {
        public Part(Shop context)
            : base(context)
        { }

        #region Name property
      
        IValueProvider<string> _Name = ValueProvider.Dynamic<string>();

        /// <summary>
        /// Name of this part
        /// </summary>
        /// <remarks>
        /// There is no need to store name in a DynamicValueProvider. And it could have been presented as an
        /// ordinary property. Only for reason of consistency it is presented as an IValueProvider.
        /// </remarks>
        public IValueProvider<string> Name
        { get { return _Name; } }

        #endregion

        #region ProductParts property

        /// <summary>
        /// Collection of all ProductPart objects associated with this part.
        /// </summary>
        /// <remarks>
        /// Note that Part doesn't have a direct reference to any ProductPart object. The association is maintained
        /// by a lookup in the total collection of ProductParts.
        /// </remarks>
        public IEnumerable<IProductPart> ProductParts
        { get { return _Context.PartToProductPartLookup[this]; } }

        /// <summary>
        /// Create a new ProductPart object associated with this part.
        /// </summary>
        /// <param name="product">The Product object this Part object is going to be associated with.</param>
        /// <param name="partCount">The Part-count for the association (The number of times this part is being used in the product)</param>
        /// <returns>The newly created ProductPart object</returns>
        public IProductPart AddProductPart(IProduct product, uint partCount)
        { return _Context.NewProductPart(product, this, partCount); }

        #endregion

        #region InStock property

        IValueProvider<uint> _InStock = ValueProvider.Dynamic<uint>();

        /// <summary>
        /// How many items of this Part are in stock.
        /// </summary>
        public IValueProvider<uint> InStock
        { get { return _InStock; } }

        /// <summary>
        /// Precompiled Observable Function for the StockStatus property
        /// </summary>
        static Func<Part, IValueProvider<StockStatusEnum>> _StockStatusF = ExpressionObserver.Compile(
            (Part t) =>
                t.InStock.Value == 0 ? StockStatusEnum.Empty :
                t.ProductParts.Where(pp => !pp.Product.Value.SaleProhibited.Value).Select(ppx => (int)ppx.PartCount.Value).Concat(new int[] {0}).Max() > (int)t.InStock.Value ? StockStatusEnum.Insufficient :
                t.ProductParts.Where(pp => !pp.Product.Value.SaleProhibited.Value).Select(ppx => (int)ppx.PartCount.Value).Concat(new int[] {0}).Sum() * 7 > (int)t.InStock.Value ? StockStatusEnum.Low :
                StockStatusEnum.Plenty
        );

        /// <summary>
        /// Gives the current status of the stock. 
        /// The Value can be 
        ///     StockStatusEnum.Empty : 0 items in stock
        ///     StockStatusEnum.Insufficient : There is a product on sale that requires more items of this part than that there are in stock 
        ///     StockStatusEnum.Low : Stock is less than 7 times that total ammount this part is required for all products on sale.
        ///     StockStatusEnum.Plenty : in all other cases
        /// </summary>
        public IValueProvider<StockStatusEnum> StockStatus
        { get { return _StockStatusF(this).OnException( (Exception ex) => StockStatusEnum.Empty ); } }

        #endregion

        #region Delete method

        /// <summary>
        /// The Value is false if this part can not be deleted. That is: there are still items in stock and/or it is
        /// still a required part for any product.
        /// </summary>
        public IValueProvider<bool> CanDelete
        { get { return WhyCanNotDelete.Select(wcnd => wcnd == string.Empty); } }

        /// <summary>
        /// Precompiled Observable Function for the WhyCanNotDelete property
        /// </summary>
        static Func<Part, IValueProvider<string>> _WhyCanNotDeleteF = ExpressionObserver.Compile(
            (Part t) =>
                t.ProductParts.Any() ? String.Format("Part still used in product '{0}'", t.ProductParts.First().Product.Value.Name.Value) :
                t.InStock.Value != 0 ? String.Format("Still {0} items in stock", t.InStock.Value) :
                string.Empty
        );

        /// <summary>
        /// Gives an explanation of why a part can not be deleted.
        /// </summary>
        public IValueProvider<string> WhyCanNotDelete
        { get { return _WhyCanNotDeleteF(this); } }

        /// <summary>
        /// Delete this part.
        /// </summary>
        public void Delete()
        { _Context.DeletePart(this); }

        #endregion
    }
}
