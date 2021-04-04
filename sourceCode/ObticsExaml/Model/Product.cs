using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Values;
using System.Linq;

namespace ObticsExaml.Model
{
    class Product : ContextElement, IProduct
    {
        public Product(Shop context)
            : base(context)
        { SaleProhibited.Value = true; }

        #region Name property

        IValueProvider<string> _Name = ValueProvider.Dynamic<string>();

        /// <summary>
        /// Name of this product
        /// </summary>
        public IValueProvider<string> Name
        { get { return _Name; } }

        #endregion

        #region ProductParts property

        /// <summary>
        /// Als ProductParts associated with this product
        /// </summary>
        /// <remarks>
        /// There is no direct association from Product to ProductPart. The associated ProductParts
        /// are found by doing a lookup in the total collection of ProductParts.
        /// </remarks>
        public IEnumerable<IProductPart> ProductParts
        { get { return _Context.ProductToProductPartLookup[this]; } }

        /// <summary>
        /// Create a new ProductPart associated with this Product
        /// </summary>
        /// <param name="part">The part needed for this product</param>
        /// <param name="partCount">the number of times it is needed for this product.</param>
        /// <returns>The new ProductPart object</returns>
        public IProductPart AddProductPart(IPart part, uint partCount)
        { return _Context.NewProductPart(this, part, partCount); }

        #endregion

        #region Delete method

        /// <summary>
        /// Gives a reason why this Product can not be deleted
        /// (always empty since a product can always be deleted.
        /// </summary>
        public IValueProvider<string> WhyCanNotDelete
        { get { return ValueProvider.Static(string.Empty); } }

        /// <summary>
        /// Indicates if this product can be deleted
        /// (always true)
        /// </summary>
        public IValueProvider<bool> CanDelete
        { get { return WhyCanNotDelete.Select(wcnd => wcnd == string.Empty); } }

        /// <summary>
        /// Delete this product.
        /// </summary>
        public void Delete()
        { _Context.DeleteProduct(this); }

        #endregion

        #region SaleProhibited property

        IValueProvider<bool> _SaleProhibited = ValueProvider.Dynamic<bool>();

        /// <summary>
        /// Set the Value of this property to true to indicated that this product can not be sold.
        /// </summary>
        public IValueProvider<bool> SaleProhibited
        { get { return _SaleProhibited; } }

        #endregion

        #region CanSell property

        /// <summary>
        /// The Value of this property indicates if this product can be sold or not
        /// a product can not be sold if It is prohibited or there are insufficient items in stock
        /// for one of its parts or there are no parts associated with this product.
        /// </summary>
        public IValueProvider<bool> CanSell
        { get { return WhyCanNotSell.Select( String.IsNullOrEmpty ).Cached(); } }

        /// <summary>
        /// Precompiled Observable Function for WhyCanNotSell property
        /// </summary>
        static Func<Product, IValueProvider<string>> _WhyCanNotSellF = ExpressionObserver.Compile(
            (Product t) =>
                t.SaleProhibited.Value ? string.Format( "Sale of product {0} is prohibited", t.Name.Value ) :
                t.ProductParts.Select(pp => (long)pp.PartCount.Value).Concat(Enumerable.Repeat(0L,1)).Sum() == 0L ? string.Format("Product {0} contains no parts", t.Name.Value) :
                t.ProductParts.Any(pp => pp.PartCount.Value > pp.Part.Value.InStock.Value) ? string.Format("There are insufficient items of part '{0}' in stock", t.ProductParts.First(pp => pp.PartCount.Value > pp.Part.Value.InStock.Value).Part.Value.Name.Value) :
                string.Empty
        );

        /// <summary>
        /// Gives a reason why this product can not be sold
        /// </summary>
        public IValueProvider<string> WhyCanNotSell
        { get { return _WhyCanNotSellF(this).OnException( (Exception ex) => "Unstable result, still calculating" ).Cached(); } }

        /// <summary>
        /// Sell an item of this product.
        /// </summary>
        public void Sell()
        {
            if (!CanSell.Value)
                throw new InvalidOperationException();

            //Subtract parts from stock
            foreach (var pp in ProductParts)
                pp.Part.Value.InStock.Value -= pp.PartCount.Value;
        }

        /// <summary>
        /// Precompiled Observable Function for the SellingStatus property
        /// </summary>
        static Func<Product, IValueProvider<SellingStatusEnum>> _SellingStatusF = ExpressionObserver.Compile(
            (Product t) =>
                !t.CanSell.Value ? SellingStatusEnum.Blocked :
                t.ProductParts.All(pp => pp.Part.Value.StockStatus.Value == StockStatusEnum.Plenty) ? SellingStatusEnum.Free :
                SellingStatusEnum.Limited
        );

        /// <summary>
        /// Indicates the sales expectancy for this product
        ///     SellingStatusEnum.Blocked : this product can not be sold.
        ///     SellingStatusEnum.Limited : the product can be sold, but if no action is taken it will soon be sold out.
        ///     StockStatusEnum.Plenty : The product can be sold freely and there are no obstacles expected soon.
        /// </summary>
        public IValueProvider<SellingStatusEnum> SellingStatus
        { get { return _SellingStatusF(this).OnException((Exception ex) => SellingStatusEnum.Blocked).Cached(); } }

        #endregion
    }
}
