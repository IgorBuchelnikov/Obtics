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
using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void SelectTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, i => i + 1)
                ),
                "SelectTest1(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int>(i => i + 1);

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, f),
                ObservableEnumerable.Select(seq, f),
                "SelectTest1(b)"
            );
        }

        [TestMethod]
        public void SelectTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, (i, s) => i + s)
                ),
                "SelectTest2(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int>((i, s) => i + s);

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, f),
                ObservableEnumerable.Select(seq, 1, f),
                "SelectTest2(b)"
            );
        }

        [TestMethod]
        public void SelectTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 5, 6 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, 2, (i, s1, s2) => i + s1 + s2)
                ),
                "SelectTest3(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, int>((i, s1, s2) => i + s1 + s2);

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, 2, f),
                ObservableEnumerable.Select(seq, 1, 2, f),
                "SelectTest3(b)"
            );
        }

        [TestMethod]
        public void SelectTest4()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 7, 8, 9 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, 2, 3, (i, s1, s2, s3) => i + s1 + s2 + s3)
                ),
                "SelectTest4(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, int, int>((i, s1, s2, s3) => i + s1 + s2 + s3);

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, 2, 3, f),
                ObservableEnumerable.Select(seq, 1, 2, 3, f),
                "SelectTest4(b)"
            );
        }
        [TestMethod]
        public void SelectTest5()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, i => ValueProvider.Static( i + 1 ))
                ),
                "SelectTest5(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i + 1));

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, f),
                ObservableEnumerable.Select(seq, f),
                "SelectTest5(b)"
            );
        }

        [TestMethod]
        public void SelectTest6()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 3, 4 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, (i, s) => ValueProvider.Static(i + s))
                ),
                "SelectTest6(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, IValueProvider<int>>((i, s) => ValueProvider.Static(i + s));

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, f),
                ObservableEnumerable.Select(seq, 1, f),
                "SelectTest6(b)"
            );
        }

        [TestMethod]
        public void SelectTest7()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 5, 6 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, 2, (i, s1, s2) => ValueProvider.Static(i + s1 + s2))
                ),
                "SelectTest7(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, IValueProvider<int>>((i, s1, s2) => ValueProvider.Static(i + s1 + s2));

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, 2, f),
                ObservableEnumerable.Select(seq, 1, 2, f),
                "SelectTest7(b)"
            );
        }

        [TestMethod]
        public void SelectTest8()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 7, 8, 9 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, 2, 3, (i, s1, s2, s3) => ValueProvider.Static(i + s1 + s2 + s3))
                ),
                "SelectTest8(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, int, IValueProvider<int>>((i, s1, s2, s3) => ValueProvider.Static(i + s1 + s2 + s3));

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, 2, 3, f),
                ObservableEnumerable.Select(seq, 1, 2, 3, f),
                "SelectTest8(b)"
            );
        }


        [TestMethod]
        public void SelectTest9()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 5 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, (i, x) => i + x)
                ),
                "SelectTest9(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int>((i, x) => i + 1);

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, f),
                ObservableEnumerable.Select(seq, f),
                "SelectTest9(b)"
            );
        }

        [TestMethod]
        public void SelectTest10()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 4, 6 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, (i, x, s) => i + x + s)
                ),
                "SelectTest10(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, int>((i, x, s) => i + x + s);

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, f),
                ObservableEnumerable.Select(seq, 1, f),
                "SelectTest10(b)"
            );
        }

        [TestMethod]
        public void SelectTest11()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 6, 8 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, 2, (i, x, s1, s2) => i + x + s1 + s2)
                ),
                "SelectTest11(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, int, int>((i, x, s1, s2) => i + x + s1 + s2);

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, 2, f),
                ObservableEnumerable.Select(seq, 1, 2, f),
                "SelectTest11(b)"
            );
        }


        [TestMethod]
        public void SelectTest12()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 5 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, (i, x) => ValueProvider.Static(i + x))
                ),
                "SelectTest12(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, IValueProvider<int>>((i, x) => ValueProvider.Static(i + 1));

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, f),
                ObservableEnumerable.Select(seq, f),
                "SelectTest12(b)"
            );
        }

        [TestMethod]
        public void SelectTest13()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 2, 4, 6 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, (i, x, s) => ValueProvider.Static(i + x + s))
                ),
                "SelectTest13(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, IValueProvider<int>>((i, x, s) => ValueProvider.Static(i + x + s));

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, f),
                ObservableEnumerable.Select(seq, 1, f),
                "SelectTest13(b)"
            );
        }

        [TestMethod]
        public void SelectTest14()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 4, 6, 8 },
                    ObservableEnumerable.Select(new int[] { 1, 2, 3 }, 1, 2, (i, x, s1, s2) => ValueProvider.Static(i + x + s1 + s2))
                ),
                "SelectTest14(a)"
            );

            var seq = new int[] { 1, 2, 3 };
            var f = new Func<int, int, int, int, IValueProvider<int>>((i, x, s1, s2) => ValueProvider.Static(i + x + s1 + s2));

            Assert.AreEqual(
                ObservableEnumerable.Select(seq, 1, 2, f),
                ObservableEnumerable.Select(seq, 1, 2, f),
                "SelectTest14(b)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest_Select()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int?[] { 1, 2, 3, null, 4, 5, 6, 7, 8, 9, null, null, 10, null },
                new int?[] { 2, 4, null, 6, 8, null, 10, 1, 3, 5, null, 7, null, 9 },
                new int?[] { 7, null, 2, null, 5, null, 1, 9, null, 3, 2, null, 6, null, 8, null, 8, 9, null, 11 },
                new int?[] { null, 3, 5, null, 6, 3, 1, 3, 2, null, 5, 5, null, 6, 2 },
                (CollectionTransformationClientSNCC<int> s) => System.Linq.Enumerable.SequenceEqual(s.Buffer, new int[] { 9, 11, 2 }),
                (frame1, frame2, frame3, frame4) =>
                {
                    var client = new CollectionTransformationClientSNCC<int>();

                    client.Source =
                        (IVersionedEnumerable<int>)ObservableEnumerable.Select(
                            ObservableEnumerable.Where(
                                global::Obtics.Collections.Transformations.NotifyVpcTransformation<IValueProvider<int?>, int?>.Create(
                                    (IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(
                                        new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }
                                     ),
                                     f => f
                                ),
                                t => t.Second.HasValue
                            ),
                            t => t.Second.GetValueOrDefault()
                        )
                    ;

                    return ValueProvider.Static(client);
                },
                "ObservableEnumerable.Sum( ObservableEnumerable.Select(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.GetValueOrDefault()) )"
            );
        }

    }
}
