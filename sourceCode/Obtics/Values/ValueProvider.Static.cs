namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Creates a static (ReadOnly) <see cref="IValueProvider{TType}"/> object with Value property set to the passes <paramref name="value"/> parameter. 
        /// </summary>
        /// <typeparam name="TType">The type of the Value property of the returned IValueProvider and the type of the <paramref name="value"/> parameter</typeparam>
        /// <param name="value">The <typeparamref name="TType"/> value the return <see cref="IValueProvider{TType}"/> is initialized with.</param>
        /// <returns>A static (ReadOnly) <see cref="IValueProvider{TType}"/> object with Value property set to the passes <paramref name="value"/> parameter.</returns>
        /// <remarks>
        /// The following statement will always return true
        /// <code>EqualityComparer&lt;TType&gt;.Default.Compare(v1,v2) == Object.Equals(ValueProvider.Static(v1),ValueProvider.Static(v2))</code>
        /// </remarks>
        public static IValueProvider<TType> Static<TType>(TType value)
        { return _Static(value).Concrete(); }

        internal static IInternalValueProvider<TType> _Static<TType>(TType value)
        { return StaticValueProvider<TType>.Create(value); }
    }
}
