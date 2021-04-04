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
        public void CountTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Count(new int[] { 1, 2, 3, 4 }).Value,
                4,
                "CountTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Count(seq),
                ObservableEnumerable.Count(seq),
                "CountTest1(b)"
            );
        }

        [TestMethod]
        public void CountTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Count(new int[] { 1, 2, 3, 4 }, i => i % 2 == 0).Value,
                2,
                "CountTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 2 == 0);

            Assert.AreEqual(
                ObservableEnumerable.Count(seq, f),
                ObservableEnumerable.Count(seq, f),
                "CountTest2(b)"
            );
        }

        [TestMethod]
        public void CountTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Count(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2 == 0)).Value,
                2,
                "CountTest3(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 2 == 0));

            Assert.AreEqual(
                ObservableEnumerable.Count(seq, f),
                ObservableEnumerable.Count(seq, f),
                "CountTest3(b)"
            );
        }
    }
}
