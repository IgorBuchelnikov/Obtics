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
    /// Summary description for UnaryTransformationTest
    /// </summary>
    [TestClass]
    public class UnaryTransformationTest
    {
        public UnaryTransformationTest()
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
                new int[] { 0, 1, 2, 3, 4, 5, 6, 6, 7, 8, 1, 1, 9 },
                9L,
                frame => UnaryTransformation<int, long>.Create(frame.Patched(), s => (long)s.Value),
                "UnaryTransformation<int, long>.Create(frame, s => (long)s.Value)"
            );
        }
        
        [TestMethod]
        public void CorrectnessTest()
        {
            var frame = new FrameIValueProviderNPC<string>();
            frame.IsReadOnly = true;

            var sequence = new object[][] { 
                new object[] { "0", 0 },
                new object[] { "1", 1 },
                new object[] { "0", 0 },
                new object[] { "23", 23 },
                new object[] { "2", 2 },
                new object[] { "0", 0 },
                new object[] { "1", 1 }
            };

            frame.SetValue((string)sequence[0][0]);

            var client = new ValueProviderClientNPC<int>();
            client.Source = UnaryTransformation<string, int>.Create(frame.Patched(), s => int.Parse(s.Value));

            Assert.AreEqual<int>(client.Buffer, (int)sequence[0][1], "new UnaryTransformation<string,int>(frame, s => int.Parse(s.Value)) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame.SetValue((string)sequence[i][0]);

                Assert.AreEqual<int>(client.Buffer, (int)sequence[i][1], "new UnaryTransformation<string,int>(frame, s => int.Parse(s.Value)) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, bool>
        {
            protected override IValueProvider<bool> Create(FrameIValueProviderNPC<int> frame)
            {
                return UnaryTransformation<int, bool>.Create(frame.Patched(), i => i.Value % 2 == 0);
            }

            public override string Prefix
            {
                get { return "new UnaryTransformation<int, bool>(frame, i => i.Value % 2 == 0)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(UnaryTransformation<int, bool>.Create(null, i => i.Value % 2 == 0),"Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_converter()
        {
            Assert.IsNull(UnaryTransformation<int, bool>.Create(ValueProvider.Static(10).Patched(), null), "Should return null when converter is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                ValueProvider.Static(23), ValueProvider.Static(2),
                new Func<IValueProvider<int>,bool>( i => i.Value % 2 == 0 ), i => i.Value % 2 == 1,
                (s, c) => UnaryTransformation<int, bool>.Create(s.Patched(), c),
                "UnaryTransformation<int, bool>"
            );
        }

    }
}
