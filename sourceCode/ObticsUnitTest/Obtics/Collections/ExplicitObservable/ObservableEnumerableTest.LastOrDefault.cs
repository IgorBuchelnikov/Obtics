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
        public void LastOrDefaultTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(new int[] { 1, 2, 3, 4 }).Value,
                4,
                "LastOrDefaultTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(new int[] { }).Value,
                default(int),
                "LastOrDefaultTest1(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(seq),
                ObservableEnumerable.LastOrDefault(seq),
                "LastOrDefaultTest1(c)"
            );
        }

        [TestMethod]
        public void LastOrDefaultTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(new int[] { 1, 2, 3, 4 }, i => i % 3 == 0).Value,
                3,
                "LastOrDefaultTest2(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(new int[] { 1, 2, 3, 4 }, i => i % 17 == 0).Value,
                default(int),
                "LastOrDefaultTest2(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 3 == 0);

            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(seq, f),
                ObservableEnumerable.LastOrDefault(seq, f),
                "LastOrDefaultTest2(c)"
            );
        }

        [TestMethod]
        public void LastOrDefaultTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 3 == 0)).Value,
                3,
                "LastOrDefaultTest3(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 17 == 0)).Value,
                default(int),
                "LastOrDefaultTest3(a)"
            );

            //TestHelper.ExpectException<InvalidOperationException>(
            //    () =>
            //    { var dummy = ObservableEnumerable.LastOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 17 == 0)).Value; }
            //);

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 3 == 0));

            Assert.AreEqual(
                ObservableEnumerable.LastOrDefault(seq, f),
                ObservableEnumerable.LastOrDefault(seq, f),
                "LastOrDefaultTest3(c)"
            );
        }
    }
}
