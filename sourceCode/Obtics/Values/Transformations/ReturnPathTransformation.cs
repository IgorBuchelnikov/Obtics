using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Obtics.Values.Transformations
{
    internal abstract class ReturnPathTransformation<TType,TReturnPathAction> : ConvertTransformationBase<TType, Tuple<IInternalValueProvider<TType>, IInternalValueProvider<bool>, TReturnPathAction>>
    {
        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<Tuple<IInternalValueProvider<TType>, IInternalValueProvider<bool>, TReturnPathAction>>.FlagsState flagsState)
        {
            if (_Prms.Second != null)
                _Prms.Second.SubscribeINC(this);

            return base.ClientSubscribesEvent(ref flagsState);
        }

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<Tuple<IInternalValueProvider<TType>, IInternalValueProvider<bool>, TReturnPathAction>>.FlagsState flagsState)
        {
            if (_Prms.Second != null)
                _Prms.Second.UnsubscribeINC(this);

            return base.ClientUnsubscribesEvent(ref flagsState);
        }

        /// <summary>
        /// ProtectedValue
        /// </summary>
        protected override TType GetValue()
        { return _Prms.First.Value; }

        protected override bool GetIsReadOnly()
        { return _Prms.Second != null && _Prms.Second.Value; }

        protected override void SourceChangeEvent(object sender)
        {
            if (object.ReferenceEquals(sender, _Prms.Second))
            {
                FlagsState flags;
                while (!GetFlags(out flags)) ;
                SendMessage(ref flags, INCEventArgs.IsReadOnlyChanged());
            }
            else
                base.SourceChangeEvent(sender);
        }
    }

    internal class ReturnPathTransformationExplicitReadOnlyCheck<TType> : ReturnPathTransformation<TType, Action<TType, bool>>
    {
        internal static ReturnPathTransformationExplicitReadOnlyCheck<TType> Create(IInternalValueProvider<TType> source, IInternalValueProvider<bool> readonlyProvider, Action<TType, bool> setAction)
        {
            if (source == null || setAction == null)
                return null;

            return Carrousel.Get<ReturnPathTransformationExplicitReadOnlyCheck<TType>, IInternalValueProvider<TType>, IInternalValueProvider<bool>, Action<TType, bool>>(source, readonlyProvider, setAction);
        }

        protected override void SetValue(TType value)
        { _Prms.Third(value, GetIsReadOnly()); }
    }

    internal class ReturnPathTransformationImplicitReadOnlyCheck<TType> : ReturnPathTransformation<TType, Action<TType>>
    {
        internal static ReturnPathTransformationImplicitReadOnlyCheck<TType> Create(IInternalValueProvider<TType> source, IInternalValueProvider<bool> readonlyProvider, Action<TType> setAction)
        {
            if (source == null || setAction == null)
                return null;

            return Carrousel.Get<ReturnPathTransformationImplicitReadOnlyCheck<TType>, IInternalValueProvider<TType>, IInternalValueProvider<bool>, Action<TType>>(source, readonlyProvider, setAction);
        }

        protected override void SetValue(TType value)
        {
            if (GetIsReadOnly())
                throw new InvalidOperationException("ReadOnly");

            _Prms.Third(value);
        }
    }

}
