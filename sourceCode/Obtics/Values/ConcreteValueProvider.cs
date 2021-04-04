using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Obtics.Values
{
    public class ConcreteValueProvider : IValueProvider, INotifyPropertyChanged, IReceiveChangeNotification
    {
        internal ConcreteValueProvider()
        { }

        internal virtual IInternalValueProvider GetSource() { return null; }

        internal IInternalValueProvider Source { get { return GetSource(); } }

        internal TvdP.Threading.TinyReaderWriterLock _readerWriterLock;

        #region IValueProvider Members

        public object Value
        {
            get { return GetSource().Value; }
            set { GetSource().Value = value; }
        }

        public bool IsReadOnly
        { get { return GetSource().IsReadOnly; } }

        #endregion

        #region INotifyPropertyChanged Members

        event PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                var source = GetSource();
                bool freshSubscribe;

                _readerWriterLock.LockForWriting();

                try
                {
                    freshSubscribe = _propertyChanged == null;

                    if (freshSubscribe)
                        source.SubscribeINC(this);

                    _propertyChanged += value;
                }
                finally
                {
                    _readerWriterLock.ReleaseForWriting();
                }

                //shield against clients that get value first
                //and only after register for changes.
                if (freshSubscribe)
                    GC.KeepAlive(source.Value);
            }

            remove
            {
                _readerWriterLock.LockForWriting();

                try
                {
                    _propertyChanged -= value;

                    if (_propertyChanged == null)
                        GetSource().UnsubscribeINC(this);
                }
                finally
                {
                    _readerWriterLock.ReleaseForWriting();
                }
            }
        }

        #endregion

        #region IReceiveChangeNotification Members

        void IReceiveChangeNotification.NotifyChanged(object sender, INCEventArgs changeArgs)
        {
            switch(changeArgs.Type)
            {
                case INCEventArgsTypes.Exception:
                {
                    var exceptionArgs = (INExceptionEventArgs)changeArgs;
                    throw exceptionArgs.Exception;
                }

                case INCEventArgsTypes.ValueChanged:
                    DelayedActionRegistry.Register(SendValuePropertyChanged);
                    break;

                case INCEventArgsTypes.IsReadOnlyChanged:
                    DelayedActionRegistry.Register(SendIsReadOnlyPropertyChanged);
                    break;
            }
        }

        void SendValuePropertyChanged()
        { Send(SIValueProvider.ValuePropertyChangedEventArgs); }

        void SendIsReadOnlyPropertyChanged()
        { Send(SIValueProvider.IsReadOnlyPropertyChangedEventArgs); }

        void Send(PropertyChangedEventArgs npcArgs)
        {
            PropertyChangedEventHandler handler;

            _readerWriterLock.LockForReading();

            try
            {
                handler = _propertyChanged;
            }
            finally
            {
                _readerWriterLock.ReleaseForReading();
            }

            if (handler != null)
                handler(this, npcArgs);
        }

        #endregion

        sealed class UntypedConcreteValueProviderImp : ConcreteValueProvider
        {
            internal IInternalValueProvider _source;

            internal override IInternalValueProvider GetSource()
            { return _source; }
        }

        static TvdP.Collections.WeakDictionary<IInternalValueProvider, bool, UntypedConcreteValueProviderImp> _concreteMappings = new TvdP.Collections.WeakDictionary<IInternalValueProvider, bool, UntypedConcreteValueProviderImp>();

        internal static ConcreteValueProvider Create(IInternalValueProvider source)
        {
            if (source == null)
                return null;

            return _concreteMappings.GetOrAdd(source, false, (s, _) => new UntypedConcreteValueProviderImp { _source = s });
        }
    }

    public class ConcreteValueProvider<T> : ConcreteValueProvider, IValueProvider<T>
    {
        IInternalValueProvider<T> _source;

        internal override IInternalValueProvider  GetSource()
        { return _source; }

        internal new IInternalValueProvider<T> Source
        { get { return _source; } }        

        #region IValueProvider<T> Members

        public new T Value
        {
            get { return _source.Value; }
            set { _source.Value = value; }
        }

        #endregion

        static TvdP.Collections.WeakDictionary<IInternalValueProvider<T>, bool, ConcreteValueProvider<T>> _concreteMappings = new TvdP.Collections.WeakDictionary<IInternalValueProvider<T>, bool, ConcreteValueProvider<T>>();

        internal static ConcreteValueProvider<T> Create(IInternalValueProvider<T> source)
        {
            if (source == null)
                return null;

            return _concreteMappings.GetOrAdd(source, false, (s, _) => new ConcreteValueProvider<T> { _source = s });
        }
    }
}
