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
        public void OfTypeTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "john", "Mary", "Lars" },
                    ObservableEnumerable.OfType<string>(new object[] { "john", "Mary", 10, "Lars" })
                ),
                "OfTypeTest1(a)"
            );

            var seq = new object[] { "john", "Mary", 10, "Lars" };

            Assert.AreEqual(
                ObservableEnumerable.OfType<string>(seq),
                ObservableEnumerable.OfType<string>(seq),
                "OfTypeTest1(b)"
            );
        }
    }
}
