using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Obtics.Data
{
    class StaticCollectionAdapter<TElement> : Obtics.Collections.IVersionedEnumerable<TElement>
    {
        public StaticCollectionAdapter(IEnumerable<TElement> source)
        { _Source = source; }

        IEnumerable<TElement> _Source;

        #region IVersionedEnumerable Members

        public Obtics.Collections.VersionNumber ContentVersion
        { get { return default(Obtics.Collections.VersionNumber); } }

        Obtics.Collections.IVersionedEnumerator Obtics.Collections.IVersionedEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IVersionedEnumerable<TElement> Members

        public Obtics.Collections.IVersionedEnumerator<TElement> GetEnumerator()
        { return Obtics.Collections.VersionedEnumerator.WithContentVersion(_Source,ContentVersion); }

        #endregion

        #region IEnumerable<TElement> Members

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion
    }

    class EnumerableRowCollectionAdapter : Obtics.Collections.ICollectionAdapter
    {
        internal static ConditionalWeakTable<object, Obtics.Collections.IVersionedEnumerable> _Carrousel = new ConditionalWeakTable<object, Obtics.Collections.IVersionedEnumerable>();
        #region ICollectionAdapter Members

        public Obtics.Collections.IVersionedEnumerable AdaptCollection(object collection)
        { return collection == null ? null : _Carrousel.GetOrAddUnderLock(collection, c => new StaticCollectionAdapter<object>(((IEnumerable)c).OfType<object>())); }

        #endregion
    }

    class EnumerableRowCollectionAdapter<TElement> : Obtics.Collections.ICollectionAdapter
    {
        #region ICollectionAdapter Members

        public Obtics.Collections.IVersionedEnumerable AdaptCollection(object collection)
        { 
            return 
                collection == null ? null : 
                EnumerableRowCollectionAdapter._Carrousel.GetOrAddUnderLock(
                    collection, 
                    c => new StaticCollectionAdapter<TElement>((IEnumerable<TElement>)c)
                )
            ; 
        }

        #endregion
    }
}
