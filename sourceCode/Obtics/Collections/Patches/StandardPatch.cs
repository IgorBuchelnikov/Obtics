using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using Obtics.Collections.Transformations;
using SL = System.Linq;

namespace Obtics.Collections.Patches
{
    //These classes provide patches for IEnumerables supporting INotifyCollectionChanged, IBindingList or INotifyPropertyChanged.
    //It is not used for IVersionedEnumerable with INotifyCollectionChanged. VersionedPatch does that.
    //They mantain buffers with a copy of the information in the source IEnumerable. Not observable source will therefore still be patched using SimpleSequencingPatch

    abstract class StandardPatchBase<TSource, TOut> : NCObservableObjectBase<TSource>, IInternalEnumerable<TOut>
    {
        ValueHybridList<TOut> _Buffer;

        protected abstract IEnumerable<TOut> SourceAsTOut { get; }

        protected override bool ClientSubscribesEvent(ref ObservableObjectBase<TSource>.FlagsState flagsState)
        {
            var collection = _Prms as ICollection;
            var syncRoot = collection != null ? collection.SyncRoot : null;

            if (syncRoot != null)
                System.Threading.Monitor.Enter(syncRoot);

            try
            {
                //we can only initialize buffer now if we can lock source
                if (syncRoot != null)
                    (_Buffer = new ValueHybridList<TOut>()).InsertRange(0, SourceAsTOut);

                var ncc = _Prms as INotifyCollectionChanged;

                if (ncc != null)
                    ncc.CollectionChanged += prms_CollectionChanged;
                else
                {
#if !SILVERLIGHT
                    var bl = _Prms as IBindingList;

                    if (bl != null)
                        bl.ListChanged += prms_ListChanged;
                    else
#endif
                    {
                        var npc = _Prms as INotifyPropertyChanged;

                        if (npc != null)
                            npc.PropertyChanged += prms_PropertyChanged;
                    }
                }
            }
            finally
            {
                if (syncRoot != null)
                    System.Threading.Monitor.Exit(syncRoot);
            }

            return true;
        }

        protected override bool ClientUnsubscribesEvent(ref ObservableObjectBase<TSource>.FlagsState flagsState)
        {
            var ncc = _Prms as INotifyCollectionChanged;

            if (ncc != null)
                ncc.CollectionChanged -= prms_CollectionChanged;
            else
            {
#if !SILVERLIGHT
                var bl = _Prms as IBindingList;

                if (bl != null)
                    bl.ListChanged -= prms_ListChanged;
                else
#endif
                {

                    var npc = _Prms as INotifyPropertyChanged;

                    if (npc != null)
                        npc.PropertyChanged -= prms_PropertyChanged;
                }
            }

            TakeSnapshot();
            _Buffer = null;

            return true;
        }


        void prms_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == SIList.ItemsIndexerPropertyName)
                prms_CollectionChanged(sender, SIOrderedNotifyCollectionChanged.ResetNotifyCollectionChangedEventArgs);
        }

        //We must assume that while we are dealing with a change event, no further changes to the
        //source collection will happen.
        void prms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            INCollectionChangedEventArgs[] message = null;

            try
            {
                ClearExternalCache(ref flags);

                if (_Buffer != null)
                {
                    TakeSnapshot();

                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            _Buffer.InsertRange(args.NewStartingIndex, SL.Enumerable.Cast<TOut>(args.NewItems));
                            break;
#if !SILVERLIGHT
                        case NotifyCollectionChangedAction.Move:
                            if (args.NewStartingIndex < args.OldStartingIndex)
                                CollectionsHelper.Rotate(_Buffer, args.NewStartingIndex, args.OldStartingIndex, args.OldStartingIndex + args.NewItems.Count);
                            else
                                CollectionsHelper.Rotate(_Buffer, args.OldStartingIndex, args.OldStartingIndex + args.NewItems.Count, args.NewStartingIndex + args.NewItems.Count);
                            break;
#endif
                        case NotifyCollectionChangedAction.Remove:
                            _Buffer.RemoveRange(args.OldStartingIndex, args.OldItems.Count);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            _Buffer.RemoveRange(args.NewStartingIndex, args.OldItems.Count);
                            _Buffer.InsertRange(args.NewStartingIndex, SL.Enumerable.Cast<TOut>(args.NewItems));
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            _Buffer.Clear();

                            //source will not change while we are dealing with the event.
                            _Buffer.InsertRange(0, SourceAsTOut);

                            //var collection = _Prms as ICollection;
                            //var syncRoot = collection == null ? null : collection.SyncRoot;

                            //if (syncRoot != null)
                            //    lock (syncRoot)
                            //        _Buffer.AddRange(SourceAsTOut);
                            //else
                            //    _Buffer.AddRange(SourceAsTOut);

                            break;
                    }
                }
                else
                {
                    args = SIOrderedNotifyCollectionChanged.ResetNotifyCollectionChangedEventArgs;

                    //ok initialize buffer. Source will not change while we are dealing with event.
                    (_Buffer = new ValueHybridList<TOut>()).InsertRange(0, SourceAsTOut);
                }

                _ContentVersion = INCEventArgs.FromNCC<TOut>(_ContentVersion, args, out message);
            }
            finally
            { Commit(ref flags); }

            if (message != null)
                SendMessages(ref flags, message);

        }

