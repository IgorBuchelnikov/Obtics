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
        public void SelectManyTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.SelectMany(new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } }, a => (int[])a)
                ),
                "SelectManyTest1(a)"
            );

            var seq = new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } };
            var f = new Func<object, IEnumerable<int>>(a => (int[])a);


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f),
                ObservableEnumerable.SelectMany(seq, f),
                "SelectManyTest1(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.SelectMany(new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } }, (a, x) => (int[])a)
                ),
                "SelectManyTest2(a)"
            );

            var seq = new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } };
            var f = new Func<object, int, IEnumerable<int>>((a, x) => (int[])a);


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f),
                ObservableEnumerable.SelectMany(seq, f),
                "SelectManyTest2(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.SelectMany(new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } }, a => ValueProvider.Static((IEnumerable<int>)a))
                ),
                "SelectManyTest3(a)"
            );

            var seq = new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } };
            var f = new Func<object, IValueProvider<IEnumerable<int>>>(a => ValueProvider.Static((IEnumerable<int>)a));


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f),
                ObservableEnumerable.SelectMany(seq, f),
                "SelectManyTest3(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.SelectMany(new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } }, (a, x) => ValueProvider.Static((IEnumerable<int>)a))
                ),
                "SelectManyTest4(a)"
            );

            var seq = new object[] { new int[] { 1, 2 }, new int[] { 3, 4 } };
            var f = new Func<object, int, IValueProvider<IEnumerable<int>>>((a, x) => ValueProvider.Static((IEnumerable<int>)a));


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f),
                ObservableEnumerable.SelectMany(seq, f),
                "SelectManyTest4(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest5()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, a => System.Linq.Enumerable.Range(0, a), (a, c) => a + c)
                ),
                "SelectManyTest5(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, IEnumerable<int>>(a => System.Linq.Enumerable.Range(0, a));
            var r = new Func<int, int, int>((a, c) => a + c);


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest5(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest6()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 4 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, (a, x) => System.Linq.Enumerable.Range(x, a), (a, c) => a + c)
                ),
                "SelectManyTest6(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, int, IEnumerable<int>>((a, x) => System.Linq.Enumerable.Range(x, a));
            var r = new Func<int, int, int>((a, c) => a + c);


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest6(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest7()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, a => System.Linq.Enumerable.Range(0, a), (a, c) => ValueProvider.Static(a + c))
                ),
                "SelectManyTest7(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, IEnumerable<int>>(a => System.Linq.Enumerable.Range(0, a));
            var r = new Func<int, int, IValueProvider<int>>((a, c) => ValueProvider.Static(a + c));


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest7(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest8()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 4 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, (a, x) => System.Linq.Enumerable.Range(x, a), (a, c) => ValueProvider.Static(a + c))
                ),
                "SelectManyTest8(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, int, IEnumerable<int>>((a, x) => System.Linq.Enumerable.Range(x, a));
            var r = new Func<int, int, IValueProvider<int>>((a, c) => ValueProvider.Static(a + c));


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest8(b)"
            );
        }


        [TestMethod]
        public void SelectManyTest9()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, a => ValueProvider.Static(System.Linq.Enumerable.Range(0, a)), (a, c) => a + c)
                ),
                "SelectManyTest9(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, IValueProvider<IEnumerable<int>>>(a => ValueProvider.Static(System.Linq.Enumerable.Range(0, a)));
            var r = new Func<int, int, int>((a, c) => a + c);


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest9(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest10()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 4 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, (a, x) => ValueProvider.Static(System.Linq.Enumerable.Range(x, a)), (a, c) => a + c)
                ),
                "SelectManyTest10(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, int, IValueProvider<IEnumerable<int>>>((a, x) => ValueProvider.Static(System.Linq.Enumerable.Range(x, a)));
            var r = new Func<int, int, int>((a, c) => a + c);


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest10(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest11()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, a => ValueProvider.Static(System.Linq.Enumerable.Range(0, a)), (a, c) => ValueProvider.Static(a + c))
                ),
                "SelectManyTest11(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, IValueProvider<IEnumerable<int>>>(a => ValueProvider.Static(System.Linq.Enumerable.Range(0, a)));
            var r = new Func<int, int, IValueProvider<int>>((a, c) => ValueProvider.Static(a + c));


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest11(b)"
            );
        }

        [TestMethod]
        public void SelectManyTest12()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 4 },
                    ObservableEnumerable.SelectMany(new int[] { 1, 2 }, (a, x) => ValueProvider.Static(System.Linq.Enumerable.Range(x, a)), (a, c) => ValueProvider.Static(a + c))
                ),
                "SelectManyTest12(a)"
            );

            var seq = new int[] { 1, 2 };
            var f = new Func<int, int, IValueProvider<IEnumerable<int>>>((a, x) => ValueProvider.Static(System.Linq.Enumerable.Range(x, a)));
            var r = new Func<int, int, IValueProvider<int>>((a, c) => ValueProvider.Static(a + c));


            Assert.AreEqual(
                ObservableEnumerable.SelectMany(seq, f, r),
                ObservableEnumerable.SelectMany(seq, f, r),
                "SelectManyTest12(b)"
            );
        }

    }
}
