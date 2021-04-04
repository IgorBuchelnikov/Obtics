using Obtics.Collections;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using System;
using System.Collections.Generic;
using Obtics.Values;
using System.Collections;
using System.Linq;
using ObticsUnitTest.Helpers;
using Obtics;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {

        [TestMethod()]
        public void WhereTest3()
        {
            Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(new int[] { 1, 7, 9, 3 }, ObservableEnumerable.Where(new int[] { 2, 1, 4, 6, 7, 9, 4, 8, 3 }, i => i % 2 == 1)), "WhereTest3(a)");

            var seq = new int[] { 1, 7, 9, 3 };
            var f = new Func<int, bool>(i => i % 3 == 1);

            Assert.AreEqual(
                ObservableEnumerable.Where(seq, f),
                ObservableEnumerable.Where(seq, f),
                "WhereTest3(b)"
            );
        }

        [TestMethod()]
        public void WhereTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 7, 9, 3 }, 
                    ObservableEnumerable.Where(
                        new int[] { 2, 1, 4, 6, 7, 9, 4, 8, 3 }, 
                        i => ValueProvider.Static(i % 2 == 1)
                    )
                ), 
                "WhereTest2(a)"
            );

            var seq = new int[] { 1, 7, 9, 3 };
            var f = new Func<int, IValueProvider<bool>>(i => ValueProvider.Static(i % 3 == 1));

            Assert.AreEqual(
                ObservableEnumerable.Where(seq, f),
                ObservableEnumerable.Where(seq, f),
                "WhereTest2(b)"
            );
        }

        [TestMethod()]
        public void WhereTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 6, 7, 8, 3 },
                    ObservableEnumerable.Where(
                        new int[] { 2, 1, 4, 6, 7, 9, 4, 8, 3 },
                        (i,x) => (i + x) % 2 == 1
                    )
                ),
                "WhereTest1(a)"
            );

            var seq = new int[] { 1, 7, 9, 3 };
            var f = new Func<int,int,bool>( (i,x) => (i+x) % 3 == 1);

            Assert.AreEqual(
                ObservableEnumerable.Where(seq, f),
                ObservableEnumerable.Where(seq, f),
                "WhereTest1(b)"
            );
        }


        [TestMethod()]
        public void WhereTest()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 6, 7, 8, 3 },
                    ObservableEnumerable.Where(
                        new int[] { 2, 1, 4, 6, 7, 9, 4, 8, 3 },
                        (i, x) => ValueProvider.Static((i + x) % 2 == 1)
                    )
                ),
                "WhereTest(a)"
            );

            var seq = new int[] { 1, 7, 9, 3 };
            var f = new Func<int, int, IValueProvider<bool>>((i, x) => ValueProvider.Static((i + x) % 3 == 1));

            Assert.AreEqual(
                ObservableEnumerable.Where(seq, f),
                ObservableEnumerable.Where(seq, f),
                "WhereTest(b)"
            );

        }

        [TestMethod]
        public void ConcurrencyTest_Where()
        {            
                AsyncTestRunnerForValueTransformation.Run(
                    new int?[] { 1, 2, 3, null, 4, 5, 6, 7, 8, 9, null, null, 10, null },
                    new int?[] { 2, 4, null, 6, 8, null, 10, 1, 3, 5, null, 7, null, 9 },
                    new int?[] { 7, null, 2, null, 5, null, 1, 9, null, 3, 2, null, 6, null, 8, null, 8, 9, null, 11 },
                    new int?[] { null, 3, 5, null, 6, 3, 1, 3, 2, null, 5, 5, null, 6, 2 },
                    (CollectionTransformationClientSNCC<global::Obtics.Tuple<IValueProvider<int?>, int?>> s) => System.Linq.Enumerable.SequenceEqual(System.Linq.Enumerable.Select(s.Buffer, t => t.Second.Value), new int[] { 9, 11, 2 }),
                    (frame1, frame2, frame3, frame4) =>
                    {
                        var client = new CollectionTransformationClientSNCC<global::Obtics.Tuple<IValueProvider<int?>, int?>>();

                        client.Source =
                            ObservableEnumerable.Where(
                                global::Obtics.Collections.Transformations.NotifyVpcTransformation<IValueProvider<int?>, int?>.Create(
                                    (IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(
                                        new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }
                                     ),
                                     f => f
                                ),
                                t => t.Second.HasValue
                            )
                        ;

                        return ValueProvider.Static(client);
                    },
                    "ObservableEnumerable.Sum( ObservableEnumerable.Select(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.GetValueOrDefault()) )"
                );
        }

    }
}
