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
        public void SkipTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 4 },
                    ObservableEnumerable.Skip(new int[] { 1, 2, 3, 4 }, 2)
                ),
                "SkipTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Skip(seq, 2),
                ObservableEnumerable.Skip(seq, 2),
                "SkipTest1(b)"
            );
        }

        [TestMethod]
        public void SkipTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 4 },
                    ObservableEnumerable.Skip(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(2))
                ),
                "SkipTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Skip(seq, ValueProvider.Static(2)),
                ObservableEnumerable.Skip(seq, ValueProvider.Static(2)),
                "SkipTest2(b)"
            );
        }
    }
}
