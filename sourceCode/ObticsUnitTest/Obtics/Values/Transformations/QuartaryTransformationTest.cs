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
using Obtics.Values;
using Obtics;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for QuartaryTransformationTest
    /// </summary>
    [TestClass]
    public class QuartaryTransformationTest
    {
        public QuartaryTransformationTest()
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
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                new int[] { 2, 4, 6, 8, 10, 1, 3, 5, 7, 9 },
                new int[] { 7, 2, 5, 1, 9, 3, 2, 6, 8, 8, 9, 11 },
                new int[] { 3, 5, 6, 3, 1, 3, 2, 5, 5, 6, 2 },
                32,
                (frame1, frame2, frame3, frame4) => QuartaryTransformation<int, int, int, int, int>.Create(frame1.Patched(), frame2.Patched(), frame3.Patched(), frame4.Patched(), (i, j, k, l) => i.Value + j.Value + k.Value + l.Value),
                "QuartaryTransformation<int, int, int, int, int>.Create(frame1, frame2, frame3, frame4, (i, j, k, l) => i.Value + j.Value + k.Value + l.Value)"
            );
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var sequence = new int[][] { 
                new int[] { 1, 2, 3, 4, 10 },
                new int[] { 2, 2, 3, 4, 11 },
                new int[] { 1, 2, 3, 4, 10 },
                new int[] { 1, 3, 3, 4, 11 },
                new int[] { 1, 2, 3, 4, 10 },
                new int[] { 1, 2, 4, 4, 11 },
                new int[] { 1, 2, 3, 4, 10 },
                new int[] { 1, 2, 3, 5, 11 },
                new int[] { 1, 2, 3, 4, 10 },
                new int[] { 2, 3, 4, 5, 14 },
                new int[] { 2, 2, 4, 4, 12 },
                new int[] { 1, 2, 3, 4, 10 },
                new int[] { 2, 3, 3, 4, 12 },
                new int[] { 2, 3, 4, 5, 14 },
                new int[] { 1, 3, 4, 4, 12 },
                new int[] { 1, 2, 3, 4, 10 }
            };

            var frames = new FrameIValueProviderNPC<int>[4];

            for (int i = 0; i < 4; ++i)
            {
                frames[i] = new FrameIValueProviderNPC<int>();
                frames[i].IsReadOnly = true;
                frames[i].SetValue(sequence[0][i]);
            }

            var client = new ValueProviderClientNPC<int>();
            client.Source = QuartaryTransformation<int, int, int, int, int>.Create(frames[0].Patched(), frames[1].Patched(), frames[2].Patched(), frames[3].Patched(), (i, j, k, l) => i.Value + j.Value + k.Value + l.Value);

            Assert.AreEqual<int>(client.Buffer, sequence[0][4], "new QuartaryTransformation<int, int, int, int, int>(frames[0], frames[1], frames[2], frames[3], (i, j, k, l) => i.Value + j.Value + k.Value + l.Value) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                for (int j = 0; j < 4; ++j)
                    frames[j].SetValue(sequence[i][j]);

                Assert.AreEqual<int>(client.Buffer, sequence[i][4], "new QuartaryTransformation<int, int, int, int, int>(frames[0], frames[1], frames[2], frames[3], (i, j, k, l) => i.Value + j.Value + k.Value + l.Value) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner1 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return QuartaryTransformation<int, int, int, int, int>.Create(frame.Patched(), ValueProvider.Static(10).Patched(), ValueProvider.Static(10).Patched(), ValueProvider.Static(10).Patched(), (a, b, c, d) => a.Value + b.Value + c.Value + d.Value);
            }

            public override string Prefix
            {
                get { return "new QuartaryTransformation<int, int, int, int, int>(frame, ValueProvider.Static(10), ValueProvider.Static(10), ValueProvider.Static(10), (a, b, c, d) => a.Value + b.Value + c.Value + d.Value)"; }
            }
        }

        class DeterministicEventRegistrationRunner2 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return QuartaryTransformation<int, int, int, int, int>.Create(ValueProvider.Static(10).Patched(), frame.Patched(), ValueProvider.Static(10).Patched(), ValueProvider.Static(10).Patched(), (a, b, c, d) => a.Value + b.Value + c.Value + d.Value);
            }

            public override string Prefix
            {
                get { return "new QuartaryTransformation<int, int, int, int, int>(ValueProvider.Static(10), frame, ValueProvider.Static(10), ValueProvider.Static(10), (a, b, c, d) => a.Value + b.Value + c.Value + d.Value)"; }
            }
        }

        class DeterministicEventRegistrationRunner3 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return QuartaryTransformation<int, int, int, int, int>.Create(ValueProvider.Static(10).Patched(), ValueProvider.Static(10).Patched(), frame.Patched(), ValueProvider.Static(10).Patched(), (a, b, c, d) => a.Value + b.Value + c.Value + d.Value);
            }

            public override string Prefix
            {
                get { return "new QuartaryTransformation<int, int, int, int, int>(ValueProvider.Static(10), ValueProvider.Static(10), frame, ValueProvider.Static(10), (a, b, c, d) => a.Value + b.Value + c.Value + d.Value)"; }
            }
        }

        class DeterministicEventRegistrationRunner4 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return QuartaryTransformation<int, int, int, int, int>.Create(ValueProvider.Static(10).Patched(), ValueProvider.Static(10).Patched(), ValueProvider.Static(10).Patched(), frame.Patched(), (a, b, c, d) => a.Value + b.Value + c.Value + d.Value);
            }

            public override string Prefix
            {
                get { return "new QuartaryTransformation<int, int, int, int, int>(ValueProvider.Static(10), ValueProvider.Static(10), ValueProvider.Static(10), frame, (a, b, c, d) => a.Value + b.Value + c.Value + d.Value)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner1()).Run();
            (new DeterministicEventRegistrationRunner2()).Run();
            (new DeterministicEventRegistrationRunner3()).Run();
            (new DeterministicEventRegistrationRunner4()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_first()
        {
            Assert.IsNull(QuartaryTransformation<int, int, int, int, int>.Create(null, ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), (fi, s, t, fo) => fi.Value + s.Value + t.Value + fo.Value), "Should return null when first is null");
        }

        [TestMethod]
        public void ArgumentsCheck_second()
        {
            Assert.IsNull(QuartaryTransformation<int, int, int, int, int>.Create(ValueProvider.Static(1).Patched(), null, ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), (fi, s, t, fo) => fi.Value + s.Value + t.Value + fo.Value), "Should return null when second is null");
        }

        [TestMethod]
        public void ArgumentsCheck_third()
        {
            Assert.IsNull(QuartaryTransformation<int, int, int, int, int>.Create(ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), null, ValueProvider.Static(1).Patched(), (fi, s, t, fo) => fi.Value + s.Value + t.Value + fo.Value), "Should return null when third is null");
        }

        [TestMethod]
        public void ArgumentsCheck_fourth()
        {
            Assert.IsNull(QuartaryTransformation<int, int, int, int, int>.Create(ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), null, (fi, s, t, fo) => fi.Value + s.Value + t.Value + fo.Value), "Should return null when fourth is null");
        }

        [TestMethod]
        public void ArgumentsCheck_converter()
        {
            Assert.IsNull(QuartaryTransformation<int, int, int, int, int>.Create(ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), ValueProvider.Static(1).Patched(), null), "Should return null when converter is null");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<bool>)ValueProvider.Static(true), ValueProvider.Static(false),
                (IValueProvider<bool>)ValueProvider.Static(true), ValueProvider.Static(false),
                (IValueProvider<bool>)ValueProvider.Static(true), ValueProvider.Static(false),
                (IValueProvider<bool>)ValueProvider.Static(true), ValueProvider.Static(false),
                new Func<IValueProvider<bool>, IValueProvider<bool>, IValueProvider<bool>, IValueProvider<bool>, bool>((fi, s, t, fo) => fi.Value ^ s.Value ^ t.Value ^ fo.Value), (fi, s, t, fo) => fi.Value | s.Value | t.Value | fo.Value,
                (fi, s, t, fo, c) => QuartaryTransformation<bool, bool, bool, bool, bool>.Create(fi.Patched(), s.Patched(), t.Patched(), fo.Patched(), c),
                "QuartaryTransformation<bool, bool, bool, bool, bool>"
            );
        }

    }
}
