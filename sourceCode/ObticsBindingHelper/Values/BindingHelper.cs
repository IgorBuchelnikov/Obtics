using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Obtics.Values
{
    public static class BindingHelper
    {
        abstract class Assigner<TValue> : IDisposable
        {
            internal IValueProvider<TValue> _source;

            internal IDisposable Initialize()
            {
                var npc = _source as INotifyPropertyChanged;

                if (npc != null)
                    npc.PropertyChanged += npc_PropertyChanged;

                npc_PropertyChanged(null, SIValueProvider.ValuePropertyChangedEventArgs);

                return this;
            }

            void npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == SIValueProvider.ValuePropertyName)
                {
                    var source = _source;

                    if (source != null)
                        Assign(source.Value);
                }
            }

            protected abstract void Assign(TValue value);

            #region IDisposable Members

            public virtual void Dispose()
            {
                var npc = _source as INotifyPropertyChanged;

                if (npc != null)
                    npc.PropertyChanged -= npc_PropertyChanged;

                _source = null;
            }

            #endregion
        }

        class ActionAssigner<TValue> : Assigner<TValue>
        {
            internal Action<TValue> _assignment;

            public override void Dispose()
            {
                _assignment = null;
                base.Dispose();
            }

            protected override void Assign(TValue value)
            {
                var a = _assignment;
 	            if(a != null)
                    a(value);
            } 
        }

        class PropInfoAssigner<TTarget,TValue> : Assigner<TValue>
            where TTarget : class
        {
            internal PropertyInfo _propInfo ;
            internal TTarget _target;

            protected override void Assign(TValue value)
            {
 	            var t = _target;
                var p = _propInfo;

                if(t != null && p != null)
                    p.SetValue(t, value, null);
            }

            public override void Dispose()
            {
                _propInfo = null;
                _target = null;
 	            base.Dispose();
            }
        }

#if !SILVERLIGHT
        class PropDescriptorAssigner<TTarget,TValue> : Assigner<TValue>
            where TTarget : class
        {
            internal PropertyDescriptor _propDescriptor ;
            internal TTarget _target;

            protected override void Assign(TValue value)
            {
 	            var t = _target;
                var p = _propDescriptor;

                if(t != null && p != null)
                    p.SetValue(t, value);
            }

            public override void Dispose()
            {
                _propDescriptor = null;
                _target = null;
 	            base.Dispose();
            }
        }
#endif
       
        public static IDisposable AssignTo<TValue>(this IValueProvider<TValue> source, Action<TValue> assignment)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if(assignment == null)
                throw new ArgumentNullException("assignment");

            return new ActionAssigner<TValue> { _source = source, _assignment = assignment }.Initialize(); 
        }

        public static IDisposable AssignTo<TTarget, TValue>(this IValueProvider<TValue> source, TTarget target, Expression<Func<TTarget, TValue>> propertyExpression)
            where TTarget : class
        {
            if (source == null)
                    throw new ArgumentNullException("source");

            if(target == null)
                throw new ArgumentNullException("target");

            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var propInfo = PropertyFinder.FindProperty(propertyExpression);

            if (!propInfo.CanWrite)
                throw new ArgumentException(string.Format("The property '{0}' can not be written to.", propInfo.Name));

#if !SILVERLIGHT
            //prefer setting via property descriptor since that supports
            //notification to bindings.

            if (propInfo.GetSetMethod() != null) //is the setter public?
            {
                var pd = TypeDescriptor.GetProperties(target).Find(propInfo.Name, false);

                if (pd != null)
                    return new PropDescriptorAssigner<TTarget, TValue> { _source = source, _propDescriptor = pd, _target = target }.Initialize();
            }
#endif

            return new PropInfoAssigner<TTarget,TValue> { _source = source, _propInfo = propInfo, _target = target }.Initialize();
        }
    }
}
