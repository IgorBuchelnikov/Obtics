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
        public void StaticTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(13).Value,
                13,
                "StaticTest1(a)"
            );

            Assert.AreEqual(
                ValueProvider.Static(13),
                ValueProvider.Static(13),
                "StaticTest1(b)"
            );
        }

    }
}
