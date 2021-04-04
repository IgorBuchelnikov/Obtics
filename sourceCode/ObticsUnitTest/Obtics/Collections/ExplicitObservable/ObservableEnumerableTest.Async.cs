using Obtics.Collections;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using System.Collections.Generic;
using System;
using Obtics.Values;
using System.Collections;
using System.Linq;
using Obtics.Async;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void AsyncTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.Async(new int[] { 1, 2, 3, 4 })
                ),
                "AsyncTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Async(seq),
                ObservableEnumerable.Async(seq),
                "AsyncTest1(b)"
            );

        }

        class DummyDispatcher : IWorkQueue
        {
            #region IWorkQueue Members

            public void QueueWorkItem(System.Threading.WaitCallback callback, object prm)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [TestMethod]
        public void AsyncTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.Async(new int[] { 1, 2, 3, 4 }, new DummyDispatcher())
                ),
                "AsyncTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var dd = new DummyDispatcher();

            Assert.AreEqual(
                ObservableEnumerable.Async(seq, dd),
                ObservableEnumerable.Async(seq, dd),
                "AsyncTest2(b)"
            );

        }
    }
}
