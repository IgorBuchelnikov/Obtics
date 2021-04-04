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
        public void FirstOrDefaultTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(new int[] { 1, 2, 3, 4 }).Value,
                1,
                "FirstOrDefaultTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(new int[] { }).Value,
                default(int),
                "FirstOrDefaultTest1(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(seq),
                ObservableEnumerable.FirstOrDefault(seq),
                "FirstOrDefaultTest1(c)"
            );
        }

        [TestMethod]
        public void FirstOrDefaultTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(new int[] { 1, 2, 3, 4 }, i => i % 3 == 0).Value,
                3,
                "FirstOrDefaultTest2(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(new int[] { 1, 2, 3, 4 }, i => i % 17 == 0).Value,
                default(int),
                "FirstOrDefaultTest2(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(i => i % 3 == 0);

            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(seq, f),
                ObservableEnumerable.FirstOrDefault(seq, f),
                "FirstOrDefaultTest2(c)"
            );
        }

        [TestMethod]
        public void FirstOrDefaultTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 3 == 0)).Value,
                3,
                "FirstOrDefaultTest3(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 17 == 0)).Value,
                default(int),
                "FirstOrDefaultTest3(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 3 == 0));

            Assert.AreEqual(
                ObservableEnumerable.FirstOrDefault(seq, f),
                ObservableEnumerable.FirstOrDefault(seq, f),
                "FirstOrDefaultTest3(c)"
            );
        }
    }
}
