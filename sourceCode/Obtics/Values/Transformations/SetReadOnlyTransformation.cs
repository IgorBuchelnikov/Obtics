
namespace Obtics.Values.Transformations
{
    /// <summary>
    /// ReadOnly
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    internal sealed class SetReadOnlyTransformation<TType> : ConvertTransformationBase<TType, IInternalValueProvider<TType>>
    {
        /// <summary>
        /// Constructor, initializes with a source
        /// </summary>
        /// <param name="source"></param>
        public static SetReadOnlyTransformation<TType> Create(IInternalValueProvider<TType> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<SetReadOnlyTransformation<TType>, IInternalValueProvider<TType>>(source);
        }

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        /// <summary>
        /// ProtectedValue
        /// </summary>
        protected override TType GetValue()
        { return _Prms.Value; }
    }
}
