using Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        static readonly IInternalValueProvider<bool> staticFalseVP = _Static(false);
        static readonly IInternalValueProvider<bool> staticTrueVP = _Static(true);

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'and' of the Values of its two source boolean values
        /// </summary>
        /// <param name="a">The first source boolean to calculate the result from.</param>
        /// <param name="b">The second source boolean to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'and' of the source values.</returns>
        /// <remarks>The returned <see cref="IValueProvider{Bool}"/> will be a static value provider.</remarks>
        public static IValueProvider<bool> And(bool a, bool b)
        { return _And(a, b).Concrete(); }

        internal static IInternalValueProvider<bool> _And(bool a, bool b)
        { return _Static(a && b); }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'and' of the Values of its two source values.
        /// </summary>
        /// <param name="a">The first source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <param name="b">The second source boolean to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'and' of the Values of its two source IValueProvider&lt;bool&gt;s</returns>
        /// <remarks>Change notifications are only propagated if the related change of the source value could ever influence the result value.</remarks>
        public static IValueProvider<bool> And(this IValueProvider<bool> a, bool b)
        { return _And(a.Patched(), b).Concrete(); }

        internal static IInternalValueProvider<bool> _And(this IInternalValueProvider<bool> a, bool b)
        { return b ? a : staticFalseVP; }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'and' of the Values of its two source values.
        /// </summary>
        /// <param name="a">The first source boolean to calculate the result from.</param>
        /// <param name="b">The second source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An <see cref="IValueProvider{Bool}"/> that represents the logical 'and' of its two source values.</returns>
        public static IValueProvider<bool> And(bool a, IValueProvider<bool> b)
        { return _And(a, b.Patched()).Concrete(); }

        internal static IInternalValueProvider<bool> _And(bool a, IInternalValueProvider<bool> b)
        { return a ? b : staticFalseVP; }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'and' of the Values of its two source IValueProvider&lt;bool&gt;s
        /// </summary>
        /// <param name="a">The first source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <param name="b">The second source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'and' of the Values of its two source IValueProvider&lt;bool&gt;s</returns>
        /// <remarks>Change notifications are only propagated if the related change of the source value could ever influence the result value.</remarks>
        public static IValueProvider<bool> And(this IValueProvider<bool> a, IValueProvider<bool> b)
        { return _And(a.Patched(), b.Patched()).Concrete(); }

        internal static IInternalValueProvider<bool> _And(this IInternalValueProvider<bool> a, IInternalValueProvider<bool> b)
        { return _Convert(a, b, (avp, bvp) => avp.Value && bvp.Value); }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'or' of the Values of its two source boolean values
        /// </summary>
        /// <param name="a">The first source boolean to calculate the result from.</param>
        /// <param name="b">The second source boolean to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'or' of the source values.</returns>
        /// <remarks>The returned <see cref="IValueProvider{Bool}"/> will be a static value provider.</remarks>
        public static IValueProvider<bool> Or(bool a, bool b)
        { return _Or(a, b).Concrete(); }

        internal static IInternalValueProvider<bool> _Or(bool a, bool b)
        { return _Static(a || b); }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'or' of the Values of its two source IValueProvider&lt;bool&gt;s
        /// </summary>
        /// <param name="a">The first source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <param name="b">The second source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'or' of the Values of its two source IValueProvider&lt;bool&gt;s</returns>
        /// <remarks>Change notifications are only propagated if the related change of the source value could ever influence the result value.</remarks>
        public static IValueProvider<bool> Or(this IValueProvider<bool> a, bool b)
        { return _Or(a.Patched(), b).Concrete(); }

        internal static IInternalValueProvider<bool> _Or(this IInternalValueProvider<bool> a, bool b)
        { return b ? staticTrueVP : a; }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'or' of the Values of its two source values.
        /// </summary>
        /// <param name="a">The first source boolean to calculate the result from.</param>
        /// <param name="b">The second source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An <see cref="IValueProvider{Bool}"/> that represents the logical 'or' of its two source values.</returns>
        public static IValueProvider<bool> Or(bool a, IValueProvider<bool> b)
        { return _Or(a, b.Patched()).Concrete(); }

        internal static IInternalValueProvider<bool> _Or(bool a, IInternalValueProvider<bool> b)
        { return a ? staticTrueVP : b; }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'or' of the Values of its two source IValueProvider&lt;bool&gt;s
        /// </summary>
        /// <param name="a">The first source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <param name="b">The second source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'or' of the Values of its two source IValueProvider&lt;bool&gt;s</returns>
        /// <remarks>Change notifications are only propagated if the related change of the source value could ever influence the result value.</remarks>
        public static IValueProvider<bool> Or(this IValueProvider<bool> a, IValueProvider<bool> b)
        { return _Or(a.Patched(), b.Patched()).Concrete(); }

        internal static IInternalValueProvider<bool> _Or(this IInternalValueProvider<bool> a, IInternalValueProvider<bool> b)
        { return _Convert(a, b, (avp, bvp) => avp.Value || bvp.Value); }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'exclusive or' of the Values of its two source IValueProvider&lt;bool&gt;s
        /// </summary>
        /// <param name="a">The first source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <param name="b">The second source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'exclusive or' of the Values of its two source IValueProvider&lt;bool&gt;s</returns>
        public static IValueProvider<bool> XOr(this IValueProvider<bool> a, IValueProvider<bool> b)
        { return _XOr(a.Patched(), b.Patched()).Concrete(); }

        internal static IInternalValueProvider<bool> _XOr(this IInternalValueProvider<bool> a, IInternalValueProvider<bool> b)
        { return _Select(a, b, (av, bv) => av ^ bv); }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'exclusive or' of the Values of its two source IValueProvider&lt;bool&gt;s
        /// </summary>
        /// <param name="a">The first source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <param name="b">The second source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'exclusive or' of the Values of its two source IValueProvider&lt;bool&gt;s</returns>
        public static IValueProvider<bool> XOr(this IValueProvider<bool> a, bool b)
        { return _XOr(a.Patched(), b).Concrete(); }

        internal static IInternalValueProvider<bool> _XOr(this IInternalValueProvider<bool> a, bool b)
        { return b ? _Invert(a) : a; }

        /// <summary>
        /// Returns an IValueProvider&lt;bool&gt; that represents the logical 'inverse' of the Value of its source IValueProvider&lt;bool&gt;
        /// </summary>
        /// <param name="a">The source IValueProvider&lt;bool&gt; to calculate the result from.</param>
        /// <returns>An IValueProvider&lt;bool&gt; that represents the logical 'inverse' of the Value of its source IValueProvider&lt;bool&gt;</returns>
        public static IValueProvider<bool> Invert(this IValueProvider<bool> a)
        { return _Invert(a.Patched()).Concrete(); }

        internal static IInternalValueProvider<bool> _Invert(this IInternalValueProvider<bool> a)
        { return _Select(a, av => !av); }


        /// <summary>
        /// Inline if. When the first value is true the second argument 'trueValue' will be returned and when it is false
        /// the third agrument falseValue will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting IValueProvider and of IValuePRovider arguments 'trueValue' and 'falseValue'</typeparam>
        /// <param name="selector">A boolean value that determines if the value of the second or third argument is returned.</param>
        /// <param name="trueValue">Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">Value if the 'selector' argument equals true.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        /// <remarks>The returned <see cref="IValueProvider{TType}"/> will be a static value provider.</remarks>
        public static IValueProvider<TType> IIf<TType>(bool selector, TType trueValue, TType falseValue)
        { return _IIf(selector, trueValue, falseValue).Concrete(); }

        internal static IInternalValueProvider<TType> _IIf<TType>(bool selector, TType trueValue, TType falseValue)
        { return _Static(selector ? trueValue : falseValue); }

        /// <summary>
        /// Inline if. The first IValueProvider 'selector' returns a boolean value. When this value is true the second argument 'trueValue' will be returned and when it is false
        /// the third agrument falseValue will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting IValueProvider and of IValuePRovider arguments 'trueValue' and 'falseValue'</typeparam>
        /// <param name="selector">The IValueProvider&lt;bool&gt; arguments whose Value determines if 'trueValue' or 'falseValue' will be returned.</param>
        /// <param name="trueValue">Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">Value if the 'selector' argument equals false.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        public static IValueProvider<TType> IIf<TType>(this IValueProvider<bool> selector, TType trueValue, TType falseValue)
        { return _IIf(selector.Patched(), trueValue, falseValue).Concrete(); }

        internal static IInternalValueProvider<TType> _IIf<TType>(this IInternalValueProvider<bool> selector, TType trueValue, TType falseValue)
        { return _Select(selector, FuncExtender<bool>.Extend(trueValue, falseValue, (sv, tv, fv) => sv ? tv : fv)); }

        /// <summary>
        /// Inline if. When the first value is true the Value proverty of the second argument <paramref name="trueValue"/> will be returned and when it is false
        /// the third agrument <paramref name="falseValue"/> will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting- and <paramref name="falseValue"/> <see cref="IValueProvider"/>s and type of argument <paramref name="trueValue"/>.</typeparam>
        /// <param name="selector">A boolean value that determines if the value of the second or third argument is returned.</param>
        /// <param name="trueValue">Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">Value if the 'selector' argument equals false.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        public static IValueProvider<TType> IIf<TType>(bool selector, IValueProvider<TType> trueValue, TType falseValue)
        { return _IIf(selector, trueValue.Patched(), falseValue); }

        internal static IInternalValueProvider<TType> _IIf<TType>(bool selector, IInternalValueProvider<TType> trueValue, TType falseValue)
        { return selector ? trueValue : _Static(falseValue); }

        /// <summary>
        /// Inline if. The first IValueProvider 'selector' returns a boolean value. When this value is true the second argument 'trueValue' will be returned and when it is false
        /// the third agrument falseValue will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting IValueProvider and of IValueProvider arguments 'trueValue' and 'falseValue'</typeparam>
        /// <param name="selector">The IValueProvider&lt;bool&gt; arguments whose Value determines if 'trueValue' or 'falseValue' will be returned.</param>
        /// <param name="trueValue">Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">Value if the 'selector' argument equals false.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        public static IValueProvider<TType> IIf<TType>(this IValueProvider<bool> selector, IValueProvider<TType> trueValue, TType falseValue)
        { return _IIf(selector.Patched(), trueValue.Patched(), falseValue).Concrete(); }

        internal static IInternalValueProvider<TType> _IIf<TType>(this IInternalValueProvider<bool> selector, IInternalValueProvider<TType> trueValue, TType falseValue)
        { return _IIf(selector, trueValue, _Static(falseValue)); }

        /// <summary>
        /// Inline if. When the first value is true the second argument <paramref name="trueValue"/> will be returned and when it is false
        /// the Value proverty of the third agrument <paramref name="falseValue"/> will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting- and <paramref name="trueValue"/> <see cref="IValueProvider"/>s and type of argument <paramref name="falseValue"/>.</typeparam>
        /// <param name="selector">A boolean value that determines if the value of the second or third argument is returned.</param>
        /// <param name="trueValue">Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">Value if the 'selector' argument equals false.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        public static IValueProvider<TType> IIf<TType>(bool selector, TType trueValue, IValueProvider<TType> falseValue)
        { return _IIf(selector, trueValue, falseValue.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _IIf<TType>(bool selector, TType trueValue, IInternalValueProvider<TType> falseValue)
        { return selector ? _Static(trueValue) : falseValue; }

        /// <summary>
        /// Inline if. The first IValueProvider 'selector' returns a boolean value. When this value is true the second argument 'trueValue' will be returned and when it is false
        /// the third agrument falseValue will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting IValueProvider and of IValuePRovider arguments 'trueValue' and 'falseValue'</typeparam>
        /// <param name="selector">The IValueProvider&lt;bool&gt; arguments whose Value determines if 'trueValue' or 'falseValue' will be returned.</param>
        /// <param name="trueValue">The IValueProvider that will be return if the Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">The IValueProvider that will be return if the Value if the 'selector' argument equals true.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        public static IValueProvider<TType> IIf<TType>(this IValueProvider<bool> selector, TType trueValue, IValueProvider<TType> falseValue)
        { return _IIf(selector.Patched(), trueValue, falseValue.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _IIf<TType>(this IInternalValueProvider<bool> selector, TType trueValue, IInternalValueProvider<TType> falseValue)
        { return _IIf(selector, _Static(trueValue), falseValue); }

        /// <summary>
        /// Inline if. When the first value is true the second argument <paramref name="trueValue"/> will be returned and when it is false
        /// the third agrument <paramref name="falseValue"/> will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting IValueProvider and of IValuePRovider arguments 'trueValue' and 'falseValue'</typeparam>
        /// <param name="selector">A boolean value that determines if the value of the second or third argument is returned.</param>
        /// <param name="trueValue">The IValueProvider that will be return if the Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">The IValueProvider that will be return if the Value if the 'selector' argument equals true.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        public static IValueProvider<TType> IIf<TType>(bool selector, IValueProvider<TType> trueValue, IValueProvider<TType> falseValue)
        { return _IIf(selector, trueValue.Patched(), falseValue.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _IIf<TType>(bool selector, IInternalValueProvider<TType> trueValue, IInternalValueProvider<TType> falseValue)
        { return (selector ? trueValue : falseValue); }

        /// <summary>
        /// Inline if. The first IValueProvider 'selector' returns a boolean value. When this value is true the second argument 'trueValue' will be returned and when it is false
        /// the third agrument falseValue will be returned.
        /// </summary>
        /// <typeparam name="TType">The Value type of the resulting IValueProvider and of IValuePRovider arguments 'trueValue' and 'falseValue'</typeparam>
        /// <param name="selector">The IValueProvider&lt;bool&gt; arguments whose Value determines if 'trueValue' or 'falseValue' will be returned.</param>
        /// <param name="trueValue">The IValueProvider that will be return if the Value if the 'selector' argument equals true.</param>
        /// <param name="falseValue">The IValueProvider that will be return if the Value if the 'selector' argument equals true.</param>
        /// <returns>An IValueProvider whose Value depending on the Value of 'selector' will be the Value of 'trueValue' or 'falseValue'</returns>
        public static IValueProvider<TType> IIf<TType>(this IValueProvider<bool> selector, IValueProvider<TType> trueValue, IValueProvider<TType> falseValue)
        { return _IIf(selector.Patched(), trueValue.Patched(), falseValue.Patched()).Concrete();  }

        internal static IInternalValueProvider<TType> _IIf<TType>(this IInternalValueProvider<bool> selector, IInternalValueProvider<TType> trueValue, IInternalValueProvider<TType> falseValue)
        { return _Convert(selector, trueValue, falseValue, (svp, tvp, fvp) => svp.Value ? tvp.Value : fvp.Value); }
    
    }
}
