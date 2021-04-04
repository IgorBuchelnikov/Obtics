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
    /// Summary description for ConvertTransformationTest
    /// </summary>
    [TestClass]
    public class ConvertTransformationTest
    {
        public ConvertTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<string, int>
        {
            static List<int> result = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<string> source = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            static List<string> filler = new List<string>(
                new string[] { "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" }
            );

            public override List<string> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return ConvertTransformation<string, int>.Create(frame, s => int.Parse(s));
            }

            public override string Prefix
            {
                get { return "Convert s => int.Parse(s)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<string, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return ConvertTransformation<string, int>.Create(frame, s => int.Parse(s));
            }

            public override string Prefix
            {
                get { return "new ConvertTransformation<string, int>(frame, s => int.Parse(s))"; }
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
            Assert.IsNull(ConvertTransformation<string, int>.Create(null, s => s.Length), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_func()
        {
            Assert.IsNull(ConvertTransformation<string, int>.Create(StaticEnumerable<string>.Create(new string[0]), null), "Should return null when func is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<string>.Create(new string[0]), StaticEnumerable<string>.Create(new string[] { "a" }),
                new Func<string, int>(s => s.Length), s => int.Parse(s),
                (s, f) => ConvertTransformation<string, int>.Create(s, f),
                "ConvertTransformation<string, int>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            AsyncTestRunnerForCollectionTransformation.Run(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
                s => (IEnumerable<string>)ConvertTransformation<int, string>.Create(s.Patched(), i => i.ToString()),
                "(IEnumerable<string>)ConvertTransformation<int, string>.Create( s.Patched(), i => i.ToString() )"
            );
        }
    }
}
