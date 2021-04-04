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

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for ExtremityAggregateTest
    /// </summary>
    [TestClass]
    public class ExtremityAggregateTest_Inverted
    {
        public ExtremityAggregateTest_Inverted()
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
                new int[] { 1, 2, 3, 4, 5, 8, 7, 4, 2, 6, 4, 7, 8, 3, 4, 2 }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                1,
                frame => ExtremityAggregateValueDescending<int>.Create(frame).OnException((InvalidOperationException ex) => 222),
                "ExtremityAggregate<int>.Create(frame, true)"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<int, int>
        {
            public override int ExpectedResult
            { get { return 1; } }

            static List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 8, 7 }
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
                return ExtremityAggregateValueDescending<int>.Create(frame).OnException((InvalidOperationException ex) => 222);
            }

            public override string Prefix
            {
                get { return "ExtremityAggregate<int>.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();
            runner.RunAllPrepPairs();
        }

        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<int, int>
        {
            public override int ExpectedResult
            { get { return 222; } }

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
                return ExtremityAggregateValueDescending<int>.Create(frame).OnException((InvalidOperationException ex) => 222);
            }

            public override string Prefix
            {
                get { return "ExtremityAggregate<int>.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<int, int>
        {
            protected override IValueProvider<int> CreateAggregate(FrameIEnumerableNPCNCC<int> frame)
            {
                return ExtremityAggregateValueDescending<int>.Create(frame).OnException((InvalidOperationException ex) => 222);
            }

            public override string Prefix
            {
                get { return "ExtremityAggregate<int>.Create(frame)"; }
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
            Assert.IsNull(ExtremityAggregateValueDescending<int>.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (new int[0]), (new int[] { 1 }),
                s => ExtremityAggregateValueDescending<int>.Create(s),
                "ExtremityAggregate<int>"
            );
        }

    }
}
