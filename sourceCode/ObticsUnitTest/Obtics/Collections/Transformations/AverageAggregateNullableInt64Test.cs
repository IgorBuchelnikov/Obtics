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
using Obtics;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for AverageAggregateTest
    /// </summary>
    [TestClass]
    public class AverageAggregateNullableInt64Test
    {
        public AverageAggregateNullableInt64Test()
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

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<long?> source = new List<long?>(
               new long?[] { null, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }
           );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                5.0d,
                frame => AverageAggregateNullableInt64.Create(frame).OnException((InvalidOperationException ex) => (long?)222L),
                "AverageAggregateNullableInt64.Create(frame)"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<long?, double?>
        {
            public override double? ExpectedResult
            { get { return 4.5; } }

            static List<long?> source = new List<long?>(
                new long?[] { null, 1, 2, 3, 4, 5, 6, 7, 8 }
            );

            public override List<long?> SourceSequence
            { get { return source; } }

            static List<long?> filler = new List<long?>(
                new long?[] { long.MaxValue - 4, long.MinValue + 5, long.MaxValue - 3, long.MaxValue - 18, 14, 12 }
            );

            public override List<long?> FillerItems
            { get { return filler; } }

            public override IValueProvider<double?> CreatePipeline(FrameIEnumerable<long?> frame)
            {
                return AverageAggregateNullableInt64.Create(frame);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableInt64.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();
            runner.RunAllPrepPairs();
        }

        /// <summary>
        /// Empty list
        /// </summary>
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<long?, double?>
        {
            public override double? ExpectedResult
            { get { return null; } }

            static List<long?> source = new List<long?>(
                new long?[] { }
            );

            public override List<long?> SourceSequence
            { get { return source; } }

            static List<long?> filler = new List<long?>(
                new long?[] { long.MaxValue - 4, long.MinValue + 5, long.MaxValue - 3, long.MaxValue - 18, 14, 12 }
            );

            public override List<long?> FillerItems
            { get { return filler; } }

            public override IValueProvider<double?> CreatePipeline(FrameIEnumerable<long?> frame)
            {
                return AverageAggregateNullableInt64.Create(frame).OnException((InvalidOperationException ex) => (double?)222.0);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableInt64.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();
            runner.RunAllPrepPairs();
        }

        /// <summary>
        /// overflow test
        /// </summary>
        class CorrectnessRunner3 : CollectionTestSequenceRunnerForAggregate<long?, double?>
        {
            public override double? ExpectedResult
            { get { return 3074457345618258604d; } }

            static List<long?> source = new List<long?>(
                new long?[] { long.MaxValue - 4, long.MinValue + 5, long.MaxValue - 3, long.MaxValue - 18, 16, 14  }
            );

            public override List<long?> SourceSequence
            { get { return source; } }

            static List<long?> filler = new List<long?>(
                new long?[] { long.MaxValue - 4, long.MinValue + 5, long.MaxValue - 3, long.MaxValue - 18, 14, 12 }
            );

            public override List<long?> FillerItems
            { get { return filler; } }

            public override IValueProvider<double?> CreatePipeline(FrameIEnumerable<long?> frame)
            {
                return AverageAggregateNullableInt64.Create(frame);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableInt64.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<long?, double?>
        {
            protected override IValueProvider<double?> CreateAggregate(FrameIEnumerableNPCNCC<long?> frame)
            {
                return AverageAggregateNullableInt64.Create(frame);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableInt64.Create(frame)"; }
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
            Assert.IsNull(AverageAggregateNullableInt64.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new Int64?[0], new Int64?[] { 1L },
                s => AverageAggregateNullableInt64.Create(s),
                "AverageAggregateNullableInt64"
            );
        }
    }
}
