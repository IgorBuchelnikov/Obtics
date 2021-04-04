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
using Obtics.Async;
using System.Threading;
using Obtics;
using Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for UnorderedBufferTransformationTest
    /// </summary>
    [TestClass]
    public class UnorderedBufferTransformationTest
    {
        public UnorderedBufferTransformationTest()
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

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<string, string>, IWorkQueue
        {
            static List<string> result = new List<string>(
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
            }

            static List<string> source = new List<string>(
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }
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
                return SortTransformation<string,int>.Create(UnorderedBufferTransformation<string>.Create(frame,this,50), s => int.Parse(s), Comparer<int>.Default ) ;
            }

            public override bool VerifyResult(object tail, string description, int count)
            {
                while (workQueue.Count > 0)
                {
                    var t = workQueue.Dequeue();
                    t.First(t.Second);
                }

                return base.VerifyResult(tail, description, count);
            }

            public override string Prefix
            {
                get { return "new UnorderedBufferTransformation<string>(frame,this)"; }
            }

            Queue<Tuple<WaitCallback, object>> workQueue = new Queue<Tuple<WaitCallback, object>>();

            #region IWorkQueue Members

            public void QueueWorkItem(WaitCallback callback, object prm)
            {
                workQueue.Enqueue(Tuple.Create(callback, prm));
            }

            #endregion
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }


        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<string, string>, IWorkQueue
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return UnorderedBufferTransformation<string>.Create(frame, this, 50);
            }

            public override string Prefix
            {
                get { return "new UnorderedBufferTransformation<string>(frame,this)"; }
            }

            #region IWorkQueue Members

            public void QueueWorkItem(WaitCallback callback, object prm)
            {
                throw new NotImplementedException();
            }

            #endregion
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
            Assert.IsNull(UnorderedBufferTransformation<string>.Create(null,new DeterministicEventRegistrationRunner(), 50), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_workQueue()
        {
            Assert.IsNull(UnorderedBufferTransformation<string>.Create(StaticEnumerable<string>.Create(new string[0]), null, 50), "Should return null when workQueue is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<string>.Create(new string[0]), StaticEnumerable<string>.Create(new string[] { "a" }),
                (IWorkQueue)new DeterministicEventRegistrationRunner(), (IWorkQueue)new CorrectnessRunner(),
                40, 50,
                (s, wk, dop) => UnorderedBufferTransformation<string>.Create(s, wk, dop),
                "UnorderedBufferTransformation<string>"
            );
        }

        class MTWorkQueue : IWorkQueue
        {
            #region IWorkQueue Members

            public void QueueWorkItem(WaitCallback callback, object prm)
            { System.Threading.ThreadPool.QueueUserWorkItem(callback, prm); }

            #endregion
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            AsyncTestRunnerForCollectionTransformation.Run(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                s => SortTransformation<int,int>.Create(UnorderedBufferTransformation<int>.Create(s, new MTWorkQueue(), 100), i => i, Comparer<int>.Default),
                "UnorderedBufferTransformation<int>.Create(s, new MTWorkQueue())"
            );
        }
    }
}
