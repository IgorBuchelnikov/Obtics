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
        public void OnException1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.OnException(new int[] { 1, 2, 3, 4 }, (Exception ex) => new int[0])
                ),
                "OnException1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            Func<Exception,IEnumerable<int>> handler = (Exception ex) => new int[0];

            Assert.AreEqual(
                ObservableEnumerable.OnException(seq, handler),
                ObservableEnumerable.OnException(seq, handler),
                "OnException1(b)"
            );
        }
    }
}
