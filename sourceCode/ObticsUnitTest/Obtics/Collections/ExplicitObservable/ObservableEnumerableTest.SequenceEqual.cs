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
        public void SequenceEqualTest1()
        {
            Assert.IsTrue(
                ObservableEnumerable.SequenceEqual(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }).Value,
                "SequenceEqualTest1(a)"
            );

            Assert.IsFalse(
                ObservableEnumerable.SequenceEqual(new int[] { 1, 2, 3 }, new int[] { 1, 4, 3 }).Value,
                "SequenceEqualTest1(b)"
            );

            var seq1 = new int[] { 1, 2, 3, 4 };
            var seq2 = new int[] { 1, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.SequenceEqual(seq1, seq2),
                ObservableEnumerable.SequenceEqual(seq1, seq2),
                "SequenceEqualTest1(c)"
            );
        }

        [TestMethod]
        public void SequenceEqualTest2()
        {
            Assert.IsTrue(
                ObservableEnumerable.SequenceEqual(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }, ObticsEqualityComparer<int>.Default).Value,
                "SequenceEqualTest2(a)"
            );

            Assert.IsFalse(
                ObservableEnumerable.SequenceEqual(new int[] { 1, 2, 3 }, new int[] { 1, 4, 3 }, ObticsEqualityComparer<int>.Default).Value,
                "SequenceEqualTest2(b)"
            );

            var seq1 = new int[] { 1, 2, 3, 4 };
            var seq2 = new int[] { 1, 3, 4 };
            var comp = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.SequenceEqual(seq1, seq2, comp),
                ObservableEnumerable.SequenceEqual(seq1, seq2, comp),
                "SequenceEqualTest2(c)"
            );
        }
    }
}
