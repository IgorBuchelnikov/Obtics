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
        public void TakeWhileTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2 },
                    ObservableEnumerable.TakeWhile(new int[] { 1, 2, 3, 4 }, v => v != 3)
                ),
                "TakeWhileTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(v => v != 2);

            Assert.AreEqual(
                ObservableEnumerable.TakeWhile(seq, f),
                ObservableEnumerable.TakeWhile(seq, f),
                "TakeWhileTest1(b)"
            );
        }

        [TestMethod]
        public void TakeWhileTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2 },
                    ObservableEnumerable.TakeWhile(new int[] { 1, 2, 3, 4 }, v => ValueProvider.Static(v != 3))
                ),
                "TakeWhileTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(v => ValueProvider.Static(v != 2));

            Assert.AreEqual(
                ObservableEnumerable.TakeWhile(seq, f),
                ObservableEnumerable.TakeWhile(seq, f),
                "TakeWhileTest2(b)"
            );
        }

        [TestMethod]
        public void TakeWhileTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2 },
                    ObservableEnumerable.TakeWhile(new int[] { 1, 2, 3, 4 }, (v,x) => v + x != 5)
                ),
                "TakeWhileTest3(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int,int, bool>((v,x) => v + x != 5);

            Assert.AreEqual(
                ObservableEnumerable.TakeWhile(seq, f),
                ObservableEnumerable.TakeWhile(seq, f),
                "TakeWhileTest3(b)"
            );
        }

        [TestMethod]
        public void TakeWhileTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2 },
                    ObservableEnumerable.TakeWhile(new int[] { 1, 2, 3, 4 }, (v, x) => ValueProvider.Static(v + x != 5))
                ),
                "TakeWhileTest4(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int, IValueProvider<bool>>((v, x) => ValueProvider.Static(v + x != 5));

            Assert.AreEqual(
                ObservableEnumerable.TakeWhile(seq, f),
                ObservableEnumerable.TakeWhile(seq, f),
                "TakeWhileTest4(b)"
            );
        }
    }
}
