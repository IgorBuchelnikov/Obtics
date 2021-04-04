using System;
using System.Collections.Generic;
using System.ComponentModel;
using SL = System.Linq;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class UnorderedDictionaryWithReturnPathTransformation<TKey, TOut> : UnorderedDictionaryTransformationBase<TKey, TOut, Tuple<UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>,IDictionaryReturnPath<TKey,TOut>>>
    {
        public static UnorderedDictionaryWithReturnPathTransformation<TKey, TOut> Create(UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut> source, IDictionaryReturnPath<TKey,TOut> returnPath)
        {
            if (source == null || returnPath == null)
                return null;

            return
                Carrousel.Get<UnorderedDictionaryWithReturnPathTransformation<TKey, TOut>, UnorderedNotifyVpcTransformation<SL.IGrouping<TKey, TOut>, TOut>, IDictionaryReturnPath<TKey,TOut>>(source, returnPath);
        }

        internal override void Initialize(Tuple<UnorderedNotifyVpcTransformation<System.Linq.IGrouping<TKey, TOut>, TOut>, IDictionaryReturnPath<TKey, TOut>> prms)
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

                if (GetState(ref flags) == State.Clients)
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

            if(_IsReadOnlyVP != null)
                _IsReadOnlyVP.SubscribeINC(this);
        }

        protected override void UnsubscribeFromSources()
        {
            base.UnsubscribeFromSources();

            if(_IsReadOnlyVP != null)
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

        protected internal override UnorderedNotifyVpcTransformation<System.Linq.IGrouping<TKey, TOut>, TOut> Source
        { get { return _Prms.First; } }

        public override void Add(TKey key, TOut value)
        { _Prms.Second.Add(this,key,value); }

        public override void Insert(TKey key, TOut value)
        { _Prms.Second.Insert(this, key, value); }

        public override void Clear()
        { _Prms.Second.Clear(this); }

        public override bool Remove(KeyValuePair<TKey, TOut> item)
        { return _Prms.Second.Remove(this,item.Key,item.Value); }

        public override bool Remove(TKey key)
        { return _Prms.Second.Remove(this, key); }

        public override bool IsSynchronized
        { get { return _Prms.Second.IsSynchronized(this); } }

        public override object SyncRoot
        { get { return _Prms.Second.SyncRoot(this); } }
    }
}
