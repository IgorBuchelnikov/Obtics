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
    /// Summary description for DictionaryTransformationTest
    /// </summary>
    [TestClass]
    public class DictionaryTransformationTest
    {
        public DictionaryTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<Tuple<string, string>, KeyValuePair<string, string>>
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

            static List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>(
                new KeyValuePair<string, string>[] { 
                    new KeyValuePair<string, string>("A", "AB"),
                    new KeyValuePair<string, string>("C", "CD"),
                    new KeyValuePair<string, string>("E", "EF"),
                    new KeyValuePair<string, string>("B", "BE"),
                    new KeyValuePair<string, string>("F", "FA"),
                    new KeyValuePair<string, string>("D", "DA")
                }
            );

            public override List<KeyValuePair<string, string>> ExpectedResult
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

            public override IEnumerable<KeyValuePair<string, string>> CreatePipeline(FrameIEnumerable<Tuple<string, string>> frame)
            {
                return DictionaryTransformation<string, string>.Create(ObservableEnumerable.Patched(frame), ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "DictionaryTransformation<string,string>.Create(frame, EqualityComparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }

        class CorrectnessRunner2 : CollectionTestSequenceRunnerForTransformation<Tuple<string, string>, string>
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

            static List<string> result = new List<string>(
                new string[] { "A", "C", "E", "B", "F", "D" }
            );

            public override List<string> ExpectedResult
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

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<Tuple<string, string>> frame)
            {
                return ((IDictionary<string,string>)DictionaryTransformation<string,string>.Create(frame, ObticsEqualityComparer<string>.Default)).Keys;
            }

            public override string Prefix
            {
                get { return "DictionaryTransformation<string,string>.Create(frame, EqualityComparer<string>.Default).Keys"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest2()
        {
            var runner = new CorrectnessRunner2();

            runner.RunAllPrepPairs();
        }
        class CorrectnessRunner3 : CollectionTestSequenceRunnerForTransformation<Tuple<string, string>, string>
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

            static List<string> result = new List<string>(
                new string[] { "AB", "CD", "EF", "BE", "FA", "DA" }
            );

            public override List<string> ExpectedResult
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

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<Tuple<string, string>> frame)
            {
                return ((IDictionary<string,string>)DictionaryTransformation<string,string>.Create(frame, ObticsEqualityComparer<string>.Default)).Values;
            }

            public override string Prefix
            {
                get { return "DictionaryTransformation<string,string>.Create(frame, EqualityComparer<string>.Default).Values"; }
            }
        }

        [TestMethod]
        public void CorrectnessTest3()
        {
            var runner = new CorrectnessRunner3();

            runner.RunAllPrepPairs();
        }

        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<Tuple<string, string>, KeyValuePair<string, string>>
        {
            protected override IEnumerable<KeyValuePair<string, string>> CreateTransformation(FrameIEnumerableNPCNCC<Tuple<string, string>> frame)
            {
                return DictionaryTransformation<string,string>.Create(frame, ObticsEqualityComparer<string>.Default);
            }

            public override string Prefix
            {
                get { return "DictionaryTransformation<string,string>.Create(SequencingPatchTransformation_Statics.Patched(frame), EqualityComparer<string>.Default)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }

        class DeterministicEventRegistrationRunner2 : CollectionDeterministicEventRegistrationRunnerForTransformation<Tuple<string, string>, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<Tuple<string, string>> frame)
            {
                return ((IDictionary<string,string>)DictionaryTransformation<string,string>.Create((frame), ObticsEqualityComparer<string>.Default)).Keys;
            }

            public override string Prefix
            {
                get { return "DictionaryTransformation<string,string>.Create((frame), EqualityComparer<string>.Default).Keys"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            var runner = new DeterministicEventRegistrationRunner2();
            runner.Run();
        }

        class DeterministicEventRegistrationRunner3 : CollectionDeterministicEventRegistrationRunnerForTransformation<Tuple<string, string>, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<Tuple<string, string>> frame)
            {
                return ((IDictionary<string,string>)DictionaryTransformation<string, string>.Create((frame), ObticsEqualityComparer<string>.Default)).Values;
            }

            public override string Prefix
            {
                get { return "DictionaryTransformation<string,string>.Create((frame), EqualityComparer<string>.Default).Values"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest3()
        {
            var runner = new DeterministicEventRegistrationRunner3();
            runner.Run();
        }

        [TestMethod, ExpectedException(typeof(System.NotSupportedException))]
        public void AddNotSupported1Test()
        {
            var r = DictionaryTransformation<string,string>.Create((new Tuple<string, string>[0]), ObticsEqualityComparer<string>.Default);
            ((IDictionary<string,string>)r).Add("A", "B");
        }

        [TestMethod, ExpectedException(typeof(System.NotSupportedException))]
        public void AddNotSupported2Test()
        {
            var r = DictionaryTransformation<string,string>.Create(new Tuple<string, string>[0], ObticsEqualityComparer<string>.Default);
            ((IDictionary<string, string>)r).Add(new KeyValuePair<string, string>("A", "B"));
        }

        [TestMethod, ExpectedException(typeof(System.NotSupportedException))]
        public void RemoveNotSupported1Test()
        {
            var r = DictionaryTransformation<string,string>.Create(new Tuple<string, string>[0], ObticsEqualityComparer<string>.Default);
            ((IDictionary<string, string>)r).Remove("A");
        }

        [TestMethod, ExpectedException(typeof(System.NotSupportedException))]
        public void RemoveNotSupported2Test()
        {
            var r = DictionaryTransformation<string,string>.Create((new Tuple<string, string>[0]), ObticsEqualityComparer<string>.Default);
            ((IDictionary<string, string>)r).Remove(new KeyValuePair<string, string>("A", "B"));
        }

        [TestMethod, ExpectedException(typeof(System.NotSupportedException))]
        public void ClearNotSupportedTest()
        {
            var r = DictionaryTransformation<string,string>.Create((new Tuple<string, string>[0]), ObticsEqualityComparer<string>.Default);
            ((IDictionary<string, string>)r).Clear();
        }

        [TestMethod, ExpectedException(typeof(System.NotSupportedException))]
        public void SetItemNotSupportedTest()
        {
            var r = DictionaryTransformation<string,string>.Create((new Tuple<string, string>[0]), ObticsEqualityComparer<string>.Default);
            ((IDictionary<string, string>)r)["X"] = "B";
        }

        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(DictionaryTransformation<string, string>.Create(null, ObticsEqualityComparer<string>.Default), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_comparer()
        {
            Assert.IsNull(DictionaryTransformation<string, string>.Create((new Tuple<string, string>[0]), null), "Should return null when comparer is null.");
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
                (s,c) => DictionaryTransformation<string,string>.Create((s), c),
                "DictionaryTransformation<string, string>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest1()
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
                    new Tuple<string,string>( "EA", "E" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "DA", "D" )
                }
            );

            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>(
                new KeyValuePair<string, string>[] { 
                    new KeyValuePair<string, string>("A", "AB"),
                    new KeyValuePair<string, string>("C", "CD"),
                    new KeyValuePair<string, string>("E", "EF"),
                    new KeyValuePair<string, string>("B", "BE"),
                    new KeyValuePair<string, string>("F", "FA"),
                    new KeyValuePair<string, string>("D", "DA")
                }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                s => DictionaryTransformation<string, string>.Create(s, ObticsEqualityComparer<string>.Default),
                "DictionaryTransformation<string, string>.Create(s, EqualityComparer<string>.Default)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
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
                    new Tuple<string,string>( "EA", "E" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "DA", "D" )
                }
            );

            List<string> result = new List<string>(
                new string[] { "A", "C", "E", "B", "F", "D" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                s => ((IDictionary<string,string>)DictionaryTransformation<string, string>.Create(s, ObticsEqualityComparer<string>.Default)).Keys,
                "DictionaryTransformation<string,string>.Create(s, EqualityComparer<string>.Default).Keys"
            );
        }

        [TestMethod]
        public void ConcurrencyTest3()
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
                    new Tuple<string,string>( "EA", "E" ),
                    new Tuple<string,string>( "AC", "A" ),
                    new Tuple<string,string>( "AB", "A" ),
                    new Tuple<string,string>( "DA", "D" )
                }
            );

            List<string> result = new List<string>(
                new string[] { "AB", "CD", "EF", "BE", "FA", "DA" }
            );

            AsyncTestRunnerForCollectionTransformation.Run(
                source.ToArray(),
                result.ToArray(),
                s => ((IDictionary<string,string>)DictionaryTransformation<string, string>.Create(s, ObticsEqualityComparer<string>.Default)).Values,
                "DictionaryTransformation<string,string>.Create(s, EqualityComparer<string>.Default).Values"
            );
        }
    }
}
