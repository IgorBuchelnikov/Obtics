
namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Creates a dynamic <see cref="IValueProvider{TResult}"/>. An IValueProvider who's Value can be read and set.
        /// </summary>
        /// <typeparam name="TType">Type of the <paramref name="value"/> parameter and type of the Value of the returned IValueProvider.</typeparam>
        /// <param name="value">Initalization value</param>
        /// <returns>A dynamic <see cref="IValueProvider{TResult}"/> with inital value as given by parameter <paramref name="value"/>.</returns>
        /// <remarks>
        /// Since the returned object is not a static value each returned object is unique. This means that
        /// <code>Object.Equals(ValueProvider.Dynamic(10),ValueProvider.Dynamic(10))</code>
        /// will always return false.
        /// This also means that any transformation pipeline depending on it will be unique.
        /// </remarks>
        public static IValueProvider<TType> Dynamic<TType>(TType value)
        { return _Dynamic(value).Concrete(); }

        internal static IInternalValueProvider<TType> _Dynamic<TType>(TType value)
        { return DynamicValueProvider<TType>.Create(value); }

        /// <summary>
        /// Creates a dynamic <see cref="IValueProvider{TResult}"/> initialized with the default value of <typeparamref name="TType"/>. An IValueProvider who's Value can be read and set.
        /// </summary>
        /// <typeparam name="TType">Type of the <paramref name="value"/> parameter and type of the Value of the returned IValueProvider.</typeparam>
        /// <returns>A dynamic <see cref="IValueProvider{TResult}"/> with initial value default(<typeparamref name="TType"/>).</returns>
        /// <remarks>
        /// Since the returned object is not a static value each returned object is unique. This means that
        /// <code>Object.Equals(ValueProvider.Dynamic&lt;int&gt;(),ValueProvider.Dynamic&lt;int&gt;())</code>
        /// will always return false.
        /// This also means that any transformation pipeline depending on it will be unique.
        /// </remarks>
        public static IValueProvider<TType> Dynamic<TType>()
        { return Dynamic(default(TType)); }
    }
}
