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
        public void AggregateTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Aggregate(
                    new int[] { 1, 2, 3, 4 },
                    (acc, item) => acc + item
                ).Value,
                10,
                "AggregateTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int, int>((acc, item) => acc + item);

            Assert.AreEqual(
                ObservableEnumerable.Aggregate(seq, f),
                ObservableEnumerable.Aggregate(seq, f),
                "AggregateTest1(b)"
            );
        }

        [TestMethod]
        public void AggregateTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Aggregate(
                    new int[] { 1, 2, 3, 4 },
                    2,
                    (acc, item) => acc + item
                ).Value,
                12,
                "AggregateTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int, int>((acc, item) => acc + item);

            Assert.AreEqual(
                ObservableEnumerable.Aggregate(seq, 2, f),
                ObservableEnumerable.Aggregate(seq, 2, f),
                "AggregateTest2(b)"
            );
        }

        [TestMethod]
        public void AggregateTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Aggregate(
                    new int[] { 1, 2, 3, 4 },
                    2,
                    (acc, item) => acc + item,
                    acc => acc / 2
                ).Value,
                6,
                "AggregateTest3(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int, int>((acc, item) => acc + item);
            var s = new Func<int, int>((acc) => acc / 2);

            Assert.AreEqual(
                ObservableEnumerable.Aggregate(seq, 2, f, s),
                ObservableEnumerable.Aggregate(seq, 2, f, s),
                "AggregateTest3(b)"
            );

        }

        [TestMethod]
        public void AggregateTest4()
        {
            Assert.AreEqual(
                ObservableEnumerable.Aggregate(
                    new int[] { 1, 2, 3, 4 },
                    2,
                    (acc, item) => acc + item,
                    acc => ValueProvider.Static( acc / 2 )
                ).Value,
                6,
                "AggregateTest4(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int, int>((acc, item) => acc + item);
            var s = new Func<int, IValueProvider<int>>((acc) => ValueProvider.Static(acc / 2));

            Assert.AreEqual(
                ObservableEnumerable.Aggregate(seq, 2, f, s),
                ObservableEnumerable.Aggregate(seq, 2, f, s),
                "AggregateTest4(b)"
            );

        }
    }
}
