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
        public void JoinTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => i % 3, (o, i) => i + o)
                ),
                "JoinTest1(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, int, int>((o, i) => i + o);

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest1(b)"
            );
        }

        [TestMethod]
        public void JoinTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => i % 3, (o, i) => i + o)
                ),
                "JoinTest2(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, int, int>((o, i) => i + o);

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest2(b)"
            );
        }

        [TestMethod]
        public void JoinTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => ValueProvider.Static(i % 3), (o, i) => i + o)
                ),
                "JoinTest3(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, int, int>((o, i) => i + o);

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest3(b)"
            );
        }

        [TestMethod]
        public void JoinTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => ValueProvider.Static(i % 3), (o, i) => i + o)
                ),
                "JoinTest4(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, int, int>((o, i) => i + o);

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest4(b)"
            );
        }

        [TestMethod]
        public void JoinTest5()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => i % 3, (o, i) => ValueProvider.Static( i + o ) )
                ),
                "JoinTest5(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, int, IValueProvider<int>>((o, i) => ValueProvider.Static( i + o ));

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest5(b)"
            );
        }

        [TestMethod]
        public void JoinTest6()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => i % 3, (o, i) => ValueProvider.Static( i + o ))
                ),
                "JoinTest6(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, int, IValueProvider<int>>((o, i) => ValueProvider.Static( i + o ));

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest6(b)"
            );
        }

        [TestMethod]
        public void JoinTest7()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => ValueProvider.Static(i % 3), (o, i) => ValueProvider.Static( i + o ))
                ),
                "JoinTest7(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, int, IValueProvider<int>>((o, i) => ValueProvider.Static( i + o ));

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest7(b)"
            );
        }

        [TestMethod]
        public void JoinTest8()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 8, 7, 10 },
                    ObservableEnumerable.Join(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => ValueProvider.Static(i % 3), (o, i) => ValueProvider.Static( i + o ))
                ),
                "JoinTest8(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, int, IValueProvider<int>>((o, i) => ValueProvider.Static( i + o ));

            Assert.AreEqual(
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                ObservableEnumerable.Join(outer, inner, oks, iks, rs),
                "JoinTest8(b)"
            );
        }

    }
}
