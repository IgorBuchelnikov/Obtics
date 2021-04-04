using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Values.Transformations;
using ObticsUnitTest.Helpers;
using Obtics;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for BinaryTransformationTest
    /// </summary>
    [TestClass]
    public class BinaryTransformationTest
    {
        public BinaryTransformationTest()
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
        public void ConcurrencyTest()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new bool[] { true, false, true, false, false, true, true, false, true },
                new bool[] { true, false, false, true, false, true, false, true, true },
                false,
                (frame1, frame2) => BinaryTransformation<bool, bool, bool>.Create(frame1.Patched(), frame2.Patched(), (b1, b2) => b1.Value ^ b2.Value),
                "BinaryTransformation<bool, bool, bool>.Create(frame1, frame2, (b1, b2) => b1.Value ^ b2.Value)"
            );
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var sequence = new int[][] { 
                new int[] { 1, 2, 3 },
                new int[] { 1, 3, 4 },
                new int[] { 1, 2, 3 },
                new int[] { 0, 2, 2 },
                new int[] { 1, 2, 3 },
                new int[] { 2, 3, 5 },
                new int[] { 2, 2, 4 },
                new int[] { 2, 3, 5 },
                new int[] { 1, 3, 4 },
                new int[] { 2, 3, 5 },
                new int[] { 1, 2, 3 }
            };

            var frame0 = new FrameIValueProviderNPC<int>();
            frame0.IsReadOnly = true;

            var frame1 = new FrameIValueProviderNPC<int>();
            frame1.IsReadOnly = true;

            frame0.SetValue(sequence[0][0]);
            frame1.SetValue(sequence[0][1]);

            var client = new ValueProviderClientNPC<int>();
            client.Source = BinaryTransformation<int, int, int>.Create(frame0.Patched(), frame1.Patched(), (i, j) => i.Value + j.Value);

            Assert.AreEqual<int>(client.Buffer, sequence[0][2], "new BinaryTransformation<int,int,int>(frame0, frame1, (i,j)=>i+j ) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue(sequence[i][0]);
                frame1.SetValue(sequence[i][1]);

                Assert.AreEqual<int>(client.Buffer, sequence[i][2], "new BinaryTransformation<int,int,int>(frame0, frame1, (i,j)=>i+j ) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner1 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return BinaryTransformation<int, int, int>.Create(frame.Patched(), ValueProvider.Static(10).Patched(), (a, b) => a.Value + b.Value);
            }

            public override string Prefix
            {
                get { return "new BinaryTransformation<int,int,int>(frame, ValueProvider.Static(10), (a,b) => a.Value + b.Value)"; }
            }
        }

        class DeterministicEventRegistrationRunner2 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return BinaryTransformation<int, int, int>.Create(ValueProvider.Static(10).Patched(), frame.Patched(), (a, b) => a.Value + b.Value);
            }

            public override string Prefix
            {
                get { return "new BinaryTransformation<int, int, int>(ValueProvider.Static(10), frame, (a, b) => a.Value + b.Value)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner1()).Run();
            (new DeterministicEventRegistrationRunner2()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_first()
        {
            Assert.IsNull(BinaryTransformation<int, int, int>.Create(null, ValueProvider.Static(1).Patched(), (f, s) => f.Value + s.Value), "Should return null when first is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_second()
        {
            Assert.IsNull(BinaryTransformation<int, int, int>.Create(ValueProvider.Static(1).Patched(), null, (f, s) => f.Value + s.Value), "Should return null when second is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_func()
        {
            Assert.IsNull(BinaryTransformation<int, int, int>.Create(ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), null), "Should return null when second is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<bool>)ValueProvider.Static(true), ValueProvider.Static(false),
                (IValueProvider<bool>)ValueProvider.Static(true), ValueProvider.Static(false),
                new Func<IValueProvider<bool>, IValueProvider<bool>, bool>((f, s) => f.Value ^ s.Value), (f, s) => f.Value | s.Value,
                (f, s, c) => BinaryTransformation<bool, bool, bool>.Create(f.Patched(), s.Patched(), c),
                "BinaryTransformation<bool,bool,bool>"
            );
        }
    }
}
