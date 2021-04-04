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
using Obtics;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for DistinctTransformationTest
    /// </summary>
    [TestClass]
    public class DistinctTransformationTest
    {
        public DistinctTransformationTest()
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
            static List<string> result = new List<string>(
                new string[] { "A", "B", "C", "D", "E" }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
            }

            static List<string> source = new List<string>(
                new string[] { "A", "B", "C", "A", "D", "B", "A", "E", "D", "A" }
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

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<string> frame)
            {
                //assume DegradeToSingleItemAddRemoveTransformation is tested properly
                return DistinctTransformation<string>.Create(frame, ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "new DistinctTransformation<string>( new DegradeToSingleItemAddRemoveTransformation<string>(frame) )"; }
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
                return DistinctTransformation<string>.Create(frame, ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "new DistinctTransformation<string>( new DegradeToSingleItemAddRemoveTransformation<string>(frame) )"; }
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
            Assert.IsNull(DistinctTransformation<string>.Create(null, ObticsEqualityComparer<string>.Default), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_comparer()
        {
            Assert.IsNull(DistinctTransformation<string>.Create(StaticEnumerable<string>.Create(new string[0]), null), "Should return null when comparer is null.");
        }


        class dummycomparer : IEqualityComparer<string>
        {
            #region IEqualityComparer<string> Members

            public bool Equals(string x, string y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(string obj)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<string>.Create(new string[0]), StaticEnumerable<string>.Create(new string[]{ "a" }),
                (IEqualityComparer<string>)ObticsEqualityComparer<string>.Default, new dummycomparer(),
                (s,c) => DistinctTransformation<string>.Create(s,c),
                "DistinctTransformation<string>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<string> source = new List<string>(
                            new string[] { "A", "B", "C", "A", "D", "B", "A", "E", "D", "A", "F", "E", "D" }
                        );

            List<string> result = new List<string>(
                new string[] { "A", "B", "C", "D", "E" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                s => DistinctTransformation<string>.Create(s, ObticsEqualityComparer<string>.Default),
                "DistinctTransformation<string>.Create(DegradeToSingleItemAddRemoveTransformation<string>.Create(s))"
            );
        }
    }
}
