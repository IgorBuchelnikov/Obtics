using Obtics.Collections;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using System.Collections.Generic;
using System;
using Obtics.Values;
using System.Collections;
using System.Linq;
using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {

        [TestMethod]
        public void MaxTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { null, 1m, 2m, 3m }).Value,
                3m,
                "MaxTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { }).Value,
                null,
                "MaxTest1(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { null }).Value,
                null,
                "MaxTest1(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest1(d)"
            );
        }

        [TestMethod]
        public void MaxTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal[] { 1m, 2m, 3m }).Value,
                3m,
                "MaxTest2(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Decimal[] { }).Value; }
            );

            var seq = new Decimal[] { 1m, 2m, 3m };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest2(c)"
            );
        }

        [TestMethod]
        public void MaxTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { null, 1.0, 2.0, 3.0 }).Value,
                3.0,
                "MaxTest3(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { }).Value,
                null,
                "MaxTest3(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { null }).Value,
                null,
                "MaxTest3(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest3(d)"
            );
        }

        [TestMethod]
        public void MaxTest4()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Double[] { 1.0, 2.0, 3.0 }).Value,
                3.0,
                "MaxTest4(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Double[] { }).Value; }
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest4(c)"
            );
        }

        [TestMethod]
        public void MaxTest5()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { null, 1f, 2f, 3f }).Value,
                3f,
                "MaxTest5(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { }).Value,
                null,
                "MaxTest5(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { null }).Value,
                null,
                "MaxTest5(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest5(d)"
            );
        }

        [TestMethod]
        public void MaxTest6()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Single[] { 1f, 2f, 3f }).Value,
                3f,
                "MaxTest6(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Single[] { }).Value; }
            );

            var seq = new Single[] { 1f, 2f, 3f };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest6(c)"
            );
        }

        [TestMethod]
        public void MaxTest7()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { null, 1, 2, 3 }).Value,
                3,
                "MaxTest7(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { }).Value,
                null,
                "MaxTest7(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { null }).Value,
                null,
                "MaxTest7(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest7(d)"
            );
        }

        [TestMethod]
        public void MaxTest8()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32[] { 1, 2, 3 }).Value,
                3,
                "MaxTest8(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Int32[] { }).Value; }
            );

            var seq = new Int32[] { 1, 2, 3 };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest8(c)"
            );
        }

        [TestMethod]
        public void MaxTest9()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { null, 1L, 2L, 3L }).Value,
                3L,
                "MaxTest9(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { }).Value,
                null,
                "MaxTest9(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { null }).Value,
                null,
                "MaxTest9(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest9(d)"
            );
        }

        [TestMethod]
        public void MaxTest10()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64[] { 1L, 2L, 3L }).Value,
                3L,
                "MaxTest10(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var rdummy =ObservableEnumerable.Max(new Int64[] { }).Value ; }
            );

            var seq = new Int64[] { 1L, 2L, 3L };

            Assert.AreEqual(
                ObservableEnumerable.Max(seq),
                ObservableEnumerable.Max(seq),
                "MaxTest10(c)"
            );
        }



        [TestMethod]
        public void MaxTest11()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { null, 1m, 2m, 3m }, v => v ).Value,
                3m,
                "MaxTest11(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { }, v => v).Value,
                null,
                "MaxTest11(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { null }, v => v).Value,
                null,
                "MaxTest11(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };
            var f = new Func<Decimal?, Decimal?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest11(d)"
            );
        }

        [TestMethod]
        public void MaxTest12()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal[] { 1m, 2m, 3m }, v => v).Value,
                3m,
                "MaxTest12(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Decimal[] { }, v => v).Value; }
            );

            var seq = new Decimal[] { 1m, 2m, 3m };
            var f = new Func<Decimal, Decimal>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest12(c)"
            );
        }

        [TestMethod]
        public void MaxTest13()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { null, 1.0, 2.0, 3.0 }, v => v).Value,
                3.0,
                "MaxTest13(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { }, v => v).Value,
                null,
                "MaxTest13(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { null }, v => v).Value,
                null,
                "MaxTest13(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };
            var f = new Func<Double?, Double?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest13(d)"
            );
        }

        [TestMethod]
        public void MaxTest14()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Double[] { 1.0, 2.0, 3.0 }, v => v).Value,
                3.0,
                "MaxTest14(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Double[] { }, v => v).Value; }
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };
            var f = new Func<Double, Double>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest14(c)"
            );
        }

        [TestMethod]
        public void MaxTest15()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { null, 1f, 2f, 3f }, v => v).Value,
                3f,
                "MaxTest15(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { }, v => v).Value,
                null,
                "MaxTest15(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { null }, v => v).Value,
                null,
                "MaxTest15(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };
            var f = new Func<Single?, Single?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest15(d)"
            );
        }

        [TestMethod]
        public void MaxTest16()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Single[] { 1f, 2f, 3f }, v => v).Value,
                3f,
                "MaxTest16(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Single[] { }, v => v).Value; }
            );

            var seq = new Single[] { 1f, 2f, 3f };
            var f = new Func<Single, Single>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest16(c)"
            );
        }

        [TestMethod]
        public void MaxTest17()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { null, 1, 2, 3 }, v => v).Value,
                3,
                "MaxTest17(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { }, v => v).Value,
                null,
                "MaxTest17(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { null }, v => v).Value,
                null,
                "MaxTest17(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };
            var f = new Func<Int32?, Int32?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest17(d)"
            );
        }

        [TestMethod]
        public void MaxTest18()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32[] { 1, 2, 3 }, v => v).Value,
                3,
                "MaxTest18(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Int32[] { }, v => v).Value; }
            );

            var seq = new Int32[] { 1, 2, 3 };
            var f = new Func<Int32, Int32>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest18(c)"
            );
        }

        [TestMethod]
        public void MaxTest19()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { null, 1L, 2L, 3L }, v => v).Value,
                3L,
                "MaxTest19(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { }, v => v).Value,
                null,
                "MaxTest19(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { null }, v => v).Value,
                null,
                "MaxTest19(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };
            var f = new Func<Int64?, Int64?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest19(d)"
            );
        }

        [TestMethod]
        public void MaxTest20()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64[] { 1L, 2L, 3L }, v => v).Value,
                3L,
                "MaxTest20(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Int64[] { }, v => v).Value; }
            );

            var seq = new Int64[] { 1L, 2L, 3L };
            var f = new Func<Int64, Int64>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest20(c)"
            );
        }





        [TestMethod]
        public void MaxTest21()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { null, 1m, 2m, 3m }, v => ValueProvider.Static(v)).Value,
                3m,
                "MaxTest21(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest21(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest21(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };
            var f = new Func<Decimal?, IValueProvider<Decimal?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest21(d)"
            );

        }

        [TestMethod]
        public void MaxTest22()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Decimal[] { 1m, 2m, 3m }, v => ValueProvider.Static(v)).Value,
                3m,
                "MaxTest22(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Decimal[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Decimal[] { 1m, 2m, 3m };
            var f = new Func<Decimal, IValueProvider<Decimal>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest22(c)"
            );

        }

        [TestMethod]
        public void MaxTest23()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { null, 1.0, 2.0, 3.0 }, v => ValueProvider.Static(v)).Value,
                3.0,
                "MaxTest23(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest23(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Double?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest23(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };
            var f = new Func<Double?, IValueProvider<Double?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest23(d)"
            );
        }

        [TestMethod]
        public void MaxTest24()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Double[] { 1.0, 2.0, 3.0 }, v => ValueProvider.Static(v)).Value,
                3.0,
                "MaxTest24(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Double[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };
            var f = new Func<Double, IValueProvider<Double>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest24(c)"
            );
        }

        [TestMethod]
        public void MaxTest25()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { null, 1f, 2f, 3f }, v => ValueProvider.Static(v)).Value,
                3f,
                "MaxTest25(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest25(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Single?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest25(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };
            var f = new Func<Single?, IValueProvider<Single?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest25(d)"
            );
        }

        [TestMethod]
        public void MaxTest26()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Single[] { 1f, 2f, 3f }, v => ValueProvider.Static(v)).Value,
                3f,
                "MaxTest26(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Single[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Single[] { 1f, 2f, 3f };
            var f = new Func<Single, IValueProvider<Single>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest26(c)"
            );
        }

        [TestMethod]
        public void MaxTest27()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { null, 1, 2, 3 }, v => ValueProvider.Static(v)).Value,
                3,
                "MaxTest27(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest27(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest27(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };
            var f = new Func<Int32?, IValueProvider<Int32?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest27(d)"
            );
        }

        [TestMethod]
        public void MaxTest28()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int32[] { 1, 2, 3 }, v => ValueProvider.Static(v)).Value,
                3,
                "MaxTest28(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Int32[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Int32[] { 1, 2, 3 };
            var f = new Func<Int32, IValueProvider<Int32>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest28(c)"
            );
        }

        [TestMethod]
        public void MaxTest29()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { null, 1L, 2L, 3L }, v => ValueProvider.Static(v)).Value,
                3L,
                "MaxTest29(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest29(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MaxTest29(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };
            var f = new Func<Int64?, IValueProvider<Int64?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest29(d)"
            );
        }

        [TestMethod]
        public void MaxTest30()
        {
            Assert.AreEqual(
                ObservableEnumerable.Max(new Int64[] { 1L, 2L, 3L }, v => ValueProvider.Static(v)).Value,
                3L,
                "MaxTest30(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Max(new Int64[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Int64[] { 1L, 2L, 3L };
            var f = new Func<Int64, IValueProvider<Int64>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Max(seq, f),
                ObservableEnumerable.Max(seq, f),
                "MaxTest30(c)"
            );
        }
    }
}
