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
        public void CastTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Cast<long>().Value,
                10L,
                "CastTest1(a)"
            );

            Assert.AreEqual(
                ValueProvider.Static(10).Cast<int>().Value,
                10,
                "CastTest1(b)"
            );

            Assert.AreEqual(
                ValueProvider.Static("A").Cast<int>().Value,
                default(int),
                "CastTest1(c)"
            );

            var s = ValueProvider.Static(10);

            Assert.AreEqual(
                s.Cast<long>(),
                s.Cast<long>(),
                "CastTest1(d)"
            );
        }
    }
}
