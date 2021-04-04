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
    /// Summary description for ConvertTransformationTest
    /// </summary>
    [TestClass]
    public class UnarySelectTransformationTest
    {
        public UnarySelectTransformationTest()
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
                new string[] { "0", "1", "2", "9", "10", "6", "3", "3", "2", "1", "5", "5" },
                5,
                frame => UnarySelectTransformation<string, int>.Create(frame.Patched(), s => int.Parse(s)),
                "ConvertTransformation<string,int>.Create(frame, s => int.Parse(s))"
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
                new object[] { "ASD", 1234},
                new object[] { "23", 23 },
                new object[] { null, 1234 },
                new object[] { "2", 2 },
                new object[] { "0", 0 },
                new object[] { "1", 1 }
            };

            frame.SetValue((string)sequence[0][0]);

            var client = new ValueProviderClientNPC<int>();
            client.Source = UnarySelectTransformation<string,int>.Create(frame.Patched(), s => int.Parse(s)).OnException((FormatException ex)=>1234);

            Assert.AreEqual<int>(client.Buffer, (int)sequence[0][1], "new ConvertTransformation<string,int>(frame, s => int.Parse(s)) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                var v = (string)sequence[i][0];

                if (v != null)
                    frame.SetValue(v);
                else
                    frame.SetExceptionValue(new FormatException());

                Assert.AreEqual<int>((int)sequence[i][1], client.Buffer, "new ConvertTransformation<string,int>(frame, s => int.Parse(s)) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, bool>
        {
            protected override IValueProvider<bool> Create(FrameIValueProviderNPC<int> frame)
            {
                return UnarySelectTransformation<int,bool>.Create(frame.Patched(), i => i % 2 == 0);
            }

            public override string Prefix
            {
                get { return "new ConvertTransformation<int,bool>(frame, i => i % 2 == 0)"; }
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
            Assert.IsNull(UnarySelectTransformation<int, bool>.Create(null, i => i % 2 == 0), "Should return null when source is null");
        }

        [TestMethod]
        public void ArgumentsCheck_converter()
        {
            Assert.IsNull(UnarySelectTransformation<int, bool>.Create(ValueProvider.Static(10).Patched(), null), "Should return null when converter is null");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                ValueProvider.Static(23), ValueProvider.Static(2),
                new Func<int, bool>(i => i % 2 == 0), i => i % 2 == 1,
                (s, c) => UnarySelectTransformation<int, bool>.Create(s.Patched(), c),
                "ConvertTransformation<int, bool>"
            );
        }

    }
}
