using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Collections.Transformations
{
    internal sealed class ExceptionTransformation<T, E> : TranslucentTransformationBase<T,IInternalEnumerable<T>,Tuple<IInternalEnumerable<T>,Func<E,bool>,Func<E,IEnumerable<T>>>> 
        where E : Exception
    {
        public static ExceptionTransformation<T, E> Create(IEnumerable<T> source, Func<E, bool> handler, Func<E, IEnumerable<T>> fallbackCollectionGenerator ) 
        {
            if (source == null || handler == null || fallbackCollectionGenerator == null)
                return null;

            var s = source.Patched();

            if (!s.HasSafeEnumerator)
                s = CacheTransformation<T>.Create(s);

            return Carrousel.Get<ExceptionTransformation<T, E>, IInternalEnumerable<T>, Func<E, bool>, Func<E, IEnumerable<T>>>(s, handler, fallbackCollectionGenerator);
        }

        public override bool HasSafeEnumerator
        { get { return _Prms.First.HasSafeEnumerator; } }

        public override bool IsMostUnordered
        { get { return _Prms.First.IsMostUnordered; } }

        public override IInternalEnumerable<T> UnorderedForm
        { get { return Create( _Prms.First.UnorderedForm, _Prms.Second, _Prms.Third ); } }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override void SourceExceptionEvent(object sender, INExceptionEventArgs args)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Initial:
                case State.Clients:
                    return;
            }

            if (!Lock(ref flags))
                goto retry;

            switch (state)
            {
                case State.Cached:
                    ClearExternalCache(ref flags);
                    goto case State.Visible;

                case State.Visible:
                case State.Hidden:
                    SetState(ref flags, State.Excepted);
                    goto case State.Excepted;

                case State.Excepted:
                    
                    var ex = args.Exception;
                    var exAsE = ex as E;
                    var handled = false;

                    if (exAsE != null)
                    {
                        try
                        {
                            handled = _Prms.Second(exAsE);                        
                        }
                        catch (Exception newEx)
                        {
                            ex = newEx;                        
                        }
                    }
            
                    if(handled)
                    {
                        var evt = INCEventArgs.CollectionReset(AdvanceContentVersion());
                        Commit(ref flags);
                        SendMessage(ref flags, evt);
                    }
                    else
                    {
                        Commit(ref flags);
                        SourceExceptionEvent(sender, INCEventArgs.Exception(ex));
                    }
                                       
                    break;
            }
        }

        protected override Tuple<bool, IEnumerator<T>> GetEnumeratorFromBuffer()
        {
            try
            {
                var enm = _Prms.First.GetEnumerator();
                return Tuple.Create(enm.ContentVersion == _SourceContentVersion, (IEnumerator<T>)enm);
            }
            catch (Exception ex)
            {
                var exAsE = ex as E;

                if (exAsE == null || !_Prms.Second(exAsE))
                    throw;

                var enm = _Prms.Third(exAsE);
                return Tuple.Create(false, enm.GetEnumerator());
            }            
        }

        protected override IEnumerator<T> GetEnumeratorDirect()
        {
            try
            {
                return _Prms.First.GetEnumerator();
            }
            catch (Exception ex)
            {
                var exAsE = ex as E;

                if (exAsE == null || !_Prms.Second(exAsE))
                    throw;

                var enm = _Prms.Third(exAsE);
                return enm.GetEnumerator();
            }
        }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            try
            {
                return _Prms.First.GetEnumerator().ContentVersion;
            }
            catch (Exception ex)
            {
                var exAsE = ex as E;

                if (exAsE == null || !_Prms.Second(exAsE))
                    throw;

                return _SourceContentVersion;
            }
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs collectionEvent, out INCEventArgs message1, out INCEventArgs[] message2)
        {
            message2 = null;
            message1 = collectionEvent.Clone(AdvanceContentVersion());

            switch (collectionEvent.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                case INCEventArgsTypes.CollectionRemove:
                case INCEventArgsTypes.CollectionReplace:  
                    return Change.Controled;
                case INCEventArgsTypes.CollectionReset:
                    return Change.Destructive;
                default:
                    throw new Exception("Unexpected event type.");
            }
        }
    }
}
