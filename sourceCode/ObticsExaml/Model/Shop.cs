using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Obtics.Collections;
using Obtics.Values;
using Obtics;
using System.Collections;

namespace ObticsExaml.Model
{
    /// <summary>
    /// 
    /// </summary>
    class Shop : IShop
    {
        #region Products

        ObservableCollection<Product> _Products = new ObservableCollection<Product>();

        public IEnumerable<IProduct> Products
        { get { return _Products.Cast<IProduct>(); } }

        public IProduct NewProduct(string name)
        {
            Product product = new Product(this);
            _Products.Add(product);
            product.Name.Value = name;
            return product;
        }

        public void DeleteProduct(IProduct product)
        {
            var prd = product as Product;

            if (prd == null || prd.Context != this)
                throw new ArgumentException();

            if (!_Products.Contains(prd) || !prd.CanDelete.Value)
                throw new InvalidOperationException();

            var pps = new List<IProductPart>( prd.ProductParts );

            foreach (var p in pps)
                _ProductParts.Remove((ProductPart)p);

            _Products.Remove(prd);
        }

        #endregion

        #region Parts

        ObservableCollection<Part> _Parts = new ObservableCollection<Part>();

        public IEnumerable<IPart> Parts
        { get { return _Parts.Cast<IPart>(); } }

        public IPart NewPart(string name, uint inStock)
        {
            Part prt = new Part(this);
            _Parts.Add(prt);
            prt.Name.Value = name;
            prt.InStock.Value = inStock;
            return prt;
        }

        internal void DeletePart(IPart part)
        {
            var prt = part as Part;

            if( prt == null || prt.Context != this )
                throw new ArgumentException();

            if( !_Parts.Contains(prt) || !prt.CanDelete.Value )
                throw new InvalidOperationException();

            _Parts.Remove(prt);
        }

        #endregion

        #region ProductParts

        ObservableCollection<ProductPart> _ProductParts = new ObservableCollection<ProductPart>();

        public IEnumerable<IProductPart> ProductParts
        { get { return _ProductParts.Cast<IProductPart>(); } }

        public IProductPart NewProductPart(IProduct product, IPart part, uint count)
        {
            var prd = product as Product;
            var prt = part as Part;

            if (prd == null || prt == null || prd.Context != this || prt.Context != this)
                throw new ArgumentException();

            if ((from pp in _ProductParts where pp.Part.Value == prt && pp.Product.Value == prd select pp).Count().Value != 0)
                throw new InvalidOperationException();

            var prdprt = new ProductPart(this, prd, prt);
            prdprt.PartCount.Value = count;

            this._ProductParts.Add(prdprt);

            return prdprt;
        }

        public void DeleteProductPart(IProductPart productPart)
        {
            var prdprt = productPart as ProductPart;

            if( prdprt == null || prdprt.Context != this )
                throw new ArgumentException();

            if(!_ProductParts.Contains(prdprt))
                throw new InvalidOperationException();

            _ProductParts.Remove(prdprt);
        }

        #endregion

        IObservableLookup<IProduct, IProductPart> _ProductToProductPartLookup;

        public IObservableLookup<IProduct, IProductPart> ProductToProductPartLookup
        {
            get
            {
                return _ProductToProductPartLookup
                    ?? (
                        _ProductToProductPartLookup =
                            ProductParts.ToLookup(prodpart => prodpart.Product.Value)
                    );
            }
        }

        IObservableLookup<IPart, IProductPart> _PartToProductPartLookup;

        public IObservableLookup<IPart, IProductPart> PartToProductPartLookup
        {
            get
            {
                return _PartToProductPartLookup
                    ?? (
                        _PartToProductPartLookup =
                            ProductParts.ToLookup(prodpart => prodpart.Part.Value)
                    );
            }
        }


    }
}
