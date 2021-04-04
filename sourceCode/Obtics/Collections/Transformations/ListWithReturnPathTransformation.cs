using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;

namespace Obtics.Collections.Transformations
{
    internal sealed class ListWithReturnPathTransformation<TElement> : ListTransformationBase<TElement, IInternalEnumerable<TElement>, Tuple<IInternalEnumerable<TElement>, IListReturnPath<TElement>>>
    {
        public static ListWithReturnPathTransformation<TElement> Create(IEnumerable<TElement> source, IListReturnPath<TElement> returnPath)
        {
            var s = source.Patched();

            return s == null || returnPath == null ? null : Carrousel.Get<ListWithReturnPathTransformation<TElement>, IInternalEnumerable<TElement>, IListReturnPath<TElement>>(s,returnPath);
        }

        public override IInternalEnumerable<TElement> UnorderedForm
        { get { return _Prms.First; } }

        internal override void Initialize(Tuple<IInternalEnumerable<TElement>, IListReturnPath<TElement>> prms)
        {
            base.Initialize(prms);
            _IsReadOnlyVP = _Prms.Second.IsReadOnly(this).Patched();
        }

        IInternalValueProvider<bool> _IsReadOnlyVP;

        public override bool IsReadOnly
        { 
            get 
            {
                return 
                    _IsReadOnlyVP != null && DoAction(_IsReadOnlyVP, irovp => irovp.Value, irovp => irovp.Value) ;
            } 
        }

        protected override void SourceValueChangeEvent(object sender)
        {
            FlagsState flags;

            while (!GetFlags(out flags)) ;

            SendMessage( ref flags, INCEventArgs.IsReadOnlyChanged() );
        }

        protected override void SubscribeOnSources()
        {
            if (_IsReadOnlyVP != null)
                _IsReadOnlyVP.SubscribeINC(this);
            base.SubscribeOnSources();
        }

        protected override void UnsubscribeFromSources()
        {
            if (_IsReadOnlyVP != null)
                _IsReadOnlyVP.UnsubscribeINC(this);
            base.UnsubscribeFromSources();
        }

        protected override IInternalEnumerable<TElement> Source
        { get { return _Prms.First; } }

        //return path delegation
        public override void Add(TElement item)
        { _Prms.Second.Add(this, item); }

        public override void Clear()
        { _Prms.Second.Clear(this); }

        public override void Insert(int index, TElement item)
        { _Prms.Second.Insert(this, index, item); }

        public override bool IsSynchronized
        { get { return _Prms.Second.IsSynchronized(this); } }

        public override bool Remove(TElement item)
        { return _Prms.Second.Remove(this,item); }

        public override void RemoveAt(int index)
        { _Prms.Second.RemoveAt(this,index); }

        public override void ReplaceAt(int index, TElement item)
        { _Prms.Second.ReplaceAt(this, index, item); }

        public override object SyncRoot
        { get { return _Prms.Second.SyncRoot(this); } }
    }
}
