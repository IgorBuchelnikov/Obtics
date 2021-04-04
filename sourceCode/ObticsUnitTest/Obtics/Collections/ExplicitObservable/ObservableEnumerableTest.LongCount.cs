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
        public void LongCountTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.LongCount(new int[] { 1, 2, 3, 4 }).Value,
                4L,
                "LongCountTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.LongCount(seq),
                ObservableEnumerable.LongCount(seq),
                "LongCountTest1(b)"
            );
        }

        [TestMethod]
        public void LongCountTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.LongCount(new int[] { 1, 2, 3, 4 }, i => i % 2 == 0).Value,
                2L,
                "LongCountTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 2 == 0);

            Assert.AreEqual(
                ObservableEnumerable.LongCount(seq, f),
                ObservableEnumerable.LongCount(seq, f),
                "LongCountTest2(b)"
            );
        }

        [TestMethod]
        public void LongCountTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.LongCount(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2 == 0)).Value,
                2L,
                "LongCountTest3(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 2 == 0));

            Assert.AreEqual(
                ObservableEnumerable.LongCount(seq, f),
                ObservableEnumerable.LongCount(seq, f),
                "LongCountTest3(b)"
            );
        }
    }
}
