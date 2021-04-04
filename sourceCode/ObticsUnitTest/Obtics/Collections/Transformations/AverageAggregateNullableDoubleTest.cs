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
    public class AverageAggregateNullableDoubleTest
    {
        public AverageAggregateNullableDoubleTest()
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
            List<double?> source = new List<double?>(
               new double?[] { null, 1d, 2d, 3d, 4d, 5d, 6d, 7d, 8d, 9d, 10d, 11d, 12d, 13d }
           );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                5d,
                frame => AverageAggregateNullableDouble.Create(frame).OnException((InvalidOperationException ex) => (double?)222.0),
                "AverageAggregateNullableDouble.Create(frame)"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<double?, double?>
        {
            public override double? ExpectedResult
            { get { return 4.5d; } }

            static List<double?> source = new List<double?>(
                new double?[] { null, 1d, 2d, 3d, 4d, 5d, 6d, 7d, 8d }
            );

            public override List<double?> SourceSequence
            { get { return source; } }

            static List<double?> filler = new List<double?>(
                new double?[] { double.MaxValue, double.MinValue, 3d, 18.45d, 14d, 12.223d }
            );

            public override List<double?> FillerItems
            { get { return filler; } }

            public override IValueProvider<double?> CreatePipeline(FrameIEnumerable<double?> frame)
            {
                return AverageAggregateNullableDouble.Create(frame);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableDouble.Create(frame)"; }
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
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<double?, double?>
        {
            public override double? ExpectedResult
            { get { return null; } }

            static List<double?> source = new List<double?>(
                new double?[] { }
            );

            public override List<double?> SourceSequence
            { get { return source; } }

            static List<double?> filler = new List<double?>(
                new double?[] { double.MaxValue - 4d, double.MinValue + 5d, 3d,  - 18.45d, 14d, 12.223d }
            );

            public override List<double?> FillerItems
            { get { return filler; } }

            public override IValueProvider<double?> CreatePipeline(FrameIEnumerable<double?> frame)
            {
                return AverageAggregateNullableDouble.Create(frame);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableDouble.Create(frame)"; }
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
        class CorrectnessRunner3 : CollectionTestSequenceRunnerForAggregate<double?, double?>
        {
            public override double? ExpectedResult
            { get { return double.PositiveInfinity; } }

            static List<double?> source = new List<double?>(
                new double?[] { double.MaxValue, double.MinValue, double.MaxValue, double.MaxValue - 18.45d, 14d, 12.223d  }
            );

            public override List<double?> SourceSequence
            { get { return source; } }

            static List<double?> filler = new List<double?>(
                new double?[] { double.MaxValue - 4d, double.MinValue + 5d, double.MaxValue - 3d, double.MaxValue - 18.45d, 14d, 12.223d }
            );

            public override List<double?> FillerItems
            { get { return filler; } }

            public override IValueProvider<double?> CreatePipeline(FrameIEnumerable<double?> frame)
            {
                return AverageAggregateNullableDouble.Create(frame);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableDouble.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<double?, double?>
        {
            protected override IValueProvider<double?> CreateAggregate(FrameIEnumerableNPCNCC<double?> frame)
            {
                return AverageAggregateNullableDouble.Create(frame);
            }

            public override string Prefix
            {
                get { return "AverageAggregateNullableDouble.Create(frame)"; }
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
            Assert.IsNull(AverageAggregateNullableDouble.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new double?[0], new double?[0],
                s => AverageAggregateNullableDouble.Create(s),
                "AverageAggregateNullableDouble"
            );
        }
    }
}
