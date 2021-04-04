using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Obtics.Values;
using Obtics.Values.Transformations;


namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Base class for 'one-pass' aggragates. When the aggregate result needs to be recalculated
    /// the entire sequence needs to be queried.
    /// </summary>
    /// <typeparam name="TOut">Type of the result value.</typeparam>
    /// <typeparam name="TPrms">Type of the 'paramters' struct.</typeparam>
    internal abstract class AggregateBase<TOut, TPrms> : CachedTransformationBase<TOut, TPrms>
    {
        TOut _Buffer ;

        protected override void SourceCollectionEvent(object sender, INCollectionChangedEventArgs collectionEvent)
        { base.SourceChangeEvent(sender); }

        protected override void SourceChangeEvent(object sender)
        {}

        protected override void InitializeBuffer()
        { _Buffer = GetValueDirect(); }

        protected override void ClearBuffer()
        { _Buffer = default(TOut); }

        protected override TOut GetValueFromBuffer()
        { return _Buffer; }
    }
}
