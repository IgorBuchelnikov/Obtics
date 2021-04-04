using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;
using System.ComponentModel;
using System.Windows;

namespace Obtics
{
    public static class WpfHelper
    {
        static readonly PropertyChangedEventArgs ValuePropertyChangedEventArgs = new PropertyChangedEventArgs("Value");

        class InitializedListener<TFE> : IValueProvider<TFE>, INotifyPropertyChanged
            where TFE : FrameworkElement
        {
            TFE _FrameworkElement;

#if SILVERLIGHT
            bool _Initialized;
#endif

            internal InitializedListener(TFE frameworkElement)
            {
                _FrameworkElement = frameworkElement;
#if SILVERLIGHT
                _FrameworkElement.Loaded += _FrameworkElement_Initialized;
#else
                _FrameworkElement.Initialized += _FrameworkElement_Initialized;
#endif
            }

            void _FrameworkElement_Initialized(object sender, EventArgs e)
            {
#if SILVERLIGHT
                _Initialized = true;
#endif
                var propertyChanged = this.PropertyChanged;

                if (propertyChanged != null)
                    propertyChanged(this, ValuePropertyChangedEventArgs);
            }

            #region IValueProvider<TFE> Members

            public TFE Value
            {
                get 
                {
#if SILVERLIGHT
                    return _Initialized ? _FrameworkElement : null;
#else
                    return _FrameworkElement.IsInitialized ? _FrameworkElement : null; 
#endif
                }
                set { throw new NotImplementedException(); }
            }

            #endregion

            #region IValueProvider Members

            object IValueProvider.Value
            {
                get { return this.Value; }
                set { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            { get { return true; } }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        static object GetInitializedListener(DependencyObject obj)
        { return (object)obj.GetValue(InitializedListenerProperty); }

        static void SetInitializedListener(DependencyObject obj, object value)
        { obj.SetValue(InitializedListenerProperty, value); }

        static readonly DependencyProperty InitializedListenerProperty =
            DependencyProperty.RegisterAttached(
                "InitializedListener", 
                typeof(object), 
                typeof(WpfHelper), 
#if SILVERLIGHT
                new PropertyMetadata(null)
#else
                new UIPropertyMetadata(null)
#endif
);

        /// <summary>
        /// Returns an instance of the given framework element when it is initialized.
        /// </summary>
        /// <typeparam name="TFE">Specific type of the framework element.</typeparam>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns> An <see cref="IValueProvider{TFE}"/> whose Value property gives the given framework element or null when <paramref name="frameworkElement"/> is null or the framework element IsInitialized property is false.</returns>
        /// <remarks>
        /// <para>
        /// This helper can solve a problem when a value transformation is built directly on named child elements
        /// and other elements bind directly to the value transformation.
        /// </para>
        /// <para>
        /// XAML code snippet:
        /// <code>
        ///     &lt:!--note order; binding before source element definition--&gt;
        ///     &lt;Label Content="{Binding RelativeSource={RelativeSource AncestorType={x:Type Window}}, Path=UpperCase.Value}" /&gt;
        ///     &lt;TextBox x:Name="SourceTextBox" /&gt;
        /// </code>
        /// 
        /// Code behind snippet:
        /// <code>
        ///     class MyWindow : Window
        ///     {
        ///         public IValueProvider&lt;string&gt; UpperCase
        ///         {
        ///             get
        ///             {
        ///                 return 
        ///                     ExpressionObserver.Execute(
        ///                         () => SourceTextBox.Text.ToUpper()
        ///                     );
        ///             }
        ///         }
        ///     }
        /// </code>
        /// </para>
        /// <para>
        /// When the windows contents are built up, the binding of the label control is executed BEFORE the
        /// TextBox is assigned to the SourceTextBox field. This SourceTextBox field is a plain field and not
        /// observable. This means that the value transformation defined in the UpperCase property getter will
        /// never notice any changes and not update its result Value.
        /// </para>
        /// <para>
        /// With the WpfHelper.WhenInitialized() extension method this problem can be solved as follows:
        /// <code>
        ///     class MyWindow : Window
        ///     {
        ///         public IValueProvider&lt;string&gt; UpperCase
        ///         {
        ///             get
        ///             {
        ///                 return 
        ///                     ExpressionObserver.Execute(
        ///                         () => this.WhenInitialized().Value.SourceTextBox.Text.ToUpper()
        ///                     );
        ///             }
        ///         }
        ///     }
        /// </code>
        /// </remarks>
        public static IValueProvider<TFE> WhenInitialized<TFE>(this TFE frameworkElement)
            where TFE : FrameworkElement
        {
            if (frameworkElement == null)
                return null;

            //only one listener per FrameworkElement instance
            object listener = GetInitializedListener(frameworkElement);

            if (listener == null)
                lock (frameworkElement)
                {
                    listener = GetInitializedListener(frameworkElement);

                    if( listener == null ) //still?
                        SetInitializedListener(frameworkElement, listener = new InitializedListener<TFE>(frameworkElement));
                }

            return (IValueProvider<TFE>)listener;
        }

        /// <summary>
        /// Tries to resilve a name in the name scope of the given <see cref="FrameworkElement"/>.
        /// </summary>
        /// <typeparam name="TChild">Type or base-type of the resolved object.</typeparam>
        /// <param name="frameworkElement">The <see cref="FrameworkElement"/> in whose name scope we try to resolve <paramref name="name"/>.</param>
        /// <param name="name">The name we try to resolve.</param>
        /// <returns>The found object or null if either <paramref name="frameworkElement"/> is null, <paramref name="name"/> is null, the name can not be found or the actual type does not match <typeparamref name="TChild"/>.</returns>
        public static TChild FindName<TChild>(this FrameworkElement frameworkElement, string name)
            where TChild : class
        { return frameworkElement == null || name == null ? null : frameworkElement.FindName(name) as TChild; }
    }
}
