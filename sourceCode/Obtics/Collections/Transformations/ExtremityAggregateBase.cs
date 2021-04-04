using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Obtics.Values;


namespace Obtics.Collections.Transformations
{
    internal abstract class ExtremityAggregateBase<TIn, TOut, TPrms> : PredictingAggregateBase<TOut, TPrms>
    {
        #region Bitflags

        const Int32 HaveBufferMask = 1 << PredictingAggregateBase<TOut, TPrms>.BitFlagIndexEnd;

        protected new const Int32 BitFlagIndexEnd = PredictingAggregateBase<TOut, TPrms>.BitFlagIndexEnd + 1;

        #endregion

        protected bool GetHaveBuffer(ref FlagsState flags)
        { return flags.GetBitFlag(HaveBufferMask); }

        protected bool SetHaveBuffer(ref FlagsState flags, bool value)
        { return flags.SetBitFlag(HaveBufferMask, value); }

        protected internal abstract IInternalEnumerable<TIn> Source { get; }

        protected sealed override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected sealed override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        protected TIn _Buffer;

        protected sealed override void ClearBuffer(ref FlagsState flags)
        { 
            _Buffer = default(TIn);
            SetHaveBuffer(ref flags, false);
        }

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using (var sourceEnumerator = Source.GetEnumerator())
            {
                var version = sourceEnumerator.ContentVersion;

                if (SetHaveBuffer(ref flags, sourceEnumerator.MoveNext()))
                    _Buffer = sourceEnumerator.Current;

                while (sourceEnumerator.MoveNext())
                {
                    var next = sourceEnumerator.Current;
                    if(CompareExtremes(_Buffer, next) < 0)
                        _Buffer = next;
                }

                return version;
            }
        }

        protected abstract int CompareExtremes(TIn first, TIn second);

        #region INotifyCollectionChanged Members

        protected override Change ProcessSourceCollectionChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args)
        {
            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TIn>)args;
                        var item = addArgs.Item;

                        if (!GetHaveBuffer(ref flags))
                        {
                            SetHaveBuffer(ref flags, true);
                            _Buffer = item;
                            return Change.Controled;
                        }
                        if (CompareExtremes(item, _Buffer) > 0)
                        {
                            _Buffer = item;
                            return Change.Controled;
                        }
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TIn>)args;

                        if (!GetHaveBuffer(ref flags) || CompareExtremes(removeArgs.Item, _Buffer) >= 0)
                            return Change.Destructive;
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TIn>)args;
                        var newItem = replaceArgs.NewItem;

                        if(!GetHaveBuffer(ref flags))
                            return Change.Destructive;

                        var c = CompareExtremes(newItem, _Buffer);

                        if (c > 0)
                        {
                            _Buffer = newItem;
                            return Change.Controled;
                        }
                        else if (
                            c < 0 
                            && CompareExtremes(replaceArgs.OldItem, _Buffer) >= 0
                        )
                            return Change.Destructive;
                        
                    }
                    break;
                case INCEventArgsTypes.CollectionReset:
                    return Change.Destructive;
                default:
                    throw new InvalidOperationException("Unknown event type.");
            }

            return Change.None;
        }

        #endregion
    }
}
