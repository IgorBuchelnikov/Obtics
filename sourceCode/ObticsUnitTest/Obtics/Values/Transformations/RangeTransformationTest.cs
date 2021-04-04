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
using VP = Obtics.Values.ValueProvider;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for RangeTransformationTest
    /// </summary>
    [TestClass]
    public class RangeTransformationTest
    {
        public RangeTransformationTest()
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
        public void CorrectnessTest1()
        {
            var sequence = new object[][] { 
                new object[] { 1, new int[] { 1, 2, 3, 4 } },
                new object[] { 2, new int[] { 2, 3, 4 } },
                new object[] { 3, new int[] { 3, 4 } },
                new object[] { 4, new int[] { 4 } },
                new object[] { 5, new int[] { } },
                new object[] { 1, new int[] { 1, 2, 3, 4 } },
                new object[] { 3, new int[] { 3, 4 } },
                new object[] { 4, new int[] { 4 } },
            };

            var frame0 = new FrameIValueProviderNPC<int>();
            frame0.IsReadOnly = true;

            frame0.SetValue((int)sequence[0][0]);

            var client1 = new CollectionTransformationClientNPC<int>();
            client1.Source = RangeTransformation.Create(VP.Patched(frame0), VP.Patched(VP.Static(5)));

            var client2 = new CollectionTransformationClientSNCC<int>();
            client2.Source = RangeTransformation.Create(VP.Patched(frame0), VP.Patched( VP.Static(5)));

            Assert.IsTrue(client1.Buffer.SequenceEqual((int[])sequence[0][1]), "RangeTransformation.Create(frame0, VP.Static(5)) (0): NPC client, initialy the result value is not correct.");
            Assert.IsTrue(client2.Buffer.SequenceEqual((int[])sequence[0][1]), "RangeTransformation.Create(frame0, VP.Static(5)) (1): SNCC client, initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue((int)sequence[i][0]);

                Assert.IsTrue(client1.Buffer.SequenceEqual((int[])sequence[i][1]), "RangeTransformation.Create(frame0, VP.Static(5)) (" + (i * 2).ToString() + "): NPC client, after changes to source the result value is not correct.");
                Assert.IsTrue(client2.Buffer.SequenceEqual((int[])sequence[i][1]), "RangeTransformation.Create(frame0, VP.Static(5)) (" + (i * 2 + 1).ToString() + "): SNCC client, after changes to source the result value is not correct.");
            }
        }

        [TestMethod]
        public void CorrectnessTest2()
        {
            var sequence = new object[][] { 
                new object[] { 1, new int[] { } },
                new object[] { 2, new int[] { 1 } },
                new object[] { 3, new int[] { 1, 2 } },
                new object[] { 4, new int[] { 1, 2, 3 } },
                new object[] { 5, new int[] { 1, 2, 3, 4 } },
                new object[] { 1, new int[] { } },
                new object[] { 3, new int[] { 1, 2 } },
                new object[] { 4, new int[] { 1, 2, 3 } },
            };

            var frame0 = new FrameIValueProviderNPC<int>();
            frame0.IsReadOnly = true;

            frame0.SetValue((int)sequence[0][0]);

            var client1 = new CollectionTransformationClientNPC<int>();
            client1.Source = RangeTransformation.Create(VP.Patched(VP.Static(1)), VP.Patched(frame0));

            var client2 = new CollectionTransformationClientSNCC<int>();
            client2.Source = RangeTransformation.Create(VP.Patched(VP.Static(1)), VP.Patched(frame0));

            Assert.IsTrue(client1.Buffer.SequenceEqual((int[])sequence[0][1]), "RangeTransformation.Create(VP.Static(1), frame0) (0): NPC client, initialy the result value is not correct.");
            Assert.IsTrue(client2.Buffer.SequenceEqual((int[])sequence[0][1]), "RangeTransformation.Create(VP.Static(1), frame0) (1): SNCC client, initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue((int)sequence[i][0]);

                Assert.IsTrue(client1.Buffer.SequenceEqual((int[])sequence[i][1]), "RangeTransformation.Create(VP.Static(1), frame0) (" + (i * 2).ToString() + "): NPC client, after changes to source the result value is not correct.");
                Assert.IsTrue(client2.Buffer.SequenceEqual((int[])sequence[i][1]), "RangeTransformation.Create(VP.Static(1), frame0) (" + (i * 2 + 1).ToString() + "): SNCC client, after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner1 : ValueProviderDeterministicEventRegistrationRunnerForSequence<int, int>
        {
            protected override IEnumerable<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return RangeTransformation.Create(VP.Patched(frame), VP.Patched(VP.Static(1)));
            }

            public override string Prefix
            {
                get { return "RangeTransformation.Create(frame, VP.Static(1))"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner1()).Run();
        }

        class DeterministicEventRegistrationRunner2 : ValueProviderDeterministicEventRegistrationRunnerForSequence<int, int>
        {
            protected override IEnumerable<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return RangeTransformation.Create(VP.Patched(VP.Static(1)), VP.Patched(frame));
            }

            public override string Prefix
            {
                get { return "RangeTransformation.Create(VP.Static(1), frame)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            (new DeterministicEventRegistrationRunner2()).Run();
        }

        [TestMethod]
        public void ArgumentsCheck_begin()
        {
            Assert.IsNull(RangeTransformation.Create(null, VP.Patched(VP.Static(1))), "Should return null when begin is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_end()
        {
            Assert.IsNull(RangeTransformation.Create(VP.Patched(VP.Static(1)), null), "Should return null when end is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                VP.Static(1), VP.Static(2),
                VP.Static(1), VP.Static(2),
                (b, e) => RangeTransformation.Create(VP.Patched(b), VP.Patched(e)),
                "RangeTransformation"
            );
        }

        [TestMethod]
        public void ConcurrencyTest()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new int[] { 1, 2, 3, 2, 4, 0, 3, 2, 0 },
                new int[] { 5, 2, 4, 6, 0, 2, 4, 3, 4 },
                (s) => Enumerable.SequenceEqual(s, new int[] { 0, 1, 2, 3 }),
                (frame1, frame2) => VP.Static(RangeTransformation.Create(VP.Patched(frame1), VP.Patched(frame2))),
                "RangeTransformation.Create(frame1, frame2)"
            );
        }
    }
}
