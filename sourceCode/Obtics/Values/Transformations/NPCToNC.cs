using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;
using System.ComponentModel;

namespace Obtics.Values.Transformations
{
    internal abstract class NPCToNCBase<TSource> : NCObservableObjectBase<TSource>
    {
        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<TSource>.FlagsState flagsState)
        {
            var npc = _Prms as INotifyPropertyChanged;
            if (npc != null)
                npc.PropertyChanged += npc_PropertyChanged;

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<TSource>.FlagsState flagsState)
        {
            var npc = _Prms as INotifyPropertyChanged;
            if (npc != null)
                npc.PropertyChanged -= npc_PropertyChanged;

            return true;
        }

        void npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var msg =
                e.PropertyName == SIValueProvider.ValuePropertyName ? (INCEventArgs)INCEventArgs.PropertyChanged() :
                e.PropertyName == SIValueProvider.IsReadOnlyPropertyName ? (INCEventArgs)INCEventArgs.IsReadOnlyChanged() :
                null
            ;

            if (msg != null)
            {
                FlagsState flags;

                while (!GetFlags(out flags)) ;

                SendMessage(ref flags, msg);
            }
        }
    }

    internal sealed class NPCToNC : NPCToNCBase<IValueProvider>, IInternalValueProvider
    {
        public static NPCToNC Create(IValueProvider source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<NPCToNC, IValueProvider>(source);
        }

        #region IValueProvider Members

        public object CheckedValue
        { get { return _Prms.Value; } }

        #endregion

        #region IValueProvider<object> Members

        public object Value
        {
            get { return _Prms.Value; }
            set { _Prms.Value = value; }
        }

        #endregion

        #region IValueProvider Members

        public bool IsReadOnly
        { get { return _Prms.IsReadOnly; } }

        #endregion
    }

    internal sealed class NPCToNC<TSource> : NPCToNCBase<IValueProvider<TSource>>, IInternalValueProvider<TSource>
    {
        public static NPCToNC<TSource> Create(IValueProvider<TSource> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<NPCToNC<TSource>, IValueProvider<TSource>>(source);
        }

        #region IValueProvider<TSource> Members

        public TSource Value
        {
            get { return _Prms.Value; }
            set { _Prms.Value = value; }
        }

        #endregion

        #region IValueProvider Members

        object IValueProvider.Value
        {
            get { return ((IValueProvider)_Prms).Value; }
            set { ((IValueProvider)_Prms).Value = value; }
        }

        public bool IsReadOnly
        { get { return _Prms.IsReadOnly; } }

        #endregion
    }
}
