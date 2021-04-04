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
        public void ReadOnlyTest1()
        {
            Assert.IsTrue(
                ValueProvider.Dynamic<int>().ReadOnly().IsReadOnly,
                "ReadOnlyTest1(a)"
            );

            var s = ValueProvider.Dynamic<int>();

            Assert.AreEqual(
                s.ReadOnly(),
                s.ReadOnly(),
                "ReadOnlyTest1(b)"
            );
        }
    }
}
