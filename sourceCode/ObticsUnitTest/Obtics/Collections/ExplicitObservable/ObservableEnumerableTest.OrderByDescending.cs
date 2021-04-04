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
        public void OrderByDescendingTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 3, 2, 1 },
                    ObservableEnumerable.OrderByDescending(new int[] { 3, 2, 4, 1 }, i => i)
                ),
                "OrderByDescendingTest1(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, int>(i => i);

            Assert.AreEqual(
                ObservableEnumerable.OrderByDescending(seq, ks),
                ObservableEnumerable.OrderByDescending(seq, ks),
                "OrderByDescendingTest1(b)"
            );
        }

        [TestMethod]
        public void OrderByDescendingTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 3, 2, 1 },
                    ObservableEnumerable.OrderByDescending(new int[] { 3, 2, 4, 1 }, i => ValueProvider.Static(i))
                ),
                "OrderByDescendingTest2(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i));

            Assert.AreEqual(
                ObservableEnumerable.OrderByDescending(seq, ks),
                ObservableEnumerable.OrderByDescending(seq, ks),
                "OrderByDescendingTest2(b)"
            );
        }

        [TestMethod]
        public void OrderByDescendingTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 3, 2, 1 },
                    ObservableEnumerable.OrderByDescending(new int[] { 3, 2, 4, 1 }, i => i, Comparer<int>.Default)
                ),
                "OrderByDescendingTest3(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, int>(i => i);
            var comp = Comparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.OrderByDescending(seq, ks, comp),
                ObservableEnumerable.OrderByDescending(seq, ks, comp),
                "OrderByDescendingTest3(b)"
            );
        }

        [TestMethod]
        public void OrderByDescendingTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 3, 2, 1 },
                    ObservableEnumerable.OrderByDescending(new int[] { 3, 2, 4, 1 }, i => ValueProvider.Static(i), Comparer<int>.Default)
                ),
                "OrderByDescendingTest4(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i));
            var comp = Comparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.OrderByDescending(seq, ks, comp),
                ObservableEnumerable.OrderByDescending(seq, ks, comp),
                "OrderByDescendingTest4(b)"
            );
        }
    }
}
