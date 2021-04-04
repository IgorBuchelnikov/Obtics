using Obtics.Values.Transformations;
using Obtics.Async;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IValueProvider<TType> Cached<TType>(this IValueProvider<TType> source)
        { return _Cached(source.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _Cached<TType>(this IInternalValueProvider<TType> source)
        { return CachedTransformation<TType>.Create(source); }
    }
}
