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
using System.Collections;
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for TypeConvertTransformationTest
    /// </summary>
    [TestClass]
    public class TypeConvertTransformationTest
    {
        public TypeConvertTransformationTest()
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
                return TypeConvertTransformation<string>.Create((IVersionedEnumerable)frame);
            }

            public override string Prefix
            {
                get { return "new TypeConvertTransformation<string>((IEnumerable)frame)"; }
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
                return TypeConvertTransformation<string>.Create((IVersionedEnumerable)frame);
            }

            public override string Prefix
            {
                get { return "new TypeConvertTransformation<string>((IEnumerable)frame)"; }
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
            Assert.IsNull(TypeConvertTransformation<int>.Create(null),"Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IVersionedEnumerable)StaticEnumerable<int>.Create(new int[0]), (IVersionedEnumerable)StaticEnumerable<int>.Create(new int[] { 1 }),
                s => TypeConvertTransformation<int>.Create(s),
                "TypeConvertTransformation<int>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<string> source = new List<string>(
                            new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "11", "12", "13" }
                        );

            List<string> result = new List<string>(
                            new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }
                        );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => TypeConvertTransformation<string>.Create((IVersionedEnumerable)frame),
                "TypeConvertTransformation<string>.Create((IVersionedEnumerable)frame)"
            );
        }
    }
}
