using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Values;

namespace ObticsUnitTest.Helpers
{
    internal abstract class CollectionTestSequenceRunnerForAggregate<TSource,TResult> : CollectionTestSequenceRunner<TSource>
    {
        public virtual IEqualityComparer<TResult> ResultComparer
        { get { return ObticsEqualityComparer<TResult>.Default; } }

        public abstract TResult ExpectedResult
        { get; }

        public abstract string Prefix
        { get; }

        public virtual bool VerifyResult(TResult res, string description, int count)
        {
            bool ok = ResultComparer.Equals( res, ExpectedResult );

            Assert.IsTrue(ok, Prefix + " (" + count.ToString() + ") after collection changes " + description + ". The result value is not correct.");

            return ok;
        }

        public abstract IValueProvider<TResult> CreatePipeline(FrameIEnumerable<TSource> frame);

        protected override bool RunActions(CollectionTestSequenceRunner<TSource>.Action firstAction, CollectionTestSequenceRunner<TSource>.Action secondAction, List<TSource> targetCopy2, string description, int count)
        {
            {
                var frame = new FrameIEnumerableNPCNCC<TSource>();
                frame.FrameBuffer = targetCopy2.GetRange(0, targetCopy2.Count);

                var cvt = CreatePipeline(frame);

                var client = new ValueProviderClientNPC<TResult>();
                client.Source = cvt;

                secondAction(frame);
                firstAction(frame);

                if (!VerifyResult(client.Buffer, "NPCSNCC to NPC " + description, count ))
                    return false;
            }

            return true;
        }
    }
}
