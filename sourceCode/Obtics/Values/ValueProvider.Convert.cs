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
        /// <typeparam name="TSource">Type of the Value of source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider, also the return type of the converter delegate.</typeparam>
        /// <param name="source">IValueProvider that is the source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource&gt;,TResult&gt;) that converts the input IValueProvider to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the valueConverter.
        /// If source or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProvider and not just the Value of that IValueProvider.
        /// This allows the converter NOT TO ACCESS the Value property of the source, possibly preventing wasting of resources.
        /// This is the only purpose the passed IValueProvider should be used for. The valueConverter should only consume the current
        /// Value of the given IValueProvider.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource, TResult>(this IValueProvider<TSource> source, Func<IValueProvider<TSource>, TResult> valueConverter)
        { return _Convert(source.Patched(),valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource, TResult>(this IInternalValueProvider<TSource> source, Func<IValueProvider<TSource>, TResult> valueConverter)
        { return UnaryTransformation<TSource, TResult>.Create(source, valueConverter); }


        /// <summary>
        /// Converts the input values of two sources to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of first source IValueProvider.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of second source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider, also the return type of the converter delegate.</typeparam>
        /// <param name="source1">IValueProvider that is the first source value for the conversion</param>
        /// <param name="source2">IValueProvider that is the second source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource1&gt;,IValueProvider&lt;TSource2&gt;,TResult&gt;) that converts the input IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the valueConverter.
        /// If source1, source2 or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource1, TSource2, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, TResult> valueConverter)
        { return _Convert(source1.Patched(), source2.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource1, TSource2, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, TResult> valueConverter)
        { return BinaryTransformation<TResult, TSource1, TSource2>.Create(source1, source2, valueConverter); }

        /// <summary>
        /// Converts the input values of three sources to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of first source IValueProvider.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of second source IValueProvider.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of third source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider, also the return type of the converter delegate.</typeparam>
        /// <param name="source1">IValueProvider that is the first source value for the conversion</param>
        /// <param name="source2">IValueProvider that is the second source value for the conversion</param>
        /// <param name="source3">IValueProvider that is the third source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource1&gt;,IValueProvider&lt;TSource2&gt;,IValueProvider&lt;TSource3&gt;,TResult&gt;) 
        /// that converts the input IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the valueConverter.
        /// If source1, source2, source3 or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource1, TSource2, TSource3, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, TResult> valueConverter)
        { return _Convert(source1.Patched(), source2.Patched(), source3.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource1, TSource2, TSource3, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, TResult> valueConverter)
        { return TertiaryTransformation<TResult, TSource1, TSource2, TSource3>.Create(source1, source2, source3, valueConverter); }

        /// <summary>
        /// Converts the input values of four sources to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of first source IValueProvider.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of second source IValueProvider.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of third source IValueProvider.</typeparam>
        /// <typeparam name="TSource4">Type of the Value of fourth source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider, also the return type of the converter delegate.</typeparam>
        /// <param name="source1">IValueProvider that is the first source value for the conversion</param>
        /// <param name="source2">IValueProvider that is the second source value for the conversion</param>
        /// <param name="source3">IValueProvider that is the third source value for the conversion</param>
        /// <param name="source4">IValueProvider that is the fourth source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource1&gt;,IValueProvider&lt;TSource2&gt;,IValueProvider&lt;TSource3&gt;,IValueProvider&lt;TSource4&gt;,TResult&gt;) 
        /// that converts the input IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the valueConverter.
        /// If source1, source2, source3, source4 or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource1, TSource2, TSource3, TSource4, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, IValueProvider<TSource4> source4, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, TResult> valueConverter)
        { return _Convert(source1.Patched(), source2.Patched(), source3.Patched(), source4.Patched(), valueConverter ).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource1, TSource2, TSource3, TSource4, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, IInternalValueProvider<TSource4> source4, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, TResult> valueConverter)
        { return QuartaryTransformation<TResult, TSource1, TSource2, TSource3, TSource4>.Create(source1, source2, source3, source4, valueConverter); }

        /// <summary>
        /// Converts the input values of multiple sources to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider, also the return type of the converter delegate.</typeparam>
        /// <param name="sources">An array of untyped IValueProviders that are the source for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider[],TResult&gt;) 
        /// that converts the input untyped IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the valueConverter.
        /// If sources, any item of sources or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources by doing so.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TResult>(this IValueProvider[] sources, Func<IValueProvider[], TResult> valueConverter)
        { return _Convert(sources.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TResult>(this IInternalValueProvider[] sources, Func<IValueProvider[], TResult> valueConverter)
        { return MultiTransformation<TResult>.Create(valueConverter, sources); }

        /// <summary>
        /// Converts the input values of multiple sources to a new value using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider, also the return type of the converter delegate.</typeparam>
        /// <param name="sources">mutiple untyped IValueProviders that are the source for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider[],TResult&gt;)
        /// that converts the input untyped IValueProviders to the result value.</param>
        /// <returns>An IValueProvider whose Value is the result of the latest call to the valueConverter.
        /// If sources, any item of sources or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources by doing so.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TResult>(Func<IValueProvider[], TResult> valueConverter, params IValueProvider[] sources)
        { return Convert(sources, valueConverter); }


        /// <summary>
        /// Converts an input value to an IValueProvider using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value of source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider.</typeparam>
        /// <param name="source">IValueProvider that is the source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource&gt;,IValueProvider&lt;TResult&gt;&gt;) that converts the input IValueProvider to a result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the valueConverter.
        /// If source or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProvider and not just the Value of that IValueProvider.
        /// This allows the converter NOT TO ACCESS the Value property of the source, possibly preventing wasting of resources.
        /// This is the only purpose the passed IValueProvider should be used for. The valueConverter should only consume the current
        /// Value of the given IValueProvider.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource, TResult>(this IValueProvider<TSource> source, Func<IValueProvider<TSource>, IValueProvider<TResult>> valueConverter)
        { return _Convert<TSource, TResult>(source.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource, TResult>(this IInternalValueProvider<TSource> source, Func<IValueProvider<TSource>, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Convert<TSource, IValueProvider<TResult>>(source, valueConverter)); }

        /// <summary>
        /// Converts two input values to an IValueProvider using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the first source IValueProvider.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the second source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider.</typeparam>
        /// <param name="source1">IValueProvider that is the first source value for the conversion</param>
        /// <param name="source2">IValueProvider that is the second source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource1&gt;,IValueProvider&lt;TSource2&gt;,IValueProvider&lt;TResult&gt;&gt;) 
        /// that converts the two input IValueProviders to a result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the valueConverter.
        /// If source1, source2 or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources by doing so.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource1, TSource2, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TResult>> valueConverter)
        { return _Convert(source1.Patched(), source2.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource1, TSource2, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Convert<TSource1, TSource2, IValueProvider<TResult>>(source1, source2, valueConverter)); }

        /// <summary>
        /// Converts three input values to an IValueProvider using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the first source IValueProvider.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the second source IValueProvider.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of the third source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider.</typeparam>
        /// <param name="source1">IValueProvider that is the first source value for the conversion</param>
        /// <param name="source2">IValueProvider that is the second source value for the conversion</param>
        /// <param name="source3">IValueProvider that is the third source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource1&gt;,IValueProvider&lt;TSource2&gt;,IValueProvider&lt;TSource3&gt;,IValueProvider&lt;TResult&gt;&gt;) 
        /// that converts the three input IValueProviders to a result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the valueConverter.
        /// If source1, source2, source3 or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources by doing so.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource1, TSource2, TSource3, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TResult>> valueConverter)
        { return _Convert(source1.Patched(), source2.Patched(), source3.Patched(), valueConverter ).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource1, TSource2, TSource3, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Convert<TSource1, TSource2, TSource3, IValueProvider<TResult>>(source1, source2, source3, valueConverter)); }

        /// <summary>
        /// Converts four input values to an IValueProvider using a given converter delegate.
        /// </summary>
        /// <typeparam name="TSource1">Type of the Value of the first source IValueProvider.</typeparam>
        /// <typeparam name="TSource2">Type of the Value of the second source IValueProvider.</typeparam>
        /// <typeparam name="TSource3">Type of the Value of the third source IValueProvider.</typeparam>
        /// <typeparam name="TSource4">Type of the Value of the fourth source IValueProvider.</typeparam>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider.</typeparam>
        /// <param name="source1">IValueProvider that is the first source value for the conversion</param>
        /// <param name="source2">IValueProvider that is the second source value for the conversion</param>
        /// <param name="source3">IValueProvider that is the third source value for the conversion</param>
        /// <param name="source4">IValueProvider that is the fourth source value for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider&lt;TSource1&gt;,IValueProvider&lt;TSource2&gt;,IValueProvider&lt;TSource3&gt;,IValueProvider&lt;TSource4&gt;,IValueProvider&lt;TResult&gt;&gt;) 
        /// that converts the three input IValueProviders to a result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the valueConverter.
        /// If source1, source2, source3, source4 or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources by doing so.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TSource1, TSource2, TSource3, TSource4, TResult>(this IValueProvider<TSource1> source1, IValueProvider<TSource2> source2, IValueProvider<TSource3> source3, IValueProvider<TSource4> source4, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, IValueProvider<TResult>> valueConverter)
        { return _Convert(source1.Patched(), source2.Patched(), source3.Patched(), source4.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TSource1, TSource2, TSource3, TSource4, TResult>(this IInternalValueProvider<TSource1> source1, IInternalValueProvider<TSource2> source2, IInternalValueProvider<TSource3> source3, IInternalValueProvider<TSource4> source4, Func<IValueProvider<TSource1>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Convert<TSource1, TSource2, TSource3, TSource4, IValueProvider<TResult>>(source1, source2, source3, source4, valueConverter)); }

        /// <summary>
        /// Converts multiple input values to an IValueProvider using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider.</typeparam>
        /// <param name="sources">An array of untyped IValueProviders that are the source for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider[],IValueProvider&lt;TResult&gt;&gt;) 
        /// that converts the multiple input IValueProviders to a result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the valueConverter.
        /// If sources, any item of sources or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources by doing so.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TResult>(this IValueProvider[] sources, Func<IValueProvider[], IValueProvider<TResult>> valueConverter)
        { return _Convert(sources.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TResult>(this IInternalValueProvider[] sources, Func<IValueProvider[], IValueProvider<TResult>> valueConverter)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Convert<IValueProvider<TResult>>(sources, valueConverter)); }

        /// <summary>
        /// Converts multiple input values to an IValueProvider using a given converter delegate.
        /// </summary>
        /// <typeparam name="TResult">Type of the Value of the result IValueProvider.</typeparam>
        /// <param name="sources">An array of untyped IValueProviders that are the source for the conversion</param>
        /// <param name="valueConverter">delegate (Func&lt;IValueProvider[],IValueProvider&lt;TResult&gt;&gt;) 
        /// that converts the multiple input IValueProviders to a result IValueProvider.</param>
        /// <returns>An IValueProvider whose Value is the Value of the IValueProvider returned by the latest call to the valueConverter.
        /// If sources, any item of sources or valueConverter equals null then the return value will be null.</returns>
        /// <remarks>
        /// The converter delegate is passed the original IValueProviders and not just the Values of those IValueProviders.
        /// This allows the converter NOT TO ACCESS the Value properties of the sources, possibly preventing wasting of resources by doing so.
        /// This is the only purpose the passed IValueProviders should be used for. The valueConverter should only consume the current
        /// Values of the given IValueProviders.
        /// </remarks>
        public static IValueProvider<TResult> Convert<TResult>(Func<IValueProvider[], IValueProvider<TResult>> valueConverter, params IValueProvider[] sources)
        { return _Convert(sources.Patched(), valueConverter).Concrete(); }

        internal static IInternalValueProvider<TResult> _Convert<TResult>(Func<IValueProvider[], IValueProvider<TResult>> valueConverter, params IInternalValueProvider[] sources)
        { return CascadeTransformation<TResult, IValueProvider<TResult>>.GeneralCreate(_Convert<IValueProvider<TResult>>(sources, valueConverter)); }
    }
}