#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// In a multithreaded environment we need to assume that while we are dealing with a changed event,
        /// the source collection will not change further.
        /// </remarks>
        void prms_ListChanged(object sender, ListChangedEventArgs e)
        {
            FlagsState flags;

            while (!GetAndLockFlags(out flags)) ;

            INCEventArgs message = null;
            INCEventArgs[] messages = null;

            try
            {
                if (_Buffer != null)
                {
                    TakeSnapshot();

                    switch (e.ListChangedType)
                    {
                        case ListChangedType.ItemAdded:
                            {
                                var index = e.NewIndex;
                                var item = (TOut)((IBindingList)_Prms)[index];
                                message = INCEventArgs.CollectionAdd<TOut>(_ContentVersion = _ContentVersion.Next, index, item);
                                _Buffer.Insert(index, item);
                            }
                            break;
                        case ListChangedType.ItemChanged:
                            {
                                var index = e.NewIndex;
                                var newItem = (TOut)((IBindingList)_Prms)[index];
                                var oldItem = _Buffer[index];
                                message = INCEventArgs.CollectionReplace<TOut>(_ContentVersion = _ContentVersion.Next, index, newItem, oldItem);
                                _Buffer[index] = newItem;
                            }
                            break;
                        case ListChangedType.ItemDeleted:
                            {
                                var index = e.NewIndex;
                                var oldItem = _Buffer[index];
                                message = INCEventArgs.CollectionRemove<TOut>(_ContentVersion = _ContentVersion.Next, index, oldItem);
                                _Buffer.RemoveAt(index);
                            }
                            break;
                        case ListChangedType.ItemMoved:
                            {
                                var oldIndex = e.OldIndex;
                                var newIndex = e.NewIndex;
                                var item = _Buffer[oldIndex];
                                messages = new INCEventArgs[] {
                                    INCEventArgs.CollectionRemove<TOut>(_ContentVersion = _ContentVersion.Next,oldIndex,item),
                                    INCEventArgs.CollectionAdd<TOut>(_ContentVersion = _ContentVersion.Next, newIndex, item)
                                };

                                if (oldIndex < newIndex)
                                    CollectionsHelper.Rotate(_Buffer, oldIndex, oldIndex + 1, newIndex);
                                else
                                    CollectionsHelper.Rotate(_Buffer, newIndex, oldIndex, oldIndex + 1);
                            }
                            break;
                        case ListChangedType.PropertyDescriptorAdded:
                        case ListChangedType.PropertyDescriptorChanged:
                        case ListChangedType.PropertyDescriptorDeleted:
                            return;
                        case ListChangedType.Reset:
                            {
                                message = INCEventArgs.CollectionReset(_ContentVersion = _ContentVersion.Next);
                                _Buffer.Clear();
                                _Buffer.InsertRange(0,SourceAsTOut);
                            }
                            break;
                        default:
                            throw new Exception("Unexpected ListChangedType.");
                    }
                }
                else
                {
                    message = INCEventArgs.CollectionReset(_ContentVersion = _ContentVersion.Next);
                    (_Buffer = new ValueHybridList<TOut>()).InsertRange(0,SourceAsTOut);
                }
            }
            finally
            { Commit(ref flags); }

            if (message != null)
                SendMessage(ref flags, message);

            if (messages != null)
                SendMessages(ref flags, messages);
        }
