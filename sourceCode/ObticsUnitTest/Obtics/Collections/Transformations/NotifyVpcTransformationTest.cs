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
using Obtics.Values;
using Obtics;
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for NotifyVpcTransformationTest
    /// </summary>
    [TestClass]
    public class NotifyVpcTransformationTest
    {
        public NotifyVpcTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<string, Tuple<string, int>>
        {
            static List<Tuple<string, int>> result = new List<Tuple<string, int>>(
                new Tuple<string, int>[] { 
                    new Tuple<string,int>( "1", 1 ),
                    new Tuple<string,int>( "2", 2 ),
                    new Tuple<string,int>( "3", 3 ),
                    new Tuple<string,int>( "4", 4 ),
                    new Tuple<string,int>( "5", 5 ),
                    new Tuple<string,int>( "6", 6 ),
                    new Tuple<string,int>( "7", 7 ),
                    new Tuple<string,int>( "8", 8 ),
                    new Tuple<string,int>( "9", 9 ),
                    new Tuple<string,int>( "0", 0 ) 
                }
            );

            public override List<Tuple<string, int>> ExpectedResult
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

            public override IEnumerable<Tuple<string, int>> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return NotifyVpcTransformation<string,int>.Create(frame, s => ValueProvider.Static(int.Parse(s)));
            }

            public override string Prefix
            {
                get { return "new NotifyVpcTransformation<string,int>(frame, s => ValueProvider.Static(int.Parse(s)))"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }


        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<string, Tuple<string, int>>
        {
            protected override IEnumerable<Tuple<string, int>> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return NotifyVpcTransformation<string, int>.Create(frame, s => ValueProvider.Static(int.Parse(s)));
            }

            public override string Prefix
            {
                get { return "new NotifyVpcTransformation<string,int>(frame, s => ValueProvider.Static(int.Parse(s)))"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }
        class CorrectnessRunner2 : CollectionTestSequenceRunnerForTransformation<int, Tuple<int, int>>
        {
            static List<Tuple<int, int>> result = new List<Tuple<int, int>>(
                new Tuple<int, int>[] { 
                    new Tuple<int,int>( 0, 1 ),
                    new Tuple<int,int>( 1, 2 ),
                    new Tuple<int,int>( 2, 3 ),
                    new Tuple<int,int>( 3, 4 ),
                    new Tuple<int,int>( 4, 5 ),
                    new Tuple<int,int>( 5, 6 ),
                    new Tuple<int,int>( 6, 7 ),
                    new Tuple<int,int>( 7, 8 )
                }
            );

            public override List<Tuple<int, int>> ExpectedResult
            {
                get { return result; }
            }

            static List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }
            );

            public override List<int> SourceSequence
            {
                get { return source; }
            }

            static List<int> filler = new List<int>(
                new int[] { 9, 10, 11, 12, 13, 14 }
            );

            public override List<int> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<Tuple<int, int>> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return NotifyVpcTransformation<int, int>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }), s => ElementAggregate<int>.Create(frame, s).OnException((System.ArgumentOutOfRangeException ex) => 222));
            }

            public override string Prefix
            {
                get { return "new NotifyVpcTransformation<int, int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, s => ElementAggregate<int>.Create(frame, s))"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner2 : CollectionDeterministicEventRegistrationRunnerForTransformation<int, Tuple<int, int>>
        {
            protected override IEnumerable<Tuple<int, int>> CreateTransformation(FrameIEnumerableNPCNCC<int> frame)
            {
                return NotifyVpcTransformation<int, int>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }), s => ElementAggregate<int>.Create(frame, s).OnException((System.ArgumentOutOfRangeException ex) => 222));
            }

            public override string Prefix
            {
                get { return "new NotifyVpcTransformation<int, int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, s => ElementAggregate<int>.Create(frame, s))"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            var runner = new DeterministicEventRegistrationRunner2();
            runner.Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(NotifyVpcTransformation<int, int>.Create(null, i => ValueProvider.Static(i + 1)), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_converter()
        {
            Assert.IsNull(NotifyVpcTransformation<int, int>.Create(StaticEnumerable<int>.Create(new int[0]), null), "Should return null when converter is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[] { 1 }),
                new System.Func<int, IValueProvider<int>>(i => ValueProvider.Static(i)), i => ValueProvider.Static(i + 1),
                (s, c) => NotifyVpcTransformation<int, int>.Create(s, c),
                "NotifyVpcTransformation<int, int>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest1()
        {
            List<Tuple<string, int>> result = new List<Tuple<string, int>>(
                new Tuple<string, int>[] { 
                    new Tuple<string,int>( "1", 1 ),
                    new Tuple<string,int>( "2", 2 ),
                    new Tuple<string,int>( "3", 3 ),
                    new Tuple<string,int>( "4", 4 ),
                    new Tuple<string,int>( "5", 5 ),
                    new Tuple<string,int>( "6", 6 ),
                    new Tuple<string,int>( "7", 7 ),
                    new Tuple<string,int>( "8", 8 ),
                    new Tuple<string,int>( "9", 9 ),
                    new Tuple<string,int>( "0", 0 ) 
                }
            );

            List<string> source = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "12", "13", "14" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => NotifyVpcTransformation<string, int>.Create(frame, s => ValueProvider.Static(int.Parse(s))),
                "NotifyVpcTransformation<string,int>.Create(frame, s => ValueProvider.Static(int.Parse(s)))"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            List<Tuple<int, int>> result = new List<Tuple<int, int>>(
                new Tuple<int, int>[] { 
                    new Tuple<int,int>( 0, 1 ),
                    new Tuple<int,int>( 1, 2 ),
                    new Tuple<int,int>( 2, 3 ),
                    new Tuple<int,int>( 3, 4 ),
                    new Tuple<int,int>( 4, 5 ),
                    new Tuple<int,int>( 5, 6 ),
                    new Tuple<int,int>( 6, 7 ),
                    new Tuple<int,int>( 7, 8 ),
                    new Tuple<int,int>( 8, 9 ),
                    new Tuple<int,int>( 9, 10 )
                }
            );

            List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => NotifyVpcTransformation<int, int>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), s => ElementAggregate<int>.Create(frame, s).OnException((System.ArgumentOutOfRangeException ex) => 333)),
                "NotifyVpcTransformation<int, int>.Create( StaticEnumerable<int>.Create( new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } ), s => ElementAggregate<int>.Create(frame, s))"
            );
        }

        [TestMethod]
        public void ConcurrencyTest3()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int?[] { 1, 2, 3, null, 4, 5, 6, 7, 8, 9, null, null, 10, null },
                new int?[] { 2, 4, null, 6, 8, null, 10, 1, 3, 5, null, 7, null, 9 },
                new int?[] { 7, null, 2, null, 5, null, 1, 9, null, 3, 2, null, 6, null, 8, null, 8, 9, null, 11 },
                new int?[] { null, 3, 5, null, 6, 3, 1, 3, 2, null, 5, 5, null, 6, 2 },
                22,
                (frame1, frame2, frame3, frame4) => ObservableEnumerable.Sum( ObservableEnumerable.Select(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.GetValueOrDefault()) ),
                "ObservableEnumerable.Sum( ObservableEnumerable.Select(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.GetValueOrDefault()) )"
            );
        }
    }
}
