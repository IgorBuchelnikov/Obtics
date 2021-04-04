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
using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void LastTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Last(new int[] { 1, 2, 3, 4 }).Value,
                4,
                "LastTest1(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Last(new int[] { }).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Last(seq),
                ObservableEnumerable.Last(seq),
                "LastTest1(c)"
            );
        }

        [TestMethod]
        public void LastTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Last(new int[] { 1, 2, 3, 4 }, i => i % 3 == 0).Value,
                3,
                "LastTest2(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Last(new int[] { 1, 2, 3, 4 }, i => i % 17 == 0).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 3 == 0);

            Assert.AreEqual(
                ObservableEnumerable.Last(seq, f),
                ObservableEnumerable.Last(seq, f),
                "LastTest2(c)"
            );
        }

        [TestMethod]
        public void LastTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Last(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 3 == 0)).Value,
                3,
                "LastTest3(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Last(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 17 == 0)).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 3 == 0));

            Assert.AreEqual(
                ObservableEnumerable.Last(seq, f),
                ObservableEnumerable.Last(seq, f),
                "LastTest3(c)"
            );
        }
    }
}
