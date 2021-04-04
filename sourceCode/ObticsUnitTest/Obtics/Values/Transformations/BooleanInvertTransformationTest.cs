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
    /// Summary description for BooleanInvertTransformationTest
    /// </summary>
    [TestClass]
    public class BooleanInvertTransformationTest
    {
        public BooleanInvertTransformationTest()
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
                false,
                frame => BooleanInvertTransformation.Create(frame.Patched()),
                "BooleanInvertTransformation.Create(frame)"
            );
 
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var sequence = new bool[][] { 
                new bool[] { true, false },
                new bool[] { false, true },
                new bool[] { true, false },
            };

            var frame0 = new FrameIValueProviderNPC<bool>();
            frame0.IsReadOnly = true;

            frame0.SetValue(sequence[0][0]);

            var client = new ValueProviderClientNPC<bool>();
            client.Source = BooleanInvertTransformation.Create(frame0.Patched());

            Assert.AreEqual<bool>(client.Buffer, sequence[0][1], "new BooleanInvertTransformation(frame0) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue(sequence[i][0]);

                Assert.AreEqual<bool>(client.Buffer, sequence[i][1], "new BooleanInvertTransformation(frame0) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<bool, bool>
        {
            protected override IValueProvider<bool> Create(FrameIValueProviderNPC<bool> frame)
            {
                return BooleanInvertTransformation.Create(frame.Patched());
            }

            public override string Prefix
            {
                get { return "new BooleanInvertTransformation(frame)"; }
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
            Assert.IsNull(BooleanInvertTransformation.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<bool>)ValueProvider.Static(true), ValueProvider.Static(false),
                s => BooleanInvertTransformation.Create(s.Patched()),
                "BooleanInvertTransformation"
            );
        }
    }
}
