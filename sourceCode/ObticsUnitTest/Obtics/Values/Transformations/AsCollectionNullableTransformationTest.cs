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
    /// Summary description for AsCollectionNullableTransformationTest
    /// </summary>
    [TestClass]
    public class AsCollectionNullableTransformationTest
    {
        public AsCollectionNullableTransformationTest()
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
            var sequence = new object[][] { 
                new object[] { 1, new int[] { 1 } },
                new object[] { 2, new int[] { 2 } },
                new object[] { 3, new int[] { } },
                new object[] { 4, new int[] { 4 } },
                new object[] { 5, new int[] { 5 } },
                new object[] { 1, new int[] { 1 } },
                new object[] { 3, new int[] { } },
                new object[] { 4, new int[] { 4 } },
            };

            var frame0 = new FrameIValueProviderNPC<int>();
            frame0.IsReadOnly = true;

            frame0.SetValue((int)sequence[0][0]);

            var client1 = new CollectionTransformationClientNPC<int>();
            client1.Source = AsCollectionNullableTransformation<int>.Create(frame0.Patched(), i => i != 3);

            var client2 = new CollectionTransformationClientSNCC<int>();
            client2.Source = AsCollectionNullableTransformation<int>.Create(frame0.Patched(), i => i != 3);

            Assert.IsTrue(client1.Buffer.SequenceEqual((int[])sequence[0][1]), "new AsCollectionNullableTransformation<int>(frame0) (0): NPC client, initialy the result value is not correct.");
            Assert.IsTrue(client2.Buffer.SequenceEqual((int[])sequence[0][1]), "new AsCollectionNullableTransformation<int>(frame0) (1): SNCC client, initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue((int)sequence[i][0]);

                Assert.IsTrue(client1.Buffer.SequenceEqual((int[])sequence[i][1]), "new AsCollectionNullableTransformation<int>(frame0) (" + (i * 2).ToString() + "): NPC client, after changes to source the result value is not correct.");
                Assert.IsTrue(client2.Buffer.SequenceEqual((int[])sequence[i][1]), "new AsCollectionNullableTransformation<int>(frame0) (" + (i * 2 + 1).ToString() + "): SNCC client, after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForSequence<bool, bool>
        {
            protected override IEnumerable<bool> Create(FrameIValueProviderNPC<bool> frame)
            {
                return AsCollectionNullableTransformation<bool>.Create(frame.Patched(), b => b);
            }

            public override string Prefix
            {
                get { return "new AsCollectionNullableTransformation<bool>(frame, false)"; }
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
            Assert.IsNull(AsCollectionNullableTransformation<bool>.Create(null, b => b), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_predicate()
        {
            Assert.IsNull(AsCollectionNullableTransformation<bool>.Create(ValueProvider.Static(true).Patched(), null), "Should return null when predicate is null");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                ValueProvider.Static(true), ValueProvider.Static(false),
                new Func<bool,bool>( i => i ), i => !i,
                (s, p) => AsCollectionNullableTransformation<bool>.Create(s.Patched(), p),
                "AsCollectionNullableTransformation<bool>"
            );
        }
    }
}
