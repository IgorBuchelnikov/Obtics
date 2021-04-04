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
        public void SingleTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Single(new int[] { 1 }).Value,
                1,
                "SingleTest1(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Single(new int[] { }).Value; }
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Single(new int[] { 1, 2, 3, 4 }).Value; }
            );

            var seq = new int[] { 1 };

            Assert.AreEqual(
                ObservableEnumerable.Single(seq),
                ObservableEnumerable.Single(seq),
                "SingleTest1(c)"
            );
        }

        [TestMethod]
        public void SingleTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Single(new int[] { 1, 2, 3, 4 }, i => i % 3 == 0).Value,
                3,
                "SingleTest2(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Single(new int[] { 1, 2, 3, 4 }, i => i % 17 == 0).Value;}
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 3 == 0);

            Assert.AreEqual(
                ObservableEnumerable.Single(seq, f),
                ObservableEnumerable.Single(seq, f),
                "SingleTest2(c)"
            );
        }

        [TestMethod]
        public void SingleTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Single(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 3 == 0)).Value,
                3,
                "SingleTest3(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Single(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 17 == 0)).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 3 == 0));

            Assert.AreEqual(
                ObservableEnumerable.Single(seq, f),
                ObservableEnumerable.Single(seq, f),
                "SingleTest3(c)"
            );
        }
    }
}
