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
        public void DynamicTest1()
        {
            Assert.AreEqual(
                ValueProvider.Dynamic<int>().Value,
                default(int),
                "DynamicTest1(a)"
            );

            Assert.IsFalse(
                ValueProvider.Dynamic<int>().IsReadOnly,
                "DynamicTest1(b)"
            );

            //*NOT* equal 
            Assert.AreNotEqual(
                ValueProvider.Dynamic<int>(),
                ValueProvider.Dynamic<int>(),
                "DynamicTest1(c)"
            );
        }

        [TestMethod]
        public void DynamicTest2()
        {
            Assert.AreEqual(
                ValueProvider.Dynamic(13).Value,
                13,
                "DynamicTest2(a)"
            );

            Assert.IsFalse(
                ValueProvider.Dynamic(13).IsReadOnly,
                "DynamicTest2(b)"
            );

            //*NOT* equal 
            Assert.AreNotEqual(
                ValueProvider.Dynamic(13),
                ValueProvider.Dynamic(13),
                "DynamicTest2(c)"
            );
        }

    }
}
