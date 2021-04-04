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
using Obtics;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void DistinctTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Distinct(new int[] { 2, 3, 2, 3, 4, 3, 4 })
                ),
                "DistinctTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Distinct(seq),
                ObservableEnumerable.Distinct(seq),
                "DistinctTest1(b)"
            );
        }

        [TestMethod]
        public void DistinctTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Distinct(new int[] { 2, 3, 2, 3, 4, 3, 4 }, ObticsEqualityComparer<int>.Default)
                ),
                "DistinctTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var comp = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.Distinct(seq, comp),
                ObservableEnumerable.Distinct(seq, comp),
                "DistinctTest2(b)"
            );
        }
    }
}
