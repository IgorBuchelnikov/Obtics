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

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void TakeTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2 },
                    ObservableEnumerable.Take(new int[] { 1, 2, 3, 4 }, 2)
                ),
                "TakeTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Take(seq, 2),
                ObservableEnumerable.Take(seq, 2),
                "TakeTest1(b)"
            );
        }

        [TestMethod]
        public void TakeTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2 },
                    ObservableEnumerable.Take(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(2))
                ),
                "TakeTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Take(seq, ValueProvider.Static(2)),
                ObservableEnumerable.Take(seq, ValueProvider.Static(2)),
                "TakeTest2(b)"
            );
        }
    }
}
