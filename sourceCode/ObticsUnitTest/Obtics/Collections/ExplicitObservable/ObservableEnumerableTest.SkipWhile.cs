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
        public void SkipWhileTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 4 },
                    ObservableEnumerable.SkipWhile(new int[] { 1, 2, 3, 4 }, v => v != 3)
                ),
                "SkipWhileTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, bool>(v => v != 2);

            Assert.AreEqual(
                ObservableEnumerable.SkipWhile(seq, f),
                ObservableEnumerable.SkipWhile(seq, f),
                "SkipWhileTest1(b)"
            );
        }

        [TestMethod]
        public void SkipWhileTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 4 },
                    ObservableEnumerable.SkipWhile(new int[] { 1, 2, 3, 4 }, v => ValueProvider.Static(v != 3))
                ),
                "SkipWhileTest2(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<bool>>(v => ValueProvider.Static(v != 2));

            Assert.AreEqual(
                ObservableEnumerable.SkipWhile(seq, f),
                ObservableEnumerable.SkipWhile(seq, f),
                "SkipWhileTest2(b)"
            );
        }

        [TestMethod]
        public void SkipWhileTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 4 },
                    ObservableEnumerable.SkipWhile(new int[] { 1, 2, 3, 4 }, (v,x) => v + x != 5)
                ),
                "SkipWhileTest3(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int,int, bool>((v,x) => v + x != 5);

            Assert.AreEqual(
                ObservableEnumerable.SkipWhile(seq, f),
                ObservableEnumerable.SkipWhile(seq, f),
                "SkipWhileTest3(b)"
            );
        }

        [TestMethod]
        public void SkipWhileTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 4 },
                    ObservableEnumerable.SkipWhile(new int[] { 1, 2, 3, 4 }, (v, x) => ValueProvider.Static(v + x != 5))
                ),
                "SkipWhileTest4(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int, IValueProvider<bool>>((v, x) => ValueProvider.Static(v + x != 5));

            Assert.AreEqual(
                ObservableEnumerable.SkipWhile(seq, f),
                ObservableEnumerable.SkipWhile(seq, f),
                "SkipWhileTest4(b)"
            );
        }
    }
}
