
using Obtics.Values.Transformations;
using System;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of <paramref name="parameter"/>, <paramref name="fallback"/> and the Value property of the returned <see cref="IValueProvider{TType}"/>.</typeparam>
        /// <param name="parameter">The value to return if not null.</param>
        /// <param name="fallback">The fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives <paramref name="parameter"/> if not null or <paramref name="fallback"/> otherwise.</returns>
        public static IValueProvider<TType> Coalesce<TType>(TType parameter, TType fallback)
        { return _Coalesce(parameter, fallback).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(TType parameter, TType fallback)
        { return _Static(parameter == null ? fallback : parameter); }

        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of <paramref name="parameter"/>, <paramref name="fallback"/> and the Value property of the returned <see cref="IValueProvider{TType}"/>.</typeparam>
        /// <param name="parameter">The value to return if not null.</param>
        /// <param name="fallback">The fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives <paramref name="parameter"/> if not null or <paramref name="fallback"/> otherwise.</returns>
        public static IValueProvider<TType> Coalesce<TType>(TType? parameter, TType fallback)
            where TType : struct
        { return _Coalesce(parameter, fallback).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(TType? parameter, TType fallback)
            where TType : struct
        { return _Static(parameter ?? fallback); }

        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of the Value property of <paramref name="parameter"/>.</typeparam>
        /// <param name="parameter">AN <see cref="IValueProvider{TType}"/> whose Value will be returned.</param>
        /// <param name="fallback">The fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives the Value property of <paramref name="parameter"/> or <paramref name="fallback"/> if the Value property is null, or null when <paramref name="parameter"/> is null.</returns>
        public static IValueProvider<TType> Coalesce<TType>(this IValueProvider<TType> parameter, TType fallback)
        { return _Coalesce(parameter.Patched(), fallback).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(this IInternalValueProvider<TType> parameter, TType fallback)
        { return _Select(parameter, FuncExtender<TType>.Extend(fallback, (pv, fb) => pv == null ? fb : pv)); }

        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of the Value property of <paramref name="parameter"/>.</typeparam>
        /// <param name="parameter">AN <see cref="IValueProvider{TType}"/> whose Value will be returned.</param>
        /// <param name="fallback">The fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives the Value property of <paramref name="parameter"/> or <paramref name="fallback"/> if the Value property is null, or null when <paramref name="parameter"/> is null.</returns>
        public static IValueProvider<TType> Coalesce<TType>(this IValueProvider<TType?> parameter, TType fallback)
            where TType : struct
        { return _Coalesce(parameter.Patched(), fallback).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(this IInternalValueProvider<TType?> parameter, TType fallback)
            where TType : struct
        { return _Select(parameter, FuncExtender<TType?>.Extend(fallback, (pv, fb) => pv ?? fb)); }

        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of <paramref name="parameter"/>, the Value property of <paramref name="fallback"/> and the Value property of the returned <see cref="IValueProvider{TType}"/>.</typeparam>
        /// <param name="parameter">The value to return if not null.</param>
        /// <param name="fallback">An <see cref="IValueProvider{TType}"/> whose Value gives the fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives <paramref name="parameter"/> if not null or the Value property of <paramref name="fallback"/> otherwise.</returns>
        public static IValueProvider<TType> Coalesce<TType>(TType parameter, IValueProvider<TType> fallback)
        { return _Coalesce(parameter, fallback.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(TType parameter, IInternalValueProvider<TType> fallback)
        { return parameter == null ? fallback.Patched() : _Static(parameter); }
        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of <paramref name="parameter"/>, the Value property of <paramref name="fallback"/> and the Value property of the returned <see cref="IValueProvider{TType}"/>.</typeparam>
        /// <param name="parameter">The value to return if not null.</param>
        /// <param name="fallback">An <see cref="IValueProvider{TType}"/> whose Value gives the fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives <paramref name="parameter"/> if not null or the Value property of <paramref name="fallback"/> otherwise.</returns>
        public static IValueProvider<TType> Coalesce<TType>(TType? parameter, IValueProvider<TType> fallback)
            where TType : struct
        { return _Coalesce(parameter, fallback.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(TType? parameter, IInternalValueProvider<TType> fallback)
            where TType : struct
        { return parameter.HasValue ? _Static(parameter.Value) : fallback.Patched() ; }

        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of the Value property of <paramref name="parameter"/>.</typeparam>
        /// <param name="parameter">An <see cref="IValueProvider{TType}"/> whose Value will be returned.</param>
        /// <param name="fallback">An <see cref="IValueProvider{TType}"/> whose Value gives the fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives the Value property of <paramref name="parameter"/> or the Value property of <paramref name="fallback"/> if the Value property is null, or null when either <paramref name="parameter"/> or <paramref name="fallback"/> is null.</returns>
        public static IValueProvider<TType> Coalesce<TType>(this IValueProvider<TType> parameter, IValueProvider<TType> fallback)
        { return _Coalesce(parameter.Patched(), fallback.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(this IInternalValueProvider<TType> parameter, IInternalValueProvider<TType> fallback)
        { return _Select(parameter, FuncExtender<TType>.Extend(fallback, (pv, fb) => (IValueProvider<TType>)(pv == null ? fb : _Static(pv)))); }

        /// <summary>
        /// Returns the value of an input parameter or a fallback value if this value is null.
        /// </summary>
        /// <typeparam name="TType">Type of the Value property of <paramref name="parameter"/>.</typeparam>
        /// <param name="parameter">An <see cref="IValueProvider{TType}"/> whose Value will be returned.</param>
        /// <param name="fallback">An <see cref="IValueProvider{TType}"/> whose Value gives the fallback value to return when the Value property of <paramref name="parameter"/> is null.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/> whose Value property gives the Value property of <paramref name="parameter"/> or the Value property of <paramref name="fallback"/> if the Value property is null, or null when either <paramref name="parameter"/> or <paramref name="fallback"/> is null.</returns>
        public static IValueProvider<TType> Coalesce<TType>(this IValueProvider<TType?> parameter, IValueProvider<TType> fallback)
            where TType : struct
        { return _Coalesce(parameter.Patched(), fallback.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _Coalesce<TType>(this IInternalValueProvider<TType?> parameter, IInternalValueProvider<TType> fallback)
            where TType : struct
        { return _Select(parameter, FuncExtender<TType?>.Extend(fallback, (pv, fb) => (IValueProvider<TType>)(pv.HasValue ? _Static(pv.Value) : fb ))); }
    }
}
