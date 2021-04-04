
namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Returns value provider as an <see cref="IValueProvider{Type}"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the Value property of the source value provider</typeparam>
        /// <param name="source">The value provider that is to be returned as an <see cref="IValueProvider{Type}"/>.</param>
        /// <returns>the source as an <see cref="IValueProvider{Type}"/>.</returns>
        public static IValueProvider<TType> AsValueProvider<TType>(this IValueProvider<TType> source)
        { return source; }
    }
}
