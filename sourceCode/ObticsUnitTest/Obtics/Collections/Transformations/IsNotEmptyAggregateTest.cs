using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics;
using Obtics.Collections.Transformations;
using ObticsUnitTest.Helpers;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for IsNotEmptyAggregateTest
    /// </summary>
    [TestClass]
    public class IsNotEmptyAggregateTest
    {
        public IsNotEmptyAggregateTest()
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
                new int[] { 1, 3, 8, 2, 4, 6, 2, 8, 7, 3, 4, 6, 2, 10, 12 }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                true,
                frame => IsNotEmptyAggregate<int>.Create(frame),
                "IsNotEmptyAggregate<int>.Create(frame)"
            );
        }
        
        class CorrectnessRunner1 : CollectionTestSequenceRunnerForAggregate<int, bool>
        {
            public override bool ExpectedResult
            { get { return true; } }

            static List<int> source = new List<int>(
                new int[] { 1, 4, 2, 0, 4, 6, 2, 8 }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { 3, 2, 1, 5, 3, 7 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<bool> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return IsNotEmptyAggregate<int>.Create(frame);
            }

            public override string Prefix
            {
                get { return "new IsNotEmptyAggregate<int>(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest1()
        {
            var runner = new CorrectnessRunner1();
            runner.RunAllPrepPairs();
        }
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<int, bool>
        {
            public override bool ExpectedResult
            { get { return false; } }

            static List<int> source = new List<int>(
                new int[] {  }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { 3, 2, 1, 5, 3, 7 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<bool> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return IsNotEmptyAggregate<int>.Create(frame);
            }

            public override string Prefix
            {
                get { return "new IsNotEmptyAggregate<int>(frame)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<int, bool>
        {
            protected override IValueProvider<bool> CreateAggregate(FrameIEnumerableNPCNCC<int> frame)
            {
                return IsNotEmptyAggregate<int>.Create(frame);
            }

            public override string Prefix
            {
                get { return "new IsNotEmptyAggregate<int>(frame)"; }
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
            Assert.IsNull(IsNotEmptyAggregate<int>.Create(null), "Should return null when source is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (new int[0]), (new int[] { 1 }),
                s => IsNotEmptyAggregate<int>.Create(s),
                "IsNotEmptyAggregate<int>"
            );
        }

    }
}
