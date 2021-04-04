using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Collections.Transformations;
using Obtics.Collections;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for JoinTransformationTest
    /// </summary>
    [TestClass]
    public class UnorderedCascadeTransformationTest
    {
        public UnorderedCascadeTransformationTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<IVersionedEnumerable<int>, int>
        {
            static List<int> result = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<IVersionedEnumerable<int>> source = new List<IVersionedEnumerable<int>>(
                new IVersionedEnumerable<int>[] { 
                    StaticEnumerable<int>.Create( new int[] { 1, 2, 3 } ),
                    StaticEnumerable<int>.Create(new int[] { 4, 5 } ),
                    StaticEnumerable<int>.Create(new int[] {} ),
                    StaticEnumerable<int>.Create(new int[] { 6, 7, 8, 9, 10 } ),
                    StaticEnumerable<int>.Create(new int[] { 11, 12 } ),
                    StaticEnumerable<int>.Create(new int[] { 13 } ),
                    StaticEnumerable<int>.Create(new int[] {} ),
                    StaticEnumerable<int>.Create(new int[] { 14, 15, 16 } ),
                    StaticEnumerable<int>.Create(new int[] { 17, 18 } ),
                    StaticEnumerable<int>.Create(new int[] { 19, 20 } )
                }
            );

            public override List<IVersionedEnumerable<int>> SourceSequence
            {
                get { return source; }
            }

            static List<IVersionedEnumerable<int>> filler = new List<IVersionedEnumerable<int>>(
                new IVersionedEnumerable<int>[] { 
                    StaticEnumerable<int>.Create(new int[] { 21, 22 } ),
                    StaticEnumerable<int>.Create(new int[] { 23 } ),
                    StaticEnumerable<int>.Create(new int[] { 24, 25 } ),
                    StaticEnumerable<int>.Create(new int[] { } ),
                    StaticEnumerable<int>.Create(new int[] { 26 } ),
                    StaticEnumerable<int>.Create(new int[] { 27, 28, 29 ,30 } )
                }
            );

            public override List<IVersionedEnumerable<int>> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<IVersionedEnumerable<int>> frame)
            {
                return SortTransformation<int,int>.Create(UnorderedCascadeTransformation<int, IVersionedEnumerable<int>>.Create(frame), i => i, Comparer<int>.Default );
            }

            public override string Prefix
            {
                get { return "new JoinTransformation<int>(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<IVersionedEnumerable<int>, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<IVersionedEnumerable<int>> frame)
            {
                return UnorderedCascadeTransformation<int, IVersionedEnumerable<int>>.Create(frame);
            }

            public override string Prefix
            {
                get { return "new JoinTransformation<int>(frame)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }


        //splits the source in 2. (list with odd number & list with even numbers)
        //adds both lists to an array and uses the array as source for a JoinTransformation.
        //So this correctness test checks if JoinTransformation processes changes in child 
        //collections correctly
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForTransformation<int, int>
        {
            static List<int> result = new List<int>(
                new int[] { 2, 3, 4, 6, 6, 8, 9, 10, 12, 12, 14, 15, 16, 18, 18, 20 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }
            );

            public override List<int> SourceSequence
            {
                get { return source; }
            }

            static List<int> filler = new List<int>(
                new int[] { 21, 22, 23, 24, 25, 26, 27, 28, 29 }
            );

            public override List<int> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<int> frame)
            {
                var oddFilter = FilterTransformation<int>.Create(frame, i => i % 3 == 0);
                var evenFilter = FilterTransformation<int>.Create(frame, i => i % 2 == 0);

                return SortTransformation<int,int>.Create( UnorderedCascadeTransformation<int, IVersionedEnumerable<int>>.Create(StaticEnumerable<IVersionedEnumerable<int>>.Create(new IVersionedEnumerable<int>[] { oddFilter, evenFilter })), i => i, Comparer<int>.Default );
            }

            public override string Prefix
            {
                get { return "new JoinTransformation<int>(new IEnumerable<int>[] { oddFilter, evenFilter })"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner2 : CollectionDeterministicEventRegistrationRunnerForTransformation<int, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<int> frame)
            {
                var oddFilter = FilterTransformation<int>.Create(frame, i => i % 2 != 0);
                var evenFilter = FilterTransformation<int>.Create(frame, i => i % 2 == 0);

                return UnorderedCascadeTransformation<int, IVersionedEnumerable<int>>.Create(StaticEnumerable<IVersionedEnumerable<int>>.Create(new IVersionedEnumerable<int>[] { oddFilter, evenFilter }));
            }

            public override string Prefix
            {
                get { return "new JoinTransformation<int>(new IEnumerable<int>[] { oddFilter, evenFilter })"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            var runner = new DeterministicEventRegistrationRunner2();
            runner.Run();
        }

        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(UnorderedCascadeTransformation<int,IEnumerable<int>>.Create(null), "Should return null when source is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<IVersionedEnumerable<int>>.Create(new IVersionedEnumerable<int>[] { null }), StaticEnumerable<IVersionedEnumerable<int>>.Create(new IVersionedEnumerable<int>[0]),
                s => UnorderedCascadeTransformation<int, IVersionedEnumerable<int>>.Create(s),
                "UnorderedCascadeTransformation<int, IVersionedEnumerable<int>>.Create(s)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest1()
        {
            var joker1 = new FrameIEnumerableNPC<int>();
            var joker2 = new FrameIEnumerableNPCNCC<int>();

            var joker1Runner = new AsyncFrameIEnumerableRunner<int>(new int[] { 1, 5, 3, 6, 2, 4, 7, 5, 3, 5, 2, 1, 4, 6, 2 }, joker1);
            var joker2Runner = new AsyncFrameIEnumerableRunner<int>(new int[] { 5, 3, 6, 7, 5, 3, 1, 4, 6, 2, 7, 3, 2 }, joker2);

            joker1Runner.Start();
            joker2Runner.Start();

            try
            {
                List<IVersionedEnumerable<int>> source = new List<IVersionedEnumerable<int>>(
                    new IVersionedEnumerable<int>[] { 
                        StaticEnumerable<int>.Create(new int[] { 1, 2, 3 } ),
                        StaticEnumerable<int>.Create(new int[] { 4, 5 } ),
                        StaticEnumerable<int>.Create(new int[] {} ),
                        StaticEnumerable<int>.Create(new int[] { 6, 7, 8, 9, 10 } ),
                        StaticEnumerable<int>.Create(new int[] { 11, 12 } ),
                        StaticEnumerable<int>.Create(new int[] { 13 } ),
                        StaticEnumerable<int>.Create(new int[] {} ),
                        StaticEnumerable<int>.Create(new int[] { 14, 15, 16 } ),
                        StaticEnumerable<int>.Create(new int[] { 17, 18 } ),
                        StaticEnumerable<int>.Create(new int[] { 19, 20 } ),
                        joker1,
                        joker2,
                        StaticEnumerable<int>.Create(new int[] { 24, 25 } )
                    }
                );

                List<int> result = new List<int>(
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }
                );

                AsyncTestRunnerForCollectionTransformation.Run(
                    source.ToArray(),
                    result.ToArray(),
                    frame => SortTransformation<int,int>.Create( UnorderedCascadeTransformation<int,IVersionedEnumerable<int>>.Create(frame), i => i, Comparer<int>.Default ) ,
                    "SortTransformation<int,int>.Create( UnorderedCascadeTransformation<int,IVersionedEnumerable<int>>.Create(frame), i => i, Comparer<int>.Default )"
                );
            }
            finally
            {
                joker1Runner.Stop();
                joker2Runner.Stop();
            }
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }
            );

            List<int> result = new List<int>(
                new int[] { 2, 3, 4, 6, 6, 8, 9, 10 }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => SortTransformation<int,int>.Create( UnorderedCascadeTransformation<int,IVersionedEnumerable<int>>.Create(StaticEnumerable<IVersionedEnumerable<int>>.Create(new IVersionedEnumerable<int>[] { FilterTransformation<int>.Create(frame, i => i % 3 == 0), FilterTransformation<int>.Create(frame, i => i % 2 == 0) })), i => i, Comparer<int>.Default ),
                "SortTransformation<int,int>.Create( UnorderedCascadeTransformation<int,IVersionedEnumerable<int>>.Create(StaticEnumerable<IVersionedEnumerable<int>>.Create(new IVersionedEnumerable<int>[] { FilterTransformation<int>.Create(frame, i => i % 3 == 0), FilterTransformation<int>.Create(frame, i => i % 2 == 0) })), i => i, Comparer<int>.Default )"
            );
        }

        [TestMethod]
        public void ConcurrencyTest3()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new IVersionedEnumerable<int>[] { new int[] { 1, 2, 3 }.Patched(), new int[] { 4 }.Patched(), null, new int[] { 5, 6 }.Patched() },
                new IVersionedEnumerable<int>[] { null, new int[] { 4, 5 }.Patched(), new int[] { 2, 5 }.Patched(), new int[] { 9 }.Patched() },
                new IVersionedEnumerable<int>[] { new int[] { 6, 7, 1 }.Patched(), null, new int[] { 4 }.Patched(), null, new int[] { 9, 6, 3, 2 }.Patched() },
                new IVersionedEnumerable<int>[] { new int[] { 2 }.Patched(), new int[] { 4, 4, 5 }.Patched(), new int[] { 5, 6, 2, 3 }.Patched(), null },
                40,
                (frame1, frame2, frame3, frame4) => ObservableEnumerable.Sum(UnorderedCascadeTransformation<int, IVersionedEnumerable<int>>.Create((IVersionedEnumerable<IVersionedEnumerable<int>>)ObservableEnumerable.Select(ObservableEnumerable.Where(NotifyVpcTransformation<IValueProvider<IVersionedEnumerable<int>>, IVersionedEnumerable<int>>.Create((IVersionedEnumerable<IValueProvider<IVersionedEnumerable<int>>>)ObservableEnumerable.Static(new IValueProvider<IVersionedEnumerable<int>>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second != null), t => t.Second))),
                "ObservableEnumerable.Sum(JoinTransformation<int>.Create((IVersionedEnumerable<IVersionedEnumerable<int>>)ObservableEnumerable.Select(ObservableEnumerable.Where(NotifyVpcTransformation<IValueProvider<IVersionedEnumerable<int>>, IVersionedEnumerable<int>>.Create((IVersionedEnumerable<IValueProvider<IVersionedEnumerable<int>>>)ObservableEnumerable.Static(new IValueProvider<IVersionedEnumerable<int>>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second != null), t => t.Second)))"
            );
        }
    }
}
