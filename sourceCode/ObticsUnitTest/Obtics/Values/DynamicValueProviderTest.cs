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
using Obtics.Values;
using Obtics;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for DynamicValueProviderTest
    /// </summary>
    [TestClass]
    public class DynamicValueProviderTest
    {
        public DynamicValueProviderTest()
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

            var dvp = ValueProvider.Dynamic<int>();

            dvp.Value = sequence[0];

            var client = new ValueProviderClientNPC<int>();
            client.Source = dvp;

            Assert.AreEqual<int>(client.Buffer, sequence[0], "ValueProvider.Dynamic() (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                dvp.Value = sequence[i];

                Assert.AreEqual<int>(client.Buffer, sequence[i], "ValueProvider.Dynamic() (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }
    }
}
