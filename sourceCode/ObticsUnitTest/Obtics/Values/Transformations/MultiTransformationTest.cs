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
using Obtics.Values.Transformations;
using Obtics;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for CrossJoinTransformationTest
    /// </summary>
    [TestClass]
    public class MultiTransformationTest
    {
        public MultiTransformationTest()
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
                30,
                (frame1, frame2, frame3) => MultiTransformation<int>.Create(args => args.Select(vp => ((IValueProvider<int>)vp).Value).Sum(), frame1.Patched(), frame2.Patched(), frame3.Patched()),
                "CrossJoinTransformation<int>.Create(args => args.Select(vp => ((IValueProvider<int>)vp).Value).Sum(), frame1, frame2, frame3)"
            );
        }
        
        [TestMethod]
        public void CorrectnessTest()
        {
            var sequence = new int[][] { 
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 1 },
                new int[] { 1, 1, 0, 0, 2 },
                new int[] { 1, 1, 1, 0, 3 },
                new int[] { 1, 1, 1, 1, 4 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 1, 0, 1, 0, 2 },
                new int[] { 1, 1, 1, 1, 4 },
                new int[] { 0, 0, 1, 1, 2 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 1, 2 },
                new int[] { 1, 1, 1, 1, 4 }
            };

            var frame = new FrameIValueProviderNPC<int>[4];

            for (int i = 0; i < 4; ++i)
            {
                frame[i] = new FrameIValueProviderNPC<int>();
                frame[i].IsReadOnly = true;
                frame[i].SetValue(sequence[0][i]);
            }

            var client = new ValueProviderClientNPC<int>();
            client.Source = MultiTransformation<int>.Create(args => args.Select(vp => ((IValueProvider<int>)vp).Value).Sum(),frame.Patched());

            Assert.AreEqual<int>(client.Buffer, (int)sequence[0][4], "new CrossJoinTransformation<int>(args => args.Select(vp => ((IValueProvider<int>)vp).Value).Sum(),frame) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                for (int j = 0; j < 4; ++j)
                    frame[j].SetValue(sequence[i][j]);

                Assert.AreEqual<int>(client.Buffer, (int)sequence[i][4], "new CrossJoinTransformation<int>(args => args.Select(vp => ((IValueProvider<int>)vp).Value).Sum(),frame) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner1 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return MultiTransformation<int>.Create(args => ((IValueProvider<int>)args[0]).Value + ((IValueProvider<int>)args[1]).Value, frame.Patched(), ValueProvider.Static(10).Patched());
            }

            public override string Prefix
            {
                get { return "new CrossJoinTransformation<int>(args => ((IValueProvider<int>)args[0]).Value + ((IValueProvider<int>)args[1]).Value, frame, ValueProvider.Static(10))"; }
            }
        }

        class DeterministicEventRegistrationRunner2 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return MultiTransformation<int>.Create(args => ((IValueProvider<int>)args[0]).Value + ((IValueProvider<int>)args[1]).Value, ValueProvider.Static(10).Patched(), frame.Patched());
            }

            public override string Prefix
            {
                get { return "new CrossJoinTransformation<int>(args => ((IValueProvider<int>)args[0]).Value + ((IValueProvider<int>)args[1]).Value, ValueProvider.Static(10), frame)"; }
            }
        }


        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner1()).Run();
            (new DeterministicEventRegistrationRunner2()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_function()
        {
            Assert.IsNull(MultiTransformation<int>.Create(null), "Should return null when function is null");
        }

        [TestMethod]
        public void ArgumentsCheck_sources()
        {
            Assert.IsNull(MultiTransformation<int>.Create(args => 10, (IInternalValueProvider[])null), "Should return null when sources is null");
        }

        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(MultiTransformation<int>.Create(args => 10, (IInternalValueProvider)null), "Should return null when a source is null");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<int>)ValueProvider.Static(1), (IValueProvider<int>)ValueProvider.Static(2),
                (IValueProvider<int>)ValueProvider.Static(3), (IValueProvider<int>)ValueProvider.Static(4),
                (IValueProvider<int>)ValueProvider.Static(5), (IValueProvider<int>)ValueProvider.Static(6),
                new Func<IValueProvider[],int>( vpa => vpa.Sum( vp => (int)vp.Value ) ), vpa => vpa.Sum( vp => (int)vp.Value - 1 ),
                (s1, s2, s3, f) => MultiTransformation<int>.Create(f, s1.Patched(), s2.Patched(), s3.Patched()),
                "CrossJoinTransformation<int>"
            );
        }

    }
}
