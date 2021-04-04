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
    /// Summary description for AllAggregateTest
    /// </summary>
    [TestClass]
    public class AllAggregateTest
    {
        public AllAggregateTest()
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
        public void ConcurrencyTest1()
        {
            List<bool> source = new List<bool>(
                new bool[] { true, true, true, true, true, true, true, true, true, true, false, true, false }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                true,
                frame => AllAggregate.Create(frame),
                "AllAggregate.Create(frame)"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<bool, bool>
        {
            public override bool ExpectedResult
            { get { return true; } }

            static List<bool> source = new List<bool>(
                new bool[] { true, true, true, true, true, true, true, true }
            );

            public override List<bool> SourceSequence
            { get { return source; } }

            static List<bool> filler = new List<bool>(
                new bool[] { false, true, false, false, true, true }
            );

            public override List<bool> FillerItems
            { get { return filler; } }

            public override IValueProvider<bool> CreatePipeline(FrameIEnumerable<bool> frame)
            {
                return AllAggregate.Create(frame);
            }

            public override string Prefix
            {
                get { return "new AllAggregate(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }


        [TestMethod]
        public void ConcurrencyTest2()
        {
            List<bool> source = new List<bool>(
                new bool[] { true, false, true, true, true, true, true, false, true, true, false, true, false }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                false,
                frame => AllAggregate.Create(frame),
                "AllAggregate.Create(frame)"
            );
        }

        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<bool, bool>
        {
            public override bool ExpectedResult
            { get { return false; } }

            static List<bool> source = new List<bool>(
                new bool[] { true, false, true, true, true, true, true, false }
            );

            public override List<bool> SourceSequence
            { get { return source; } }

            static List<bool> filler = new List<bool>(
                new bool[] { true, true, true, true, false, true }
            );

            public override List<bool> FillerItems
            { get { return filler; } }

            public override IValueProvider<bool> CreatePipeline(FrameIEnumerable<bool> frame)
            {
                return AllAggregate.Create(frame);
            }

            public override string Prefix
            {
                get { return "new AllAggregate(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();

            runner.RunAllPrepPairs();
        }

        class CorrectnessRunner3 : CollectionTestSequenceRunnerForAggregate<bool, bool>
        {
            public override bool ExpectedResult
            { get { return true; } }

            static List<bool> source = new List<bool>(
                new bool[] {  }
            );

            public override List<bool> SourceSequence
            { get { return source; } }

            static List<bool> filler = new List<bool>(
                new bool[] { false, false, true, true, false, true }
            );

            public override List<bool> FillerItems
            { get { return filler; } }

            public override IValueProvider<bool> CreatePipeline(FrameIEnumerable<bool> frame)
            {
                return AllAggregate.Create(frame);
            }

            public override string Prefix
            {
                get { return "new AllAggregate(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<bool, bool>
        {
            protected override IValueProvider<bool> CreateAggregate(FrameIEnumerableNPCNCC<bool> frame)
            {
                return AllAggregate.Create(frame);
            }

            public override string Prefix
            {
                get { return "new AllAggregate(frame)"; }
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
            Assert.IsNull(AllAggregate.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new bool[0], new bool[] { false },
                s => AllAggregate.Create(s),
                "AllAggregate"
            );
        }

    }
}
