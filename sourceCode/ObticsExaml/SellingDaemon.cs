using System;
using System.Collections.Generic;
using System.Text;
using ObticsExaml.Model;
using System.Threading;
using Obtics;
using Obtics.Values;
using System.Linq;

namespace ObticsExaml
{
    public class SellingDaemon
    {
        public SellingDaemon()
        { 
            _Timer = new System.Threading.Timer(new TimerCallback(TimerCallback));
            _Random = new Random();
        }

        #region IContextElement Members

        protected IShop _Context;

        public IShop Context
        { 
            get { return _Context; }
            set
            {
                if (_Context != value)
                {
                    if (_Context != null)
                        _Timer.Change(Timeout.Infinite, Timeout.Infinite);

                    _Context = value;

                    if (_Context != null)
                        KickTimer();
                }
            }
        }

        #endregion

        IValueProvider<string> _LastSaleMessage = ValueProvider.Dynamic("Nothing sold yet.");

        public IValueProvider<string> LastSaleMessage
        { get { return _LastSaleMessage; } }


        System.Threading.Timer _Timer ;
        Random _Random ;

        void TimerCallback(object state)
        {
            try
            {
                TrySell();
            }
            finally
            {
                KickTimer();
            }
        }

        private void KickTimer()
        {
            var nrProducts = _Context.Products.Count();
            _Timer.Change(TimeSpan.FromSeconds(1 + (int)(_Random.Next(0, 10) / Math.Sqrt((double)nrProducts + 1.0))), TimeSpan.FromMilliseconds(-1));
        }

        private void TrySell()
        {
            var products = _Context.Products;

            var nrProducts = products.Count();

            var product = nrProducts > 0 ? _Context.Products.ElementAt(_Random.Next(0, nrProducts)) : null ;

            if (product != null)
                if (product.CanSell.Value)
                {
                    product.Sell();
                    _LastSaleMessage.Value = String.Format("Sold product '{0}'.", product.Name.Value);
                }
                else
                    _LastSaleMessage.Value = String.Format("Failed to sell product '{0}' because: {1}", product.Name.Value, product.WhyCanNotSell.Value);
        }
    }
}
