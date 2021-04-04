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
        public void GetValueOrDefaultTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).GetValueOrDefault(),
                10,
                "GetValueOrDefaultTest1(a)"
            );

            Assert.AreEqual(
                ((IValueProvider<int>)null).GetValueOrDefault(),
                default(int),
                "GetValueOrDefaultTest1(b)"
            );
        }

        [TestMethod]
        public void GetValueOrDefaultTest2()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).GetValueOrDefault(2),
                10,
                "GetValueOrDefaultTest2(a)"
            );

            Assert.AreEqual(
                ((IValueProvider<int>)null).GetValueOrDefault(2),
                2,
                "GetValueOrDefaultTest2(b)"
            );
        }


    }
}
