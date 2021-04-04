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
using OC = Obtics.Collections;
using Obtics;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for LookupTransformationTest
    /// </summary>
    [TestClass]
    public class LookupTransformationTest
    {
        public LookupTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<Tuple<string, string>, IGrouping<string, string>>
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
                    new Tuple<string,string>( "EA", "E" )
                }
            );

            public override List<Tuple<string, string>> SourceSequence
            {
                get { return source; }
            }

            public override List<IGrouping<string, string>> ExpectedResult
            {
                get { return null; }
            }

            public override bool VerifyResult(object tail, string description, int count)
            {
                var grps = new List<IGrouping<string, string>>((IEnumerable<IGrouping<string, string>>)tail);

                bool res = 
                    grps.Select(grp => grp.Key).SequenceEqual(new string[] { "A", "C", "E", "B", "F" })
                    && Enumerable.SequenceEqual(grps[0], new string[] { "AB", "AD", "AC", "AB" })
                    && Enumerable.SequenceEqual(grps[1], new string[] { "CD" })
                    && Enumerable.SequenceEqual(grps[2], new string[] { "EF", "EA", "EA" })
                    && Enumerable.SequenceEqual(grps[3], new string[] { "BE" })
                    && Enumerable.SequenceEqual(grps[4], new string[] { "FA" });

                Assert.IsTrue(res, Prefix + " (" + count.ToString() + ") after collection changes " + description + ". The result collection is not correct.");

                return res;
            }

            static List<Tuple<string, string>> filler = new List<Tuple<string, string>>(
                new Tuple<string, string>[] { 
                    new Tuple<string,string>( "AD", "A" ),
                    new Tuple<string,string>( "QE", "Q" ),
                    new Tuple<string,string>( "FA", "F" ),
                    new Tuple<string,string>( "QA", "Q" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "DA", "D" )
                }
            );

            public override List<Tuple<string, string>> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<IGrouping<string, string>> CreatePipeline(FrameIEnumerable<Tuple<string, string>> frame)
            {
                return LookupTransformation<string, string>.Create(frame, ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "new LookupTransformation<string, string>(frame, EqualityComparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<Tuple<string, string>, IGrouping<string, string>>
        {
            protected override IEnumerable<IGrouping<string, string>> CreateTransformation(FrameIEnumerableNPCNCC<Tuple<string, string>> frame)
            {
                return LookupTransformation<string, string>.Create(frame, ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "new LookupTransformation<string, string>(frame, EqualityComparer<string>.Default)"; }
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
            Assert.IsNull(LookupTransformation<string, string>.Create(null, ObticsEqualityComparer<string>.Default), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_comparer()
        {
            Assert.IsNull(LookupTransformation<string, string>.Create(new Tuple<string, string>[0], null), "Should return null when comparer is null.");
        }


        class Comparer : IEqualityComparer<string>
        {
            #region IEqualityComparer<string> Members

            public bool Equals(string x, string y)
            {
                return false;
            }

            public int GetHashCode(string obj)
            {
                return 0;
            }

            #endregion
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                new Tuple<string, string>[0], new Tuple<string, string>[0],
                (IEqualityComparer<string>)ObticsEqualityComparer<string>.Default, new Comparer(),
                (s, c) => LookupTransformation<string, string>.Create(s, c),
                "LookupTransformation<string, string>"
            );
        }


        bool VerifyConcurrencyTestResult(IEnumerable<IGrouping<string, string>> tail)
        {
            var grps = new List<IGrouping<string, string>>(tail);

            return
                grps.Select(grp => grp.Key).SequenceEqual(new string[] { "A", "C", "E", "B", "F" })
                && Enumerable.SequenceEqual(grps[0], new string[] { "AB", "AD", "AC", "AB" })
                && Enumerable.SequenceEqual(grps[1], new string[] { "CD" })
                && Enumerable.SequenceEqual(grps[2], new string[] { "EF", "EA", "EA" })
                && Enumerable.SequenceEqual(grps[3], new string[] { "BE" })
                && Enumerable.SequenceEqual(grps[4], new string[] { "FA" });
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            List<Tuple<string, string>> source = new List<Tuple<string, string>>(
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
                    new Tuple<string,string>( "EA", "E" ),
                    new Tuple<string,string>( "CD", "C" ),
                    new Tuple<string,string>( "EF", "E" ),
                    new Tuple<string,string>( "AD", "A" )
                }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                VerifyConcurrencyTestResult,
                frame => LookupTransformation<string, string>.Create(frame, ObticsEqualityComparer<string>.Default),
                "LookupTransformation<string, string>.Create(frame, EqualityComparer<string>.Default)"
            );
        }
    }
}
