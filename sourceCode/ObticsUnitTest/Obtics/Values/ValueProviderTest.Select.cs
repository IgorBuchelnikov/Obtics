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
        public void SelectTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(i => i + 2).Value,
                12,
                "SelectTest1(a)"
            );

            var s = ValueProvider.Static(10);
            var c = new Func<int, int>(i => i + 2);

            Assert.AreEqual(
                s.Select(c),
                s.Select(c),
                "SelectTest1(c)"
            );
        }

        [TestMethod]
        public void SelectTest2()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(i => ValueProvider.Static(i + 2)).Value,
                12,
                "SelectTest2(a)"
            );

            var s = ValueProvider.Static(10);
            var c = new Func<int, IValueProvider<int>>(i => ValueProvider.Static(i + 2));

            Assert.AreEqual(
                s.Select(c),
                s.Select(c),
                "SelectTest2(c)"
            );
        }

        [TestMethod]
        public void SelectTest3()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(ValueProvider.Static(3), (f, s) => f + s + 2).Value,
                15,
                "SelectTest3(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var c = new Func<int, int, int>((f, s) => f + s + 2);

            Assert.AreEqual(
                first.Select(second, c),
                first.Select(second, c),
                "SelectTest3(c)"
            );
        }

        [TestMethod]
        public void SelectTest4()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(ValueProvider.Static(3), (f, s) => ValueProvider.Static(f + s + 2)).Value,
                15,
                "SelectTest4(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var c = new Func<int, int, IValueProvider<int>>((f, s) => ValueProvider.Static(f + s + 2));

            Assert.AreEqual(
                first.Select(second, c),
                first.Select(second, c),
                "SelectTest4(c)"
            );
        }

        [TestMethod]
        public void SelectTest5()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(ValueProvider.Static(3), ValueProvider.Static(4), (f, s, t) => f + s + t + 2).Value,
                19,
                "SelectTest5(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var c = new Func<int, int, int, int>((f, s, t) => f + s + t + 2);

            Assert.AreEqual(
                first.Select(second, third, c),
                first.Select(second, third, c),
                "SelectTest5(c)"
            );
        }

        [TestMethod]
        public void SelectTest6()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(ValueProvider.Static(3), ValueProvider.Static(4), (f, s, t) => ValueProvider.Static(f + s + t + 2)).Value,
                19,
                "SelectTest6(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var c = new Func<int, int, int, IValueProvider<int>>((f, s, t) => ValueProvider.Static(f + s + t + 2));

            Assert.AreEqual(
                first.Select(second, third, c),
                first.Select(second, third, c),
                "SelectTest6(c)"
            );
        }

        [TestMethod]
        public void SelectTest7()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(ValueProvider.Static(3), ValueProvider.Static(4), ValueProvider.Static(5), (f, s, t, fo) => f + s + t + fo + 2).Value,
                24,
                "SelectTest7(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var fourth = ValueProvider.Static(5);
            var c = new Func<int, int, int, int, int>((f, s, t, fo) => f + s + t + fo + 2);

            Assert.AreEqual(
                first.Select(second, third, fourth, c),
                first.Select(second, third, fourth, c),
                "SelectTest7(c)"
            );
        }

        [TestMethod]
        public void SelectTest8()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Select(ValueProvider.Static(3), ValueProvider.Static(4), ValueProvider.Static(5), (f, s, t, fo) => ValueProvider.Static(f + s + t + fo + 2)).Value,
                24,
                "SelectTest8(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var fourth = ValueProvider.Static(5);
            var c = new Func<int, int, int, int, IValueProvider<int>>((f, s, t, fo) => ValueProvider.Static(f + s + t + fo + 2));

            Assert.AreEqual(
                first.Select(second, third, fourth, c),
                first.Select(second, third, fourth, c),
                "SelectTest8(c)"
            );
        }

        [TestMethod]
        public void SelectTest9()
        {
            Assert.AreEqual(
                new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }.Select(vps => ((int)vps[0]).ToString() + ((string)vps[1])).Value,
                "10S",
                "SelectTest9(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<object[], string>(vps => ((int)vps[0]).ToString() + ((string)vps[1]));

            Assert.AreEqual(
                s.Select(c),
                s.Select(c),
                "SelectTest9(c)"
            );
        }


        [TestMethod]
        public void SelectTest10()
        {
            Assert.AreEqual(
                new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }.Select(vps => ValueProvider.Static(((int)vps[0]).ToString() + ((string)vps[1]))).Value,
                "10S",
                "SelectTest10(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<object[], IValueProvider<string>>(vps => ValueProvider.Static(((int)vps[0]).ToString() + ((string)vps[1])));

            Assert.AreEqual(
                s.Select(c),
                s.Select(c),
                "SelectTest10(c)"
            );
        }

        [TestMethod]
        public void SelectTest11()
        {
            Assert.AreEqual(
                ValueProvider.Select(vps => ((int)vps[0]).ToString() + ((string)vps[1]), new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }).Value,
                "10S",
                "SelectTest11(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<object[], string>(vps => ((int)vps[0]).ToString() + ((string)vps[1]));

            Assert.AreEqual(
                ValueProvider.Select(c, s),
                ValueProvider.Select(c, s),
                "SelectTest11(c)"
            );
        }


        [TestMethod]
        public void SelectTest12()
        {
            Assert.AreEqual(
                ValueProvider.Select(vps => ValueProvider.Static(((int)vps[0]).ToString() + ((string)vps[1])), new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }).Value,
                "10S",
                "SelectTest12(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<object[], IValueProvider<string>>(vps => ValueProvider.Static(((int)vps[0]).ToString() + ((string)vps[1])));

            Assert.AreEqual(
                ValueProvider.Select(c, s),
                ValueProvider.Select(c, s),
                "SelectTest12(c)"
            );
        }

    }
}
