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
    /// Summary description for SkipTransformationTest
    /// </summary>
    [TestClass]
    public class SkipTransformationTest
    {
        public SkipTransformationTest()
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
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            static List<string> result = new List<string>(
                new string[] { 
                    "1", "2", "3", "4", "5", "6", "7", "8",
                    "2", "3", "4", "5", "6", "7", "8",
                    "4", "5", "6", "7", "8",
                    "6", "7", "8",
                    "8"
                }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
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
                //assume DegradeToMultiItemAddRemoveReplaceTransformation and JoinTransformation have been
                //tested properly
                var degradedFrame = frame;

                return
                    CascadeTransformation<string, IVersionedEnumerable<string>>.Create(
                        StaticEnumerable<IVersionedEnumerable<string>>.Create(
                            new IVersionedEnumerable<string>[] { 
                                SkipTransformation<string>.Create(degradedFrame,0),
                                SkipTransformation<string>.Create(degradedFrame,1),
                                SkipTransformation<string>.Create(degradedFrame,3),
                                SkipTransformation<string>.Create(degradedFrame,5),
                                SkipTransformation<string>.Create(degradedFrame,7),
                                SkipTransformation<string>.Create(degradedFrame,9),
                            }
                        )
                    );
            }

            public override string Prefix
            {
                get { return "Join SkipTransformation<string>.Create(frame,0/1/3/5/7/9)"; }
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
                return SkipTransformation<string>.Create(frame, 0);
            }

            public override string Prefix
            {
                get { return "SkipTransformation<string>.Create(DegradeToMultiItemAddRemoveReplaceTransformation<string>.Create(frame), 0)"; }
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
            var r = SkipTransformation<int>.Create(null, 0);
            Assert.IsNull(r, "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[]{1}),
                3, 4,
                (s, i) => SkipTransformation<int>.Create(s, i),
                "SkipTransformation<int>"
            );
        }

        static IEnumerable<string> CreateConcurrencyTestPipeline(IVersionedEnumerable<string> frame)
        {
            //assume DegradeToMultiItemAddRemoveReplaceTransformation and JoinTransformation have been
            //tested properly
            var degradedFrame = frame;

            return
                CascadeTransformation<string, IVersionedEnumerable<string>>.Create(
                    StaticEnumerable<IVersionedEnumerable<string>>.Create(
                        new IVersionedEnumerable<string>[] { 
                                SkipTransformation<string>.Create(degradedFrame,0),
                                SkipTransformation<string>.Create(degradedFrame,1),
                                SkipTransformation<string>.Create(degradedFrame,3),
                                SkipTransformation<string>.Create(degradedFrame,5),
                                SkipTransformation<string>.Create(degradedFrame,7),
                                SkipTransformation<string>.Create(degradedFrame,10),
                            }
                    )
                );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {


            List<string> source = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" }
            );

            List<string> result = new List<string>(
                            new string[] { 
                    "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                    "2", "3", "4", "5", "6", "7", "8", "9", "10",
                    "4", "5", "6", "7", "8", "9", "10",
                    "6", "7", "8", "9", "10",
                    "8", "9", "10"
                }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                CreateConcurrencyTestPipeline,
                "CreateConcurrencyTestPipeline"
            );
        }
    }
}
