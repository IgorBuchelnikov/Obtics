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
using Obtics;
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for ConvertToPairsTransformationTest
    /// </summary>
    [TestClass]
    public class ConvertToPairsTransformationTest
    {
        public ConvertToPairsTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<string, Tuple<string, string>>
        {
            static List<string> source = new List<string>(
                new string[] { "AB", "CD", "EF", "AD", "BE", "FA", "EA", "AC", "AB", "DA" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }


            static List<Tuple<string, string>> result = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "CD", "C" ),
                    new Tuple<string,string>( "EF", "E" ),
                    new Tuple<string,string>( "AD", "A" ),
                    new Tuple<string,string>( "BE", "B" ),
                    new Tuple<string,string>( "FA", "F" ),
                    new Tuple<string,string>( "EA", "E" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "DA", "D" )
                }
            );

            public override List<Tuple<string, string>> ExpectedResult
            {
                get { return result; }
            }

            static List<string> filler = new List<string>(
                new string[] { "AD", "BE", "FA", "EA", "AC", "AB", "DA" }
            );

            public override List<string> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<Tuple<string, string>> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return ConvertToPairsTransformation<string, string>.Create(frame, s => s.Substring(0,1));
            }

            public override string Prefix
            {
                get { return "new ConvertToPairsTransformation<string, string>(frame, s => s.Substring(0,1))"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<string,Tuple<string, string>>
        {
            protected override IEnumerable<Tuple<string, string>> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return ConvertToPairsTransformation<string, string>.Create(frame, s => s.Substring(0, 1));
            }

            public override string Prefix
            {
                get { return "new ConvertToPairsTransformation<string, string>(frame, s => s.Substring(0,1))"; }
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
            Assert.IsNull(ConvertToPairsTransformation<string, string>.Create(null, s => s.Substring(0, 1)), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_converter()
        {
            Assert.IsNull(ConvertToPairsTransformation<string, string>.Create(StaticEnumerable<string>.Create(new string[0]), null), "Should return null when converter is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<string>.Create(new string[0]), StaticEnumerable<string>.Create(new string[] { "a" }),
                new System.Func<string, string>(s => s.Substring(0, 1)), s => s.Substring(1, 2),
                (s, c) => ConvertToPairsTransformation<string, string>.Create(s, c),
                "ConvertPairsSecondTransformation<string, string>"
            );
        }
        [TestMethod]
        public void ConcurrencyTest()
        {
            List<string> source = new List<string>(
                new string[] { "AB", "CD", "EF", "AD", "BE", "FA", "EA", "AC", "AB", "DA", "CA", "ED", "AD" }
            );

            List<Tuple<string, string>> result = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "CD", "C" ),
                    new Tuple<string,string>( "EF", "E" ),
                    new Tuple<string,string>( "AD", "A" ),
                    new Tuple<string,string>( "BE", "B" ),
                    new Tuple<string,string>( "FA", "F" ),
                    new Tuple<string,string>( "EA", "E" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "DA", "D" )
                }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                s => ConvertToPairsTransformation<string, string>.Create(s, str => str.Substring(0, 1)),
                "ConvertToPairsTransformation<string, string>.Create(s, str => str.Substring(0, 1))"
            );
        }
    }
}
