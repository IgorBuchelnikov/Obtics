using System;
using System.ComponentModel;
using System.Reflection;
using Obtics.Collections;
using TvdP.Collections;
using System.Threading;

#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
#endif

namespace Obtics.Values.Transformations
{


    internal sealed class PropertyTransformation<TIn, TOut> : CascadingTransformationBase<TOut, TIn, Tuple<IInternalValueProvider<TIn>, PropertyTransformation<TIn, TOut>.PropertyClass>>
        where TIn : class
    {
#if SILVERLIGHT
        internal class DPChangeListener : DependencyObject
        {
            static ConcurrentDictionary<PropertyTransformation<TIn, TOut>, DPChangeListener> _KeepAliveTable = new ConcurrentDictionary<PropertyTransformation<TIn, TOut>, DPChangeListener>();

            internal object Stub
            {
                get { return (object)GetValue(StubProperty); }
                set { SetValue(StubProperty, value); }
            }

            // Using a DependencyProperty as the backing store for Stub.  This enables animation, styling, binding, etc...
            internal static readonly DependencyProperty StubProperty =
                DependencyProperty.Register("Stub", typeof(object), typeof(DPChangeListener), new PropertyMetadata(null, StubChanged));

            static void StubChanged(object sender, DependencyPropertyChangedEventArgs args)
            {
                var listener = (DPChangeListener)sender;
                var owner = listener._Owner;

                owner.Buffer_PropertyChanged(sender, null);                
            }

            PropertyTransformation<TIn, TOut> _Owner;

            internal static void Register(PropertyTransformation<TIn, TOut> owner, TIn itm)
            {
                var listener = new DPChangeListener { _Owner = owner };

                var binding = new Binding();
                binding.Source = itm;
                binding.Path = new PropertyPath("{0}", owner._Prms.Second._PropertyInfo);
                binding.Mode = BindingMode.OneWay;

                BindingOperations.SetBinding(listener, StubProperty, binding);

                _KeepAliveTable[owner] = listener;                
            }

            internal static void Unregister(PropertyTransformation<TIn, TOut> owner, TIn itm)
            {
                DPChangeListener listener;

                if (_KeepAliveTable.TryGetValue(owner, out listener))
                {
                    listener.ClearValue(StubProperty);
                    _KeepAliveTable.TryRemove(owner, out listener);
                }                
            }
        }
#endif

        internal class PropertyClass
        {
            public TOut GetValue(TIn obj)
            { return _GetMethod(obj); }

            Func<TIn, TOut> _GetMethod;
            public readonly PropertyInfo _PropertyInfo;
            public readonly bool _Static;

            public PropertyClass(PropertyInfo pi)
            {
                _PropertyInfo = pi;
                var getPrm = System.Linq.Expressions.Expression.Parameter(typeof(TIn), "obj");
                var getMemberAcccess = System.Linq.Expressions.Expression.Property(getPrm, pi);
                var getLamda = System.Linq.Expressions.Expression.Lambda<Func<TIn, TOut>>(getMemberAcccess, getPrm);
                _GetMethod = getLamda.Compile();
            }
        }

        public static Cache<PropertyInfo, PropertyClass> _PropertyCache = new Cache<PropertyInfo, PropertyClass>();

        #region Constructor

        public static PropertyTransformation<TIn, TOut> Create(IInternalValueProvider<TIn> source, PropertyInfo propInfo)
        {
            if (source == null || propInfo == null)
                return null;

            if (!propInfo.DeclaringType.IsAssignableFrom(typeof(TIn)))
                throw new ArgumentException("propInfo not a property of TIn");

            if (!propInfo.CanRead)
                throw new ArgumentException("propInfo not a readable property");

            Type propType = propInfo.PropertyType;

            if (!typeof(TOut).IsAssignableFrom(propType))
                throw new ArgumentException("TOut not assignable from property type");

            PropertyClass pc;

            var propCache = _PropertyCache;

            if (!propCache.TryGetItem(propInfo, out pc))
                pc = propCache.GetOldest(propInfo, new PropertyClass(propInfo));

            return Carrousel.Get<PropertyTransformation<TIn, TOut>, IInternalValueProvider<TIn>, PropertyTransformation<TIn, TOut>.PropertyClass>(source, pc);
        }

        static Cache<Tuple<Type, string>, PropertyInfo> _PropInfoCache = new Cache<Tuple<Type, string>, PropertyInfo>();

        public static PropertyTransformation<TIn, TOut> Create(IInternalValueProvider<TIn> source, string propertyName)
        {
            if (propertyName == null)
                return null;

            PropertyInfo propInfo;
            var key = Tuple.Create(typeof(TIn),propertyName);

            if(!_PropInfoCache.TryGetItem(key, out propInfo))
            {
                propInfo = typeof(TIn).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if(propInfo == null)
                    foreach(var iface in typeof(TIn).GetInterfaces())
                        if( (propInfo = iface.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)) != null )
                            break;

                _PropInfoCache.GetOldest(key, propInfo);
            }
           
            return Create(source, propInfo);
        }

        #endregion

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override void SubscribeOnItm(TIn itm)
        {
            if (itm != null)
            {
#if SILVERLIGHT
                var npc = itm as INotifyPropertyChanged;

                if (npc != null)
                    npc.PropertyChanged += npc_PropertyChanged;
                else if(itm is DependencyObject)
                    DPChangeListener.Register(this, itm);
#else
                PropertyDescriptor pd;

                if (
                    !ObticsEqualityComparer<TIn>.IsPatched
                    && (pd = TypeDescriptor.GetProperties(itm).Find(Property._PropertyInfo.Name, false)) != null
                    && pd.SupportsChangeEvents
                )
                    pd.AddValueChanged(itm, Buffer_PropertyChanged);
                else
                {
                    var npc = itm as INotifyPropertyChanged;

                    if (npc != null)
                        npc.PropertyChanged += npc_PropertyChanged;
                }
#endif
            }
        }

        protected override void UnsubscribeFromItm(TIn itm)
        {
            if (itm != null)
            {
#if SILVERLIGHT
                var npc = itm as INotifyPropertyChanged;

                if (npc != null)
                    npc.PropertyChanged -= npc_PropertyChanged;
                else if (itm is DependencyObject)
                    DPChangeListener.Unregister(this, itm);
#else
                PropertyDescriptor pd;

                if (
                    !ObticsEqualityComparer<TIn>.IsPatched
                    && (pd = TypeDescriptor.GetProperties(itm).Find(Property._PropertyInfo.Name, false)) != null
                    && pd.SupportsChangeEvents
                )
                    pd.RemoveValueChanged(itm, Buffer_PropertyChanged);
                else
                {
                    var npc = itm as INotifyPropertyChanged;

                    if (npc != null)
                        npc.PropertyChanged -= npc_PropertyChanged;
                }
#endif
            }
        }

        protected override TIn GetItmFromSource()
        { return _Prms.First.Value; }

        PropertyClass Property
        { get { return _Prms.Second; } }

        void npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Property._PropertyInfo.Name)
                base.ItmChangeEvent(sender);
        }

        void Buffer_PropertyChanged(object sender, EventArgs args)
        { base.ItmChangeEvent(sender); }

        protected override TOut GetValueFromItm(TIn itm)
        {
            //may throw null ref exception
            return Property.GetValue(itm);
        }
    }
}
