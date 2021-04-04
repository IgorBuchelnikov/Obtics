using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics;

namespace ObticsUnitTest.Helpers
{
    internal abstract class CollectionTestSequenceRunnerForTransformation<TSource,TResult> : CollectionTestSequenceRunner<TSource>
    {
        public virtual IEqualityComparer<TResult> ResultComparer
        { get { return ObticsEqualityComparer<TResult>.Default; } }

        public abstract List<TResult> ExpectedResult
        { get; }

        public abstract string Prefix
        { get; }

        public virtual bool VerifyResult(object tail, string description, int count )
        {
            bool ok = ((IEnumerable<TResult>)tail).SequenceEqual(ExpectedResult, ResultComparer);

            Assert.IsTrue(ok, Prefix + " (" + count.ToString() + ") after collection changes " + description + ". The result collection is not correct.");

            return ok;
        }

        public abstract IEnumerable<TResult> CreatePipeline(FrameIEnumerable<TSource> frame);

        protected override bool RunActions(CollectionTestSequenceRunner<TSource>.Action firstAction, CollectionTestSequenceRunner<TSource>.Action secondAction, List<TSource> targetCopy2, string description, int count)
        {
            {
                var frame = new FrameIEnumerableNPCNCC<TSource>();
                frame.FrameBuffer = targetCopy2.GetRange(0, targetCopy2.Count);
                //frame.ContentVersion = 1;

                var cvt = CreatePipeline(frame);

                var client = new CollectionTransformationClientNPC<TResult>();
                client.Source = cvt;

                secondAction(frame);
                firstAction(frame);

                if (!VerifyResult(client.Buffer, "NPCSNCC to NPC " + description, (count - 1) * 2 + 1))
                    return false;
            }


            {
                var frame = new FrameIEnumerableNPCNCC<TSource>();
                frame.FrameBuffer = targetCopy2.GetRange(0, targetCopy2.Count);
                //frame.ContentVersion = 1;

                var cvt = CreatePipeline(frame);

                var client = new CollectionTransformationClientSNCC<TResult>();
                client.Source = cvt;

                secondAction(frame);
                firstAction(frame);

                if (!VerifyResult(client.Buffer, "NPCSNCC to SNCC " + description, (count - 1) * 2 + 2))
                    return false;
            }

            return true;
        }
    }
}
