
namespace Obtics.Values.Transformations
{
    internal sealed class TypeConvertTransformation<TType> : ConvertTransformationBase<TType, IInternalValueProvider>
    {
        public static TypeConvertTransformation<TType> Create(IInternalValueProvider source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<TypeConvertTransformation<TType>, IInternalValueProvider>(source);
        }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        protected override TType GetValue()
        {
            var res = Caster<TType>.Cast(_Prms.Value);
            return res.Second ? res.First : default(TType) ;            
        }
    }
}
