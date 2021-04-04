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
        public void ElementAtTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.ElementAt(new int[] { 1, 2, 3, 4 }, 2).Value,
                3,
                "ElementAtTest1(a)"
            );

            ObticsUnitTest.Helpers.TestHelper.ExpectException<ArgumentOutOfRangeException>(
                () => { var dummy = ObservableEnumerable.ElementAt(new int[] { 1, 2, 3, 4 }, 13).Value; }
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.ElementAt(seq, 3),
                ObservableEnumerable.ElementAt(seq, 3),
                "ElementAtTest1(c)"
            );
        }

        [TestMethod]
        public void ElementAtTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.ElementAt(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(2)).Value,
                3,
                "ElementAtTest2(a)"
            );

            TestHelper.ExpectException<ArgumentOutOfRangeException>(
                ()=>
                {
                    var dummy = ObservableEnumerable.ElementAt(new int[] { 1, 2, 3, 4 }, ValueProvider.Static(13)).Value;
                }
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.ElementAt(seq, ValueProvider.Static(3)),
                ObservableEnumerable.ElementAt(seq, ValueProvider.Static(3)),
                "ElementAtTest2(c)"
            );
        }
    }
}
