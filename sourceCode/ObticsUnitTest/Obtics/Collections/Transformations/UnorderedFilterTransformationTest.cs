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
using Obtics;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for UnorderedFilterTransformationTest
    /// </summary>
    [TestClass]
    public class UnorderedFilterTransformationTest
    {
        public UnorderedFilterTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<int, int>
        {
            static List<int> result = new List<int>(
                new int[] { 3, 3, 6, 9 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<int> source = new List<int>(
                new int[] { 1, 3, 2, 6, 4, 8, 3, 5, 2, 9 }
            );

            public override List<int> SourceSequence
            {
                get { return source; }
            }

            static List<int> filler = new List<int>(
                new int[] { 11, 12, 13, 9, 15, 16, 17, 18, 19, 20 }
            );

            public override List<int> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<int> frame)
            {
                //assume DegradeToSingleItemAddRemoveTransformation is tested properly
                return SortTransformation<int,int>.Create( UnorderedFilterTransformation<int>.Create(frame, i => i % 3 == 0), i => i, Comparer<int>.Default );
            }

            public override string Prefix
            {
                get { return "SortTransformation<int,int>.Create( UnorderedFilterTransformation<int>.Create(frame, i => i % 3 == 0), i => i, Comparer<int>.Default )"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<int, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<int> frame)
            {
                return UnorderedFilterTransformation<int>.Create(frame, i => i % 3 == 0);
            }

            public override string Prefix
            {
                get { return "new UnorderedFilterTransformation<int>(frame, i => i % 3 == 0)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(UnorderedFilterTransformation<int>.Create(null, i => i % 3 == 0), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_predicate()
        {
            Assert.IsNull(UnorderedFilterTransformation<int>.Create( StaticEnumerable<int>.Create(new int[0]), null), "Should return null when predicate is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[]{1}),
                new Func<int,bool>( i => i % 3 == 0 ), i => i % 3 == 1,
                (s, p) => UnorderedFilterTransformation<int>.Create(s, p),
                "UnorderedFilterTransformation<int>"
            );
        }
        [TestMethod]
        public void ConcurrencyTest1()
        {
            List<int> source = new List<int>(
                            new int[] { 1, 3, 2, 6, 4, 8, 3, 5, 2, 9, 10, 11, 12 }
                        );

            List<int> result = new List<int>(
                            new int[] { 3, 3, 6, 9 }
                        );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => SortTransformation<int,int>.Create( UnorderedFilterTransformation<int>.Create(frame, i => i % 3 == 0), i => i, Comparer<int>.Default ),
                "SortTransformation<int,int>.Create( UnorderedFilterTransformation<int>.Create(frame, i => i % 3 == 0), i => i, Comparer<int>.Default )"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int?[] { 1, 2, 3, null, 4, 5, 6, 7, 8, 9, null, null, 10, null },
                new int?[] { 2, 4, null, 6, 8, null, 10, 1, 3, 5, null, 7, null, 9 },
                new int?[] { 7, null, 2, null, 5, null, 1, 9, null, 3, 2, null, 6, null, 8, null, 8, 9, null, 11 },
                new int?[] { null, 3, 5, null, 6, 3, 1, 3, 2, null, 5, 5, null, 6, 2, null },
                2,
                (frame1, frame2, frame3, frame4) => CountAggregate<global::Obtics.Tuple<IValueProvider<int?>, int?>>.Create(UnorderedFilterTransformation<global::Obtics.Tuple<IValueProvider<int?>, int?>>.Create(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.HasValue)),
                "CountAggregate<Tuple<IValueProvider<int?>, int?>>.Create(UnorderedFilterTransformation<Tuple<IValueProvider<int?>, int?>>.Create(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.HasValue))"
            );
        }
    }
}
