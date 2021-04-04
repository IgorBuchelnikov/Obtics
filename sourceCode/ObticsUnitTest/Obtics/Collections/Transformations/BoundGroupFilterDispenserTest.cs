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
    /// Summary description for BoundGroupFilterDispenserTest
    /// </summary>
    [TestClass]
    public class BoundGroupFilterDispenserTest
    {
        public BoundGroupFilterDispenserTest()
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


        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<Tuple<string, string>, Tuple<string, string>>
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

            public override List<Tuple<string, string>> ExpectedResult
            {
                get { return source; }
            }

            public override List<Tuple<string, string>> SourceSequence
            {
                get { return source; }
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
                return BoundGroupFilterDispenser<string, string>.Create(frame, ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "BoundGroupFilterDispenser<string, string>(frame, EqualityComparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<Tuple<string, string>, Tuple<string, string>>
        {
            protected override IEnumerable<Tuple<string, string>> CreateTransformation(FrameIEnumerableNPCNCC<Tuple<string, string>> frame)
            {
                return BoundGroupFilterDispenser<string, string>.Create(frame, ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "new BoundGroupFilterDispenser<string, string>(frame, EqualityComparer<string>.Default)"; }
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
            Assert.IsNull(BoundGroupFilterDispenser<string, string>.Create(null, ObticsEqualityComparer<string>.Default), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_equalityComparer()
        {
            Assert.IsNull(BoundGroupFilterDispenser<string, string>.Create(StaticEnumerable<Tuple<string, string>>.Create(new Tuple<string, string>[0]), null), "Should return null when equalityComparer is null.");
        }

        class Comparer : IEqualityComparer<int>
        {
            #region IEqualityComparer<int> Members

            public bool Equals(int x, int y)
            { return true; }

            public int GetHashCode(int obj)
            { return 0; }

            #endregion
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<Tuple<int, int>>.Create(new Tuple<int, int>[0]), StaticEnumerable<Tuple<int, int>>.Create(new Tuple<int, int>[] { Tuple.Create(1,2) }),
                (IEqualityComparer<int>)ObticsEqualityComparer<int>.Default, new Comparer(),
                (s, c) => BoundGroupFilterDispenser<int, int>.Create(s, c),
                "BoundGroupFilterDispenser<int, int>"
            );
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
                    new Tuple<string,string>( "DA", "D" ),
                    new Tuple<string,string>( "AD", "A" ),
                    new Tuple<string,string>( "BE", "B" ),
                    new Tuple<string,string>( "FA", "F" )
                }
            );

            List<Tuple<string, string>> result = new List<Tuple<string, string>>(
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

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                s => BoundGroupFilterDispenser<string, string>.Create(s, ObticsEqualityComparer<string>.Default),
                "BoundGroupFilterDispenser<string, string>.Create(s, EqualityComparer<string>.Default)"
            );

        }
    }
}
