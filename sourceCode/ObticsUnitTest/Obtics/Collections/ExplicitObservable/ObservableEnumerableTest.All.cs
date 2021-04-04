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
        public void AllTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.All(
                    new bool[] { true, false },
                    b => b
                ).Value,
                false,
                "AllTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.All(
                    new bool[] { true, true },
                    b => b
                ).Value,
                true,
                "AllTest1(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.All(
                    new bool[] { },
                    b => b
                ).Value,
                true,
                "AllTest1(c)"
            );

            var seq = new bool[] { true, false };
            var f = new Func<bool, bool>(b => b);

            Assert.AreEqual(
                ObservableEnumerable.All(seq, f),
                ObservableEnumerable.All(seq, f),
                "AllTest1(d)"
            );
        }


        [TestMethod]
        public void AllTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.All(
                    new bool[] { true, false },
                    b => ValueProvider.Static(b)
                ).Value,
                false,
                "AllTest2(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.All(
                    new bool[] { true, true },
                    b => ValueProvider.Static(b)
                ).Value,
                true,
                "AllTest2(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.All(
                    new bool[] { },
                    b => ValueProvider.Static(b)
                ).Value,
                true,
                "AllTest2(c)"
            );

            var seq = new bool[] { true, false };
            var f = new Func<bool, IValueProvider<bool>>(b => ValueProvider.Static(b));

            Assert.AreEqual(
                ObservableEnumerable.All(seq, f),
                ObservableEnumerable.All(seq, f),
                "AllTest2(d)"
            );

        }
    }
}
