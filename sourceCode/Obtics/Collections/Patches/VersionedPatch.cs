using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using SL = System.Linq;
using Obtics.Collections.Transformations;

namespace Obtics.Collections.Patches
{
    //patches IVersionedEnumerable objects that support INotifyCollectionChanged. The source already offers
    //sequencing information. The event model just needs to be translated to INotifyChanged
    //These classes don't buffer the source collection so ordering is irrelevant. They are their own unordered form.

    /// <summary>
    /// Base for transformation objects that transform an IVersionedEnumerable with INotifyCollectionChanged
    /// to an IVersionedEnumerable with INotifyChanged (internal change notification mechanism)
    /// </summary>
    /// <typeparam name="TOut">Type of the sequence items</typeparam>
    /// <typeparam name="TSource">Type of the source sequence. Must be an <see cref="IVersionedEnumerable"/>.</typeparam>
    abstract class VersionedPatchBase<TOut, TSource> : TranslucentTransformationBase<TOut, TSource, TSource>
        where TSource : IVersionedEnumerable
    {
        public override bool IsMostUnordered
        { get { return true; } }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            var enm = _Prms.GetEnumerator();

            try
            {
                return enm.ContentVersion;
            }
            finally
            {
                var disposable = enm as IDisposable;

                if (disposable != null)
                    disposable.Dispose();
            }
        }

        protected override void SubscribeOnSources()
        {
            var ncc = _Prms as INotifyCollectionChanged;

            if (ncc != null)
                ncc.CollectionChanged += ncc_CollectionChanged;
        }

        protected override void UnsubscribeFromSources()
        {
            var ncc = _Prms as INotifyCollectionChanged;

            if (ncc != null)
                ncc.CollectionChanged -= ncc_CollectionChanged;
        }

        protected override TranslucentTransformationBase<TOut, TSource, TSource>.Change ProcessIncrementalChangeEvent(ref ObservableObjectBase<TSource>.FlagsState flags, INCollectionChangedEventArgs collectionEvent, out INCEventArgs message1, out INCEventArgs[] message2)
        { throw new NotImplementedException(); }

        protected override void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        { throw new NotImplementedException(); }

        void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Initial:
                case State.Hidden:
                case State.Clients:
                    return;
            }

            if (!Lock(ref flags))
                goto retry;

            INCollectionChangedEventArgs[] messages = null;
            INCollectionChangedEventArgs message = null;

            try
            {
                var newSourceVersion = SIOrderedNotifyCollectionChanged.CollectionVersion(e);

                if (newSourceVersion == null || state == State.Excepted)
                    ResetAction(ref flags, state, ref message, newSourceVersion);
                else
                {

                    switch (newSourceVersion.Value.IsInRelationTo(_SourceContentVersion))
                    {
                        case VersionRelation.Future:
                            ResetAction(ref flags, state, ref message, newSourceVersion);
                            break;
                        case VersionRelation.Next:
                            {
                                if (e.Action == NotifyCollectionChangedAction.Reset)
                                    ResetAction(ref flags, state, ref message, newSourceVersion);
                                else
                                {
                                    if (state == State.Cached)
                                        ClearExternalCache(ref flags);

                                    _SourceContentVersion = newSourceVersion.Value;
                                    _ContentVersion = INCEventArgs.FromNCC<TOut>(_ContentVersion, e, out messages);
                                    SetState(ref flags, State.Visible);
                                }
                            }
                            break;
                        case VersionRelation.Past:
                            break;
                    }
                }
            }
            finally
            { Commit(ref flags); }

            if (message != null)
                SendMessage(ref flags, message);

            if (messages != null)
                foreach (var m in messages)
                    SendMessage(ref flags, m);
        }

        private void ResetAction(ref FlagsState flags, TranslucentTransformationBase<TOut, TSource, TSource>.State state, ref INCollectionChangedEventArgs message, VersionNumber? newSourceVersion)
        {
            if (state == State.Cached)
                ClearExternalCache(ref flags);

            _SourceContentVersion = newSourceVersion.GetValueOrDefault();
            message = INCEventArgs.CollectionReset(AdvanceContentVersion());
            SetState(ref flags, State.Hidden);
        }
    }

    /// <summary>
    /// A transformation object that transforms an untyped <see cref="IVersionedEnumerable"/> with <see cref="INotifyCollectionChanged"/>
    /// to an untyped <see cref="IVersionedEnumerable"/> with <see cref="INotifyChanged"/> (internal change notification mechanism)
    /// </summary>
    sealed class VersionedPatch : VersionedPatchBase<object, IVersionedEnumerable>
    {
        public static VersionedPatch Create(IVersionedEnumerable source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<VersionedPatch, IVersionedEnumerable>(source);
        }      

        #region IVersionedEnumerable Members

        protected override IEnumerator<object> GetEnumeratorDirect()
        { return ObjectEnumerator(_Prms.GetEnumerator()); }

        static IEnumerator<object> ObjectEnumerator(IEnumerator enm)
        {
            try
            {
                while (enm.MoveNext())
                    yield return enm.Current;
            }
            finally
            {
                SIDisposable.Dispose(enm);
            }
        }

        protected override Tuple<bool, IEnumerator<object>> GetEnumeratorFromBuffer()
        {
            var senm = _Prms.GetEnumerator();

            return Tuple.Create(senm.ContentVersion == _SourceContentVersion, ObjectEnumerator(senm));
        }

        #endregion
    }

    /// <summary>
    /// A transformation object that transforms an <see cref="IVersionedEnumerable{TType}"/> with <see cref="INotifyCollectionChanged"/>
    /// to an <see cref="IVersionedEnumerable{TType}"/> with <see cref="INotifyChanged"/> (internal change notification mechanism)
    /// </summary>
    /// <typeparam name="TType">Type of the sequence elements</typeparam>
    sealed class VersionedPatch<TType> : VersionedPatchBase<TType, IVersionedEnumerable<TType>>
    {
        public static VersionedPatch<TType> Create(IVersionedEnumerable<TType> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<VersionedPatch<TType>, IVersionedEnumerable<TType>>(source);
        }

        #region IVersionedEnumerable<TType> Members

        protected override IEnumerator<TType> GetEnumeratorDirect()
        { return _Prms.GetEnumerator(); }

        protected override Tuple<bool, IEnumerator<TType>> GetEnumeratorFromBuffer()
        {
            var senm = _Prms.GetEnumerator();
            return Tuple.Create(senm.ContentVersion == _SourceContentVersion, (IEnumerator<TType>)senm);
        }

        #endregion
    }


    sealed class VersionedPatchAdapterClass : ICollectionAdapter
    {
        #region ICollectionAdapter Members

        public IVersionedEnumerable AdaptCollection(object collection)
        {
            return
                VersionedPatch.Create((IVersionedEnumerable)collection);
        }

        #endregion

        internal static readonly VersionedPatchAdapterClass _Instance = new VersionedPatchAdapterClass();
    }


    sealed class VersionedPatchAdapterClass<TElement> : ICollectionAdapter
    {
        #region ICollectionAdapter Members

        public IVersionedEnumerable AdaptCollection(object collection)
        {
            return
                VersionedPatch<TElement>.Create((IVersionedEnumerable<TElement>)collection);
        }

        #endregion
    }

}
