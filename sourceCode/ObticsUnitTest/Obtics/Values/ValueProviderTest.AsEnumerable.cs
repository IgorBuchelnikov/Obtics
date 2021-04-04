using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Values;
using Obtics;

namespace ObticsUnitTest.Obtics.Values
{
    public partial class ValueProviderTest
    {
        [TestMethod]
        public void AsEnumerableTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1 },
                    ValueProvider.Static(1).AsEnumerable()
                ),
                "AsEnumerableTest1(a)"
            );

            var s = ValueProvider.Static(1);

            Assert.AreEqual(
                s.AsEnumerable(),
                s.AsEnumerable(),
                "AsEnumerableTest1(b)"
            );
        }

        [TestMethod]
        public void AsEnumerableTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1 },
                    ValueProvider.Static(1).AsEnumerable(v => v != 2)
                ),
                "AsEnumerableTest2(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] {},
                    ValueProvider.Static(2).AsEnumerable(v => v != 2)
                ),
                "AsEnumerableTest2(b)"
            );

            var s = ValueProvider.Static(1);
            var p = new Func<int, bool>(v => v == 2);

            Assert.AreEqual(
                s.AsEnumerable(p),
                s.AsEnumerable(p),
                "AsEnumerableTest2(c)"
            );
        }

    }
}
