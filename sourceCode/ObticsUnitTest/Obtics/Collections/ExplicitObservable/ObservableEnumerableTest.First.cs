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
        public void FirstTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.First(new int[] { 1, 2, 3, 4 }).Value,
                1,
                "FirstTest1(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.First(new int[] { }).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.First(seq),
                ObservableEnumerable.First(seq),
                "FirstTest1(c)"
            );
        }

        [TestMethod]
        public void FirstTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.First(new int[] { 1, 2, 3, 4 }, i => i % 3 == 0).Value,
                3,
                "FirstTest2(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.First(new int[] { 1, 2, 3, 4 }, i => i % 17 == 0).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 3 == 0);

            Assert.AreEqual(
                ObservableEnumerable.First(seq, f),
                ObservableEnumerable.First(seq, f),
                "FirstTest2(c)"
            );
        }

        [TestMethod]
        public void FirstTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.First(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 3 == 0)).Value,
                3,
                "FirstTest3(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.First(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 17 == 0)).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 3 == 0));

            Assert.AreEqual(
                ObservableEnumerable.First(seq, f),
                ObservableEnumerable.First(seq, f),
                "FirstTest3(c)"
            );
        }
    }
}
