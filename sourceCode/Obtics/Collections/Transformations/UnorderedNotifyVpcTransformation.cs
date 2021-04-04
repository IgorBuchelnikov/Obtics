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
    internal sealed class UnorderedNotifyVpcTransformation<TType, TVP> : OpaqueTransformationBase<Tuple<TType, TVP>, IInternalEnumerable<TType>, Tuple<IInternalEnumerable<TType>, Func<TType, IValueProvider<TVP>>>>
    {
        public static UnorderedNotifyVpcTransformation<TType, TVP> Create(IEnumerable<TType> s, Func<TType, IValueProvider<TVP>> converter)
        {
            var source = s.PatchedUnordered();

            if (source == null || converter == null)
                return null;

            return Carrousel.Get<UnorderedNotifyVpcTransformation<TType, TVP>, IInternalEnumerable<TType>, Func<TType, IValueProvider<TVP>>>(source, converter);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        public override bool HasSafeEnumerator
        { get { return true; } }

        internal IInternalEnumerable<TType> _Source
        { get { return _Prms.First; } }

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
        { return Tuple.Create(item, _Prms.Second(item).GetValueOrDefault()); }

        class Notifier : IDisposable, IReceiveChangeNotification
        {
            internal Notifier(TType inItem, IValueProvider<TVP> valueProvider, UnorderedNotifyVpcTransformation<TType,TVP> owner)
            {
                valueProvider = valueProvider.Patched();

                _Owner = owner;
                _ValueProvider = valueProvider;
                _Count = 1;

                var npc = valueProvider as INotifyChanged;

                if (npc != null)
                    npc.SubscribeINC(this);

                _Item = Tuple.Create(inItem, valueProvider.GetValueOrDefault());
            }

            internal Tuple<TType, TVP> _Item;

            readonly UnorderedNotifyVpcTransformation<TType,TVP> _Owner;
            internal IValueProvider<TVP> _ValueProvider;
            internal int _Count;

            #region IDisposable Members

            public void Dispose()
            {
                //only accessed internaly.. no locking

                var npc = _ValueProvider as INotifyChanged;

                if (npc != null)
                    npc.UnsubscribeINC(this);

                _ValueProvider = null;
                _Count = 0;

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

        Notifier CreateNotifier(TType item, UnorderedNotifyVpcTransformation<TType, TVP> owner)
        { return new Notifier(item, _Prms.Second(item), owner); }

        Dictionary<Tuple<TType>, Notifier> _Buffer;

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

            INCEventArgs[] messages = null;
            INCEventArgs message = null;

            try
            {
                if (notifier._ValueProvider == null)
                    return;

                var buffer = _Buffer;

                switch (state)
                {
                    case State.Excepted:
                        switch (eventArgs.Type)
                        {
                            case INCEventArgsTypes.ValueChanged:
                                message = INCEventArgs.CollectionReset(AdvanceContentVersion());
                                SetState(ref flags, State.Hidden);
                                break;

                            case INCEventArgsTypes.Exception:
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

                        switch (eventArgs.Type)
                        {
                            case INCEventArgsTypes.ValueChanged:
                                var nfCount = notifier._Count;

                                if (!oldItem.Equals(notifier._Item))
                                    if (nfCount > 1)
                                    {
                                        messages = new INCEventArgs[nfCount];

                                        for (int i = 0; i < nfCount; ++i)
                                            messages[i] =
                                                INCEventArgs.CollectionReplace(
                                                    AdvanceContentVersion(),
                                                    -1,
                                                    notifier._Item,
                                                    oldItem
                                                )
                                            ;
                                    }
                                    else if (nfCount == 1)
                                    {
                                        message =
                                            INCEventArgs.CollectionReplace(
                                                AdvanceContentVersion(),
                                                -1,
                                                notifier._Item,
                                                oldItem
                                            )
                                        ;
                                    }

                                break;

                            case INCEventArgsTypes.Exception:
                                AdvanceContentVersion();
                                message = eventArgs;
                                break;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                //should not be possible to corrupt buffer
                //lets not waste it.
                SetState(ref flags, State.Visible);
                message = INCEventArgs.Exception(ex);                
            }
            finally
            { Commit(ref flags); }

            if (message != null)
                SendMessage(ref flags, message);

            if (messages != null)
                foreach (var msg in messages)
                    SendMessage(ref flags, msg);
        }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            var buffer = _Buffer;

            using (var sourceEnumerator = _Source.GetEnumerator())
            {
                buffer = new Dictionary<Tuple<TType>, Notifier>();

                while (sourceEnumerator.MoveNext())
                {
                    var current = sourceEnumerator.Current;
                    var key = Tuple.Create(current);
                    Notifier notifier;

                    if (buffer.TryGetValue(key, out notifier))
                        ++notifier._Count;
                    else
                        buffer.Add(key, CreateNotifier(current, this));
                }

                _Buffer = buffer;

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override void ClearBuffer(ref FlagsState flags)
        {
            foreach (var kvp in _Buffer)
                kvp.Value.Dispose();

            _Buffer = null;
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message, out INCEventArgs[] messages)
        {
            messages = null;

            var buffer = _Buffer;
            var ccn = AdvanceContentVersion();
            var res = Change.Controled;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        message =
                            INCEventArgs.CollectionAdd<Tuple<TType, TVP>>(
                                ccn,
                                -1,
                                AddNewItem(
                                    buffer,
                                    ((INCollectionAddEventArgs<TType>)args).Item
                                )
                            );
                    }
                    break;

                case INCEventArgsTypes.CollectionRemove:
                    {
                        message =
                            INCEventArgs.CollectionRemove<Tuple<TType, TVP>>(
                                ccn,
                                -1,
                                RemoveOldItem(
                                    buffer,
                                    ((INCollectionRemoveEventArgs<TType>)args).Item
                                )
                            );
                    }
                    break;

                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;
                        message =
                            INCEventArgs.CollectionReplace<Tuple<TType, TVP>>(
                                ccn,
                                -1,
                                AddNewItem(buffer, replaceArgs.NewItem),
                                RemoveOldItem(buffer, replaceArgs.OldItem)
                            );
                    }
                    break;

                case INCEventArgsTypes.CollectionReset:
                    res = Change.Destructive;
                    message = INCEventArgs.CollectionReset(ccn);
                    break;

                default:
                    throw new Exception("Unexpected event type.");
            }
            
            return res;
        }

        private static Tuple<TType, TVP> RemoveOldItem(Dictionary<Tuple<TType>, UnorderedNotifyVpcTransformation<TType, TVP>.Notifier> buffer, TType item)
        {
            var key = Tuple.Create(item);
            Notifier notifier = buffer[key];

            var resItem = notifier._Item;

            if (--notifier._Count == 0)
            {
                buffer.Remove(key);
                notifier.Dispose();
            }

            return resItem;
        }

        private Tuple<TType, TVP> AddNewItem(Dictionary<Tuple<TType>, UnorderedNotifyVpcTransformation<TType, TVP>.Notifier> buffer, TType item)
        {
            var key = Tuple.Create(item);
            Notifier notifier;

            if (buffer.TryGetValue(key, out notifier))
                ++notifier._Count;
            else
                buffer.Add(key, notifier = CreateNotifier(item, this));

            return notifier._Item;
        }

        IEnumerable<Tuple<TType, TVP>> BuildBufferEnumerable()
        {
            foreach (var kvp in _Buffer)
                for (int i = 0, end = kvp.Value._Count; i < end; ++i)
                    yield return kvp.Value._Item;
        }

        protected override IEnumerator<Tuple<TType, TVP>> GetEnumeratorDirect()
        { return SL.Enumerable.ToList(SL.Enumerable.Select(_Source, (Func<TType, Tuple<TType, TVP>>)Convert)).GetEnumerator(); }

        protected override IEnumerator<Tuple<TType, TVP>> GetEnumeratorFromBuffer()
        { return BuildBufferEnumerable().GetEnumerator(); }

        protected override IEnumerable<Tuple<TType, TVP>> GetSnapshotEnumerable()
        { return SL.Enumerable.ToArray( BuildBufferEnumerable() ); }
    }
}
