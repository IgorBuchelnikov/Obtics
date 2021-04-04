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
using Obtics.Collections;
using System.Threading;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for SequenceEqualsAggregateTest
    /// </summary>
    [TestClass]
    public class SequenceEqualsAggregateTest
    {
        public SequenceEqualsAggregateTest()
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
            var source = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 10, 11, 12, 13 };

            //NCC
            FrameIEnumerableNPCNCC<int> frame1 = new FrameIEnumerableNPCNCC<int>();
            AsyncFrameIEnumerableRunner<int> frameRunner1 = new AsyncFrameIEnumerableRunner<int>(source, frame1);
            FrameIEnumerableNPC<int> frame2 = new FrameIEnumerableNPC<int>();
            AsyncFrameIEnumerableRunner<int> frameRunner2 = new AsyncFrameIEnumerableRunner<int>(source, frame2);
            frameRunner1.Start();
            frameRunner2.Start();
            Thread.Sleep(0);
            var pipeline = SequenceEqualsAggregate<int>.Create(frame1, frame2, ObticsEqualityComparer<int>.Default);
            string prefix = "SequenceEqualsAggregate<int>.Create(frame1, frame2, EqualityComparer<int>.Default)";

            var clientRunner = new AsynValueTransformationClientRunner<bool>(
                new ValueProviderClient<bool>[] {
                    new ValueProviderClient<bool>(),
                    new ValueProviderClientNPC<bool>(),
                    new ValueProviderClient<bool>(),
                    new ValueProviderClientNPC<bool>(),
                },
                pipeline
            );
            var clientRunner2 = new AsynValueTransformationClientRunner<bool>(
                new ValueProviderClient<bool>[] {
                    new ValueProviderClient<bool>(),
                    new ValueProviderClientNPC<bool>(),
                    new ValueProviderClient<bool>(),
                    new ValueProviderClientNPC<bool>(),
                },
                pipeline
            );
            clientRunner.Start();
            clientRunner2.Start();
            Thread.Sleep(1000);

            ValueProviderClientNPC<bool>[] clients = new ValueProviderClientNPC<bool>[20];

            for (int i = 0; i < clients.Length; ++i)
            {
                clients[i] = new ValueProviderClientNPC<bool>();
                clients[i].Source = pipeline;
                Thread.Sleep(500 / clients.Length);
            }

            frameRunner1.Stop();
            frameRunner2.Stop();
            clientRunner.Stop();
            clientRunner2.Stop();

            for (int i = 0; i < 50 && (frameRunner1.IsRunning || frameRunner2.IsRunning || clientRunner.IsRunning || clientRunner2.IsRunning); ++i)
                Thread.Sleep(100);

            Assert.IsFalse(frameRunner1.IsRunning, prefix + ": FrameRunner1 thread is still running on NCC test.");
            Assert.IsFalse(frameRunner2.IsRunning, prefix + ": FrameRunner2 thread is still running on NCC test.");
            Assert.IsFalse(clientRunner.IsRunning, prefix + ": ClientRunner thread is still running on NCC test.");
            Assert.IsFalse(clientRunner2.IsRunning, prefix + ": ClientRunner2 thread is still running on NCC test.");

            for (int i = 0; i < clients.Length; ++i)
                Assert.IsTrue(clients[i].Buffer, prefix + ": Client " + i.ToString() + " did not return the expected result on NCC test.");

        }


        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<int, bool>
        {
            public override bool ExpectedResult
            { get { return true; } }

            static List<int> source = new List<int>(
                new int[] { 1, 2, 3, 3, 2, 1 }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { 2, 4, 5, 3, 14, 12 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<bool> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return SequenceEqualsAggregate<int>.Create(frame, ReverseTransformation<int>.Create(frame), ObticsEqualityComparer<int>.Default);
            }

            public override string Prefix
            {
                get { return "new SequenceEqualsAggregate<int>(frame, new ReverseTransformation<int>(frame), EqualityComparer<int>.Default)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();
            runner.RunAllPrepPairs();
        }

        class CorrectnessRunner2 : CollectionTestSequenceRunnerForAggregate<int, bool>
        {
            public override bool ExpectedResult
            { get { return false; } }

            static List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6 }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { 3, 2, 1, 6, 5, 4 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<bool> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return SequenceEqualsAggregate<int>.Create(frame, ReverseTransformation<int>.Create(frame), ObticsEqualityComparer<int>.Default);
            }

            public override string Prefix
            {
                get { return "new SequenceEqualsAggregate<int>(frame, new ReverseTransformation<int>(frame), EqualityComparer<int>.Default)"; }
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
                return SequenceEqualsAggregate<int>.Create(frame, ReverseTransformation<int>.Create(frame), ObticsEqualityComparer<int>.Default);
            }

            public override string Prefix
            {
                get { return "new SequenceEqualsAggregate<int>(frame, new ReverseTransformation<int>(frame), EqualityComparer<int>.Default)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }


        [TestMethod]
        public void ArgumentsCheck_first()
        {
            Assert.IsNull(SequenceEqualsAggregate<int>.Create(null, new int[0], ObticsEqualityComparer<int>.Default), "Should return null when first is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_second()
        {
            Assert.IsNull(SequenceEqualsAggregate<int>.Create(new int[0], null, ObticsEqualityComparer<int>.Default), "Should return null when second is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_comparer()
        {
            Assert.IsNull(SequenceEqualsAggregate<int>.Create(new int[0], new int[0], null), "Should return null when comparer is null.");
        }

        class Comparer : IEqualityComparer<int>
        {
            #region IEqualityComparer<int> Members

            public bool Equals(int x, int y)
            { return false; }

            public int GetHashCode(int obj)
            { return 0; }

            #endregion
        }
        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new int[0], new int[0],
                new int[0], new int[0],
                (IEqualityComparer<int>)ObticsEqualityComparer<int>.Default, new Comparer(),
                (f, s, c) => SequenceEqualsAggregate<int>.Create(f, s, c),
                "SequenceEqualsAggregate<int>"
            );
        }

    }
}
