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
        public void GroupJoinTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => i % 3, (o, iseq) => ObservableEnumerable.Sum(iseq).Value + o)
                ),
                "GroupJoinTest1(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, IEnumerable<int>, int>((o, iseq) => ObservableEnumerable.Sum(iseq).Value + o);

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest1(b)"
            );
        }

        [TestMethod]
        public void GroupJoinTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => i % 3, (o, iseq) => ObservableEnumerable.Sum(iseq).Value + o)
                ),
                "GroupJoinTest2(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, IEnumerable<int>, int>((o, iseq) => ObservableEnumerable.Sum(iseq).Value + o);

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest2(b)"
            );
        }

        [TestMethod]
        public void GroupJoinTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => ValueProvider.Static(i % 3), (o, iseq) => ObservableEnumerable.Sum(iseq).Value + o)
                ),
                "GroupJoinTest3(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, IEnumerable<int>, int>((o, iseq) => ObservableEnumerable.Sum(iseq).Value + o);

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest3(b)"
            );
        }

        [TestMethod]
        public void GroupJoinTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => ValueProvider.Static(i % 3), (o, iseq) => ObservableEnumerable.Sum(iseq).Value + o)
                ),
                "GroupJoinTest4(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, IEnumerable<int>, int>((o, iseq) => ObservableEnumerable.Sum(iseq).Value + o);

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest4(b)"
            );
        }

        [TestMethod]
        public void GroupJoinTest5()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => i % 3, (o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o) )
                ),
                "GroupJoinTest5(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, IEnumerable<int>, IValueProvider<int>>((o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o));

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest5(b)"
            );
        }

        [TestMethod]
        public void GroupJoinTest6()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => i % 3, (o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o))
                ),
                "GroupJoinTest6(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, int>(i => i % 3);
            var rs = new Func<int, IEnumerable<int>, IValueProvider<int>>((o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o));

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest6(b)"
            );
        }

        [TestMethod]
        public void GroupJoinTest7()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => o, i => ValueProvider.Static(i % 3), (o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o))
                ),
                "GroupJoinTest7(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, int>(o => o);
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, IEnumerable<int>, IValueProvider<int>>((o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o));

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest7(b)"
            );
        }

        [TestMethod]
        public void GroupJoinTest8()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 0, 9, 15 },
                    ObservableEnumerable.GroupJoin(new int[] { 0, 1, 2 }, new int[] { 1, 7, 5, 8 }, o => ValueProvider.Static(o), i => ValueProvider.Static(i % 3), (o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o))
                ),
                "GroupJoinTest8(a)"
            );

            var outer = new int[] { 0, 1, 2 };
            var inner = new int[] { 1, 7, 5, 8 };
            var oks = new Func<int, IValueProvider<int>>(o => ValueProvider.Static(o));
            var iks = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 3));
            var rs = new Func<int, IEnumerable<int>, IValueProvider<int>>((o, iseq) => ObservableEnumerable.Sum(iseq).Select(sumv => sumv + o));

            Assert.AreEqual(
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                ObservableEnumerable.GroupJoin(outer, inner, oks, iks, rs),
                "GroupJoinTest8(b)"
            );
        }

    }
}
