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
    /// Summary description for SumAggregateTest
    /// </summary>
    [TestClass]
    public class SumAggregateNullableInt64Test
    {
        public SumAggregateNullableInt64Test()
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
                new long?[] { null, 1L, 2L, 3L, 4L, 5L, 6L, 7L, 8L, 9L, 10L, 11L, 12L, 13L }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                45,
                frame => SumAggregateNullableInt64.Create(frame),
                "SumAggregateNullableInt64.Create(frame)"
            );
        }
        
        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<long?, long?>
        {
            public override long? ExpectedResult
            { get { return 36; } }

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

            public override IValueProvider<long?> CreatePipeline(FrameIEnumerable<long?> frame)
            {
                return SumAggregateNullableInt64.Create(frame).OnException((OverflowException ex) => (long?)222L);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableInt64.Create(frame)"; }
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
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<long?, long?>
        {
            public override long? ExpectedResult
            { get { return 0; } }

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

            public override IValueProvider<long?> CreatePipeline(FrameIEnumerable<long?> frame)
            {
                return SumAggregateNullableInt64.Create(frame).OnException((OverflowException ex) => (long?)222L);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableInt64.Create(frame)"; }
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
        class CorrectnessRunner3 : CollectionTestSequenceRunnerForAggregate<long?, long?>
        {
            public override long? ExpectedResult
            { get { return 222L; } }

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

            public override IValueProvider<long?> CreatePipeline(FrameIEnumerable<long?> frame)
            {
                return SumAggregateNullableInt64.Create(frame).OnException((OverflowException ex) => (long?)222L);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableInt64.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<long?, long?>
        {
            protected override IValueProvider<long?> CreateAggregate(FrameIEnumerableNPCNCC<long?> frame)
            {
                return SumAggregateNullableInt64.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableInt64.Create(frame)"; }
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
                var r = SumAggregateNullableInt64.Create(null);
            Assert.IsNull(r, "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new Int64?[0], new Int64?[] { 1L },
                s => SumAggregateNullableInt64.Create(s),
                "SumAggregateNullableInt64"
            );
        }

    }
}
