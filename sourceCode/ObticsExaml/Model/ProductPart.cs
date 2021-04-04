using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Values;

namespace ObticsExaml.Model
{
    class ProductPart : ContextElement, IProductPart
    {
        public ProductPart(Shop context, Product product, Part part)
            : base(context)
        {
            _Product = product;
            _Part = part;
        }

        #region Product property

        Product _Product;

        /// <summary>
        /// The unchangeable product associated with this ProductPart
        /// (the product our associated Part is needed for).
        /// </summary>
        public IValueProvider<IProduct> Product
        { get { return ValueProvider.Static<IProduct>(_Product); } }

        #endregion

        #region Part property

        Part _Part;

        /// <summary>
        /// THe unchangeable part associated with this ProductPart
        /// (the part needed for our associated product)
        /// </summary>
        public IValueProvider<IPart> Part
        { get { return ValueProvider.Static<IPart>(_Part); } }

        #endregion

        #region PartCount property

        IValueProvider<uint> _PartCount = ValueProvider.Dynamic<uint>();

        /// <summary>
        /// THe number of times our Part is needed for 1 isntance of our Product.
        /// </summary>
        public IValueProvider<uint> PartCount
        { get { return _PartCount; } }

        #endregion

        #region Delete method

        /// <summary>
        /// Delete this ProductPart (remove the association between our Product and our Part)
        /// </summary>
        public void Delete()
        { this._Context.DeleteProductPart(this); }

        #endregion
    }
}
