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
using Obtics.Collections;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for AccumulativeAggregateTest
    /// </summary>
    [TestClass]
    public class AccumulativeAggregateTest
    {
        public AccumulativeAggregateTest()
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

        class CorrectnessRunner3 : CollectionTestSequenceRunnerForAggregate<string, string>
        {
            public override string ExpectedResult
            { get { return "55"; } }

            static List<string> source = new List<string>(
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }
            );

            public override List<string> SourceSequence
            { get { return source; } }

            static List<string> filler = new List<string>(
                new string[] { "10", "11", "12", "13", "14", "15" }
            );

            public override List<string> FillerItems
            { get { return filler; } }

            public override IValueProvider<string> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return AccumulativeAggregate<string, int, string>.Create(frame, 10, (acc, i) => acc + int.Parse(i), t => t.ToString());
            }

            public override string Prefix
            {
                get { return "new AccumulativeAggregate<string, int, string>(frame, 10, (acc, i) => acc + int.Parse(i), t => t.ToString())"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner3 : CollectionDeterministicEventRegistrationRunnerForAggregate<string, string>
        {
            protected override IValueProvider<string> CreateAggregate(FrameIEnumerableNPCNCC<string> source)
            {
                return AccumulativeAggregate<string, int, string>.Create(source, 10, (acc, i) => acc + int.Parse(i), t => t.ToString());
            }

            public override string Prefix
            {
                get { return "new AccumulativeAggregate<string, int, string>(source, 10, (acc, i) => acc + int.Parse(i), t => t.ToString())"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest3()
        {
            var runner = new DeterministicEventRegistrationRunner3();
            runner.Run();
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new string[0], new string[0],
                "A", "B",
                new Func<string,string,string>( (acc, i) => acc + i ), (acc, i) => i + acc,
                new Func<string,string>( s => s ), s => string.Empty,
                (source, seed, func, selector) => AccumulativeAggregate<string, string, string>.Create(source, seed, func, selector),
                "AccumulativeAggregate<string, string, string>"
            );
        }

        [TestMethod]
        public void ArgumentsCheck_func()
        {
            Assert.IsNull(AccumulativeAggregate<string, string, string>.Create(new string[] { }, string.Empty, null, s => s), "Should return null when func is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(AccumulativeAggregate<string, string, string>.Create(null, string.Empty, (a, s) => a + s, a => a), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_resultSelector()
        {
            Assert.IsNull(AccumulativeAggregate<string, string, string>.Create(new string[] { }, string.Empty, (a, s) => a + s, null), "Should return null when resultSelector is null.");
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<string> source = new List<string>(
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                "55",
                frame => AccumulativeAggregate<string, int, string>.Create(frame, 10, (acc, i) => acc + int.Parse(i), t => t.ToString()),
                "AccumulativeAggregate<string, int, string>.Create(frame, 10, (acc, i) => acc + int.Parse(i), t => t.ToString())"
            );
        }
    }
}
