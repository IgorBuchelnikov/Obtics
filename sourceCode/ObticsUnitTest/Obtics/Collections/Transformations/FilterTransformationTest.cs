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
using Obtics.Values;
using Obtics;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for FilterTransformationTest
    /// </summary>
    [TestClass]
    public class FilterTransformationTest
    {
        public FilterTransformationTest()
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
                new int[] { 3, 6, 3, 9 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<int> source = new List<int>(
                new int[] { 1, 3, 2, 6, 4, 8, 3, 5, 2, 9 }
            );

            public override List<int> SourceSequence
            {
                get { return source; }
            }

            static List<int> filler = new List<int>(
                new int[] { 11, 12, 13, 9, 15, 16, 17, 18, 19, 20 }
            );

            public override List<int> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<int> frame)
            {
                //assume DegradeToSingleItemAddRemoveTransformation is tested properly
                return FilterTransformation<int>.Create(frame, i => i % 3 == 0);
            }

            public override string Prefix
            {
                get { return "new FilterTransformation<int>(frame, i => i % 3 == 0)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }


        class CorrectnessRunner2 : CollectionTestSequenceRunnerForTransformation<Tuple<string, string>, Tuple<string, string>>
        {
            static List<Tuple<string, string>> source = new List<Tuple<string, string>>(
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

            public override List<Tuple<string, string>> SourceSequence
            {
                get { return source; }
            }

            static List<Tuple<string, string>> result = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "AD", "A" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "AB", "A" ),
                }
            );

            public override List<Tuple<string, string>> ExpectedResult
            {
                get { return result; }
            }

            static List<Tuple<string, string>> filler = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AD", "A" ),
                    new Tuple<string,string>( "BE", "B" ),
                    new Tuple<string,string>( "FA", "F" ),
                    new Tuple<string,string>( "EA", "E" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "DA", "D" )
                }
            );

            public override List<Tuple<string, string>> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<Tuple<string, string>> CreatePipeline(FrameIEnumerable<Tuple<string, string>> frame)
            {
                return FilterTransformation<Tuple<string,string>>.Create(frame, t => t.Second == "A");
            }

            public override string Prefix
            {
                get { return "FilterTransformation<Tuple<string,string>>.Create(frame, t => t.Second == \"A\");"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<int, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<int> frame)
            {
                return FilterTransformation<int>.Create(frame, i => i % 3 == 0);
            }

            public override string Prefix
            {
                get { return "new FilterTransformation<int>(frame, i => i % 3 == 0)"; }
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
            Assert.IsNull(FilterTransformation<int>.Create(null, i => i % 3 == 0), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_predicate()
        {
            Assert.IsNull(FilterTransformation<int>.Create( StaticEnumerable<int>.Create(new int[0]), null), "Should return null when predicate is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[]{1}),
                new System.Func<int,bool>( i => i % 3 == 0 ), i => i % 3 == 1,
                (s, p) => FilterTransformation<int>.Create(s, p),
                "FilterTransformation<int>"
            );
        }
        [TestMethod]
        public void ConcurrencyTest1()
        {
            List<int> source = new List<int>(
                            new int[] { 1, 3, 2, 6, 4, 8, 3, 5, 2, 9, 10, 11, 12 }
                        );

            List<int> result = new List<int>(
                            new int[] { 3, 6, 3, 9 }
                        );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => FilterTransformation<int>.Create(frame, i => i % 3 == 0),
                "FilterTransformation<int>.Create(frame, i => i % 3 == 0)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int?[] { 1, 2, 3, null, 4, 5, 6, 7, 8, 9, null, null, 10, null },
                new int?[] { 2, 4, null, 6, 8, null, 10, 1, 3, 5, null, 7, null, 9 },
                new int?[] { 7, null, 2, null, 5, null, 1, 9, null, 3, 2, null, 6, null, 8, null, 8, 9, null, 11 },
                new int?[] { null, 3, 5, null, 6, 3, 1, 3, 2, null, 5, 5, null, 6, 2, null },
                2,
                (frame1, frame2, frame3, frame4) => CountAggregate<global::Obtics.Tuple<IValueProvider<int?>, int?>>.Create(FilterTransformation<global::Obtics.Tuple<IValueProvider<int?>, int?>>.Create(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.HasValue)),
                "CountAggregate<Tuple<IValueProvider<int?>, int?>>.Create(FilterTransformation<Tuple<IValueProvider<int?>, int?>>.Create(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.HasValue))"
            );
        }
    }
}
