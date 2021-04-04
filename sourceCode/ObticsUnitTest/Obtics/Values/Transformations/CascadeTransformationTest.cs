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
using Obtics;
using Obtics.Values.Transformations;
using Obtics.Values;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for JoinTransformationTest
    /// </summary>
    [TestClass]
    public class CascadeTransformationTest
    {
        public CascadeTransformationTest()
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
            var frame0 = new FrameIValueProviderNPC<int>();
            frame0.IsReadOnly = true;

            var frame1 = new FrameIValueProviderNPC<int>();
            frame1.IsReadOnly = true;

            var frame = new FrameIValueProviderNPC<IValueProvider<int>>();
            frame.IsReadOnly = true;

            var sequence = new object[][] { 
                new object[] { frame0, 5, 5 },
                new object[] { null, 6, 6 },
                new object[] { frame1, 3, 3 },
                new object[] { null, 4, 4 },
                new object[] { frame0, null, 6 },
                new object[] { null, 7, 7 },
                new object[] { frame1, null, 4 },
                new object[] { frame0, null, 7 },
            };

            if (sequence[0][0] != null)
                frame.SetValue((IValueProvider<int>)sequence[0][0]);

            if (sequence[0][1] != null)
                ((FrameIValueProviderNPC<int>)frame.Value).SetValue((int)sequence[0][1]);

            var client = new ValueProviderClientNPC<int>();
            client.Source = CascadeTransformation<int, IValueProvider<int>>.Create(frame.Patched());

            Assert.AreEqual<int>(client.Buffer, (int)sequence[0][2], "new JoinTransformation<int>(frame0) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                if (sequence[i][0] != null)
                    frame.SetValue((IValueProvider<int>)sequence[i][0]);

                if (sequence[i][1] != null)
                    ((FrameIValueProviderNPC<int>)frame.Value).SetValue((int)sequence[i][1]);

                Assert.AreEqual<int>(client.Buffer, (int)sequence[i][2], "new JoinTransformation<int>(frame0) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner1 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return CascadeTransformation<int, IValueProvider<int>>.Create(ValueProvider.Static((IValueProvider<int>)frame).Patched());
            }

            public override string Prefix
            {
                get { return "new JoinTransformation<int,IValueProvider<int>,IValueProvider<IValueProvider<int>>>( new StaticValueProvider<IValueProvider<int>>( frame ) )"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest1()
        {
            (new DeterministicEventRegistrationRunner1()).Run();
        }

        class DeterministicEventRegistrationRunner2 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<IValueProvider<int>, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<IValueProvider<int>> frame)
            {
                return CascadeTransformation<int, IValueProvider<int>>.Create(frame.Patched()).OnException((NullReferenceException ex) => 0);
            }

            public override string Prefix
            {
                get { return "new JoinTransformation<int, IValueProvider<int>, IValueProvider<IValueProvider<int>>>(frame)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            (new DeterministicEventRegistrationRunner2()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(CascadeTransformation<int, IValueProvider<int>>.Create(null), "Should return null when source is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<IValueProvider<int>>)ValueProvider.Static(ValueProvider.Static(23)),
                (IValueProvider<IValueProvider<int>>)ValueProvider.Static(ValueProvider.Static(24)),
                s => CascadeTransformation<int, IValueProvider<int>>.Create(s.Patched()),
                "JoinTransformation<int, IValueProvider<int>, IValueProvider<IValueProvider<int>>>"
            );
        }

    }
}
