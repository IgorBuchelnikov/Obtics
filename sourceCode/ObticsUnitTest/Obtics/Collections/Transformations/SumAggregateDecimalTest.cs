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
    public class SumAggregateDecimalTest
    {
        public SumAggregateDecimalTest()
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
            List<decimal> source = new List<decimal>(
                new decimal[] { 1m, 2m, 3m, 4m, 5m, 6m, 7m, 8m, 9m, 10m, 11m, 12m, 13m }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                55,
                frame => SumAggregateDecimal.Create(frame),
                "SumAggregateDecimal.Create(frame)"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<decimal, decimal>
        {
            public override decimal ExpectedResult
            { get { return 36m; } }

            static List<decimal> source = new List<decimal>(
                new decimal[] { 1m, 2m, 3m, 4m, 5m, 6m, 7m, 8m }
            );

            public override List<decimal> SourceSequence
            { get { return source; } }

            static List<decimal> filler = new List<decimal>(
                new decimal[] {  -4m,  +5m, -3m, -18.45m, 14m, 12.223m }
            );

            public override List<decimal> FillerItems
            { get { return filler; } }

            public override IValueProvider<decimal> CreatePipeline(FrameIEnumerable<decimal> frame)
            {
                return SumAggregateDecimal.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateDecimal.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();
            runner.RunAllPrepPairs();
        }


        /// <summary>
        /// overflow test
        /// </summary>
        class CorrectnessRunner4 : CollectionTestSequenceRunnerForAggregate<decimal, decimal>
        {
            public override decimal ExpectedResult
            { get { return 1.05m; } }

            static List<decimal> source = new List<decimal>(
                new decimal[] { 0.05m, 1m, 0m, 0.02m, -0.1m, 0.2m, -0.2m, 0.08m }
            );

            public override List<decimal> SourceSequence
            { get { return source; } }

            static List<decimal> filler = new List<decimal>(
                new decimal[] { 0.7m, 0.0004m, -3m, 2m, 1.3m, 1.44m }
            );

            public override List<decimal> FillerItems
            { get { return filler; } }

            public override IValueProvider<decimal> CreatePipeline(FrameIEnumerable<decimal> frame)
            {
                return SumAggregateDecimal.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateDecimal.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest4()
        {
            var runner = new CorrectnessRunner4();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<decimal, decimal>
        {
            protected override IValueProvider<decimal> CreateAggregate(FrameIEnumerableNPCNCC<decimal> frame)
            {
                return SumAggregateDecimal.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateDecimal.Create(frame)"; }
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
            var r = SumAggregateDecimal.Create(null);
            Assert.IsNull(r, "Should return null when source is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new decimal[0], new decimal[] { 1m },
                s => SumAggregateDecimal.Create(s),
                "SumAggregateDecimal"
            );
        }

    }
}
