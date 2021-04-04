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
    /// Summary description for SumAggregateTest
    /// </summary>
    [TestClass]
    public class SumAggregateNullableSingleTest
    {
        public SumAggregateNullableSingleTest()
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
            List<float?> source = new List<float?>(
                new float?[] { null, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                45f,
                frame => SumAggregateNullableSingle.Create(frame),
                "SumAggregateNullableSingle.Create(frame)"
            );
        }
        
        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<float?, float?>
        {
            public override float? ExpectedResult
            { get { return 36f; } }

            static List<float?> source = new List<float?>(
                new float?[] { null, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f }
            );

            public override List<float?> SourceSequence
            { get { return source; } }

            static List<float?> filler = new List<float?>(
                new float?[] { float.MaxValue, float.MinValue, 3f, 18.45f, 14f, 12.223f }
            );

            public override List<float?> FillerItems
            { get { return filler; } }

            public override IValueProvider<float?> CreatePipeline(FrameIEnumerable<float?> frame)
            {
                return SumAggregateNullableSingle.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableSingle.Create(frame)"; }
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
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<float?, float?>
        {
            public override float? ExpectedResult
            { get { return default(float); } }

            static List<float?> source = new List<float?>(
                new float?[] { }
            );

            public override List<float?> SourceSequence
            { get { return source; } }

            static List<float?> filler = new List<float?>(
                new float?[] { float.MaxValue - 4f, float.MinValue + 5f, 3f,  - 18.45f, 14f, 12.223f }
            );

            public override List<float?> FillerItems
            { get { return filler; } }

            public override IValueProvider<float?> CreatePipeline(FrameIEnumerable<float?> frame)
            {
                return SumAggregateNullableSingle.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableSingle.Create(frame)"; }
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
        class CorrectnessRunner3 : CollectionTestSequenceRunnerForAggregate<float?, float?>
        {
            public override float? ExpectedResult
            { get { return float.PositiveInfinity; } }

            static List<float?> source = new List<float?>(
                new float?[] { float.MaxValue, float.MinValue, float.MaxValue, float.MaxValue - 18.45f, 14f, 12.223f  }
            );

            public override List<float?> SourceSequence
            { get { return source; } }

            static List<float?> filler = new List<float?>(
                new float?[] { float.MaxValue - 4f, float.MinValue + 5f, float.MaxValue - 3f, float.MaxValue - 18.45f, 14f, 12.223f }
            );

            public override List<float?> FillerItems
            { get { return filler; } }

            public override IValueProvider<float?> CreatePipeline(FrameIEnumerable<float?> frame)
            {
                return SumAggregateNullableSingle.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableSingle.Create(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<float?, float?>
        {
            protected override IValueProvider<float?> CreateAggregate(FrameIEnumerableNPCNCC<float?> frame)
            {
                return SumAggregateNullableSingle.Create(frame);
            }

            public override string Prefix
            {
                get { return "SumAggregateNullableSingle.Create(frame)"; }
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
                var r = SumAggregateNullableSingle.Create(null);
            Assert.IsNull(r, "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new Single?[0], new Single?[0],
                s => SumAggregateNullableSingle.Create(s),
                "SumAggregateNullableSingle"
            );
        }
    }
}
