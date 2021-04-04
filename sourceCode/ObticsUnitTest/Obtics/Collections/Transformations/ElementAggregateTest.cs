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
using Obtics;
using Obtics.Values.Transformations;
using Obtics.Values;
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for ElementAggregateTest
    /// </summary>
    [TestClass]
    public class ElementAggregateTest
    {
        public ElementAggregateTest()
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

        [TestMethod]
        public void ConcurrencyTest1()
        {
            List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }
            );

            AsyncTestRunnerForCollectionAggregate.Run(
                source.ToArray(),
                "1:4:8:1",
                frame => 
                {
                    var extendedFrame = global::Obtics.Collections.ObservableEnumerable.Concat(frame, source);

                    return
                        MultiTransformation<string>.Create(
                            args => args[0].Value.ToString() + ":" + args[1].Value.ToString() + ":" + args[2].Value.ToString() + ":" + args[3].Value.ToString(),
                            (IInternalValueProvider<int>)ElementAggregate<int>.Create(extendedFrame, 0),
                            (IInternalValueProvider<int>)ElementAggregate<int>.Create(extendedFrame, 3),
                            (IInternalValueProvider<int>)ElementAggregate<int>.Create(extendedFrame, 7),
                            (IInternalValueProvider<int>)ElementAggregate<int>.Create(extendedFrame, 10)
                        );
                },
                "CrossJoinTransformation<string>.Create("
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int?[] { 1, 2, 3, null, 4, 5, 6, 7, 8, 9, null, null, 10, null },
                new int?[] { 2, 4, null, 6, 8, null, 10, 1, 3, 5, null, 7, null, 9 },
                new int?[] { 7, null, 2, null, 5, null, 1, 9, null, 3, 2, null, 6, null, 8, null, 8, 9, null, 11 },
                new int?[] { null, 3, 5, null, 6, 3, 1, 3, 2, null, 5, 5, null, 6, 2 },
                9,
                (frame1, frame2, frame3, frame4) => 
                    ElementAggregate<int>.Create( 
                        (IVersionedEnumerable<int>)ObservableEnumerable.Select( 
                            ObservableEnumerable.Where( 
                                ObservableEnumerable.Select<IValueProvider<int?>,int?>( 
                                    ObservableEnumerable.Static( new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 } ), 
                                    vp => vp
                                ), 
                                v => v.HasValue 
                            ), 
                            v => v.Value 
                        ), 
                        0
                    ).OnException((ArgumentOutOfRangeException ex) => 333)
                    ,
                "ElementAggregate<int>.Create( (IVersionedEnumerable<int>)ObservableEnumerable.Select( ObservableEnumerable.Where( ObservableEnumerable.Select<IValueProvider<int?>,int?>( ObservableEnumerable.Static( new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 } ), vp => vp), v => v.HasValue ), v => v.Value ), 0)"
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
                9,
                (frame1, frame2, frame3, frame4) => 
                    ElementAggregate<int>.Create(
                        (IVersionedEnumerable<int>)ObservableEnumerable.Select(
                            ObservableEnumerable.Where( 
                                NotifyVpcTransformation<IValueProvider<int?>, int?>.Create(
                                    (IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(
                                        new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }
                                     ), 
                                     f => f
                                ), 
                                t => t.Second.HasValue
                            ), 
                            t => t.Second.GetValueOrDefault()
                        ), 
                        0
                    ).OnException((ArgumentOutOfRangeException ex) => 333),
                "ObservableEnumerable.Sum( ObservableEnumerable.Select(NotifyVpcTransformation<IValueProvider<int?>, int?>.Create((IVersionedEnumerable<IValueProvider<int?>>)ObservableEnumerable.Static(new IValueProvider<int?>[] { frame1, frame2, frame3, frame4 }), f => f), t => t.Second.GetValueOrDefault()) )"
            );
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForAggregate<int, string>
        {
            public override string ExpectedResult
            { get { return "1:4:8:222"; } }

            static List<int> source = new List<int>(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }
            );

            public override List<int> SourceSequence
            { get { return source; } }

            static List<int> filler = new List<int>(
                new int[] { int.MaxValue - 4, int.MinValue + 5, int.MaxValue - 3, int.MaxValue - 18, 14, 12 }
            );

            public override List<int> FillerItems
            { get { return filler; } }

            public override IValueProvider<string> CreatePipeline(FrameIEnumerable<int> frame)
            {
                return MultiTransformation<string>.Create(
                    args => args[0].Value.ToString() + ":" + args[1].Value.ToString() + ":" + args[2].Value.ToString() + ":" + args[3].Value.ToString(),
                    ElementAggregate<int>.Create(frame, 0)._OnException(_ => true, (ArgumentOutOfRangeException ex) => 222),
                    ElementAggregate<int>.Create(frame, 3)._OnException(_ => true, (ArgumentOutOfRangeException ex) => 222),
                    ElementAggregate<int>.Create(frame, 7)._OnException(_ => true, (ArgumentOutOfRangeException ex) => 222),
                    ElementAggregate<int>.Create(frame, 10)._OnException(_ => true, (ArgumentOutOfRangeException ex) => 222)
                );
            }

            public override string Prefix
            {
                get { return "ElementAggregate<int>.Create(frame, 0/3/7/10)"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();
            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForAggregate<int, int>
        {
            protected override IValueProvider<int> CreateAggregate(FrameIEnumerableNPCNCC<int> frame)
            {
                return ElementAggregate<int>.Create(frame, 0).OnException((ArgumentOutOfRangeException ex) => 222);
            }

            public override string Prefix
            {
                get { return "ElementAggregate<int>.Create(frame, 0)"; }
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
            Assert.IsNull(ElementAggregate<int>.Create(null, 0), "Should return null when source is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IVersionedEnumerable<int>)ObservableEnumerable.Static(new int[0]), (IVersionedEnumerable<int>)ObservableEnumerable.Static(new int[] { 1 }),
                1, 2,
                (s, i) => ElementAggregate<int>.Create(s, i).OnException((ArgumentOutOfRangeException ex) => 333),
                "ElementAggregate<int>"
            );
        }
    }
}
