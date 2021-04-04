﻿using System;
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
using OE = Obtics.Collections.ObservableEnumerable;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for FindFirstAggregateTest
    /// </summary>
    [TestClass]
    public class FindFirstAggregateTest
    {
        public FindFirstAggregateTest()
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
                8,
                frame => FindFirstAggregate<int>.Create(frame, i => i % 2 == 0).OnException((InvalidOperationException ex) => 222),
                "FindFirstAggregate<int>.Create(frame, i => i % 2 == 0)"
            );
        }
        
        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<int, int>
        {
            public override int ExpectedResult
            { get { return 8; } }

            static List<int> source = new List<int>(
                new int[] { 1, 3, 8, 2, 4, 6, 2, 8 }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { 3, 2, 1, 2, 3, 7 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<int> CreatePipeline(FrameIEnumerable<int> frame)
            { return FindFirstAggregate<int>.Create(frame, i => i % 2 == 0).OnException((InvalidOperationException ex) => 222); }

            public override string Prefix
            { get { return "new FindFirstAggregate<int>(frame, i => i % 2 == 0)"; } }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<int, int>
        {
            protected override IValueProvider<int> CreateAggregate(FrameIEnumerableNPCNCC<int> frame)
            {
                return FindFirstAggregate<int>.Create(frame, i => i % 2 == 0).OnException((InvalidOperationException ex) => 222);
            }

            public override string Prefix
            {
                get { return "new FindFirstAggregate<int>(frame, i => i % 2 == 0)"; }
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
            Assert.IsNull(FindFirstAggregate<int>.Create(null, i => i % 3 == 0), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_predicate()
        {
            Assert.IsNull(FindFirstAggregate<int>.Create(new int[0], null), "Should return null when predicate is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (new int[0]), (new int[] { 1 }),
                new Func<int, bool>(i => i % 3 == 0), i => i % 3 == 1,
                (s, p) => FindFirstAggregate<int>.Create(s, p),
                "FindFirstAggregate<int>"
            );
        }

    }
}
