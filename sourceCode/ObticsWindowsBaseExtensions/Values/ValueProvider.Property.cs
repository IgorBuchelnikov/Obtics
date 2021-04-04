using System.Reflection;
using Obtics.Values.Transformations;
using System.Windows;
using System;

namespace Obtics.Values
{
    public static partial class ValueProviderWindowsBaseExtensions
    {
        /// <summary>
        /// struct that wraps a typed live DependencyObject reference and can retrieve live property values from the refered DependencyObject.
        /// </summary>
        /// <typeparam name="TObject">The type of the refered DependencyObject. The type is constrained to be a <see cref="DependencyObject"/> or derived class.</typeparam>
        public struct DependencyPropertyGetter<TObject> where TObject : DependencyObject
        {
            /// <summary>
            /// Constructs an <see cref="DependencyPropertyGetter{TObject}"/> instance.
            /// </summary>
            /// <param name="obj">An <see cref="IValueProvider{TObject}"/> of TObject that will become the wrapped live DependencyPropertyGetter reference.</param>
            public DependencyPropertyGetter(IValueProvider<TObject> obj)
            { _Object = obj.Patched(); }

            IInternalValueProvider<TObject> _Object;

            /// <summary>
            /// Gets the live value of the indicated property from the wrapped live DependencyObject. 
            /// </summary>
            /// <typeparam name="TProperty">Type of the property.</typeparam>
            /// <param name="dependencyProperty">A <see cref="DependencyProperty"/> object that indicates the property to retrieve the value from.</param>
            /// <returns>An <see cref="IValueProvider{TProperty}"/> of <typeparamref name="TProperty"/> representing the live value of the property indicated by <paramref name="dependencyProperty"/>.</returns>
            public IValueProvider<TProperty> Get<TProperty>(DependencyProperty dependencyProperty)
            { return _Property<TObject, TProperty>(_Object, dependencyProperty); }

            /// <summary>
            /// Gets the live value of the indicated property from the wrapped live DependencyObject. 
            /// </summary>
            /// <typeparam name="TProperty">Type of the property.</typeparam>
            /// <param name="name">The name of the property to retrieve the value from.</param>
            /// <returns>An <see cref="IValueProvider{TProperty}"/> of <typeparamref name="TProperty"/> representing the live value of the property indicated by <paramref name="name"/>.</returns>
            public IValueProvider<TProperty> Get<TProperty>(string name)
            { return ValueProvider.Property<TObject, TProperty>(_Object, name); }
        }

        /// <summary>
        /// Creates a 'property getter struct' that can retrieve individual live properties on the source DependencyObject.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of IValueProvider <paramref name="source"/>. Type of the object the property value is gotten from. This must be <see cref="DependencyObject"/> or a derived type.</typeparam>
        /// <param name="source">The <see cref="IValueProvider{TSource}"/> who's Value provides the DependencyObject we are taking the property from.</param>
        /// <returns>A <see cref="PropertyGetter{TSource}"/> of <typeparamref name="TSource"/> that can retrieve individual properties.</returns>
        /// <remarks>
        /// Other than the standard 'Property' methods, this construct allows the retrieval of properties without
        /// having to explicitly specify the type of the source object.
        /// <code> ccProvider.Property&lt;ContentControl,object&gt;( "Content" ) </code>
        /// can be written as:
        /// <code> ccProvider.Properties().Get&lt;object&gt;( "Content" ) </code>
        /// </remarks>
        public static DependencyPropertyGetter<TSource> DependencyProperties<TSource>(this IValueProvider<TSource> source)
            where TSource : DependencyObject
        { return new DependencyPropertyGetter<TSource>(source); }

        /// <summary>
        /// Creates a 'property getter struct' that can retrieve individual live properties on the source DependencyObject.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of IValueProvider <paramref name="source"/>. Type of the object the property value is gotten from. This must be <see cref="DependencyObject"/> or a derived type.</typeparam>
        /// <param name="source">The <typeparamref name="TSource"/> object we are taking the property from.</param>
        /// <returns>A <see cref="PropertyGetter{TSource}"/> of <typeparamref name="TSource"/> that can retrieve individual properties.</returns>
        /// <remarks>
        /// Other than the standard 'Property' methods, this construct allows the retrieval of properties without
        /// having to explicitly specify the type of the source object.
        /// <code> ValueProvider.Property&lt;ContentControl,object&gt;( contentControl, "Content" ) </code>
        /// can be written as:
        /// <code> ValueProvider.Properties(contentControl).Get&lt;object&gt;( "Content" ) </code>
        /// </remarks>
        public static DependencyPropertyGetter<TSource> DependencyProperties<TSource>(TSource source)
            where TSource : DependencyObject
        { return new DependencyPropertyGetter<TSource>(ValueProvider._Static(source)); }


        /// <summary>
        /// Gets the value of the dependency property indicated by <paramref name="dependencyProperty"/> from the dependency object provided by <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of IValueProvider <paramref name="source"/>. Type of the dependency object the property value is gotten from. This must be <see cref="DependencyObject"/> or a derived type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="dependencyProperty"/></typeparam>
        /// <param name="source">The <see cref="IValueProvider{TSource}"/> who's Value provides the dependency object we are taking the property from.</param>
        /// <param name="dependencyProperty">The <see cref="DependencyProperty"/> object indicating the property we are getting the value from.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from the active Value of <paramref name="source"/>. 
        /// This means the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when either the Value of <paramref name="source"/> changes
        /// or the property indicated by <paramref name="dependencyProperty"/> on the current <paramref name="source"/> Value changes.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(this IValueProvider<TSource> source, DependencyProperty dependencyProperty)
            where TSource : DependencyObject
        { return _Property<TSource, TProperty>(ValueProvider.Patched(source), dependencyProperty); }

        internal static IInternalValueProvider<TProperty> _Property<TSource, TProperty>(this IInternalValueProvider<TSource> source, DependencyProperty dependencyProperty)
            where TSource : DependencyObject
        { return DependencyPropertyTransformation<TSource, TProperty>.Create(source, dependencyProperty); }

        /// <summary>
        /// Gets the value of the dependency property indicated by <paramref name="dependencyProperty"/> from the dependency object <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of <paramref name="source"/>. Type of the dependency object the property value is gotten from. This must be <see cref="DependencyObject"/> or a derived type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="dependencyProperty"/></typeparam>
        /// <param name="source">The <typeparamref name="TSource"/> object we are getting the property from.</param>
        /// <param name="dependencyProperty">The <see cref="DependencyProperty"/> object indicating the property we are getting the value from.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from <paramref name="source"/>. 
        /// This means the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when the property indicated 
        /// by <paramref name="dependencyProperty"/> changes value.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(TSource source, DependencyProperty dependencyProperty)
            where TSource : DependencyObject
        { return _Property<TSource, TProperty>( ValueProvider._Static(source), dependencyProperty); }
    }
}
