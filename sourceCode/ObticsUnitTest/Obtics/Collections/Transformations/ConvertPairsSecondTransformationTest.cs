using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObticsUnitTest.Helpers;
using Obtics.Collections.Transformations;
using Obtics;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for ConvertPairsSecondTransformationTest
    /// </summary>
    [TestClass]
    public class ConvertPairsSecondTransformationTest
    {
        public ConvertPairsSecondTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<Pair<string, string>, string>
        {
            static List<Pair<string, string>> source = new List<Pair<string, string>>(
                new Pair<string, string>[] { 
                    new Pair<string,string>( "AB", "A" ),
                    new Pair<string,string>( "CD", "C" ),
                    new Pair<string,string>( "EF", "E" ),
                    new Pair<string,string>( "AD", "A" ),
                    new Pair<string,string>( "BE", "B" ),
                    new Pair<string,string>( "FA", "F" ),
                    new Pair<string,string>( "EA", "E" ),
                    new Pair<string,string>( "AC", "A" ),
                    new Pair<string,string>( "AB", "A" ),
                    new Pair<string,string>( "DA", "D" )
                }
            );

            public override List<Pair<string, string>> SourceSequence
            {
                get { return source; }
            }


            static List<string> result = new List<string>(
                new string[] { "A", "C", "E", "A", "B", "F", "E", "A", "A", "D" }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
            }

            static List<Pair<string, string>> filler = new List<Pair<string, string>>(
                new Pair<string, string>[] { 
                    new Pair<string,string>( "AD", "A" ),
                    new Pair<string,string>( "BE", "B" ),
                    new Pair<string,string>( "FA", "F" ),
                    new Pair<string,string>( "EA", "E" ),
                    new Pair<string,string>( "AC", "A" ),
                    new Pair<string,string>( "AB", "A" ),
                    new Pair<string,string>( "DA", "D" )
                }
            );

            public override List<Pair<string, string>> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<Pair<string, string>> frame)
            {
                return new ConvertPairsSecondTransformation<string, string>(frame);
            }

            public override string Prefix
            {
                get { return "new ConvertPairsSecondTransformation<string, string>( frame )"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<Pair<string, string>, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCSNCC<Pair<string, string>> frame)
            {
                return new ConvertPairsSecondTransformation<string, string>(frame);
            }

            public override string Prefix
            {
                get { return "new ConvertPairsSecondTransformation<string, string>(frame)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }

#if VERIFY_PARAMETERS

        [TestMethod]
        public void ArgumentsCheck_source()
        {
            try
            {
                var r = new ConvertPairsSecondTransformation<string, string>(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreSame("source", ex.ParamName, "ParamName not the expected value");
                return;
            }

            Assert.Fail("Expected ArgumentNullException");
        }

#endif

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new Pair<string, string>[0], new Pair<string, string>[0],
                s => new ConvertPairsSecondTransformation<string, string>(s),
                "ConvertPairsSecondTransformation<string, string>"
            );
        }
    }
}
