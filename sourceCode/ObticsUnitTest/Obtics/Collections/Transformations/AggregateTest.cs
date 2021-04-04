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
    /// Summary description for AggregateTest
    /// </summary>
    [TestClass]
    public class AggregateTest
    {
        public AggregateTest()
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
            List<string> source = new List<string>(
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                45,
                frame => Aggregate<string, int>.Create(frame, enm => { int acc = 0; foreach (string s in enm) acc += int.Parse(s); return acc; }),
                "Aggregate<string, int>.Create(frame, enm => { int acc = 0; foreach (string s in enm) acc += int.Parse(s); return acc; })"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<string, int>
        {
            public override int ExpectedResult
            { get { return 45; } }

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

            public override IValueProvider<int> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return Aggregate<string, int>.Create(frame, enm => { int acc = 0; foreach (string s in enm) acc += int.Parse(s); return acc; });
            }

            public override string Prefix
            {
                get { return "new Aggregate<string, int>(frame, enm => { int acc = 0; foreach (string s in enm) acc += int.Parse(s); return acc; })"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<string, int>
        {
            protected override IValueProvider<int> CreateAggregate(FrameIEnumerableNPCNCC<string> source)
            {
                return Aggregate<string, int>.Create(source, enm => { int acc = 0; foreach (string s in enm) acc += int.Parse(s); return acc; });
            }

            public override string Prefix
            {
                get { return "new Aggregate<string, int>(source, enm => { int acc = 0; foreach (string s in enm) acc += int.Parse(s); return acc; })"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }

        [TestMethod]
        public void ArgumentsCheck_func()
        {
            Assert.IsNull(Aggregate<string, int>.Create(new string[0], null), "Should return null when func is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(Aggregate<string, int>.Create(null, enm => { int acc = 0; foreach (string s in enm) acc += int.Parse(s); return acc; }), "Should return null when source is null."); 
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new string[0], new string[0],
                new Func<IEnumerable<string>, int>(enm => 1), enm => 2,
                (s, f) => Aggregate<string, int>.Create(s, f),
                "Aggregate<string, int>"
            );
        }
    }
}
