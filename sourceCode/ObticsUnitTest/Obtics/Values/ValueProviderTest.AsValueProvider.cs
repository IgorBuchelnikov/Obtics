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
        public void AsValueProviderTest1()
        {
            Assert.AreEqual(
                1,
                ValueProvider.Static(1).AsValueProvider().Value,
                "AsValueProviderTest1(a)"
            );

            var s = ValueProvider.Static(1);

            Assert.AreEqual(
                s.AsValueProvider(),
                s.AsValueProvider(),
                "AsValueProviderTest1(b)"
            );
        }

    }
}
