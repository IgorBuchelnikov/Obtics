using System;
using System.Linq;
using Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Converts an input value to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value of the <paramref name="source"/> IValueProvider. And Type of the argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also the return type of <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source">IValueProvider that provides the source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource,TResult}"/>) that converts the Value of the <paramref name="source"/> IValueProvider to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Value of the <paramref name="source"/> IValueProvider. This means
        /// that implicitly the Value of <paramref name="source"/> is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource, TResult>(this IValueProvider<TSource> source, Func<TSource, TResult> valueConverter)
        { return _Select(source.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TSource, TResult>(this IInternalValueProvider<TSource> source, Func<TSource, TResult> valueConverter)
        { return UnarySelectTransformation<TSource, TResult>.Create(source, valueConverter); }

        /// <summary>
        /// Converts two input values to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the <paramref name="source1"/> IValueProvider. And Type of the first argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the <paramref name="source2"/> IValueProvider. And Type of the second argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also the return type of <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source1">IValueProvider that provides the first source value for the conversion.</param>
        /// <param name="source2">IValueProvider that provides the second source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource1,TSource2,TResult}"/>) that converts the Values of the source IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source1"/>, <paramref name="source2"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of each source is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource1,TSource2,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource1, TSource2, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, Func<TSource1, TSource2, TResult> valueConverter)
        { return _Select(source1.Patched(), source2.Patched(), valueConverter).Concrete(); }

        
        internal static IInternalValueProvider<TResult> _Select<TSource1, TSource2, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, Func<TSource1, TSource2, TResult> valueConverter)
        {
            return 
                BinarySelectTransformation<TResult, TSource1, TSource2>.Create(
                    source1, 
                    source2,
                    valueConverter
                );
        }

        /// <summary>
        /// Converts three input values to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the <paramref name="source1"/> IValueProvider. And Type of the first argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the <paramref name="source2"/> IValueProvider. And Type of the second argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of the <paramref name="source3"/> IValueProvider. And Type of the third argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also the return type of <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source1">IValueProvider that provides the first source value for the conversion.</param>
        /// <param name="source2">IValueProvider that provides the second source value for the conversion.</param>
        /// <param name="source3">IValueProvider that provides the third source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource1,TSource2,TSource3,TResult}"/>) that converts the Values of the source IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source1"/>, <paramref name="source2"/>, <paramref name="source3"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of each source is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource1,TSource2,TSource3,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource1, TSource2, TSource3, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, Func<TSource1, TSource2, TSource3, TResult> valueConverter)
        { return _Select(source1.Patched(), source2.Patched(), source3.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TSource1, TSource2, TSource3, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, Func<TSource1, TSource2, TSource3, TResult> valueConverter)
        {
            return
                TertiarySelectTransformation<TResult, TSource1, TSource2, TSource3>.Create(
                    source1,
                    source2,
                    source3,
                    valueConverter
                );
        }

        /// <summary>
        /// Converts four input values to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the <paramref name="source1"/> IValueProvider. And Type of the first argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the <paramref name="source2"/> IValueProvider. And Type of the second argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of the <paramref name="source3"/> IValueProvider. And Type of the third argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource4">Type of the Value of the <paramref name="source4"/> IValueProvider. And Type of the fourth argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also the return type of <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source1">IValueProvider that provides the first source value for the conversion.</param>
        /// <param name="source2">IValueProvider that provides the second source value for the conversion.</param>
        /// <param name="source3">IValueProvider that provides the third source value for the conversion.</param>
        /// <param name="source4">IValueProvider that provides the fourth source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource1,TSource2,TSource3,TSource4,TResult}"/>) that converts the Values of the source IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source1"/>, <paramref name="source2"/>, <paramref name="source3"/>, <paramref name="source4"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of each source is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource1,TSource2,TSource3,TSource4,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource1, TSource2, TSource3, TSource4, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, IValueProvider<TSource4> source4, Func<TSource1, TSource2, TSource3, TSource4, TResult> valueConverter)
        { return _Select(source1.Patched(), source2.Patched(), source3.Patched(), source4.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TSource1, TSource2, TSource3, TSource4, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, IInternalValueProvider<TSource4> source4, Func<TSource1, TSource2, TSource3, TSource4, TResult> valueConverter)
        {
            return
                QuartarySelectTransformation<TResult, TSource1, TSource2, TSource3, TSource4>.Create(
                    source1,
                    source2,
                    source3,
                    source4,
                    valueConverter
                );
        }

        /// <summary>
        /// Converts multiple input values to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also the return type of <paramref name="valueConverter"/>.</typeparam>
        /// <param name="sources">Array of untyped IValueProviders that provide the source values for <paramref name="valueConverter"/>.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{IValueProviderArray,TResult}"/>) that converts the Values of the source IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="sources"/>, any of its items or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of each source is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TResult>(this IValueProvider[] sources, Func<object[], TResult> valueConverter)
        { return _Select(sources.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TResult>(this IInternalValueProvider[] sources, Func<object[], TResult> valueConverter)
        {
            return
                MultiSelectTransformation<TResult>.Create(
                    valueConverter,
                    sources
                );
        }

        /// <summary>
        /// Converts multiple input values to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also the return type of <paramref name="valueConverter"/>.</typeparam>
        /// <param name="valueConverter">delegate (<see cref="Func{IValueProviderArray,TResult}"/>) that converts the Values of the source IValueProviders to the result value.</param>
        /// <param name="sources">Array of untyped IValueProviders that provide the source values for <paramref name="valueConverter"/>.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="sources"/>, any of its items or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of each source is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TResult>(Func<object[], TResult> valueConverter, params IValueProvider[] sources)
        { return Select(sources, valueConverter); }



        /// <summary>
        /// Converts an input value to an <see cref="IValueProvider{TResult}"/> using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value of the <paramref name="source"/> IValueProvider. And Type of the argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also type of the Value of the IValueProvider returned by <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source">IValueProvider that provides the source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource,IValueProvider}"/>) that converts the Value of the <paramref name="source"/> IValueProvider to the result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Value of the <paramref name="source"/> IValueProvider. This means
        /// that implicitly the Value of <paramref name="source"/> is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource, TResult>(this IValueProvider<TSource> source, Func<TSource, IValueProvider<TResult>> valueConverter)
        { return _Select(source.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TSource, TResult>(this IInternalValueProvider<TSource> source, Func<TSource, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Select<TSource, IValueProvider<TResult>>(source, valueConverter)); }

        /// <summary>
        /// Converts two input values to an <see cref="IValueProvider{TResult}"/> using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the <paramref name="source1"/> IValueProvider. And Type of the first argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the <paramref name="source2"/> IValueProvider. And Type of the second argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also type of the Value of the IValueProvider returned by <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source1">IValueProvider that provides the first source value for the conversion.</param>
        /// <param name="source2">IValueProvider that provides the second source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource1,TSource2,IValueProvider}"/>) that converts the Values of the source IValueProviders to the result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source1"/>, <paramref name="source2"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of the sources is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource1,TSource2,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource1, TSource2, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, Func<TSource1, TSource2, IValueProvider<TResult>> valueConverter)
        { return _Select(source1.Patched(), source2.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TSource1, TSource2, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, Func<TSource1, TSource2, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Select<TSource1, TSource2, IValueProvider<TResult>>(source1, source2, valueConverter)); }

        /// <summary>
        /// Converts three input values to an <see cref="IValueProvider{TResult}"/> using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the <paramref name="source1"/> IValueProvider. And Type of the first argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the <paramref name="source2"/> IValueProvider. And Type of the second argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of the <paramref name="source3"/> IValueProvider. And Type of the third argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also type of the Value of the IValueProvider returned by <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source1">IValueProvider that provides the first source value for the conversion.</param>
        /// <param name="source2">IValueProvider that provides the second source value for the conversion.</param>
        /// <param name="source3">IValueProvider that provides the third source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource1,TSource2,TSource3,IValueProvider}"/>) 
        /// that converts the Values of the source IValueProviders to the result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source1"/>, <paramref name="source2"/>, <paramref name="source3"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of the sources is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource1,TSource2,TSource3,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource1, TSource2, TSource3, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, Func<TSource1, TSource2, TSource3, IValueProvider<TResult>> valueConverter)
        { return _Select(source1.Patched(), source2.Patched(), source3.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TSource1, TSource2, TSource3, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, Func<TSource1, TSource2, TSource3, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Select<TSource1, TSource2, TSource3, IValueProvider<TResult>>(source1, source2, source3, valueConverter)); }

        /// <summary>
        /// Converts four input values to an <see cref="IValueProvider{TResult}"/> using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the <paramref name="source1"/> IValueProvider. And Type of the first argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the <paramref name="source2"/> IValueProvider. And Type of the second argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of the <paramref name="source3"/> IValueProvider. And Type of the third argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TSource4">Type of the Value of the <paramref name="source4"/> IValueProvider. And Type of the fourth argument to <paramref name="valueConverter"/>.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also type of the Value of the IValueProvider returned by <paramref name="valueConverter"/>.</typeparam>
        /// <param name="source1">IValueProvider that provides the first source value for the conversion.</param>
        /// <param name="source2">IValueProvider that provides the second source value for the conversion.</param>
        /// <param name="source3">IValueProvider that provides the third source value for the conversion.</param>
        /// <param name="source4">IValueProvider that provides the fourth source value for the conversion.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{TSource1,TSource2,TSource3,TSource4,IValueProvider}"/>) 
        /// that converts the Values of the source IValueProviders to the result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="source1"/>, <paramref name="source2"/>, <paramref name="source3"/>, <paramref name="source4"/> or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of the sources is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TSource1,TSource2,TSource3,TSource4,TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TSource1, TSource2, TSource3, TSource4, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, IValueProvider<TSource4> source4, Func<TSource1, TSource2, TSource3, TSource4, IValueProvider<TResult>> valueConverter)
        { return _Select(source1.Patched(), source2.Patched(), source3.Patched(), source4.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TSource1, TSource2, TSource3, TSource4, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, IInternalValueProvider<TSource4> source4, Func<TSource1, TSource2, TSource3, TSource4, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Select<TSource1, TSource2, TSource3, TSource4, IValueProvider<TResult>>(source1, source2, source3, source4, valueConverter)); }

        /// <summary>
        /// Converts multiple input values to an <see cref="IValueProvider{TResult}"/> using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also type of the Value of the IValueProvider returned by <paramref name="valueConverter"/>.</typeparam>
        /// <param name="sources">Array of untyped IValueProviders that provide the source values for <paramref name="valueConverter"/>.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{IValueProviderArray,IValueProvider}"/>)         
        /// that converts the Values of the source IValueProviders to the result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="sources"/>, any of its items or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of each source is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TResult>(this IValueProvider[] sources, Func<object[], IValueProvider<TResult>> valueConverter)
        { return _Select(sources.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TResult>(this IInternalValueProvider[] sources, Func<object[], IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Select<IValueProvider<TResult>>(sources, valueConverter)); }

        /// <summary>
        /// Converts multiple input values to an <see cref="IValueProvider{TResult}"/> using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider. Also type of the Value of the IValueProvider returned by <paramref name="valueConverter"/>.</typeparam>
        /// <param name="sources">Array of untyped IValueProviders that provide the source values for <paramref name="valueConverter"/>.</param>
        /// <param name="valueConverter">delegate (<see cref="Func{IValueProviderArray,IValueProvider}"/>)         
        /// that converts the Values of the source IValueProviders to the result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the <paramref name="valueConverter"/>.
        /// If <paramref name="sources"/>, any of its items or <paramref name="valueConverter"/> equals null then the return value will be null.</returns>
        /// <remarks>
        /// <paramref name="valueConverter"/> is passed the current Values of the source IValueProviders. This means
        /// that implicitly the Value of each source is collected whenever <paramref name="valueConverter"/> is called. To
        /// prevent this implicit value collection use <see cref="Convert{TResult}"/>
        /// </remarks>
        public static IValueProvider<TResult> Select<TResult>(Func<object[], IValueProvider<TResult>> valueConverter, params IValueProvider[] sources)
        { return _Select(sources.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Select<TResult>(Func<object[], IValueProvider<TResult>> valueConverter, params IInternalValueProvider[] sources)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Select<IValueProvider<TResult>>(sources, valueConverter)); }
    }
}
