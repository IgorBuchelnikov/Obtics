using Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Returns a read only version of the <paramref name="source"/> <see cref="IValueProvider{TType}"/>
        /// </summary>
        /// <typeparam name="TType">Type of the Value property of the <paramref name="source"/> <see cref="IValueProvider{TType}"/></typeparam>
        /// <param name="source">The IValueProvider to return a read only version of.</param>
        /// <returns>A read only version of the <paramref name="source"/> <see cref="IValueProvider{TType}"/>
        /// If the <paramref name="source"/> parameter is null then null is returned instead.</returns>
        public static IValueProvider<TType> ReadOnly<TType>(this IValueProvider<TType> source)
        { return _ReadOnly(source.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _ReadOnly<TType>(this IInternalValueProvider<TType> source)
        { return SetReadOnlyTransformation<TType>.Create(source); }
    }
}
