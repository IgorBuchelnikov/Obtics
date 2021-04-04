using Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Casts an untyped source IValueProvider to a Typed IValueProvider.
        /// </summary>
        /// <typeparam name="TType">The Value type of the IValueProvider to cast to.</typeparam>
        /// <param name="source">The untyped IValueProvider that needs to be converted to a typed IValueProvider</param>
        /// <returns>A Typed IValueProvider.</returns>
        /// <remarks>
        /// Alternatively, when the source is a typed IValueProvider but of with a different Value type than the intended type
        /// and this original Value type is a value-type (like int, long or a struct), 
        /// the Select method can be used with a Func that returns the value casted to the target type. 
        /// This may be more efficient than the Cast from an untyped source because the casting from an untyped source
        /// may require some boxing and unboxing.
        /// </remarks>
        public static IValueProvider<TType> Cast<TType>(this IValueProvider source)
        { return _Cast<TType>(source.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _Cast<TType>(this IInternalValueProvider source)
        { return (source as IInternalValueProvider<TType>) ?? TypeConvertTransformation<TType>.Create(source); }
    }
}
