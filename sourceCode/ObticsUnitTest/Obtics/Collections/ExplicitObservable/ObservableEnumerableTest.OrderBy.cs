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
        public void OrderByTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.OrderBy(new int[] { 3, 2, 4, 1 }, i => i)
                ),
                "OrderByTest1(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, int>(i => i);

            Assert.AreEqual(
                ObservableEnumerable.OrderBy(seq, ks),
                ObservableEnumerable.OrderBy(seq, ks),
                "OrderByTest1(b)"
            );
        }

        [TestMethod]
        public void OrderByTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.OrderBy(new int[] { 3, 2, 4, 1 }, i => ValueProvider.Static(i))
                ),
                "OrderByTest2(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i));

            Assert.AreEqual(
                ObservableEnumerable.OrderBy(seq, ks),
                ObservableEnumerable.OrderBy(seq, ks),
                "OrderByTest2(b)"
            );
        }

        [TestMethod]
        public void OrderByTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.OrderBy(new int[] { 3, 2, 4, 1 }, i => i, Comparer<int>.Default)
                ),
                "OrderByTest3(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, int>(i => i);
            var comp = Comparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.OrderBy(seq, ks, comp),
                ObservableEnumerable.OrderBy(seq, ks, comp),
                "OrderByTest3(b)"
            );
        }

        [TestMethod]
        public void OrderByTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.OrderBy(new int[] { 3, 2, 4, 1 }, i => ValueProvider.Static(i), Comparer<int>.Default)
                ),
                "OrderByTest4(a)"
            );

            var seq = new int[] { 3, 2, 4, 1 };
            var ks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i));
            var comp = Comparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.OrderBy(seq, ks, comp),
                ObservableEnumerable.OrderBy(seq, ks, comp),
                "OrderByTest4(b)"
            );
        }
    }
}
