﻿using System;
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
    /// Summary description for TypeConvertTransformationTest
    /// </summary>
    [TestClass]
    public class TypeConvertTransformationTest
    {
        public TypeConvertTransformationTest()
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
                frame => TypeConvertTransformation<long>.Create(frame.Patched()),
                "TypeConvertTransformation<long>.Create(frame)"
            );
        }
        [TestMethod]
        public void CorrectnessTest()
        {
            var sequence = new int[] { 1, 2, 3, 4, 1, 3, 1 };

            var frame0 = new FrameIValueProviderNPC<object>();
            frame0.IsReadOnly = true;

            frame0.SetValue(sequence[0]);

            var client = new ValueProviderClientNPC<int>();
            client.Source = TypeConvertTransformation<int>.Create(frame0.Patched());

            Assert.AreEqual<int>(client.Buffer, sequence[0], "new TypeConvertTransformation<int>(frame0) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue(sequence[i]);

                Assert.AreEqual<int>(client.Buffer, sequence[i], "new TypeConvertTransformation<int>(frame0) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return TypeConvertTransformation<int>.Create(frame.Patched());
            }

            public override string Prefix
            {
                get { return "new TypeConvertTransformation<int>((IValueProvider)frame)"; }
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
            Assert.IsNull(TypeConvertTransformation<int>.Create(null), "Should return null when source is null");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider)ValueProvider.Static(23), ValueProvider.Static(2),
                s => TypeConvertTransformation<int>.Create(s.Patched()),
                "TypeConvertTransformation<int>"
            );
        }

    }
}
