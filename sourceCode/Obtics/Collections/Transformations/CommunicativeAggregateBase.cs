using System.Collections.Generic;
using System.Collections.Specialized;
using Obtics.Values;
using System;


namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Base class for aggregates where all elements of the source sequence contribute to the result value in a
    /// way that is independent of the order in which they occur. The contribution of each individual element can
    /// also be substracted of the result.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TAcc"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TPrms"></typeparam>
    internal abstract class CommunicativeAggregateBase<TIn, TOut, TAcc, TPrms> : PredictingAggregateBase<TOut, TPrms>
    {
        protected internal abstract IInternalEnumerable<TIn> Source
        { get; }

        protected override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        protected TAcc _Acc;

        protected virtual TAcc GetInititialTotalValue()
        { return default(TAcc); }

        /// <summary>
        /// Add the contribution of this item to the result value.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        protected abstract Change AddItem(TIn item);

        /// <summary>
        /// Remove the contribution of this item from the result value.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        protected abstract Change RemoveItem(TIn item);

        protected override VersionNumber InitializeBuffer(ref FlagsState flags)
        {
            using (var sourceEnumerator = Source.GetEnumerator())
            {
                _Acc = GetInititialTotalValue();

                while (sourceEnumerator.MoveNext())
                    AddItem(sourceEnumerator.Current);

                return sourceEnumerator.ContentVersion;
            }
        }

        protected override void ClearBuffer(ref FlagsState flags)
        { _Acc = default(TAcc); }

        #region INotifyCollectionChanged Members

        protected override Change ProcessSourceCollectionChangedNotification(ref FlagsState flags, object sender, INCollectionChangedEventArgs args)
        {
            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    return AddItem(((INCollectionAddEventArgs<TIn>)args).Item);
                case INCEventArgsTypes.CollectionRemove:
                    return RemoveItem(((INCollectionRemoveEventArgs<TIn>)args).Item);
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TIn>)args;
                        var c = AddItem(replaceArgs.NewItem);
                        if (c != Change.Destructive)
                        {
                            var c2 = RemoveItem(replaceArgs.OldItem);
                            if (c2 != Change.None)
                                c = c2;
                        }
                        return c;
                    }
                case INCEventArgsTypes.CollectionReset:
                    return Change.Destructive;
                default:
                    throw new InvalidOperationException("Unknown event type.");
            }
        }

        #endregion
    }
}
