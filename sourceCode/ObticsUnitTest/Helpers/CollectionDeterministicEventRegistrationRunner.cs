using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif



namespace ObticsUnitTest.Helpers
{
    internal abstract class CollectionDeterministicEventRegistrationRunner<TSource>
    {
        public virtual void Run()
        {
            string prefix = Prefix;
            var source = new FrameIEnumerableNPCNCC<TSource>();

            object cvt = CreatePipeline(source);

            var npcClient = AddNPCClient(cvt);

            if (npcClient != null)
            {
                Assert.IsTrue(source.CollectionChangedClientsCount != 0 || source.PropertyChangedClientsCount != 0, prefix + "(1) After adding and resetting NPC client no event registrations at source.");

                RemoveNPCClient(npcClient);

                Assert.IsTrue(source.CollectionChangedClientsCount == 0 && source.PropertyChangedClientsCount == 0, prefix + "(2) After removing a single NPC client some event registrations linger.");
            }

            var nccClient = AddNCCClient(cvt);

            if (nccClient != null)
            {
                Assert.IsTrue(source.CollectionChangedClientsCount != 0 || source.PropertyChangedClientsCount != 0, prefix + "(3) After adding and resetting NCC client no event registrations at source.");

                RemoveNCCClient(nccClient);

                Assert.IsTrue(source.CollectionChangedClientsCount == 0 && source.PropertyChangedClientsCount == 0, prefix + "(4) After removing a single NCC client some event registrations linger.");
            }

            var npcClient1 = AddNPCClient(cvt);
            var npcClient2 = AddNPCClient(cvt);
            var nccClient1 = AddNCCClient(cvt);
            var nccClient2 = AddNCCClient(cvt);

            if (npcClient1 != null || npcClient2 != null || nccClient1 != null || nccClient2 != null)
                Assert.IsTrue(source.CollectionChangedClientsCount != 0 || source.PropertyChangedClientsCount != 0, prefix + "(5) After adding and resetting multiple NPC and NCC clients no event registrations at source.");

            if( nccClient1 != null )
                RemoveNCCClient(nccClient1);

            if( npcClient1 != null )
                RemoveNPCClient(npcClient1);

            if (npcClient2 != null || nccClient2 != null)
                Assert.IsTrue(source.CollectionChangedClientsCount != 0 || source.PropertyChangedClientsCount != 0, prefix + "(6) After removing an NPC and NCC clients other clients remain but there are no event registrations at source.");

            if( nccClient2 != null )
                RemoveNCCClient(nccClient2);

            if( npcClient2 != null )
                RemoveNPCClient(npcClient2);

            Assert.IsTrue(source.CollectionChangedClientsCount == 0 && source.PropertyChangedClientsCount == 0, prefix + "(7) After removing all NCC and NPC clients some event registrations linger.");
        }

        protected abstract void RemoveNPCClient(object npcClient);

        protected abstract object AddNPCClient(object cvt);

        protected abstract void RemoveNCCClient(object nccClient);

        protected abstract object AddNCCClient(object cvt);

        protected abstract object CreatePipeline(FrameIEnumerableNPCNCC<TSource> source);

        public abstract string Prefix { get; }
    }
}
