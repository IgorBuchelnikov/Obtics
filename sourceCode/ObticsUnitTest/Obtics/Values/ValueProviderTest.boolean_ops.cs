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
        public void AndTest1()
        {
            Assert.IsTrue(
                ValueProvider.Static(true).And(ValueProvider.Static(true)).Value,
                "AndTest1(a)"
            );

            Assert.IsFalse(
                ValueProvider.Static(false).And(ValueProvider.Static(true)).Value,
                "AndTest1(b)"
            );

            Assert.IsFalse(
                ValueProvider.Static(true).And(ValueProvider.Static(false)).Value,
                "AndTest1(c)"
            );

            Assert.IsFalse(
                ValueProvider.Static(false).And(ValueProvider.Static(false)).Value,
                "AndTest1(d)"
            );


            var first = ValueProvider.Static(true);
            var second = ValueProvider.Static(false);

            Assert.AreEqual(
                first.And(second),
                first.And(second),
                "AndTest1(e)"
            );
        }

        [TestMethod]
        public void OrTest1()
        {
            Assert.IsTrue(
                ValueProvider.Static(true).Or(ValueProvider.Static(true)).Value,
                "OrTest1(a)"
            );

            Assert.IsTrue(
                ValueProvider.Static(false).Or(ValueProvider.Static(true)).Value,
                "OrTest1(b)"
            );

            Assert.IsTrue(
                ValueProvider.Static(true).Or(ValueProvider.Static(false)).Value,
                "OrTest1(c)"
            );

            Assert.IsFalse(
                ValueProvider.Static(false).Or(ValueProvider.Static(false)).Value,
                "OrTest1(d)"
            );


            var first = ValueProvider.Static(true);
            var second = ValueProvider.Static(false);

            Assert.AreEqual(
                first.Or(second),
                first.Or(second),
                "OrTest1(e)"
            );
        }

        [TestMethod]
        public void XOrTest1()
        {
            Assert.IsFalse(
                ValueProvider.Static(true).XOr(ValueProvider.Static(true)).Value,
                "XOrTest1(a)"
            );

            Assert.IsTrue(
                ValueProvider.Static(false).XOr(ValueProvider.Static(true)).Value,
                "XOrTest1(b)"
            );

            Assert.IsTrue(
                ValueProvider.Static(true).XOr(ValueProvider.Static(false)).Value,
                "XOrTest1(c)"
            );

            Assert.IsFalse(
                ValueProvider.Static(false).XOr(ValueProvider.Static(false)).Value,
                "XOrTest1(d)"
            );


            var first = ValueProvider.Static(true);
            var second = ValueProvider.Static(false);

            Assert.AreEqual(
                first.XOr(second),
                first.XOr(second),
                "XOrTest1(e)"
            );
        }

        [TestMethod]
        public void InvertTest1()
        {
            Assert.IsTrue(
                ValueProvider.Static(false).Invert().Value,
                "InvertTest1(a)"
            );

            Assert.IsFalse(
                ValueProvider.Static(true).Invert().Value,
                "InvertTest1(b)"
            );

            var s = ValueProvider.Static(true);

            Assert.AreEqual(
                s.Invert(),
                s.Invert(),
                "InvertTest1(e)"
            );
        }

        [TestMethod]
        public void IIfTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(true).IIf(ValueProvider.Static(1), ValueProvider.Static(2)).Value,
                1,
                "IIfTest1(a)"
            );

            Assert.AreEqual(
                ValueProvider.Static(false).IIf(ValueProvider.Static(1), ValueProvider.Static(2)).Value,
                2,
                "IIfTest1(b)"
            );

            var s = ValueProvider.Static(true);
            var first = ValueProvider.Static(1);
            var second = ValueProvider.Static(2);

            Assert.AreEqual(
                s.IIf(first, second),
                s.IIf(first, second),
                "IIfTest1(e)"
            );
        }

    }
}
