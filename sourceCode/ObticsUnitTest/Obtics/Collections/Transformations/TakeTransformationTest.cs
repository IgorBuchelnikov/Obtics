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
    /// Summary description for TakeTransformationTest
    /// </summary>
    [TestClass]
    public class TakeTransformationTest
    {
        public TakeTransformationTest()
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
                    "1", 
                    "1", "2", "3",
                    "1", "2", "3", "4", "5", 
                    "1", "2", "3", "4", "5", "6", "7", 
                    "1", "2", "3", "4", "5", "6", "7", "8"
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
                                TakeTransformation<string>.Create(degradedFrame,0),
                                TakeTransformation<string>.Create(degradedFrame,1),
                                TakeTransformation<string>.Create(degradedFrame,3),
                                TakeTransformation<string>.Create(degradedFrame,5),
                                TakeTransformation<string>.Create(degradedFrame,7),
                                TakeTransformation<string>.Create(degradedFrame,9),
                            }
                        )
                    );
            }

            public override string Prefix
            {
                get { return "Join new TakeTransformation<string>(frame,0/1/3/5/7/9)"; }
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
                return TakeTransformation<string>.Create(frame, 0);
            }

            public override string Prefix
            {
                get { return "new TakeTransformation<string>(DegradeToMultiItemAddRemoveReplaceTransformation<string>.Create(frame), 0)"; }
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
            Assert.IsNull(TakeTransformation<int>.Create(null, 0), "Should return null when source is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[] { 1 }),
                3, 4,
                (s, i) => TakeTransformation<int>.Create(s, i),
                "TakeTransformation<int>"
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
                                TakeTransformation<string>.Create(degradedFrame,0),
                                TakeTransformation<string>.Create(degradedFrame,1),
                                TakeTransformation<string>.Create(degradedFrame,3),
                                TakeTransformation<string>.Create(degradedFrame,5),
                                TakeTransformation<string>.Create(degradedFrame,7),
                                TakeTransformation<string>.Create(degradedFrame,10),
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
                    "1", 
                    "1", "2", "3",
                    "1", "2", "3", "4", "5", 
                    "1", "2", "3", "4", "5", "6", "7", 
                    "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"
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
