using System.Reflection;
using Obtics.Values.Transformations;
using System;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// struct that wraps a typed live object reference and can retrieve live property values from the refered object.
        /// </summary>
        /// <typeparam name="TObject">The type of the refered object. This type is constrained to be a reference type.</typeparam>
        public struct PropertyGetter<TObject> where TObject : class
        {
            /// <summary>
            /// Constructs an <see cref="PropertyGetter{TObject}"/> instance.
            /// </summary>
            /// <param name="obj">An <see cref="IValueProvider{TObject}"/> of TObject that will become the wrapped live object reference.</param>
            public PropertyGetter(IValueProvider<TObject> obj)
            { _Object = obj.Patched(); }

            IInternalValueProvider<TObject> _Object;

            /// <summary>
            /// Gets the live value of the indicated property from the wrapped live object. 
            /// </summary>
            /// <typeparam name="TProperty">Type of the property.</typeparam>
            /// <param name="propInfo">A <see cref="PropertyInfo"/> object that indicates the property to retrieve the value from.</param>
            /// <returns>An <see cref="IValueProvider{TProperty}"/> of <typeparamref name="TProperty"/> representing the live value of the property indicated by <paramref name="propInfo"/>.</returns>
            public IValueProvider<TProperty> Get<TProperty>(PropertyInfo propInfo)
            { return _Property<TObject, TProperty>(_Object, propInfo).Concrete(); }

            /// <summary>
            /// Gets the live value of the indicated property from the wrapped live object. 
            /// </summary>
            /// <typeparam name="TProperty">Type of the property.</typeparam>
            /// <param name="name">The name of the property to retrieve the value from.</param>
            /// <returns>An <see cref="IValueProvider{TProperty}"/> of <typeparamref name="TProperty"/> representing the live value of the property indicated by <paramref name="name"/>.</returns>
            public IValueProvider<TProperty> Get<TProperty>(string name)
            { return _Property<TObject, TProperty>(_Object, name).Concrete(); }
        }

        /// <summary>
        /// Creates a 'property getter struct' that can retrieve individual live properties on the source object.
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of IValueProvider <paramref name="source"/>. Type of the object the property value is gotten from. This must be a reference type.</typeparam>
        /// <param name="source">The <see cref="IValueProvider{TSource}"/> who's Value provides the object we are taking the property from.</param>
        /// <returns>A <see cref="PropertyGetter{TSource}"/> of <typeparamref name="TSource"/> that can retrieve individual properties.</returns>
        /// <remarks>
        /// Other than the standard 'Property' methods, this construct allows the retrieval of properties without
        /// having to explicitly specify the type of the source object.
        /// <code> dictProvider.Property&lt;Dictionary&lt;string,int&gt;,int&gt;( "Count" ) </code>
        /// can be written as:
        /// <code> dictProvider.Properties().Get&lt;int&gt;( "Count" ) </code>
        /// </remarks>
        public static PropertyGetter<TSource> Properties<TSource>(this IValueProvider<TSource> source)
            where TSource : class
        { return new PropertyGetter<TSource>(source); }

        /// <summary>
        /// Creates a 'property getter struct' that can retrieve individual live properties on the source object.
        /// </summary>
        /// <typeparam name="TSource">Type of <paramref name="source"/> where the property value is gotten from. This must be a reference type.</typeparam>
        /// <param name="source">The <typeparamref name="TSource"/> object we are taking the property from.</param>
        /// <returns>A <see cref="PropertyGetter{TSource}"/> of <typeparamref name="TSource"/> that can retrieve individual properties.</returns>
        /// <remarks>
        /// Other than the standard 'Property' methods, this construct allows the retrieval of properties without
        /// having to explicitly specify the type of the source object.
        /// <code> ValueProvider.Property&lt;Dictionary&lt;string,int&gt;,int&gt;( dict, "Count" ) </code>
        /// can be written as:
        /// <code> ValueProvider.Properties(dict).Get&lt;int&gt;( "Count" ) </code>
        /// </remarks>
        public static PropertyGetter<TSource> Properties<TSource>(TSource source)
            where TSource : class
        { return new PropertyGetter<TSource>(_Static(source)); }

        /// <summary>
        /// Gets the value of the property indicated by <paramref name="propInfo"/> from the object provided by <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of IValueProvider <paramref name="source"/>. Type of the object the property value is gotten from. This must be a reference type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="propInfo"/></typeparam>
        /// <param name="source">The <see cref="IValueProvider{TSource}"/> who's Value provides the object we are taking the property from.</param>
        /// <param name="propInfo">The <see cref="PropertyInfo"/> object indicating the property we are getting the value from.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value. 
        /// If either <paramref name="source"/> or <paramref name="propInfo"/> equals null then null is returned.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from the active Value of <paramref name="source"/>. 
        /// It does this by using the <see cref="System.ComponentModel.PropertyDescriptor"/> for the property. Provided that
        /// the object returned by the Value of <paramref name="source"/> sends change notifications for the property
        /// this means that the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when either the Value of <paramref name="source"/> changes
        /// or the property indicated by <paramref name="propInfo"/> on the current <paramref name="source"/> Value changes.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(this IValueProvider<TSource> source, PropertyInfo propInfo)
            where TSource : class
        { return _Property<TSource, TProperty>(source.Patched(), propInfo).Concrete(); }

        internal static IInternalValueProvider<TProperty> _Property<TSource, TProperty>(this IInternalValueProvider<TSource> source, PropertyInfo propInfo)
            where TSource : class
        { return PropertyTransformation<TSource, TProperty>.Create(source, propInfo); }

        /// <summary>
        /// Gets the value of the property indicated by <paramref name="propName"/> from the object provided by <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of IValueProvider <paramref name="source"/>. Type of the object the property value is gotten from. This must be a reference type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="propName"/></typeparam>
        /// <param name="source">The <see cref="IValueProvider{TSource}"/> who's Value provides the object we are taking the property from.</param>
        /// <param name="propName">The <see cref="string"/> indicating the property we are getting the value from.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value. 
        /// If either <paramref name="source"/> or <paramref name="propName"/> equals null then null is returned.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from the active Value of <paramref name="source"/>. 
        /// It does this by using the <see cref="System.ComponentModel.PropertyDescriptor"/> for the property. Provided that
        /// the object returned by the Value of <paramref name="source"/> sends change notifications for the property
        /// this means that  the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when either the Value of <paramref name="source"/> changes
        /// or the property indicated by <paramref name="propName"/> on the current <paramref name="source"/> Value changes.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(this IValueProvider<TSource> source, string propName)
            where TSource : class
        { return _Property<TSource, TProperty>(source.Patched(), propName).Concrete(); }

        internal static IInternalValueProvider<TProperty> _Property<TSource, TProperty>(this IInternalValueProvider<TSource> source, string propName)
            where TSource : class
        { return PropertyTransformation<TSource, TProperty>.Create(source, propName); }


        /// <summary>
        /// Gets the value of the property indicated by <paramref name="propName"/> from the object provided by <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the Value property of IValueProvider <paramref name="source"/>. Type of the object the property value is gotten from. This must be a reference type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="propName"/></typeparam>
        /// <param name="source">The <see cref="IValueProvider{TSource}"/> who's Value provides the object we are taking the property from.</param>
        /// <param name="propertyExpression">The lambda expression indicating the property we are getting the value from. It must be of form x => x.p where p is a property.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value. 
        /// If either <paramref name="source"/> or <paramref name="propertyExpression"/> equals null then null is returned.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from the active Value of <paramref name="source"/>. 
        /// It does this by using the <see cref="System.ComponentModel.PropertyDescriptor"/> for the property. Provided that
        /// the object returned by the Value of <paramref name="source"/> sends change notifications for the property
        /// this means that  the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when either the Value of <paramref name="source"/> changes
        /// or the property indicated by <paramref name="propertyExpression"/> on the current <paramref name="source"/> Value changes.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(this IValueProvider<TSource> source, System.Linq.Expressions.Expression<Func<TSource, TProperty>> propertyExpression)
            where TSource : class
        { return _Property(source.Patched(), propertyExpression).Concrete(); }

        internal static IInternalValueProvider<TProperty> _Property<TSource, TProperty>(this IInternalValueProvider<TSource> source, System.Linq.Expressions.Expression<Func<TSource, TProperty>> propertyExpression)
            where TSource : class
        {
            if (propertyExpression == null)
                return null;

            return _Property<TSource,TProperty>(source, PropertyFinder.FindProperty(propertyExpression));
        }

        /// <summary>
        /// Gets the value of the property indicated by <paramref name="propInfo"/> from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of <paramref name="source"/> where the property value is gotten from. This must be a reference type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="propInfo"/></typeparam>
        /// <param name="source">The <typeparamref name="TSource"/> object we are taking the property from.</param>
        /// <param name="propInfo">The <see cref="PropertyInfo"/> object indicating the property on <paramref name="source"/> we are getting the value from.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value. 
        /// If either <paramref name="source"/> or <paramref name="propInfo"/> equals null then null is returned instead.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from <paramref name="source"/>. 
        /// It does this by using the <see cref="System.ComponentModel.PropertyDescriptor"/> for the property. Provided that
        /// <paramref name="source"/> sends change notifications for the property
        /// this means that the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when 
        /// the property indicated by <paramref name="propInfo"/> on <paramref name="source"/> changes.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(TSource source, PropertyInfo propInfo)
            where TSource : class
        { return _Property<TSource, TProperty>(_Static(source), propInfo).Concrete(); }

        /// <summary>
        /// Gets the value of the property indicated by <paramref name="propName"/> from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of <paramref name="source"/> where the property value is gotten from. This must be a reference type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="propName"/></typeparam>
        /// <param name="source">The <typeparamref name="TSource"/> object we are taking the property from.</param>
        /// <param name="propName">The <see cref="string"/> indicating the property on <paramref name="source"/> we are getting the value from.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value. 
        /// If either <paramref name="source"/> or <paramref name="propName"/> equals null then null is returned instead.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from <paramref name="source"/>. 
        /// It does this by using the <see cref="System.ComponentModel.PropertyDescriptor"/> for the property. Provided that
        /// <paramref name="source"/> sends change notifications for the property
        /// this means that the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when 
        /// the property indicated by <paramref name="propName"/> on <paramref name="source"/> changes.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(TSource source, string propName)
            where TSource : class
        { return _Property<TSource, TProperty>(_Static(source), propName).Concrete(); }

        /// <summary>
        /// Gets the value of the property indicated by <paramref name="propName"/> from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">Type of <paramref name="source"/> where the property value is gotten from. This must be a reference type.</typeparam>
        /// <typeparam name="TProperty">Type of the property indicated by <paramref name="propName"/></typeparam>
        /// <param name="source">The <typeparamref name="TSource"/> object we are taking the property from.</param>
        /// <param name="propertyExpression">The lambda expression indicating the property we are getting the value from. It must be of form x => x.p where p is a property.</param>
        /// <returns>An <see cref="IValueProvider{TProperty}"/> who's Value will hold the property value. 
        /// If either <paramref name="source"/> or <paramref name="propertyExpression"/> equals null then null is returned instead.</returns>
        /// <remarks>
        /// The pipeline generated here will register for property change notifications from <paramref name="source"/>. 
        /// It does this by using the <see cref="System.ComponentModel.PropertyDescriptor"/> for the property. Provided that
        /// <paramref name="source"/> sends change notifications for the property
        /// this means that the Value of the result <see cref="IValueProvider{TProperty}"/> will be updated when 
        /// the property indicated by <paramref name="propertyExpression"/> on <paramref name="source"/> changes.
        /// </remarks>
        public static IValueProvider<TProperty> Property<TSource, TProperty>(TSource source, System.Linq.Expressions.Expression<Func<TSource, TProperty>> propertyExpression)
            where TSource : class
        { return _Property<TSource, TProperty>(_Static(source), propertyExpression).Concrete(); }
    }
}
