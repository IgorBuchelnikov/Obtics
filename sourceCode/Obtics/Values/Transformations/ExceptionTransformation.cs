using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Obtics.Values.Transformations
{
    internal class ExceptionTransformation<TType, TException> : ConvertTransformationBase<TType, Tuple<IInternalValueProvider<TType>,Func<TException,bool>,Func<TException, TType>>>
        where TException : Exception
    {
        public static ExceptionTransformation<TType, TException> Create(IInternalValueProvider<TType> source, Func<TException,bool> handler, Func<TException, TType> fallbackGenerator)
        {
            if (source == null || handler == null || fallbackGenerator == null)
                return null;

            return Carrousel.Get<ExceptionTransformation<TType, TException>, IInternalValueProvider<TType>, Func<TException, bool>, Func<TException, TType>>(source, handler, fallbackGenerator);
        }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override TType GetValue()
        {
            try 
            { return _Prms.First.Value; }
            catch (TException ex) 
            {
                if (!_Prms.Second(ex))
                    throw;

                return _Prms.Third(ex); 
            }
        }

        protected override void SourceExceptionEvent(object sender, INCEventArgs evt)
        {
            var exEvtArgs = ((INExceptionEventArgs)evt);

            var exception = exEvtArgs.Exception as TException;

            if (exception != null)
            {
                try
                {
                    if (_Prms.Second(exception))
                    {
                        base.SourceChangeEvent(sender);
                        return;
                    }
                }
                catch (Exception ex)
                { 
                    base.SourceExceptionEvent(sender, INCEventArgs.Exception(ex));
                    return;
                }
            }
            
            base.SourceExceptionEvent(sender, evt);
        }
    }
}
