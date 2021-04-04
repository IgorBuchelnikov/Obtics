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
        [TestMethod()]
        public void AnyTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new bool[] { true, false },
                    b => b
                ).Value,
                true,
                "AnyTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new bool[] { false, false },
                    b => b
                ).Value,
                false,
                "AnyTest1(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new bool[] { },
                    b => b
                ).Value,
                false,
                "AnyTest1(c)"
            );

            var seq = new bool[] { true, false };
            var f = new Func<bool, bool>(b => b);

            Assert.AreEqual(
                ObservableEnumerable.Any(seq, f),
                ObservableEnumerable.Any(seq, f),
                "AnyTest1(d)"
            );
        }

        [TestMethod()]
        public void AnyTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new bool[] { true, false },
                    b => ValueProvider.Static( b )
                ).Value,
                true,
                "AnyTest2(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new bool[] { false, false },
                    b => ValueProvider.Static(b)
                ).Value,
                false,
                "AnyTest2(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new bool[] { },
                    b => ValueProvider.Static(b)
                ).Value,
                false,
                "AnyTest2(c)"
            );

            var seq = new bool[] { true, false };
            var f = new Func<bool, IValueProvider<bool>>(b => ValueProvider.Static(b));

            Assert.AreEqual(
                ObservableEnumerable.Any(seq, f),
                ObservableEnumerable.Any(seq, f),
                "AnyTest2(d)"
            );
        }

        [TestMethod()]
        public void AnyTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new object[] { 10, 11 }
                ).Value,
                true,
                "AnyTest3(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Any(
                    new object[] { }
                ).Value,
                false,
                "AnyTest3(b)"
            );

            var seq = new bool[] { true, false };

            Assert.AreEqual(
                ObservableEnumerable.Any(seq),
                ObservableEnumerable.Any(seq),
                "AnyTest3(c)"
            );

        }
    }
}
