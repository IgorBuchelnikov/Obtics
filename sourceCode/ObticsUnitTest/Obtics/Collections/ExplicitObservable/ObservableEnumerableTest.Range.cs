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
        public void RangeTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Range(2, 3)
                ),
                "RangeTest1(a)"
            );

            var begin = 1;
            var count = 3;

            Assert.AreEqual(
                ObservableEnumerable.Range(begin, count),
                ObservableEnumerable.Range(begin, count),
                "RangeTest1(b)"
            );
        }

        [TestMethod]
        public void RangeTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Range(2, ValueProvider.Static(3))
                ),
                "RangeTest2(a)"
            );

            var begin = 1;
            var count = ValueProvider.Static(3);

            Assert.AreEqual(
                ObservableEnumerable.Range(begin, count),
                ObservableEnumerable.Range(begin, count),
                "RangeTest2(b)"
            );
        }

        [TestMethod]
        public void RangeTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Range(ValueProvider.Static(2), 3)
                ),
                "RangeTest3(a)"
            );

            var begin = ValueProvider.Static(1);
            var count = 3;

            Assert.AreEqual(
                ObservableEnumerable.Range(begin, count),
                ObservableEnumerable.Range(begin, count),
                "RangeTest3(b)"
            );
        }

        [TestMethod]
        public void RangeTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Range(ValueProvider.Static(2), ValueProvider.Static(3))
                ),
                "RangeTest4(a)"
            );

            var begin = ValueProvider.Static(1);
            var count = ValueProvider.Static(3);

            Assert.AreEqual(
                ObservableEnumerable.Range(begin, count),
                ObservableEnumerable.Range(begin, count),
                "RangeTest4(b)"
            );
        }
    }
}
