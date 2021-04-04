using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Collections.Transformations;
using ObticsUnitTest.Helpers;
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for GroupingTransformationTest
    /// </summary>
    [TestClass]
    public class GroupingTransformationTest
    {
        public GroupingTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<string, string>
        {

            static List<string> source = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            public override List<string> ExpectedResult
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

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return GroupingTransformation<string, string>.Create(frame, "X");
            }

            public override string Prefix
            {
                get { return "GroupingTransformation<string, string>(frame, \"X\")"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<string, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return GroupingTransformation<string, string>.Create(frame, "X");
            }

            public override string Prefix
            {
                get { return "new GroupingTransformation<string, string>(frame, \"X\")"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }

        [TestMethod]
        public void KeyPropertyTest()
        {
            var r = GroupingTransformation<string, string>.Create( StaticEnumerable<string>.Create(new string[0]), "X");
            Assert.AreSame(r.Key, "X", "Key property does not return expected value");
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(GroupingTransformation<string, string>.Create(null, "X"), "Should return null when comparer is null.");
        }


        class Comparer : IEqualityComparer<string>
        {
            #region IEqualityComparer<string> Members

            public bool Equals(string x, string y)
            { return false; }

            public int GetHashCode(string obj)
            { return 0; }

            #endregion
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<string>.Create(new string[0]), StaticEnumerable<string>.Create(new string[]{ "a" }),
                "X", "Y",
                (s, k) => GroupingTransformation<string, string>.Create(s, k),
                "GroupingTransformation<string, string>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<string> source = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "12", "13", "14" }
            );

            List<string> result = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => GroupingTransformation<string, string>.Create(frame, "X"),
                "GroupingTransformation<string, string>.Create(frame, \"X\")"
            );
        }
    }
}
