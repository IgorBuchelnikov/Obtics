using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics;
using Obtics.Values;

namespace ObticsUnitTest.Helpers
{
    internal abstract class CollectionDeterministicEventRegistrationRunnerForAggregate<TSource,TResult> : CollectionDeterministicEventRegistrationRunner<TSource>
    {
        protected override void RemoveNPCClient(object npcClient)
        {
            ((ValueProviderClientNPC<TResult>)npcClient).Source = null;
        }

        protected override object AddNPCClient(object cvt)
        {
            var cac = new ValueProviderClientNPC<TResult>();
            cac.Source = (IValueProvider<TResult>)cvt;
            return cac;
        }

        protected sealed override void RemoveNCCClient(object nccClient)
        {}

        protected sealed override object AddNCCClient(object cvt)
        { return null; }

        protected sealed override object CreatePipeline(FrameIEnumerableNPCNCC<TSource> source)
        { return CreateAggregate(source); }

        protected abstract IValueProvider<TResult> CreateAggregate(FrameIEnumerableNPCNCC<TSource> source);
    }
}
