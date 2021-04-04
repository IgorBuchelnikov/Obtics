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
using Obtics.Collections.Transformations;
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for ReverseTransformationTest
    /// </summary>
    [TestClass]
    public class ReverseTransformationTest
    {
        public ReverseTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<int, int>
        {
            static List<int> result = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<int> source = new List<int>(
                new int[] { 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 }
            );

            public override List<int> SourceSequence
            {
                get { return source; }
            }

            static List<int> filler = new List<int>(
                new int[] { 10, 11, 12, 13, 14, 15 }
            );

            public override List<int> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return ReverseTransformation<int>.Create(frame);
            }

            public override string Prefix
            {
                get { return "new ReverseTransformation<int>(frame)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<int, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<int> frame)
            {
                return ReverseTransformation<int>.Create(frame);
            }

            public override string Prefix
            {
                get { return "new ReverseTransformation<int>(frame)"; }
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
            Assert.IsNull(ReverseTransformation<int>.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[]{ 1 }),
                s => ReverseTransformation<int>.Create(s),
                "BooleanInvertTransformation"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<int> source = new List<int>(
                new int[] { 0, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, -1, -2 }
            );

            List<int> result = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => ReverseTransformation<int>.Create(frame),
                "ReverseTransformation<int>.Create(frame)"
            );
        }
    }
}
