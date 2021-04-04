using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Obtics.Values.Transformations;
using ObticsUnitTest.Helpers;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for AsCollectionTransformationTest
    /// </summary>
    [TestClass]
    public class AsCollectionTransformationTest
    {
        public AsCollectionTransformationTest()
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
            var sequence = new int[] { 1, 2, 3, 4, 1, 3, 1 };

            var frame0 = new FrameIValueProviderNPC<int>();
            frame0.IsReadOnly = true;

            frame0.SetValue(sequence[0]);

            var client1 = new CollectionTransformationClientNPC<int>();
            client1.Source = AsCollectionTransformation<int>.Create(frame0);

            var client2 = new CollectionTransformationClientSNCC<int>();
            client2.Source = AsCollectionTransformation<int>.Create(frame0);

            Assert.IsTrue(client1.Buffer.SequenceEqual(new int[] { sequence[0] }), "new AsCollectionTransformation<int>(frame0) (0): NPC client, initialy the result value is not correct.");
            Assert.IsTrue(client2.Buffer.SequenceEqual(new int[] { sequence[0] }), "new AsCollectionTransformation<int>(frame0) (1): SNCC client, initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue(sequence[i]);

                Assert.IsTrue(client1.Buffer.SequenceEqual(new int[] { sequence[i] }), "new AsCollectionTransformation<int>(frame0) (" + (i * 2).ToString() + "): NPC client, after changes to source the result value is not correct.");
                Assert.IsTrue(client2.Buffer.SequenceEqual(new int[] { sequence[i] }), "new AsCollectionTransformation<int>(frame0) (" + (i * 2 + 1).ToString() + "): SNCC client, after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForSequence<bool, bool>
        {
            protected override IEnumerable<bool> Create(FrameIValueProviderNPC<bool> frame)
            {
                return AsCollectionTransformation<bool>.Create(frame);
            }

            public override string Prefix
            {
                get { return "new AsCollectionTransformation<bool>(frame)"; }
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
            Assert.IsNull(AsCollectionTransformation<bool>.Create(null), "Should return null when source is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                ValueProvider.Static(true), ValueProvider.Static(false),
                s => AsCollectionTransformation<bool>.Create(s),
                "AsCollectionTransformation<bool>"
            );
        }
    }
}
