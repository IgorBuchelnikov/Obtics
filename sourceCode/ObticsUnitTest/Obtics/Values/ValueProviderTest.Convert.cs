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
        public void ConvertTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(i => i.Value + 2).Value,
                12,
                "ConvertTest1(a)"
            );

            var s = ValueProvider.Static(10);
            var c = new Func<IValueProvider<int>, int>(i => i.Value + 2);

            Assert.AreEqual(
                s.Convert(c),
                s.Convert(c),
                "ConvertTest1(c)"
            );
        }

        [TestMethod]
        public void ConvertTest2()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(i => ValueProvider.Static(i.Value + 2)).Value,
                12,
                "ConvertTest2(a)"
            );

            var s = ValueProvider.Static(10);
            var c = new Func<IValueProvider<int>, IValueProvider<int>>(i => ValueProvider.Static(i.Value + 2));

            Assert.AreEqual(
                s.Convert(c),
                s.Convert(c),
                "ConvertTest2(c)"
            );
        }

        [TestMethod]
        public void ConvertTest3()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(ValueProvider.Static(3), (f, s) => f.Value + s.Value + 2).Value,
                15,
                "ConvertTest3(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var c = new Func<IValueProvider<int>, IValueProvider<int>, int>((f, s) => f.Value + s.Value + 2);

            Assert.AreEqual(
                first.Convert(second, c),
                first.Convert(second, c),
                "ConvertTest3(c)"
            );
        }

        [TestMethod]
        public void ConvertTest4()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(ValueProvider.Static(3), (f, s) => ValueProvider.Static(f.Value + s.Value + 2)).Value,
                15,
                "ConvertTest4(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var c = new Func<IValueProvider<int>, IValueProvider<int>, IValueProvider<int>>((f, s) => ValueProvider.Static(f.Value + s.Value + 2));

            Assert.AreEqual(
                first.Convert(second, c),
                first.Convert(second, c),
                "ConvertTest4(c)"
            );
        }

        [TestMethod]
        public void ConvertTest5()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(ValueProvider.Static(3), ValueProvider.Static(4), (f, s, t) => f.Value + s.Value + t.Value + 2).Value,
                19,
                "ConvertTest5(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var c = new Func<IValueProvider<int>, IValueProvider<int>, IValueProvider<int>, int>((f, s, t) => f.Value + s.Value + t.Value + 2);

            Assert.AreEqual(
                first.Convert(second, third, c),
                first.Convert(second, third, c),
                "ConvertTest5(c)"
            );
        }

        [TestMethod]
        public void ConvertTest6()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(ValueProvider.Static(3), ValueProvider.Static(4), (f, s, t) => ValueProvider.Static(f.Value + s.Value + t.Value + 2)).Value,
                19,
                "ConvertTest6(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var c = new Func<IValueProvider<int>, IValueProvider<int>, IValueProvider<int>, IValueProvider<int>>((f, s, t) => ValueProvider.Static(f.Value + s.Value + t.Value + 2));

            Assert.AreEqual(
                first.Convert(second, third, c),
                first.Convert(second, third, c),
                "ConvertTest6(c)"
            );
        }

        [TestMethod]
        public void ConvertTest7()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(ValueProvider.Static(3), ValueProvider.Static(4), ValueProvider.Static(5), (f, s, t, fo) => f.Value + s.Value + t.Value + fo.Value + 2).Value,
                24,
                "ConvertTest7(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var fourth = ValueProvider.Static(5);
            var c = new Func<IValueProvider<int>, IValueProvider<int>, IValueProvider<int>, IValueProvider<int>, int>((f, s, t, fo) => f.Value + s.Value + t.Value + fo.Value + 2);

            Assert.AreEqual(
                first.Convert(second, third, fourth, c),
                first.Convert(second, third, fourth, c),
                "ConvertTest7(c)"
            );
        }

        [TestMethod]
        public void ConvertTest8()
        {
            Assert.AreEqual(
                ValueProvider.Static(10).Convert(ValueProvider.Static(3), ValueProvider.Static(4), ValueProvider.Static(5), (f, s, t, fo) => ValueProvider.Static(f.Value + s.Value + t.Value + fo.Value + 2)).Value,
                24,
                "ConvertTest8(a)"
            );

            var first = ValueProvider.Static(10);
            var second = ValueProvider.Static(3);
            var third = ValueProvider.Static(4);
            var fourth = ValueProvider.Static(5);
            var c = new Func<IValueProvider<int>, IValueProvider<int>, IValueProvider<int>, IValueProvider<int>, IValueProvider<int>>((f, s, t, fo) => ValueProvider.Static(f.Value + s.Value + t.Value + fo.Value + 2));

            Assert.AreEqual(
                first.Convert(second, third, fourth, c),
                first.Convert(second, third, fourth, c),
                "ConvertTest8(c)"
            );
        }

        [TestMethod]
        public void ConvertTest9()
        {
            Assert.AreEqual(
                new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }.Convert(vps => ((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value).Value,
                "10S",
                "ConvertTest9(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<IValueProvider[], string>(vps => ((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value);

            Assert.AreEqual(
                s.Convert(c),
                s.Convert(c),
                "ConvertTest9(c)"
            );
        }


        [TestMethod]
        public void ConvertTest10()
        {
            Assert.AreEqual(
                new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }.Convert(vps => ValueProvider.Static(((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value)).Value,
                "10S",
                "ConvertTest10(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<IValueProvider[], IValueProvider<string>>(vps => ValueProvider.Static(((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value));

            Assert.AreEqual(
                s.Convert(c),
                s.Convert(c),
                "ConvertTest10(c)"
            );
        }

        [TestMethod]
        public void ConvertTest11()
        {
            Assert.AreEqual(
                ValueProvider.Convert(vps => ((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value, new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }).Value,
                "10S",
                "ConvertTest11(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<IValueProvider[], string>(vps => ((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value);

            Assert.AreEqual(
                ValueProvider.Convert(c, s),
                ValueProvider.Convert(c, s),
                "ConvertTest11(c)"
            );
        }


        [TestMethod]
        public void ConvertTest12()
        {
            Assert.AreEqual(
                ValueProvider.Convert(vps => ValueProvider.Static(((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value), new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") }).Value,
                "10S",
                "ConvertTest12(a)"
            );

            var s = new IValueProvider[] { ValueProvider.Static(10), ValueProvider.Static("S") };
            var c = new Func<IValueProvider[], IValueProvider<string>>(vps => ValueProvider.Static(((IValueProvider<int>)vps[0]).Value.ToString() + ((IValueProvider<string>)vps[1]).Value));

            Assert.AreEqual(
                ValueProvider.Convert(c, s),
                ValueProvider.Convert(c, s),
                "ConvertTest12(c)"
            );
        }

    }
}
