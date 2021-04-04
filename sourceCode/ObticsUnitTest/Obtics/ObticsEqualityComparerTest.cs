using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using System.Collections.ObjectModel;
using Obtics.Values;
using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.Obtics
{
    /// <summary>
    /// Summary description for ObticsEqualityComparer
    /// </summary>
    [TestClass]
    public class ObticsEqualityComparerTest
    {
        public ObticsEqualityComparerTest()
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

        [global::Obtics.ObticsEqualityComparerAttribute(typeof(CObjEqualityComparer))]
        public class CObj
        { public int V; }

        public class DObj : CObj
        { }

        public class CObjEqualityComparer : IEqualityComparer<CObj>, System.Collections.IEqualityComparer
        {
            #region IEqualityComparer<CObj> Members

            public bool Equals(CObj x, CObj y)
            {
                return x.V == y.V;
            }

            public int GetHashCode(CObj obj)
            {
                return obj.V.GetHashCode();
            }

            #endregion

            #region IEqualityComparer Members

            public bool Equals(object x, object y)
            {
                return Equals((CObj)x, (CObj)y);
            }

            public int GetHashCode(object obj)
            {
                return GetHashCode((CObj)obj);
            }

            #endregion
        }

        [TestMethod]
        public void SettingObticsEqualityComparerAttributeChangesObticsBehaviour()
        {
            var source = new ObservableCollection<int>();

            var transformation =
                ExpressionObserver.Execute(
                    source,
                    s => s.Select(i => new DObj() { V = i }).OrderBy(o => o.V).Select(o => o.V)
                ).Cascade();

            var client = new CollectionTransformationClientSNCC<int>();
            client.Source = transformation;

            source.Add(1);
            source.Add(2);
            source.Add(3);
            source.Remove(1);
            source.Remove(2);

            Assert.AreEqual(3, client.Buffer.Sum(), "Expected 4 as total");
        }
    }
}
