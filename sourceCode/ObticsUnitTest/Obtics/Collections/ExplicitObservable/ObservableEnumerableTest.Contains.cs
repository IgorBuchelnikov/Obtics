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
        public void ContainsTest1()
        {
            Assert.IsTrue(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, 2).Value,
                "ContainsTest1(a)"
            );

            Assert.IsFalse(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, 7).Value,
                "ContainsTest1(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Contains(seq, 3),
                ObservableEnumerable.Contains(seq, 3),
                "ContainsTest1(c)"
            );
        }

        [TestMethod]
        public void ContainsTest2()
        {
            Assert.IsTrue(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, 2, ObticsEqualityComparer<int>.Default).Value,
                "ContainsTest2(a)"
            );

            Assert.IsFalse(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, 7, ObticsEqualityComparer<int>.Default).Value,
                "ContainsTest2(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.Contains(seq, 3, c),
                ObservableEnumerable.Contains(seq, 3, c),
                "ContainsTest2(c)"
            );
        }

        [TestMethod]
        public void ContainsTest3()
        {
            Assert.IsTrue(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(2)).Value,
                "ContainsTest3(a)"
            );

            Assert.IsFalse(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(7)).Value,
                "ContainsTest3(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Contains(seq, ValueProvider.Static(3)),
                ObservableEnumerable.Contains(seq, ValueProvider.Static(3)),
                "ContainsTest3(c)"
            );
        }

        [TestMethod]
        public void ContainsTest4()
        {
            Assert.IsTrue(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(2), ObticsEqualityComparer<int>.Default).Value,
                "ContainsTest4(a)"
            );

            Assert.IsFalse(
                ObservableEnumerable.Contains(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(7), ObticsEqualityComparer<int>.Default).Value,
                "ContainsTest4(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.Contains(seq, ValueProvider.Static(3), c),
                ObservableEnumerable.Contains(seq, ValueProvider.Static(3), c),
                "ContainsTest4(c)"
            );
        }

    }
}
