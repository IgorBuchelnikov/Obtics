using System;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;


namespace Obtics.Values.Transformations
{
    internal sealed class DependencyPropertyTransformation<TIn, TOut> : CascadingTransformationBase<TOut, TIn, Tuple<IInternalValueProvider<TIn>,DependencyProperty>>
        where TIn : DependencyObject
    {
        #region Constructor

        public static DependencyPropertyTransformation<TIn, TOut> Create(IInternalValueProvider<TIn> source, DependencyProperty dependencyProperty)
        {
            if (source == null || dependencyProperty == null)
                return null;

#if SILVERLIGHT
            //how to check type?
#else
            var propType = dependencyProperty.PropertyType;

            if (!typeof(TOut).IsAssignableFrom(propType))
                return null;
#endif
            return Carrousel.Get<DependencyPropertyTransformation<TIn, TOut>, IInternalValueProvider<TIn>, DependencyProperty>(source, dependencyProperty);
        }

        #endregion

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override void SubscribeOnItm(TIn itm)
        {
            //TODO: Can DependencyPropertyDescriptor.AddValueChanged be trusted with misbehaving equality comparer?
            //  (ObticsEqualityComparer<TIn>.IsPatched)

            if (itm != null)
            {
                var pd = DependencyPropertyDescriptor.FromProperty(_Property, itm.GetType());

                if (pd != null)
                    pd.AddValueChanged(itm, Buffer_PropertyChanged);
            }
        }

        protected override void UnsubscribeFromItm(TIn itm)
        {
            if (itm != null)
            {
                var pd = DependencyPropertyDescriptor.FromProperty(_Property, itm.GetType());

                if (pd != null)
                    pd.RemoveValueChanged(itm, Buffer_PropertyChanged);
            }
        }

        IInternalValueProvider<TIn> _Source
        { get { return _Prms.First; } }

        DependencyProperty _Property
        { get { return _Prms.Second; } }

        protected override TIn GetItmFromSource()
        { return _Source.Value; }

        void Buffer_PropertyChanged(object sender, EventArgs args)
        { base.ItmChangeEvent(sender); }

        protected override TOut GetValueFromItm(TIn itm)
        {
            //May raise null reference exception
            return (TOut)itm.GetValue(_Property);
        }
    }
}
