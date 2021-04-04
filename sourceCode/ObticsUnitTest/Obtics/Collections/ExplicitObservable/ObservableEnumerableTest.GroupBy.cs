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
using Obtics;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void GroupByTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 2, 4 },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2))
                ),
                "GroupByTest1(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2), g => g.Key)
                ),
                "GroupByTest1(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f),
                ObservableEnumerable.GroupBy(seq, f),
                "GroupByTest1(c)"
            );
        }

        [TestMethod]
        public void GroupByTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 2, 4 },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static( i % 2 )))
                ),
                "GroupByTest2(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2)), g => g.Key)
                ),
                "GroupByTest2(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f),
                ObservableEnumerable.GroupBy(seq, f),
                "GroupByTest2(c)"
            );
        }

        [TestMethod]
        public void GroupByTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, (k, g) => System.Linq.Enumerable.ElementAt(g, k))
                ),
                "GroupByTest3(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var r = new Func<int, IEnumerable<int>, int>((k, g) => System.Linq.Enumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r),
                ObservableEnumerable.GroupBy(seq, f, r),
                "GroupByTest3(c)"
            );
        }


        [TestMethod]
        public void GroupByTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), (k, g) => System.Linq.Enumerable.ElementAt(g, k))
                ),
                "GroupByTest4(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var r = new Func<int, IEnumerable<int>, int>((k, g) => System.Linq.Enumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r),
                ObservableEnumerable.GroupBy(seq, f, r),
                "GroupByTest4(c)"
            );
        }


        [TestMethod]
        public void GroupByTest5()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, (k, g) => ObservableEnumerable.ElementAt(g, k))
                ),
                "GroupByTest5(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var r = new Func<int, IEnumerable<int>, IValueProvider<int>>((k, g) => ObservableEnumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r),
                ObservableEnumerable.GroupBy(seq, f, r),
                "GroupByTest5(c)"
            );
        }


        [TestMethod]
        public void GroupByTest6()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), (k, g) => ObservableEnumerable.ElementAt(g, (int)k))
                ),
                "GroupByTest6(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var r = new Func<int, IEnumerable<int>, IValueProvider<int>>((k, g) => ObservableEnumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r),
                ObservableEnumerable.GroupBy(seq, f, r),
                "GroupByTest6(c)"
            );
        }

        [TestMethod]
        public void GroupByTest7()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString()))
                ),
                "GroupByTest7(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString()), g => g.Key)
                ),
                "GroupByTest7(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, string>(i => i.ToString());

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es),
                ObservableEnumerable.GroupBy(seq, f, es),
                "GroupByTest7(c)"
            );
        }

        [TestMethod]
        public void GroupByTest8()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString()))
                ),
                "GroupByTest8(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString()), g => g.Key)
                ),
                "GroupByTest8(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, string>(i => i.ToString());

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es),
                ObservableEnumerable.GroupBy(seq, f, es),
                "GroupByTest8(c)"
            );
        }
        [TestMethod]
        public void GroupByTest9()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString())))
                ),
                "GroupByTest9(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString())), g => g.Key)
                ),
                "GroupByTest9(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es),
                ObservableEnumerable.GroupBy(seq, f, es),
                "GroupByTest9(c)"
            );
        }

        [TestMethod]
        public void GroupByTest10()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString())))
                ),
                "GroupByTest10(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString())), g => g.Key)
                ),
                "GroupByTest10(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es),
                ObservableEnumerable.GroupBy(seq, f, es),
                "GroupByTest10(c)"
            );
        }


        [TestMethod]
        public void GroupByTest11()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString(), (k, g) => System.Linq.Enumerable.ElementAt(g, k))
                ),
                "GroupByTest11(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest11(b)"
            );
        }

        [TestMethod]
        public void GroupByTest12()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString(), (k, g) => System.Linq.Enumerable.ElementAt(g, k))
                ),
                "GroupByTest12(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest12(b)"
            );
        }
        [TestMethod]
        public void GroupByTest13()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy<int,int,string,string>(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString()), (k, g) => (string)System.Linq.Enumerable.ElementAt(g, k))
                ),
                "GroupByTest13(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest13(b)"
            );
        }

        [TestMethod]
        public void GroupByTest14()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy<int, int, string, string>(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString()), (k, g) => (string)System.Linq.Enumerable.ElementAt(g, (int)k))
                ),
                "GroupByTest14(a)"
            );


            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest14(b)"
            );
        }


        [TestMethod]
        public void GroupByTest15()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString(), (k, g) => ObservableEnumerable.ElementAt(g, k))
                ),
                "GroupByTest15(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest15(b)"
            );
        }

        [TestMethod]
        public void GroupByTest16()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString(), (k, g) => ObservableEnumerable.ElementAt(g, (int)k))
                ),
                "GroupByTest16(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest16(b)"
            );
        }
        [TestMethod]
        public void GroupByTest17()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString()), (int k, IEnumerable<string> g) => ObservableEnumerable.ElementAt(g, k))
                ),
                "GroupByTest17(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest17(b)"
            );
        }

        [TestMethod]
        public void GroupByTest18()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString()), (int k, IEnumerable<string> g) => ObservableEnumerable.ElementAt(g, k))
                ),
                "GroupByTest18(a)"
            );


            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r),
                ObservableEnumerable.GroupBy(seq, f, es, r),
                "GroupByTest18(b)"
            );
        }



        [TestMethod]
        public void GroupByTest19()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 2, 4 },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, ObticsEqualityComparer<int>.Default))
                ),
                "GroupByTest19(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, ObticsEqualityComparer<int>.Default), g => g.Key)
                ),
                "GroupByTest19(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, c),
                ObservableEnumerable.GroupBy(seq, f, c),
                "GroupByTest19(c)"
            );
        }

        [TestMethod]
        public void GroupByTest20()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 2, 4 },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), ObticsEqualityComparer<int>.Default))
                ),
                "GroupByTest20(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), ObticsEqualityComparer<int>.Default), g => g.Key)
                ),
                "GroupByTest20(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, c),
                ObservableEnumerable.GroupBy(seq, f, c),
                "GroupByTest20(c)"
            );
        }

        [TestMethod]
        public void GroupByTest21()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, (k, g) => System.Linq.Enumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest21(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var r = new Func<int, IEnumerable<int>, int>((k, g) => System.Linq.Enumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r, c),
                ObservableEnumerable.GroupBy(seq, f, r, c),
                "GroupByTest21(c)"
            );
        }


        [TestMethod]
        public void GroupByTest22()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), (k, g) => System.Linq.Enumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest22(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var r = new Func<int, IEnumerable<int>, int>((k, g) => System.Linq.Enumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r, c),
                ObservableEnumerable.GroupBy(seq, f, r, c),
                "GroupByTest22(c)"
            );
        }


        [TestMethod]
        public void GroupByTest23()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, (k, g) => ObservableEnumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest23(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var r = new Func<int, IEnumerable<int>, IValueProvider<int>>((k, g) => ObservableEnumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r, c),
                ObservableEnumerable.GroupBy(seq, f, r, c),
                "GroupByTest23(c)"
            );
        }


        [TestMethod]
        public void GroupByTest24()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 3, 2 },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), (k, g) => ObservableEnumerable.ElementAt(g, (int)k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest24(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var r = new Func<int, IEnumerable<int>, IValueProvider<int>>((k, g) => ObservableEnumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, r, c),
                ObservableEnumerable.GroupBy(seq, f, r, c),
                "GroupByTest24(c)"
            );
        }

        [TestMethod]
        public void GroupByTest25()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString(), ObticsEqualityComparer<int>.Default))
                ),
                "GroupByTest25(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString(), ObticsEqualityComparer<int>.Default), g => g.Key)
                ),
                "GroupByTest25(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, string>(i => i.ToString());
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, c),
                ObservableEnumerable.GroupBy(seq, f, es, c),
                "GroupByTest25(c)"
            );
        }

        [TestMethod]
        public void GroupByTest26()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString(), ObticsEqualityComparer<int>.Default))
                ),
                "GroupByTest26(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString(), ObticsEqualityComparer<int>.Default), g => g.Key)
                ),
                "GroupByTest26(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, string>(i => i.ToString());
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, c),
                ObservableEnumerable.GroupBy(seq, f, es, c),
                "GroupByTest26(c)"
            );
        }
        [TestMethod]
        public void GroupByTest27()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString()), ObticsEqualityComparer<int>.Default))
                ),
                "GroupByTest27(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString()), ObticsEqualityComparer<int>.Default), g => g.Key)
                ),
                "GroupByTest27(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, c),
                ObservableEnumerable.GroupBy(seq, f, es, c),
                "GroupByTest27(c)"
            );
        }

        [TestMethod]
        public void GroupByTest28()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "1", "3", "2", "4" },
                    ObservableEnumerable.Concat(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString()), ObticsEqualityComparer<int>.Default))
                ),
                "GroupByTest28(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 0 },
                    ObservableEnumerable.Select(ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString()), ObticsEqualityComparer<int>.Default), g => g.Key)
                ),
                "GroupByTest28(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, c),
                ObservableEnumerable.GroupBy(seq, f, es, c),
                "GroupByTest28(c)"
            );
        }


        [TestMethod]
        public void GroupByTest29()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString(), (k, g) => System.Linq.Enumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest29(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest29(b)"
            );
        }

        [TestMethod]
        public void GroupByTest30()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString(), (k, g) => System.Linq.Enumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest30(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest30(b)"
            );
        }
        [TestMethod]
        public void GroupByTest31()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy<int, int, string, string>(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString()), (k, g) => (string)System.Linq.Enumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest31(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest31(b)"
            );
        }

        [TestMethod]
        public void GroupByTest32()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy<int, int, string, string>(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString()), (k, g) => (string)System.Linq.Enumerable.ElementAt(g, (int)k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest32(a)"
            );


            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, string>((k, g) => System.Linq.Enumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest32(b)"
            );
        }


        [TestMethod]
        public void GroupByTest33()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => i.ToString(), (k, g) => ObservableEnumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest33(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest33(b)"
            );
        }

        [TestMethod]
        public void GroupByTest34()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => i.ToString(), (k, g) => ObservableEnumerable.ElementAt(g, (int)k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest34(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, string>(i => i.ToString());
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest34(b)"
            );
        }
        [TestMethod]
        public void GroupByTest35()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i % 2, i => ValueProvider.Static(i.ToString()), (int k, IEnumerable<string> g) => ObservableEnumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest35(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, int>(i => i % 2);
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest35(b)"
            );
        }

        [TestMethod]
        public void GroupByTest36()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "3", "2" },
                    ObservableEnumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => ValueProvider.Static(i % 2), i => ValueProvider.Static(i.ToString()), (int k, IEnumerable<string> g) => ObservableEnumerable.ElementAt(g, k), ObticsEqualityComparer<int>.Default)
                ),
                "GroupByTest36(a)"
            );


            var seq = new int[] { 1, 2, 3, 4 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i % 2));
            var es = new Func<int, IValueProvider<string>>(i => ValueProvider.Static(i.ToString()));
            var r = new Func<int, IEnumerable<string>, IValueProvider<string>>((k, g) => ObservableEnumerable.ElementAt(g, k));
            var c = ObticsEqualityComparer<int>.Default;

            Assert.AreEqual(
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                ObservableEnumerable.GroupBy(seq, f, es, r, c),
                "GroupByTest36(b)"
            );
        }


    }
}
