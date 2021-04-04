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
    /// Summary description for ConvertPairsGroupingTransformationTest
    /// </summary>
    [TestClass]
    public class ConvertPairsGroupingTransformationTest
    {
        public ConvertPairsGroupingTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<Tuple<string, string>, string>
        {
            static List<Tuple<string, string>> source = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AB", "X" ),
                    new Tuple<string,string>( "CD", "X" ),
                    new Tuple<string,string>( "EF", "X" ),
                    new Tuple<string,string>( "AD", "X" ),
                    new Tuple<string,string>( "BE", "X" ),
                    new Tuple<string,string>( "FA", "X" ),
                    new Tuple<string,string>( "EA", "X" ),
                    new Tuple<string,string>( "AC", "X" ),
                    new Tuple<string,string>( "AB", "X" ),
                    new Tuple<string,string>( "DA", "X" )
                }
            );

            public override List<Tuple<string, string>> SourceSequence
            {
                get { return source; }
            }


            static List<string> result = new List<string>(
                new string[] { "AB", "CD", "EF", "AD", "BE", "FA", "EA", "AC", "AB", "DA" }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
            }

            static List<Tuple<string, string>> filler = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AD", "X" ),
                    new Tuple<string,string>( "BE", "X" ),
                    new Tuple<string,string>( "FA", "X" ),
                    new Tuple<string,string>( "EA", "X" ),
                    new Tuple<string,string>( "AC", "X" ),
                    new Tuple<string,string>( "AB", "X" ),
                    new Tuple<string,string>( "DA", "X" )
                }
            );

            public override List<Tuple<string, string>> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<Tuple<string, string>> frame)
            {
                //We assume here that the Correctness of GroupingTransformation has been properly tested
                return ConvertPairsGroupingTransformation<string, string, GroupingTransformation<string, Tuple<string, string>>>.Create(GroupingTransformation<string, Tuple<string, string>>.Create(frame, "X"));
            }

            public override string Prefix
            {
                get { return "new ConvertPairsGroupingTransformation<string, string>(new GroupingTransformation<string, Pair<string, string>>(frame, \"X\"))"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<Tuple<string, string>, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<Tuple<string, string>> frame)
            {
                return ConvertPairsGroupingTransformation<string, string, GroupingTransformation<string, Tuple<string, string>>>.Create(GroupingTransformation<string, Tuple<string, string>>.Create(frame, "X"));
            }

            public override string Prefix
            {
                get { return "new ConvertPairsGroupingTransformation<string, string>(new GroupingTransformation<string, Pair<string, string>>(frame, \"X\"))"; }
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
            Assert.IsNull(ConvertPairsGroupingTransformation<string, string, GroupingTransformation<string, Tuple<string, string>>>.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                GroupingTransformation<string, Tuple<string, string>>.Create( StaticEnumerable<Tuple<string, string>>.Create( new Tuple<string, string>[0] ), "X"),
                GroupingTransformation<string, Tuple<string, string>>.Create( StaticEnumerable<Tuple<string, string>>.Create( new Tuple<string, string>[] { Tuple.Create("","") } ), "Y"),
                s => ConvertPairsGroupingTransformation<string, string, GroupingTransformation<string, Tuple<string, string>>>.Create(s),
                "ConvertPairsGroupingTransformation<string, string>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<Tuple<string, string>> source = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AB", "X" ),
                    new Tuple<string,string>( "CD", "X" ),
                    new Tuple<string,string>( "EF", "X" ),
                    new Tuple<string,string>( "AD", "X" ),
                    new Tuple<string,string>( "BE", "X" ),
                    new Tuple<string,string>( "FA", "X" ),
                    new Tuple<string,string>( "EA", "X" ),
                    new Tuple<string,string>( "AC", "X" ),
                    new Tuple<string,string>( "AB", "X" ),
                    new Tuple<string,string>( "DA", "X" ),
                    new Tuple<string,string>( "BE", "X" ),
                    new Tuple<string,string>( "FA", "X" ),
                    new Tuple<string,string>( "EA", "X" )
                }
            );

            List<string> result = new List<string>(
                new string[] { "AB", "CD", "EF", "AD", "BE", "FA", "EA", "AC", "AB", "DA" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                s => ConvertPairsGroupingTransformation<string, string, GroupingTransformation<string, Tuple<string, string>>>.Create(GroupingTransformation<string, Tuple<string, string>>.Create(s, "X")),
                "ConvertPairsGroupingTransformation<string, string, GroupingTransformation<string, Tuple<string, string>>>.Create(GroupingTransformation<string, Tuple<string, string>>.Create(s, \"X\"))"
            );
        }
    }
}
