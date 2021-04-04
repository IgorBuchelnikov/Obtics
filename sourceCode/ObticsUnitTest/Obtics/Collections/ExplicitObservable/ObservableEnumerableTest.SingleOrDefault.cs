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
        public void SingleOrDefaultTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.SingleOrDefault(new int[] { 1 }).Value,
                1,
                "SingleOrDefaultTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.SingleOrDefault(new int[] { }).Value,
                default(int),
                "SingleOrDefaultTest1(b)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                ()=>
                { var dummy = ObservableEnumerable.SingleOrDefault(new int[] { 1, 2, 3, 4 }).Value; }
            );

            var seq = new int[] { 1 };

            Assert.AreEqual(
                ObservableEnumerable.SingleOrDefault(seq),
                ObservableEnumerable.SingleOrDefault(seq),
                "SingleOrDefaultTest1(c)"
            );
        }

        [TestMethod]
        public void SingleOrDefaultTest2()
        {
            Assert.AreEqual(
                3,
                ObservableEnumerable.SingleOrDefault(new int[] { 1, 2, 3, 4 }, i => i % 3 == 0).Value,
                "SingleOrDefaultTest2(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.SingleOrDefault(new int[] { 1, 2, 3, 4 }, i => i % 2 == 0).Value; }
            );

            Assert.AreEqual(
                default(int),
                ObservableEnumerable.SingleOrDefault(new int[] { 1, 2, 3, 4 }, i => i % 17 == 0).Value,
                "SingleOrDefaultTest2(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 3 == 0);

            Assert.AreEqual(
                ObservableEnumerable.SingleOrDefault(seq, f),
                ObservableEnumerable.SingleOrDefault(seq, f),
                "SingleOrDefaultTest2(c)"
            );
        }

        [TestMethod]
        public void SingleOrDefaultTest3()
        {
            Assert.AreEqual(
                3,
                ObservableEnumerable.SingleOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 3 == 0)).Value,
                "SingleOrDefaultTest3(a)"
            );

            Assert.AreEqual(
                default(int),
                ObservableEnumerable.SingleOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 17 == 0)).Value,
                "SingleOrDefaultTest3(b)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.SingleOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2 == 0)).Value;}
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 3 == 0));

            Assert.AreEqual(
                ObservableEnumerable.SingleOrDefault(seq, f),
                ObservableEnumerable.SingleOrDefault(seq, f),
                "SingleOrDefaultTest3(c)"
            );
        }
    }
}
