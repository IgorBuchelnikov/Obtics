using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using SL = System.Linq;

namespace Obtics.Collections.Patches
{   
    /// <summary>
    /// Patch for not observable (as far as we know) source collections.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    class NotObservablePatchBase<TSource> : IInternalEnumerable
        where TSource : IEnumerable
    {
        internal TSource _Source;


        #region IInternalEnumerable Members

        public IInternalEnumerable UnorderedForm
        { get { return this; } }

        public bool IsMostUnordered
        { get { return true; } }

        public bool HasSafeEnumerator
        { get { return false; } }

        #endregion

        #region IVersionedEnumerable Members

        public VersionNumber ContentVersion
        { get { return default(VersionNumber); } }

        public IVersionedEnumerator GetEnumerator()
        {
            return
                VersionedEnumerator.WithContentVersion( 
                    System.Linq.Enumerable.Cast<object>(_Source), 
                    ContentVersion
                )
            ;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region INotifyChanged Members

        public void SubscribeINC(IReceiveChangeNotification consumer)
        {}

        public void UnsubscribeINC(IReceiveChangeNotification consumer)
        {}

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as NotObservablePatchBase<TSource>;
            return 
                other != null 
                && ObticsEqualityComparer<TSource>.Default.Equals(_Source, other._Source);
        }

        public override int GetHashCode()
        {
            return 
                748387928 ^ ObticsEqualityComparer<TSource>.Default.GetHashCode(_Source);
        }
    }

    /// <summary>
    /// Takes a non-patched source and magicks a sequence for it's notify collection changed events. Preferable there should be only
    /// 1 SequencePatch for a given non-versiined source. If this object would receive change notifications out of order then the result
    /// will be messed up.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    sealed class NotObservablePatch<TType> : NotObservablePatchBase<IEnumerable<TType>>, IInternalEnumerable<TType>
    {
        #region IInternalEnumerable<TType> Members

        public new IInternalEnumerable<TType> UnorderedForm
        { get { return this; } }

        #endregion

        #region IVersionedEnumerable<TType> Members

        public new IVersionedEnumerator<TType> GetEnumerator()
        { return VersionedEnumerator.WithContentVersion(_Source,ContentVersion); }

        #endregion

        #region IEnumerable<TType> Members

        IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as NotObservablePatch<TType>;
            return
                other != null
                && ObticsEqualityComparer<IEnumerable<TType>>.Default.Equals(_Source, other._Source);
        }

        public override int GetHashCode()
        {
            return
                99383722 ^ ObticsEqualityComparer<IEnumerable<TType>>.Default.GetHashCode(_Source);
        }    
    }


    sealed class NotObservablePatchAdapterClass : ICollectionAdapter
    {
        #region ICollectionAdapter Members

        public IVersionedEnumerable AdaptCollection(object collection)
        {
            return
                new NotObservablePatchBase<IEnumerable> { _Source = (IEnumerable)collection };
        }

        #endregion

        internal static readonly NotObservablePatchAdapterClass _Instance = new NotObservablePatchAdapterClass();
    }

    sealed class NotObservablePatchAdapterClass<TElement> : ICollectionAdapter
    {
        #region ICollectionAdapter Members

        public IVersionedEnumerable AdaptCollection(object collection)
        {
            return
                new NotObservablePatch<TElement> { _Source = (IEnumerable<TElement>)collection };
        }

        #endregion
    }
}
