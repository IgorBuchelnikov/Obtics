using System;
using SL = System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Obtics.Values;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;

namespace Obtics.Collections.Transformations
{
    internal sealed class NotifyVpcTransformation<TType, TVP> : OpaqueTransformationBase<Tuple<TType, TVP>, IInternalEnumerable<TType>, Tuple<IInternalEnumerable<TType>, Func<TType, IValueProvider<TVP>>>>
    {
        public static NotifyVpcTransformation<TType, TVP> Create(IEnumerable<TType> s, Func<TType, IValueProvider<TVP>> converter)
        {
            var source = s.Patched();

            if (source == null || converter == null)
                return null;

            return Carrousel.Get<NotifyVpcTransformation<TType, TVP>, IInternalEnumerable<TType>, Func<TType, IValueProvider<TVP>>>(source, converter);
        }

        public override IInternalEnumerable<Tuple<TType, TVP>> UnorderedForm
        { get { return UnorderedNotifyVpcTransformation<TType, TVP>.Create(_Source, _Prms.Second); } }

        internal IInternalEnumerable<TType> _Source
        { get { return _Prms.First; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        /// <summary>
        /// publication of Converter specially for DynamicSortTransformation so that it doesn't need
        /// tp maintain it's own copy.
        /// </summary>
        /// <remarks>
        /// Externally accessed but just returning a truly static value.. no lock
        /// </remarks>
        internal Func<TType, IValueProvider<TVP>> Converter
        { get { return _Prms.Second; } }

        Tuple<TType, TVP> Convert(TType item)
        { return Tuple.Create(item, _Prms.Second(item).Value); }

        class Notifier : Skiplist<Notifier>.Node, IDisposable, IReceiveChangeNotification
        {
            internal Notifier(TType inItem, IValueProvider<TVP> valueProvider, NotifyVpcTransformation<TType, TVP> owner)
            {
                valueProvider = valueProvider.Patched();

                _Owner = owner;
                _ValueProvider = valueProvider;

                var npc = valueProvider as INotifyChanged;

                if (npc != null)
                    npc.SubscribeINC(this);

                _Item = Tuple.Create(inItem, valueProvider.GetValueOrDefault());
            }

            internal Tuple<TType, TVP> _Item;

            internal Tuple<TType, TVP> Item
            { get { return _Item; } }

            readonly NotifyVpcTransformation<TType, TVP> _Owner;
            internal IValueProvider<TVP> _ValueProvider;

            #region IDisposable Members

            public void Dispose()
            {
                //only accessed internaly.. no locking

                var npc = _ValueProvider as INotifyChanged;

                if (npc != null)
                    npc.UnsubscribeINC(this);

                _ValueProvider = null;

                GC.SuppressFinalize(this);
            }

            #endregion

            #region IReceiveChangeNotification Members

            public void NotifyChanged(object sender, INCEventArgs changeArgs)
            {
                if (changeArgs.IsValueEvent || changeArgs.IsExeptionEvent)
                    DelayedActionRegistry.Register(
                        () => _Owner.ProcessNotification(this, changeArgs)                        
                    );
            }

            #endregion
        }

        Notifier CreateNotifier(TType item, NotifyVpcTransformation<TType, TVP> owner)
        { return new Notifier(item, _Prms.Second(item), owner); }

        Skiplist<Notifier> _Buffer;

        void ProcessNotification(Notifier notifier, INCEventArgs eventArgs)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Initial:
                case State.Clients:
                case State.Hidden:
                    return;
            }

            if (!Lock(ref flags))
                goto retry;

            INCEventArgs message = null;

            try
            {
                if (notifier._ValueProvider == null)
                    return;

                var buffer = _Buffer;

                var nfIndex = buffer.IndexOf(notifier);

                switch (state)
                {
                    case State.Excepted:
                        switch(eventArgs.Type)
                        {
                            case INCEventArgsTypes.ValueChanged :
                                message = INCEventArgs.CollectionReset(AdvanceContentVersion());
                                SetState(ref flags, State.Hidden);
                                break;

                            case INCEventArgsTypes.Exception :
                                message = eventArgs;
                                break;
                        }

                        break;

                    case State.Snapshot:
                        TakeSnapshot();
                        SetState(ref flags, State.Visible);
                        goto case State.Visible;

                    case State.Cached:
                        ClearExternalCache(ref flags);
                        SetState(ref flags, State.Visible);
                        goto case State.Visible;

                    case State.Visible:
                        var oldItem = notifier._Item;
                        notifier._Item = Tuple.Create(oldItem.First, notifier._ValueProvider.GetValueOrDefault());

                        switch(eventArgs.Type)
                        {
                            case INCEventArgsTypes.ValueChanged :
                                message =
                                    INCEventArgs.CollectionReplace(
                                        AdvanceContentVersion(),
                                        nfIndex,
                                        notifier._Item,
                                        oldItem
                                    )
                                ;

                                break;

                            case INCEventArgsTypes.Exception :
                                AdvanceContentVersion();
                                message = eventArgs;
                                break;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                SetState(ref flags, State.Visible);
                message = INCEventArgs.Exception(ex);
            }
            finally
            { Commit(ref flags); }

            if (message != null)
                SendMessage(ref flags, message);
        }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using (var sourceEnumerator = _Source.GetEnumerator())
            {
                var buffer = new Skiplist<Notifier>();

                while (sourceEnumerator.MoveNext())
                    buffer.Add(CreateNotifier(sourceEnumerator.Current, this));

                _Buffer = buffer;

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override void ClearBuffer(ref FlagsState flags)
        {
            foreach (Notifier n in _Buffer)
                n.Dispose();

            _Buffer = null;
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message, out INCEventArgs[] messages)
        {
            var buffer = _Buffer;
            messages = null;
            var res = Change.Controled;
            var ccn = AdvanceContentVersion();

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;
                        var ix = addArgs.Index;
                        var notifier = CreateNotifier(addArgs.Item, this);

                        buffer.Insert(ix, notifier);

                        message =
                            INCEventArgs.CollectionAdd(
                                ccn,
                                ix,
                                notifier.Item
                            );
                    }
                    break;

                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;
                        var ix = removeArgs.Index;
                        var notifier = buffer[ix];

                        message =
                            INCEventArgs.CollectionRemove(
                                ccn,
                                ix,
                                notifier.Item
                            );

                        buffer.RemoveAt(ix);
                        notifier.Dispose();
                    }
                    break;

                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;
                        var ix = replaceArgs.Index;
                        var oldNotifier = buffer[ix];
                        var newNotifier = CreateNotifier(replaceArgs.NewItem, this);

                        buffer[ix] = newNotifier;

                        message =
                            INCEventArgs.CollectionReplace(
                                ccn,
                                ix,
                                newNotifier.Item,
                                oldNotifier.Item
                            );

                        oldNotifier.Dispose();
                    }
                    break;

                case INCEventArgsTypes.CollectionReset:
                    res = Change.Destructive;
                    message = INCEventArgs.CollectionReset(ccn);
                    break;

                default:
                    throw new Exception("Unexpected event type;");
            }

            return res;
        }

        IEnumerable<Tuple<TType, TVP>> BuildBufferEnumerable()
        { return SL.Enumerable.Select(_Buffer, n => n.Item); }

        protected override IEnumerator<Tuple<TType, TVP>> GetEnumeratorDirect()
        { return SL.Enumerable.ToList(SL.Enumerable.Select(_Source, (Func<TType, Tuple<TType, TVP>>)Convert)).GetEnumerator(); }

        protected override IEnumerator<Tuple<TType, TVP>> GetEnumeratorFromBuffer()
        { return BuildBufferEnumerable().GetEnumerator(); }

        protected override IEnumerable<Tuple<TType, TVP>> GetSnapshotEnumerable()
        { return SL.Enumerable.ToArray(BuildBufferEnumerable()); }
    }
}
