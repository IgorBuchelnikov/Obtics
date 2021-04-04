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
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for ExceptionTransformationTest
    /// </summary>
    [TestClass]
    public class ExceptionTransformationTest
    {
        public ExceptionTransformationTest()
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
        public void CorrectnessTest()
        {
            var frame = new FrameIValueProviderNPC<string>();
            frame.IsReadOnly = true;

            var sequence = new object[][] { 
                new object[] { "0", "0" },
                new object[] { "1", "1" },
                new object[] { "0", "0" },
                new object[] { "23", "23" },
                new object[] { "2", "2" },
                new object[] { "0", "0" },
                new object[] { "1", "1" }
            };

            frame.SetValue((string)sequence[0][0]);

            var client = new ValueProviderClientNPC<string>();
            client.Source = ExceptionTransformation<string, Exception>.Create(frame.Patched(), ex => true, ex => "0");

            Assert.AreEqual<string>((string)sequence[0][1], client.Buffer, "ExceptionTransformation<string, Exception>.Create(frame, ex => \"0\"): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame.SetValue((string)sequence[i][0]);

                Assert.AreEqual<string>((string)sequence[i][1], client.Buffer, "ExceptionTransformation<string, Exception>.Create(frame, ex => \"0\") (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }



        [TestMethod]
        public void CorrectnessTest2()
        {
            var frame = new FrameIValueProviderNPC<decimal>();
            frame.IsReadOnly = true;

            var sequence = new object[][] { 
                new object[] { 1.0m , 1.0m },
                new object[] { 2.0m, 0.5m },
                new object[] { 0.0m, 1234.0m },
                new object[] { 1.0m, 1.0m },
                new object[] { 1.0m, 1.0m },
                new object[] { 0.0m, 1234.0m },
                new object[] { 2.0m, 0.5m },
                new object[] { 1.0m , 1.0m },
            };

            frame.SetValue((decimal)sequence[0][0]);

            var client = new ValueProviderClientNPC<decimal>();
            client.Source = ExceptionTransformation<decimal, Exception>.Create(UnarySelectTransformation<decimal, decimal>.Create(frame.Patched(), d => 1.0m / d), ex => true, ex => 1234.0m);

            Assert.AreEqual<decimal>((decimal)sequence[0][1], client.Buffer, "ExceptionTransformation<decimal, Exception>.Create(UnarySelectTransformation<decimal, decimal>.Create(frame, d => 1.0m / d), ex => 1234.0m): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame.SetValue((decimal)sequence[i][0]);

                Assert.AreEqual<decimal>((decimal)sequence[i][1], client.Buffer, "ExceptionTransformation<decimal, Exception>.Create(UnarySelectTransformation<decimal, decimal>.Create(frame, d => 1.0m / d), ex => 1234.0m) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return ExceptionTransformation<int, Exception>.Create(frame.Patched(), e => true, e => 10);
            }

            public override string Prefix
            {
                get { return "ExceptionTransformation<int, Exception>.Create(frame, e => true, e => 10)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner()).Run();
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                ValueProvider.Static(23), ValueProvider.Static(2),
                new Func<Exception, bool>(e => true), e => false,
                new Func<Exception, int>(i => 100), e => 200,
                (s, h, fcg) => ExceptionTransformation<int, Exception>.Create(s.Patched(), h, fcg),
                "ExceptionTransformation<int, Exception>"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int[] { 1, 2, 3, 4, 5, 8, 7, 4, 2, 6, 4, 7, 8, 3, 4, 2 },
                77,
                frame =>
                    frame.Select(
                        v =>
                        {
                            if (v == 2)
                                throw new InvalidOperationException();

                            return v;
                        }
                    )
                    .OnException((InvalidOperationException ex) => 77),
                "OnException((InvalidOperationException ex) => 77)"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int[] { 1, 2, 3, 4, 5, 8, 7, 4, 2, 6, 4, 7, 8, 3, 4, 2 },
                2,
                frame =>
                    frame.Select(
                        v =>
                        {
                            if (v == 4)
                                throw new InvalidOperationException();

                            return v;
                        }
                    )
                    .OnException((InvalidOperationException ex) => 77),
                "OnException((InvalidOperationException ex) => 77)"
            );
        }

    }
}
