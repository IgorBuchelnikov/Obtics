using System.ComponentModel;

namespace Obtics.Values.Transformations
{
    internal sealed class CascadeWithDefaultTransformation<TType, TItm> : CascadingTransformationBase<TType, IInternalValueProvider<TType>, Tuple<IInternalValueProvider<TItm>,TType>>
        where TItm : IValueProvider<TType>
    {
        #region Constructor

        public static IInternalValueProvider<TType> GeneralCreate(IInternalValueProvider<TItm> source, TType dflt)
        {
            var sourceAsStatic = source as StaticValueProvider<TItm>;

            if (sourceAsStatic != null)
            {
                //Coalesce leads to CodeVerificationException :-/
                var res = (IValueProvider<TType>)sourceAsStatic.Value;

                if (res == null)
                    return StaticValueProvider<TType>.Create(dflt);
                else
                    return res.Patched();
            }

            return Create(source, dflt);
        }

        public static CascadeWithDefaultTransformation<TType, TItm> Create(IInternalValueProvider<TItm> source, TType dflt)
        {
            if (source == null)
                return null;

            return Carrousel.Get<CascadeWithDefaultTransformation<TType, TItm>, IInternalValueProvider<TItm>, TType>(source,dflt);
        }

        #endregion

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override void SubscribeOnItm(IInternalValueProvider<TType> itm)
        {
            if (itm != null)
                itm.SubscribeINC(this);
        }

        protected override void UnsubscribeFromItm(IInternalValueProvider<TType> itm)
        {
            if (itm != null)
                itm.UnsubscribeINC(this);
        }

        protected override void SourceChangeEvent(object sender)
        {
            if (object.ReferenceEquals(sender, _Prms.First))
                base.SourceChangeEvent(sender);
            else
                base.ItmChangeEvent(sender);
        }

        protected override void SourceExceptionEvent(object sender, INCEventArgs evt)
        {
            if (object.ReferenceEquals(sender, _Prms.First))
                base.SourceExceptionEvent(sender, evt);
            else
                base.ItmExceptionEvent(sender, evt);
        }

        protected override IInternalValueProvider<TType> GetItmFromSource()
        { return _Prms.First.Value.Patched(); }

        protected override TType GetValueFromItm(IInternalValueProvider<TType> itm)
        { return itm == null ? _Prms.Second : itm.Value; }
    }
}
