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
    public class SumAggregateInt32Test
    {
        public SumAggregateInt32Test()
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
            List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                55,
                frame => SumAggregateInt32.Create(frame),
                "SumAggregateInt32.Create(frame)"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<int, int>
        {
            public override int ExpectedResult
            { get { return 36; } }

            static List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { int.MaxValue - 4, int.MinValue + 5, int.MaxValue - 3, int.MaxValue - 18, 14, 12 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<int> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return SumAggregateInt32.Create(frame).OnException((OverflowException ex) => 222);
            }

            public override string Prefix
            {
                get { return "SumAggregateInt32.Create(frame)"; }
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
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<int, int>
        {
            public override int ExpectedResult
            { get { return 0; } }

            static List<int> source = new List<int>(
                new int[] { }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { int.MaxValue - 4, int.MinValue + 5, int.MaxValue - 3, int.MaxValue - 18, 14, 12 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<int> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return SumAggregateInt32.Create(frame).OnException((OverflowException ex) => 222);
            }

            public override string Prefix
            {
                get { return "SumAggregateInt32.Create(frame)"; }
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
        class CorrectnessRunner3 : CollectionTestSequenceRunnerForAggregate<int, int>
        {
            public override int ExpectedResult
            { get { return 333; } }

            static List<int> source = new List<int>(
                new int[] { int.MaxValue - 4, int.MinValue + 5, int.MaxValue - 3, int.MaxValue - 18, 14, 14  }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { int.MaxValue - 4, int.MinValue + 5, int.MaxValue - 3, int.MaxValue - 18, 14, 12 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<int> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return
                    SumAggregateInt32.Create(frame)
                        .OnException((OverflowException ex) => 333)
                    ;
            }

            public override string Prefix
            {
                get { return "SumAggregateInt32.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<int, int>
        {
            protected override IValueProvider<int> CreateAggregate(FrameIEnumerableNPCNCC<int> frame)
            {
                return SumAggregateInt32.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateInt32.Create(frame)"; }
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
                var r = SumAggregateInt32.Create(null);
            Assert.IsNull(r, "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new Int32[0], new Int32[] { 1 },
                s => SumAggregateInt32.Create(s),
                "SumAggregateInt32"
            );
        }

    }
}
