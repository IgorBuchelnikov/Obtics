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
        public void ElementAtOrDefaultTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.ElementAtOrDefault(new int[] { 1, 2, 3, 4 }, 2).Value,
                3,
                "ElementAtOrDefaultTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.ElementAtOrDefault(new int[] { 1, 2, 3, 4 }, 13).Value,
                default(int),
                "ElementAtOrDefaultTest1(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.ElementAtOrDefault(seq, 3),
                ObservableEnumerable.ElementAtOrDefault(seq, 3),
                "ElementAtOrDefaultTest1(c)"
            );
        }

        [TestMethod]
        public void ElementAtOrDefaultTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.ElementAtOrDefault(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(2)).Value,
                3,
                "ElementAtOrDefaultTest2(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.ElementAtOrDefault(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(13)).Value,
                default(int),
                "ElementAtOrDefaultTest2(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.ElementAtOrDefault(seq, ValueProvider.Static(3)),
                ObservableEnumerable.ElementAtOrDefault(seq, ValueProvider.Static(3)),
                "ElementAtOrDefaultTest2(c)"
            );
        }
    }
}
