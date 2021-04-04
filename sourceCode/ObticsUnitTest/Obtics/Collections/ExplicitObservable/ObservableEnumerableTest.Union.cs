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
        [TestMethod()]
        public void UnionTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 1, 4, 6, 3, 5 },
                    ObservableEnumerable.Union(
                        new int[] { 2, 1, 4, 6 },
                        new int[] { 2, 3, 4, 5 }
                    )
                ),
                "UnionTest1(a)"
            );

            var seq1 = new int[] { 2, 1, 4, 6 };
            var seq2 = new int[] { 2, 3, 4, 5 };

            Assert.AreEqual(
                ObservableEnumerable.Union(seq1, seq2),
                ObservableEnumerable.Union(seq1, seq2),
                "UnionTest1(b)"
            );
        }

        [TestMethod()]
        public void UnionTest()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 1, 4, 6, 3, 5 },
                    ObservableEnumerable.Union(
                        new int[] { 2, 1, 4, 6 },
                        new int[] { 2, 3, 4, 5 },
                        ObticsEqualityComparer<int>.Default
                    )
                ),
                "UnionTest(a)"
            );

            var seq1 = new int[] { 2, 1, 4, 6 };
            var seq2 = new int[] { 2, 3, 4, 5 };
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.Union(seq1, seq2, c),
                ObservableEnumerable.Union(seq1, seq2, c),
                "UnionTest(b)"
            );

        }
    }
}
