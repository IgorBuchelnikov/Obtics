using System;
using System.Text;
using System.Collections.Generic;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Collections.Transformations;
using Obtics.Values;
using Obtics.Collections;
using Obtics;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for DynamicSortTransformationTest
    /// </summary>
    [TestClass]
    public class DynamicSortTransformationTest
    {
        public DynamicSortTransformationTest()
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
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            static List<string> result = new List<string>(
                new string[] { "AB", "AC", "AD", "BA", "BB", "BD", "CA", "CC", "DA", "DE" }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
            }

            static List<string> filler = new List<string>(
                new string[] { "AA", "DE", "CB", "BA", "BA", "DA" }
            );

            public override List<string> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 2)), Comparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static( s.Substring(0, 1) ), Comparer<string>.Default)"; }
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
                return DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 1)), Comparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static( s.Substring(0, 1) ), Comparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }

        class CorrectnessRunner2 : CollectionTestSequenceRunnerForTransformation<string, int>
        {
            static List<string> source = new List<string>(
                new string[] { "D", "I", "A", "J", "B", "E", "G", "H", "C", "F" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            static List<int> result = new List<int>(
                new int[] { 2, 4, 8, 0, 5, 9, 6, 7, 1, 3 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<string> filler = new List<string>(
                new string[] { "A", "D", "C", "B", "B", "D" }
            );

            public override List<string> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return DynamicSortTransformation<int, string>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), i => ElementAggregate<string>.Create(frame, i).OnException((ArgumentOutOfRangeException ex) => string.Empty), Comparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "DynamicSortTransformation<int, string>.Create( new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, i => ElementAggregate<string>.Create(frame, i), Comparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner2 : CollectionDeterministicEventRegistrationRunnerForTransformation<string, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return DynamicSortTransformation<int, string>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), i => ElementAggregate<string>.Create(frame, i).OnException((ArgumentOutOfRangeException ex) => "222"), Comparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "DynamicSortTransformation<int, string>.Create( new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, i => ElementAggregate<string>.Create(frame, i), Comparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            var runner = new DeterministicEventRegistrationRunner2();
            runner.Run();
        }


        class CorrectnessRunner3 : CollectionTestSequenceRunnerForTransformation<string, string>
        {
            static List<string> source = new List<string>(
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            static List<string> result = new List<string>(
                new string[] { "AB", "AC", "AD", "BA", "BB", "BD", "CA", "CC", "DA", "DE" }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
            }

            static List<string> filler = new List<string>(
                new string[] { "AA", "DE", "CB", "BA", "BA", "DA" }
            );

            public override List<string> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return (DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 1)), Comparer<string>.Default)).CreateOrderedEnumerable(s => ValueProvider.Static(s.Substring(1, 1)), Comparer<string>.Default, false);
            }

            public override string Prefix
            {
                get { return "(DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static( s.Substring(0, 1) ), Comparer<string>.Default)).CreateOrderedEnumerable(s => ValueProvider.Static(s.Substring(1, 1)), Comparer<string>.Default, false)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner3 : CollectionDeterministicEventRegistrationRunnerForTransformation<string, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return (DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 1)), Comparer<string>.Default)).CreateOrderedEnumerable(s => ValueProvider.Static(s.Substring(1, 1)), Comparer<string>.Default, false);
            }

            public override string Prefix
            {
                get { return "(DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 1)), Comparer<string>.Default)).CreateOrderedEnumerable(s => ValueProvider.Static(s.Substring(1, 1)), Comparer<string>.Default, false)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest3()
        {
            var runner = new DeterministicEventRegistrationRunner3();
            runner.Run();
        }


        class CorrectnessRunner4 : CollectionTestSequenceRunnerForTransformation<string, int>
        {
            static List<string> source = new List<string>(
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            static List<int> result = new List<int>(
                new int[] { 8, 4, 2, 0, 9, 5, 7, 6, 3, 1 }
            );

            public override List<int> ExpectedResult
            {
                get { return result; }
            }

            static List<string> filler = new List<string>(
                new string[] { "AA", "DE", "CB", "BA", "BA", "DA" }
            );

            public override List<string> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<int> CreatePipeline(FrameIEnumerable<string> frame)
            {
                var firsts = frame.Select(s => s.Substring(0, 1));
                var seconds = frame.Select(s => s.Substring(1, 1));
                return 
                    DynamicSortTransformation<int, string>.Create(
                        StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), 
                        i => 
                            ElementAggregate<string>.Create(firsts.Patched(), i)
                                .OnException((ArgumentOutOfRangeException ex) => "222"), 
                        Comparer<string>.Default
                    )
                    .CreateOrderedEnumerable(
                        i => 
                            ElementAggregate<string>.Create(seconds.Patched(), i)
                                .OnException((ArgumentOutOfRangeException ex) => "222"), 
                        Comparer<string>.Default, false
                    )
                ;
            }

            public override string Prefix
            {
                get { return "(DynamicSortTransformation<int, string>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, i => new ElementAggregate<string>(firsts, i), Comparer<string>.Default)).CreateOrderedEnumerable(i => ElementAggregate<string>.Create(seconds, i), Comparer<string>.Default, false)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest4()
        {
            var runner = new CorrectnessRunner4();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner4 : CollectionDeterministicEventRegistrationRunnerForTransformation<string, int>
        {
            protected override IEnumerable<int> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                var firsts = frame.Select(s => s.Substring(0, 1));
                var seconds = frame.Select(s => s.Substring(1, 1));
                return (DynamicSortTransformation<int, string>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), i => ElementAggregate<string>.Create(firsts.Patched(), i).OnException((ArgumentOutOfRangeException ex) => "222"), Comparer<string>.Default)).CreateOrderedEnumerable(i => ElementAggregate<string>.Create(seconds.Patched(), i).OnException((ArgumentOutOfRangeException ex)=>"333"), Comparer<string>.Default, false);
            }

            public override string Prefix
            {
                get { return "(DynamicSortTransformation<int, string>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, i => new ElementAggregate<string>(firsts, i), Comparer<string>.Default)).CreateOrderedEnumerable(i => ElementAggregate<string>.Create(seconds, i), Comparer<string>.Default, false)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest4()
        {
            var runner = new DeterministicEventRegistrationRunner4();
            runner.Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(DynamicSortTransformation<int, string>.Create(null, i => ValueProvider.Static(i.ToString()), Comparer<string>.Default), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_keySelector()
        {
            Assert.IsNull(DynamicSortTransformation<int, string>.Create( StaticEnumerable<int>.Create(new int[0]), null, Comparer<string>.Default), "Should return null when keySelector is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_comparer()
        {
            Assert.IsNull(DynamicSortTransformation<int, string>.Create( StaticEnumerable<int>.Create(new int[0]), i => ValueProvider.Static(i.ToString()), null), "Should return null when comparer is null.");
        }


        class Comparer : IComparer<string>
        {
            public int Compare(string x, string y)
            { return 0; }
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[] { 1 }),
                new Func<int, IValueProvider<string>>( i => ValueProvider.Static( i.ToString() )), i => ValueProvider.Static(string.Empty),
                (IComparer<string>)Comparer<string>.Default, new Comparer(),
                (s, ks, c) => DynamicSortTransformation<int, string>.Create(s, ks, c),
                "DynamicSortTransformation<int, string>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest1()
        {
            List<string> source = new List<string>(
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB", "AA", "DE", "CB" }
            );

            List<string> result = new List<string>(
                new string[] { "AB", "AC", "AD", "BA", "BB", "BD", "CA", "CC", "DA", "DE" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 2)), Comparer<string>.Default),
                "DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 2)), Comparer<string>.Default)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            List<string> source = new List<string>(
                new string[] { "D", "I", "A", "J", "B", "E", "G", "H", "C", "F", "Q", "J", "I" }

            );

            List<int> result = new List<int>(
                new int[] { 2, 4, 8, 0, 5, 9, 6, 7, 1, 3 }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => DynamicSortTransformation<int, string>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), i => ElementAggregate<string>.Create(frame, i).OnException((ArgumentOutOfRangeException ex) => "333"), Comparer<string>.Default),
                "DynamicSortTransformation<int, string>.Create( StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), i => ElementAggregate<string>.Create(frame, i), Comparer<string>.Default)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest3()
        {
            List<string> source = new List<string>(
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB", "AA", "DE", "CB" }
            );

            List<string> result = new List<string>(
                new string[] { "AB", "AC", "AD", "BA", "BB", "BD", "CA", "CC", "DA", "DE" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => (DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 1)), Comparer<string>.Default)).CreateOrderedEnumerable(s => ValueProvider.Static(s.Substring(1, 1)), Comparer<string>.Default, false),
                "(DynamicSortTransformation<string, string>.Create(frame, s => ValueProvider.Static(s.Substring(0, 1)), Comparer<string>.Default)).CreateOrderedEnumerable(s => ValueProvider.Static(s.Substring(1, 1)), Comparer<string>.Default, false)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest4()
        {
            List<string> source = new List<string>(
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB", "AA", "DE", "CB" }
            );

            List<int> result = new List<int>(
                new int[] { 8, 4, 2, 0, 9, 5, 7, 6, 3, 1 }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => 
                    (
                        DynamicSortTransformation<int, string>.Create(
                            StaticEnumerable<int>.Create(
                                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                            ), 
                            i =>  frame.Select(s => s.Substring(0, 1)).ElementAtOrDefault(i).Select(s => s == null ? "333" : s), 
                            Comparer<string>.Default
                        )
                    ).CreateOrderedEnumerable(
                        i => frame.Select(s => s.Substring(1, 1)).ElementAtOrDefault(i).Select(s => s == null ? "333" : s ),
                        Comparer<string>.Default, 
                        false
                    ),
                "(DynamicSortTransformation<int, string>.Create(StaticEnumerable<int>.Create(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), i => ElementAggregate<string>.Create(frame.Select(s => s.Substring(0, 1)), i), Comparer<string>.Default)).CreateOrderedEnumerable(i => ElementAggregate<string>.Create(frame.Select(s => s.Substring(1, 1)), i), Comparer<string>.Default, false)"
            );
        }
    }
}
