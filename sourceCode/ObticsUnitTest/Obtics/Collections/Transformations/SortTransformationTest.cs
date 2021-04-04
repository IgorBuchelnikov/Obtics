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
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for SortTransformationTest
    /// </summary>
    [TestClass]
    public class SortTransformationTest
    {
        public SortTransformationTest()
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
                return SortTransformation<string,string>.Create(frame, s => s.Substring(0,2), Comparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "new SortTransformation<string,string>(frame, s => s.Substring(0,2), Comparer<string>.Default)"; }
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
                return SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "new SortTransformation<string,string>(frame, s => s.Substring(0,1), Comparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }

        class CorrectnessRunner2 : CollectionTestSequenceRunnerForTransformation<string, string>
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
                return (SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable(s => s.Substring(1, 1), Comparer<string>.Default, false);
            }

            public override string Prefix
            {
                get { return "(new SortTransformation<string, string>(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable( s => s.Substring(1, 1), Comparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();

            runner.RunAllPrepPairs();
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
                new string[] { "AD", "AC", "AB", "BD", "BB", "BA", "CC", "CA", "DE", "DA" }
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
                return (SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable(s => s.Substring(1, 1), Comparer<string>.Default, true);
            }

            public override string Prefix
            {
                get { return "(new SortTransformation<string, string>(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable( s => s.Substring(1, 1), Comparer<string>.Default, true)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner2 : CollectionDeterministicEventRegistrationRunnerForTransformation<string, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return (SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable(s => s.Substring(1, 1), Comparer<string>.Default, false);
            }

            public override string Prefix
            {
                get { return "(new SortTransformation<string, string>(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable( s => s.Substring(1, 1), Comparer<string>.Default, false)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            var runner = new DeterministicEventRegistrationRunner2();
            runner.Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source1()
        {
            Assert.IsNull(SortTransformation<int, int>.Create((IVersionedEnumerable<int>)null, i => i, Comparer<int>.Default), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_source2()
        {
            Assert.IsNull(SortTransformation<int, int>.Create((IVersionedEnumerable<int>)null, i => i, Comparer<int>.Default), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_keySelector1()
        {
            Assert.IsNull(SortTransformation<int, int>.Create(StaticEnumerable<int>.Create(new int[0]), null, Comparer<int>.Default), "Should return null when keySelector is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_keySelector2()
        {
            Assert.IsNull(SortTransformation<int, int>.Create( StaticEnumerable<int>.Create(new int[0]) , null, Comparer<int>.Default), "Should return null when keySelector is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_comparer()
        {
            Assert.IsNull(SortTransformation<int, int>.Create( StaticEnumerable<int>.Create(new int[0]), i => i, null), "Should return null when comparer is null.");
        }

        class Comparer : IComparer<int>
        {

            #region IComparer<int> Members

            public int Compare(int x, int y)
            { return 0; }

            #endregion
        }
        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<int>.Create(new int[0]), StaticEnumerable<int>.Create(new int[] { 1 }),
                new Func<int, int>(i => i), i => -i,
                (IComparer<int>)Comparer<int>.Default, new Comparer(),
                (s, ks, c) => SortTransformation<int, int>.Create(s, ks, c),
                "SortTransformation<int, int>"
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
                frame => SortTransformation<string, string>.Create(frame, s => s.Substring(0, 2), Comparer<string>.Default),
                "SortTransformation<string,string>.Create(frame, s => s.Substring(0,2), Comparer<string>.Default)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            List<string> source = new List<string>(
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB", "AC", "AA", "BE" }
            );

            List<string> result = new List<string>(
                new string[] { "AB", "AC", "AD", "BA", "BB", "BD", "CA", "CC", "DA", "DE" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => (SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable(s => s.Substring(1, 1), Comparer<string>.Default, false),
                "(SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable(s => s.Substring(1, 1), Comparer<string>.Default, false)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest3()
        {
            List<string> source = new List<string>(
                new string[] { "BA", "DE", "AD", "DA", "AC", "BD", "CC", "CA", "AB", "BB", "ED", "BD", "AB" }
            );

            List<string> result = new List<string>(
                new string[] { "AD", "AC", "AB", "BD", "BB", "BA", "CC", "CA", "DE", "DA" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                frame => (SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable(s => s.Substring(1, 1), Comparer<string>.Default, true),
                "(SortTransformation<string, string>.Create(frame, s => s.Substring(0, 1), Comparer<string>.Default)).CreateOrderedEnumerable(s => s.Substring(1, 1), Comparer<string>.Default, true)"
            );
        }
    }
}
