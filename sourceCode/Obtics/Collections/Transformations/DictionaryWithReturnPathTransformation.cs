using System;
using System.Collections.Generic;
using System.ComponentModel;
using SL = System.Linq;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class DictionaryWithReturnPathTransformation<TKey, TOut> : DictionaryTransformationBase<TKey, TOut, Tuple<NotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>,IDictionaryReturnPath<TKey,TOut>>>
    {
        public static DictionaryWithReturnPathTransformation<TKey, TOut> Create(IEnumerable<Tuple<TOut, TKey>> s, IEqualityComparer<TKey> comparer, IDictionaryReturnPath<TKey,TOut> returnPath)
        {
            var source = s.Patched();

            if (source == null || comparer == null || returnPath == null)
                return null;

            return
                Carrousel.Get<DictionaryWithReturnPathTransformation<TKey, TOut>, NotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>, IDictionaryReturnPath<TKey,TOut>>(
                    CreateSource(comparer, source), 
                    returnPath
                );
        }

        public override IInternalEnumerable<KeyValuePair<TKey, TOut>> UnorderedForm
        {
            get 
            { 
                return 
                    UnorderedDictionaryWithReturnPathTransformation<TKey, TOut>.Create(
                        (UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>)_Prms.First.UnorderedForm, 
                        _Prms.Second
                    ); 
            }
        }

        internal override void Initialize(Tuple<NotifyVpcTransformation<System.Linq.IGrouping<TKey, TOut>, TOut>, IDictionaryReturnPath<TKey, TOut>> prms)
        {
            base.Initialize(prms);
            _IsReadOnlyVP = _Prms.Second.IsReadOnly(this).Patched();
        }

        IInternalValueProvider<bool> _IsReadOnlyVP;

        public override bool IsReadOnly
        { 
            get 
            {
                if (_IsReadOnlyVP == null)
                    return false;

                FlagsState flags;

            retry:
                while (!GetFlags(out flags)) ;

                if(GetState(ref flags) == State.Clients)
                {
                    if (!Lock(ref flags))
                        goto retry;

                    SubscribeOnSources();

                    SetState(ref flags, State.Visible);
                    Commit(ref flags);
                }

                return _IsReadOnlyVP.Value; 
            } 
        }

        protected override void SubscribeOnSources()
        {
            base.SubscribeOnSources();

            if (_IsReadOnlyVP != null)
                _IsReadOnlyVP.SubscribeINC(this);
        }

        protected override void UnsubscribeFromSources()
        {
            base.UnsubscribeFromSources();

            if (_IsReadOnlyVP != null)
                _IsReadOnlyVP.UnsubscribeINC(this);
        }

        protected override void SourceValueChangeEvent(object sender)
        {
            if (object.ReferenceEquals(sender, _IsReadOnlyVP))
            {
                FlagsState flags;

                while (!GetFlags(out flags)) ;

                SendMessage(ref flags, INCEventArgs.IsReadOnlyChanged());
            }
        }

        protected internal override NotifyVpcTransformation<System.Linq.IGrouping<TKey, TOut>, TOut> Source
        { get { return _Prms.First; } }

        public override void Add(TKey key, TOut value)
        { _Prms.Second.Add(this, key, value); }

        public override void Insert(TKey key, TOut value)
        { _Prms.Second.Insert(this, key, value); }

        public override void Clear()
        { _Prms.Second.Clear(this); }

        public override bool Remove(KeyValuePair<TKey, TOut> item)
        { return _Prms.Second.Remove(this, item.Key, item.Value); }

        public override bool Remove(TKey key)
        { return _Prms.Second.Remove(this, key); }

        public override bool IsSynchronized
        { get { return _Prms.Second.IsSynchronized(this); } }

        public override object SyncRoot
        { get { return _Prms.Second.SyncRoot(this); } }
    }
}
