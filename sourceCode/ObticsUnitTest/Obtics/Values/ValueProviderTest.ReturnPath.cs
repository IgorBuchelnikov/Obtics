using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Values;
using Obtics;

namespace ObticsUnitTest.Obtics.Values
{
    public partial class ValueProviderTest
    {
        [TestMethod]
        public void ReturnPathTest1()
        {
            var src = ValueProvider.Dynamic(3);
            var rp = src.ReturnPath(v => src.Value = v);

            Assert.AreEqual(
                rp.Value,
                3,
                "ReturnPathTest1(a)"
            );

            rp.Value = 5;

            Assert.AreEqual(
                rp.Value,
                5,
                "ReturnPathTest1(b)"
            );

            Action<int> returnPath = x => src.Value = x;

            Assert.AreSame(
                src.ReturnPath(returnPath),
                src.ReturnPath(returnPath),
                "ReturnPathTest1(c)"
            );
        }

        [TestMethod]
        public void ReturnPathTest2()
        {
            var src = ValueProvider.Dynamic(3);
            var roVp = ValueProvider.Dynamic(false);

            var rp = src.ReturnPath(v => src.Value = v, roVp);

            Assert.AreEqual(
                rp.Value,
                3,
                "ReturnPathTest2(a)"
            );

            rp.Value = 5;

            Assert.AreEqual(
                rp.Value,
                5,
                "ReturnPathTest2(b)"
            );

            roVp.Value = true;

            try
            {
                rp.Value = 7;
                Assert.Fail("ReturnPathTest2(c)");
            }
            catch (InvalidOperationException)
            { }

            Action<int> returnPath = x => src.Value = x;

            Assert.AreSame(
                src.ReturnPath(returnPath, roVp),
                src.ReturnPath(returnPath, roVp),
                "ReturnPathTest2(d)"
            );
        }


        [TestMethod]
        public void ReturnPathTest3()
        {
            var src = ValueProvider.Dynamic(3);
            var roVp = ValueProvider.Dynamic(false);

            var rp = src.ReturnPath((v, ro) => { if (!ro) src.Value = v; }, roVp);

            Assert.AreEqual(
                rp.Value,
                3,
                "ReturnPathTest3(a)"
            );

            rp.Value = 5;

            Assert.AreEqual(
                rp.Value,
                5,
                "ReturnPathTest3(b)"
            );

            roVp.Value = true;

            rp.Value = 7;

            Assert.AreEqual(
                rp.Value,
                5,
                "ReturnPathTest3(c)"
            );

            Action<int,bool> returnPath = (x, ro) => src.Value = x;

            Assert.AreSame(
                src.ReturnPath(returnPath, roVp),
                src.ReturnPath(returnPath, roVp),
                "ReturnPathTest3(d)"
            );
        }

    }
}
