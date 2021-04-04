using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Obtics.Values;
using System.ComponentModel;
using ObticsExaml.Model;
using System.Linq;

namespace ObticsExaml
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {        
        public Window1()
        {            
            InitializeComponent();
            InitializeContext();
        }

        IShop _Shop;

        public IShop Shop
        { get { return _Shop; } }


        #region SelectedProduct IValueProvider property

        private IValueProvider<IProduct> _SelectedProduct = ValueProvider.Dynamic<IProduct>();

        /// <summary>
        /// Represents the Product currently selected on screen.
        /// </summary>
        public IValueProvider<IProduct> SelectedProduct { get { return _SelectedProduct; } }

        #endregion

        #region SelectedPart IValueProvider property

        private IValueProvider<IPart> _SelectedPart = ValueProvider.Dynamic<IPart>();

        /// <summary>
        /// The currently selected part on the screen.
        /// </summary>
        public IValueProvider<IPart> SelectedPart { get { return _SelectedPart; } }

        #endregion

        #region SelectedProductPart IValueProvider property

        private IValueProvider<IProductPart> _SelectedProductPart = ValueProvider.Dynamic<IProductPart>();

        public IValueProvider<IProductPart> SelectedProductPart { get { return _SelectedProductPart; } }

        #endregion

        /// <summary>
        /// Link the selectable items. If both SelectedProduct and SelectedPart get set then SelectedProductPart
        /// gets set to either null or the ProductPart linking the selected product and part.
        /// When ProductPart gets set to a value other than null then both SelectedPart and SelectedProduct
        /// get set to the product and part linkend by the selected ProductPart.
        /// </summary>
        void LinkSelectableItems()
        {
            ((INotifyPropertyChanged)_SelectedProductPart).PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "Value" && _SelectedProductPart.Value != null)
                {
                    if (_SelectedProduct.Value != _SelectedProductPart.Value.Product.Value)
                        _SelectedProduct.Value = _SelectedProductPart.Value.Product.Value;

                    if (_SelectedPart.Value != _SelectedProductPart.Value.Part.Value)
                        _SelectedPart.Value = _SelectedProductPart.Value.Part.Value;
                }
            };

            PropertyChangedEventHandler handler = delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "Value")
                {
                    if (_SelectedProductPart.Value == null || _SelectedProduct.Value != _SelectedProductPart.Value.Product.Value || _SelectedPart.Value != _SelectedProductPart.Value.Part.Value)
                    {
                        if (_SelectedProduct.Value == null || _SelectedPart.Value == null)
                            _SelectedProductPart.Value = null;
                        else
                            _SelectedProductPart.Value = _SelectedProduct.Value.ProductParts.FirstOrDefault(pp => pp.Part.Value == _SelectedPart.Value);
                    }
                }
            };

            ((INotifyPropertyChanged)_SelectedProduct).PropertyChanged += handler;
            ((INotifyPropertyChanged)_SelectedPart).PropertyChanged += handler;
        }

        #region ProductPanel control

        /// <summary>
        /// If the product detail panel should be enabled.
        /// </summary>
        /// <remarks>
        /// True if and only if a Product has bee selected on screen. 
        /// Therefore a simple value transformation of SelectedProduct
        /// </remarks>
        public IValueProvider<bool> ProductPanelEnabled
        { get { return _SelectedProduct.Select(p => p != null); } }


        /// <summary>
        /// Event handler for the NewProductButton. Creates a new product when invoked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Finds a unique name for the new product and creates a roduct with that name.
        /// </remarks>
        private void NewProductButton_Click(object sender, RoutedEventArgs e)
        {
            string name;
            int nameCounter = 1;
            var existingNames = Shop.Products.Select(p => p.Name.Value);

            do
            {
                name = string.Format("* New Product ({0})", nameCounter++);
            }
            while (existingNames.Contains(name));

            //select newly added product
            ProductsList.SelectedItem = Shop.NewProduct(name);
        }

        /// <summary>
        /// ToolTip for the DeleteSelectedProduct button
        /// </summary>
        /// <remarks>
        /// Combines an explicit reason comming from the selected product with the fact if an actual
        /// product has been selected or not.
        /// Naturaly code is not the place for display text; but this is just a demostration.
        /// For performance reasons it would be wise to precompile the lambda expression. Here instead
        /// the whole ValueProvider is constructed inside the method body.
        /// </remarks>
        public IValueProvider<string> DeleteSelectedProductToolTip
        {
            get
            {
                return
                    ExpressionObserver.Execute(
                        this, 
                        t =>
                            t.SelectedProduct.Value == null ? "Can't delete product; No product selected" :
                            String.Format(
                                (
                                    t.SelectedProduct.Value.WhyCanNotDelete.Value == string.Empty ? "Delete product '{0}'" :
                                    "Can't delete product '{0}'; " + t.SelectedProduct.Value.WhyCanNotDelete.Value
                                ),
                                t.SelectedProduct.Value.Name.Value
                            )
                    )
                    .OnException( 
                        (Exception ex) => 
                            String.Empty 
                    );
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        { 
            SelectedProduct.Value.Delete();
            SelectedProduct.Value = null;
        }

        /// <summary>
        /// Gives the visibility value for the 'Can't sell product; ...'  warning
        /// on the screen. Visible if a product has been selected and the product
        /// can't be sold.
        /// </summary>
        public IValueProvider<Visibility> CantSellSelectedProductWarningVisibility
        {
            get
            {
                return ExpressionObserver.Execute(
                    this,
                    t => t.SelectedProduct.Value == null || t.SelectedProduct.Value.CanSell.Value ? Visibility.Hidden : Visibility.Visible
                )
                .OnException(
                    (Exception ex) =>
                        Visibility.Hidden
                );
            }
        }

        #endregion

        #region Part panel control

        /// <summary>
        /// If the part detail panel should be enabled or not.
        /// </summary>
        public IValueProvider<bool> PartPanelEnabled
        { get { return _SelectedPart.Select(p => p != null); } }

        /// <summary>
        /// Select all products the currently selected part is used in
        /// and sort those by name.
        /// </summary>
        public IEnumerable<IProduct> ProductsWhereSelectedPartUsedIn
        {
            get
            {
                return
                    ExpressionObserver.Execute(this, t =>
                            from pp in t.SelectedPart.Value.ProductParts
                            select pp.Product.Value into p
                            orderby p.Name.Value
                            select p                            
                    )
                    .OnException(
                        (Exception ex) =>
                            null
                    )
                    .Cascade();
            }
        }

        /// <summary>
        /// ToolTip for the DeleteSelectedPart button
        /// </summary>
        /// <remarks>
        /// Combines an explicit reason comming from the selected part with the fact if an actual
        /// part has been selected or not.
        /// Naturaly code is not the place for display text; but this is just a demonstration.
        /// </remarks>
        public IValueProvider<string> DeleteSelectedPartToolTip
        {
            get
            {
                return
                    ExpressionObserver.Execute(this, t =>                        
                            t.SelectedPart.Value == null ? "Can't delete part; No part selected" :
                            String.Format(
                                (
                                    t.SelectedPart.Value.WhyCanNotDelete.Value == string.Empty ? "Delete part '{0}'" :
                                    "Can't delete part '{0}'; " + t.SelectedPart.Value.WhyCanNotDelete.Value
                                ),
                                SelectedPart.Value.Name.Value
                            )
                    )
                    .OnException(
                        (Exception ex) =>
                            String.Empty
                    );
            }
        }


        /// <summary>
        /// Event handler for the NewPartButton. Creates a new part when invoked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Finds a unique name for the new part and creates a roduct with that name.
        /// </remarks>
        private void NewPartButton_Click(object sender, RoutedEventArgs e)
        {
            string name ;
            int nameCounter = 1;
            var existingNames = Shop.Parts.Select(p => p.Name.Value);

            do
            {
                name = string.Format("* New Part ({0})", nameCounter++);
            }
            while (existingNames.Contains(name));

            //select newly added part
            SelectedPart.Value = Shop.NewPart(name, 0);
        }

        private void DeletePartButton_Click(object sender, RoutedEventArgs e)
        { 
            SelectedPart.Value.Delete();
            SelectedPart.Value = null;
        }

        #endregion

        #region Part add and remove from Product buttons control (ProductPart control)

        private void AddPartToProductButton_Click(object sender, RoutedEventArgs e)
        { SelectedProduct.Value.AddProductPart(SelectedPart.Value,1); }

        /// <summary>
        /// Indicates if the currently selected Part (if any) can be added
        /// as ProductPart to the currently selected Product (if any).
        /// </summary>
        public IValueProvider<bool> CanAddPartToProduct
        {
            get
            {
                return ExpressionObserver.Execute(this, t =>
                    t.SelectedProduct.Value != null
                    && !t.SelectedProduct.Value.ProductParts.Select(pp => pp.Part.Value).Contains(t.SelectedPart.Value)
                )
                .OnException(
                    (Exception ex) =>
                        false
                );
            }
        }

        private void RemovePartFromProductButton_Click(object sender, RoutedEventArgs e)
        { 
            SelectedProductPart.Value.Delete();
            SelectedProductPart.Value = null;
        }

        /// <summary>
        /// Indicates if the currently selected ProductPart (if any) can be removed
        /// from the currently selected product (if any).
        /// </summary>
        public IValueProvider<bool> CanRemovePartFromProduct
        { 
            get
            {
                return ExpressionObserver.Execute(this, t =>
                    t.SelectedProductPart.Value != null
                    && t.SelectedProductPart.Value.Product.Value == t.SelectedProduct.Value
                )
                .OnException(
                    (Exception ex) =>
                        false
                );
            }
        }

        #endregion


        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ctrl = (System.Windows.Controls.Primitives.Selector)sender;

            if (ctrl.IsKeyboardFocusWithin)
                ctrl.GetBindingExpression(System.Windows.Controls.Primitives.Selector.SelectedItemProperty).UpdateSource();
            else
                ctrl.GetBindingExpression(System.Windows.Controls.Primitives.Selector.SelectedItemProperty).UpdateTarget();

        }
       
        
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            LinkSelectableItems();
        }

        private void InitializeContext()
        {
            var shop = (Model.Shop)this.FindResource("shop") ;

            var WheelSmall = shop.NewPart("Wheel - Small", 100);
            var WheelLarge = shop.NewPart("Wheel - Large", 100);
            var BoltSmall = shop.NewPart("Bolt - Small", 400);
            var BoltMedium = shop.NewPart("Bolt - Medium", 400);
            var BoltLarge = shop.NewPart("Bolt - Large", 400);
            var Strap = shop.NewPart("Strap", 50);
            var Tire = shop.NewPart("Tire", 60);
            var BicycleFrame = shop.NewPart("Bicycle Frame", 50);
            var TricycleFrame = shop.NewPart("Tricycle Frame", 50);
            var Pedle = shop.NewPart("Pedle", 50);
            var Valve = shop.NewPart("Valve", 20);
            var RubberTube = shop.NewPart("Rubber tube", 30);
            var Cylinder = shop.NewPart("Cylinder", 50);
            var TwoToneBell = shop.NewPart("Two-tone bell", 7);

            var Skates = shop.NewProduct("Skates");
            var Bicycle = shop.NewProduct("Bicycle");
            var Tricycle = shop.NewProduct("Tricycle");
            var Pump = shop.NewProduct("Pump");

            Skates.AddProductPart(WheelSmall, 8);
            Skates.AddProductPart(Strap, 2);
            Skates.AddProductPart(BoltSmall, 8);
            Skates.AddProductPart(BoltMedium, 4);

            Bicycle.AddProductPart(WheelLarge, 2);
            Bicycle.AddProductPart(BicycleFrame, 1);
            Bicycle.AddProductPart(BoltSmall, 2);
            Bicycle.AddProductPart(BoltMedium, 6);
            Bicycle.AddProductPart(BoltLarge, 4);
            Bicycle.AddProductPart(Pedle, 2);
            Bicycle.AddProductPart(Tire, 2);

            Tricycle.AddProductPart(WheelLarge, 3);
            Tricycle.AddProductPart(TricycleFrame, 1);
            Tricycle.AddProductPart(BoltSmall, 2);
            Tricycle.AddProductPart(BoltMedium, 8);
            Tricycle.AddProductPart(BoltLarge, 7);
            Tricycle.AddProductPart(Pedle, 2);
            Tricycle.AddProductPart(Tire, 3);

            Pump.AddProductPart(Valve, 1);
            Pump.AddProductPart(Cylinder, 1);
            Pump.AddProductPart(RubberTube, 1);
            Pump.AddProductPart(BoltMedium, 1);

            Skates.SaleProhibited.Value = false;
            Bicycle.SaleProhibited.Value = false;
            Tricycle.SaleProhibited.Value = false;
            Pump.SaleProhibited.Value = false;

            this.DataContext = _Shop = shop;
        }

        private void AddStockButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPart.Value.InStock.Value += Convert.ToUInt32(((Button)sender).Tag);
        }
    }
}
