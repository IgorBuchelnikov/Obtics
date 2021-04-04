using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Async;
using Obtics;
using System.Threading;
using Obtics.Values.Transformations;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for BufferTransformationTest
    /// </summary>
    [TestClass]
    public class BufferTransformationTest : IWorkQueue
    {
        public BufferTransformationTest()
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
            AsyncTestRunnerForValueTransformation.Run(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                13,
                frame => BufferTransformation<int>.Create(frame.Patched(), new MTWorkQueue()),
                "BufferTransformation<int>.Create(frame, new MTWorkQueue())"
            );
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var frame = new FrameIValueProviderNPC<string>();
            frame.IsReadOnly = true;

            var sequence = new string[] { 
                "0", "1", "0", "23", "2", "0", "1"
            };

            frame.SetValue(sequence[0]);

            var client = new ValueProviderClientNPC<string>();
            client.Source = BufferTransformation<string>.Create(frame.Patched(), this);

            Assert.AreEqual(client.Buffer, sequence[0], "BufferTransformation<string>.Create(frame, this): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame.SetValue(sequence[i]);
                Assert.AreEqual(client.Buffer, sequence[i-1], "BufferTransformation<string>.Create(frame, this) (" + i.ToString() + "): before dispatching change propagation value is not correct.");
                Dispatch();
                Assert.AreEqual(client.Buffer, sequence[i], "BufferTransformation<string>.Create(frame, this) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>, IWorkQueue
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return BufferTransformation<int>.Create(frame.Patched(), this);
            }

            public override string Prefix
            {
                get { return "BufferTransformation<int>.Create(frame, this)"; }
            }

            #region IWorkQueue Members

            public void QueueWorkItem(WaitCallback callback, object prm)
            {
                throw new System.NotImplementedException();
            }

            #endregion
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(BufferTransformation<int>.Create(null, this), "Should return null when source is null");
        }

        [TestMethod]
        public void ArgumentsCheck_workQueue()
        {
            Assert.IsNull(BufferTransformation<int>.Create(ValueProvider.Static(10).Patched(), null), "Should return null when workQueue is null");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                ValueProvider.Static(23), ValueProvider.Static(2),
                (IWorkQueue)this, (IWorkQueue)new DeterministicEventRegistrationRunner(),
                (s, wk) => BufferTransformation<int>.Create(s.Patched(), wk),
                "BufferTransformation<int>"
            );
        }

        void Dispatch()
        {
            while (workQueue.Count > 0)
            {
                var t = workQueue.Dequeue();
                t.First(t.Second);
            }
        }

        Queue<Tuple<WaitCallback, object>> workQueue = new Queue<Tuple<WaitCallback, object>>();

        #region IWorkQueue Members

        public void QueueWorkItem(WaitCallback callback, object prm)
        {
            workQueue.Enqueue(Tuple.Create(callback, prm));
        }

        #endregion
    }
}
