﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics;

namespace ObticsUnitTest.Helpers
{
    internal abstract class ValueProviderDeterministicEventRegistrationRunnerForSequence<TSource, TResult> : ValueProviderDeterministicEventRegistrationRunner<TSource>
    {
        protected override void RemoveNPCClient(object npcClient)
        {
            ((CollectionTransformationClientNPC<TResult>)npcClient).Source = null;
        }

        protected override object AddNPCClient(object cvt)
        {
            var cac = new CollectionTransformationClientNPC<TResult>();
            cac.Source = (IEnumerable<TResult>)cvt;
            return cac;
        }

        protected override void RemoveNCCClient(object nccClient)
        {
            ((CollectionTransformationClientSNCC<TResult>)nccClient).Source = null;
        }

        protected override object AddNCCClient(object cvt)
        {
            var cac = new CollectionTransformationClientSNCC<TResult>();
            cac.Source = (IEnumerable<TResult>)cvt;
            return cac;
        }

        protected sealed override object CreatePipeline(FrameIValueProviderNPC<TSource> source)
        { return Create(source); }

        protected abstract IEnumerable<TResult> Create(FrameIValueProviderNPC<TSource> source);
    }
}
