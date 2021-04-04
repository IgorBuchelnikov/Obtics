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
    
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void RepeatTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 2, 2 },
                    ObservableEnumerable.Repeat(2, 3)
                ),
                "RepeatTest1(a)"
            );

            var value = 1;
            var count = 3;

            Assert.AreEqual(
                ObservableEnumerable.Repeat(value, count),
                ObservableEnumerable.Repeat(value, count),
                "RepeatTest1(b)"
            );
        }

        [TestMethod]
        public void RepeatTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 2, 2 },
                    ObservableEnumerable.Repeat(2, ValueProvider.Static(3))
                ),
                "RepeatTest2(a)"
            );

            var value = 1;
            var count = ValueProvider.Static(3);

            Assert.AreEqual(
                ObservableEnumerable.Repeat(value, count),
                ObservableEnumerable.Repeat(value, count),
                "RepeatTest2(b)"
            );
        }

        [TestMethod]
        public void RepeatTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 2, 2 },
                    ObservableEnumerable.Repeat(ValueProvider.Static(2), 3)
                ),
                "RepeatTest3(a)"
            );

            var value = ValueProvider.Static(1);
            var count = 3;

            Assert.AreEqual(
                ObservableEnumerable.Repeat(value, count),
                ObservableEnumerable.Repeat(value, count),
                "RepeatTest3(b)"
            );
        }

        [TestMethod]
        public void RepeatTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 2, 2 },
                    ObservableEnumerable.Repeat(ValueProvider.Static(2), ValueProvider.Static(3))
                ),
                "RepeatTest4(a)"
            );

            var value = ValueProvider.Static(1);
            var count = ValueProvider.Static(3);

            Assert.AreEqual(
                ObservableEnumerable.Repeat(value, count),
                ObservableEnumerable.Repeat(value, count),
                "RepeatTest4(b)"
            );
        }
    }
}
