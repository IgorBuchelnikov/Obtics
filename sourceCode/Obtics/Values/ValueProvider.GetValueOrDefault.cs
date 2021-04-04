
namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Retrieves the value of the current <see cref="IValueProvider{TSource}"/> object, or a default value when the object reference is null.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IValueProvider{TSource}"/> whose value to get.</param>
        /// <returns>The value of <paramref name="source"/> or default(<typeparamref name="TSource"/>) when <paramref name="source"/> is null.</returns>
        public static TSource GetValueOrDefault<TSource>(this IValueProvider<TSource> source)
        { return source != null ? source.Value : default(TSource); }

        /// <summary>
        /// Retrieves the value of the current <see cref="IValueProvider{TSource}"/> object, or a sprecified default value when the object reference is null.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IValueProvider{TSource}"/> whose value to get.</param>
        /// <param name="defaultValue">The default value to return when <paramref name="source"/> is null.</param>
        /// <returns>The value of <paramref name="source"/> or <paramref name="defaultValue"/> when <paramref name="source"/> is null.</returns>
        public static TSource GetValueOrDefault<TSource>(this IValueProvider<TSource> source, TSource defaultValue)
        { return source != null ? source.Value : defaultValue; }
    }
}