#endif

        VersionNumber _ContentVersion;

        #region IVersionedEnumerable<TOut> Members

        void TakeSnapshot()
        { XLazySnapshotEnumerator.TakeSnapshot<TOut>(this, _ContentVersion, () => SL.Enumerable.ToArray((IEnumerable<TOut>)_Buffer ?? SL.Enumerable.Empty<TOut>())); }

        class SafeEnumeratorProvider
        {
            public TOut[] Items;
            public VersionNumber SN;

            public IVersionedEnumerator<TOut> GetEnumerator()
            { return VersionedEnumerator.WithContentVersion(Items, SN); }
        }

        public IVersionedEnumerator<TOut> GetEnumerator()
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            if (GetExternalCacheHint(ref flags))
            {
                object cachedValue;

                if (TryGetExternalCache(out cachedValue))
                    return ((SafeEnumeratorProvider)cachedValue).GetEnumerator();
            }

            if (!Lock(ref flags))
                goto retry;

            try
            {
                if (GetExternalCacheHint(ref flags))
                {
                    object cachedValue;

                    if (TryGetExternalCache(out cachedValue))
                        return ((SafeEnumeratorProvider)cachedValue).GetEnumerator();
                }

                //We do SourceAsTOut.ToList() because we promised to be cached. ie not raise exceptions after the enumerator is aquired.
                IEnumerable<TOut> source = _Buffer;
                if (source == null)
                    source = SL.Enumerable.ToList(SourceAsTOut);
                var res = XLazySnapshotEnumerator.Create<TOut>(this, _ContentVersion, source.GetEnumerator());

                if (GetIsPivotNode(ref flags))
                {
                    TakeSnapshot();

                    List<TOut> list = new List<TOut>();

                    while (res.MoveNext())
                        list.Add(res.Current);

                    var sep = new SafeEnumeratorProvider { Items = list.ToArray(), SN = res.ContentVersion };

                    SetExternalCache(ref flags, sep);

                    res.Dispose();

                    res = sep.GetEnumerator();
                }

                return res;
            }
            finally
            { Commit(ref flags); }
        }

        #endregion

        #region IVersionedEnumerable Members

        IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region IEnumerable<TOut> Members

        IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region IInternalEnumerable<TOut> Members

        public IInternalEnumerable<TOut> UnorderedForm
        {
            get { return this; }
        }

        #endregion

        #region IInternalEnumerable Members

        IInternalEnumerable IInternalEnumerable.UnorderedForm
        { get { return this; } }

        public bool IsMostUnordered
        { get { return true; } }

        public bool HasSafeEnumerator
        { get { return true; } }

        #endregion
    }

    sealed class StandardPatch<TSource> : StandardPatchBase<TSource, object>
        where TSource : class, IEnumerable
    {
        public static StandardPatch<TSource> Create(TSource prms)
        {
            if (prms == null)
                return null;

            return Carrousel.Get<StandardPatch<TSource>, TSource>(prms);
        }

        protected override IEnumerable<object> SourceAsTOut
        { get { return SL.Enumerable.Cast<object>(_Prms); } }
    }

    sealed class StandardPatch<TSource, TOut> : StandardPatchBase<TSource, TOut>
        where TSource : class, IEnumerable<TOut>
    {
        public static StandardPatch<TSource, TOut> Create(TSource prms)
        {
            if (prms == null)
                return null;

            return Carrousel.Get<StandardPatch<TSource, TOut>, TSource>(prms);
        }

        protected override IEnumerable<TOut> SourceAsTOut
        { get { return _Prms; } }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    sealed class StandardPatchAdapterClass<TSource> : ICollectionAdapter
        where TSource : class, IEnumerable
    {
        #region ICollectionAdapter Members

        public IVersionedEnumerable AdaptCollection(object collection)
        {
            return (IVersionedEnumerable)StandardPatch<TSource>.Create((TSource)collection);

            ////verify that ICollection actually has a syncRoot
            //var typedCollection = collection as ICollection;
            //var typedSequence = (TSource)collection;

            //return typedCollection != null && typedCollection.SyncRoot != null ? (IVersionedEnumerable)MtSequencingPatchTransformation<TSource>.Create(typedSequence) : SequencingPatchTransformation.Create(typedSequence);
        }

        #endregion
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    sealed class StandardPatchAdapterClass<TSource, TElement> : ICollectionAdapter
        where TSource : class, IEnumerable<TElement>
    {
        #region ICollectionAdapter Members

        public IVersionedEnumerable AdaptCollection(object collection)
        {
            return (IVersionedEnumerable)StandardPatch<TSource, TElement>.Create((TSource)collection);

            ////verify that ICollection actually has a syncRoot
            //var typedCollection = collection as ICollection;
            //var typedSequence = (TSource)collection;

            //return typedCollection != null && typedCollection.SyncRoot != null ? (IVersionedEnumerable)MtSequencingPatchTransformation<TSource, TElement>.Create(typedSequence) : SequencingPatchTransformation<TElement>.Create(typedSequence);
        }

        #endregion
    }
}
