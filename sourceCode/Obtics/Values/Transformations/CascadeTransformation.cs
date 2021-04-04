using System.ComponentModel;

namespace Obtics.Values.Transformations
{
    internal sealed class CascadeTransformation<TType, TItm> : CascadingTransformationBase<TType, IInternalValueProvider<TType>, IInternalValueProvider<TItm>>
        where TItm : IValueProvider<TType>
    {
        #region Constructor

        public static IInternalValueProvider<TType> GeneralCreate(IInternalValueProvider<TItm> source)
        {
            var sourceAsStatic = source as StaticValueProvider<TItm>;

            if (sourceAsStatic != null)
            {
                //Coalesce leads to CodeVerificationException :-/
                var res = (IValueProvider<TType>)sourceAsStatic.Value;

                if (res != null)
                    return res.Patched();
            }

            return Create(source);
        }

        public static CascadeTransformation<TType, TItm> Create(IInternalValueProvider<TItm> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<CascadeTransformation<TType, TItm>, IInternalValueProvider<TItm>>(source);
        }

        #endregion

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        protected override void SubscribeOnItm(IInternalValueProvider<TType> itm)
        {
            if(itm != null)
                itm.SubscribeINC(this); 
        }

        protected override void UnsubscribeFromItm(IInternalValueProvider<TType> itm)
        {
            if(itm != null)
                itm.UnsubscribeINC(this); 
        }

        protected override void SourceChangeEvent(object sender)
        {
            if (object.ReferenceEquals(sender, _Prms))
                base.SourceChangeEvent(sender);
            else
                base.ItmChangeEvent(sender);
        }

        protected override void SourceExceptionEvent(object sender, INCEventArgs evt)
        {
            if (object.ReferenceEquals(sender, _Prms))
                base.SourceExceptionEvent(sender, evt);
            else
                base.ItmExceptionEvent(sender, evt);
        }

        protected override IInternalValueProvider<TType> GetItmFromSource()
        { return _Prms.Value.Patched(); }

        protected override TType GetValueFromItm(IInternalValueProvider<TType> itm)
        {
            //May throw null ref exception
            return itm.Value; 
        }
    }
}
