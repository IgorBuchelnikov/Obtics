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
        public void ExceptTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 6 },
                    ObservableEnumerable.Except(
                        new int[] { 2, 1, 4, 6 },
                        new int[] { 2, 3, 4, 5 }
                    )
                ),
                "ExceptTest1(a)"
            );

            var seq1 = new int[] { 2, 1, 4, 6 };
            var seq2 = new int[] { 2, 3, 4, 5 };

            Assert.AreEqual(
                ObservableEnumerable.Except(seq1, seq2),
                ObservableEnumerable.Except(seq1, seq2),
                "ExceptTest1(b)"
            );
        }

        [TestMethod()]
        public void ExceptTest()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 6 },
                    ObservableEnumerable.Except(
                        new int[] { 2, 1, 4, 6 },
                        new int[] { 2, 3, 4, 5 },
                        ObticsEqualityComparer<int>.Default
                    )
                ),
                "ExceptTest(a)"
            );

            var seq1 = new int[] { 2, 1, 4, 6 };
            var seq2 = new int[] { 2, 3, 4, 5 };
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.Except(seq1, seq2, c),
                ObservableEnumerable.Except(seq1, seq2, c),
                "ExceptTest(b)"
            );

        }
    }
}
