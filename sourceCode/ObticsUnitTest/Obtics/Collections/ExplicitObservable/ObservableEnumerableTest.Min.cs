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
        public void MinTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { null, 1m, 2m, 3m }).Value,
                1m,
                "MinTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { }).Value,
                null,
                "MinTest1(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { null }).Value,
                null,
                "MinTest1(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest1(d)"
            );
        }

        [TestMethod]
        public void MinTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal[] { 1m, 2m, 3m }).Value,
                1m,
                "MinTest2(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Decimal[] { }).Value;}
            );

            var seq = new Decimal[] { 1m, 2m, 3m };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest2(c)"
            );
        }

        [TestMethod]
        public void MinTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { null, 1.0, 2.0, 3.0 }).Value,
                1.0,
                "MinTest3(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { }).Value,
                null,
                "MinTest3(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { null }).Value,
                null,
                "MinTest3(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest3(d)"
            );
        }

        [TestMethod]
        public void MinTest4()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Double[] { 1.0, 2.0, 3.0 }).Value,
                1.0,
                "MinTest4(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Double[] { }).Value;}
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest4(c)"
            );
        }

        [TestMethod]
        public void MinTest5()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { null, 1f, 2f, 3f }).Value,
                1f,
                "MinTest5(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { }).Value,
                null,
                "MinTest5(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { null }).Value,
                null,
                "MinTest5(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest5(d)"
            );
        }

        [TestMethod]
        public void MinTest6()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Single[] { 1f, 2f, 3f }).Value,
                1f,
                "MinTest6(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Single[] { }).Value;}
            );

            var seq = new Single[] { 1f, 2f, 3f };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest6(c)"
            );
        }

        [TestMethod]
        public void MinTest7()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { null, 1, 2, 3 }).Value,
                1,
                "MinTest7(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { }).Value,
                null,
                "MinTest7(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { null }).Value,
                null,
                "MinTest7(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest7(d)"
            );
        }

        [TestMethod]
        public void MinTest8()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32[] { 1, 2, 3 }).Value,
                1,
                "MinTest8(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Int32[] { }).Value;}
            );

            var seq = new Int32[] { 1, 2, 3 };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest8(c)"
            );
        }

        [TestMethod]
        public void MinTest9()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { null, 1L, 2L, 3L }).Value,
                1L,
                "MinTest9(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { }).Value,
                null,
                "MinTest9(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { null }).Value,
                null,
                "MinTest9(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest9(d)"
            );
        }

        [TestMethod]
        public void MinTest10()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64[] { 1L, 2L, 3L }).Value,
                1L,
                "MinTest10(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Int64[] { }).Value; }
            );

            var seq = new Int64[] { 1L, 2L, 3L };

            Assert.AreEqual(
                ObservableEnumerable.Min(seq),
                ObservableEnumerable.Min(seq),
                "MinTest10(c)"
            );
        }



        [TestMethod]
        public void MinTest11()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { null, 1m, 2m, 3m }, v => v ).Value,
                1m,
                "MinTest11(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { }, v => v).Value,
                null,
                "MinTest11(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { null }, v => v).Value,
                null,
                "MinTest11(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };
            var f = new Func<Decimal?, Decimal?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest11(d)"
            );
        }

        [TestMethod]
        public void MinTest12()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal[] { 1m, 2m, 3m }, v => v).Value,
                1m,
                "MinTest12(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy =ObservableEnumerable.Min(new Decimal[] { }, v => v).Value; }
            );

            var seq = new Decimal[] { 1m, 2m, 3m };
            var f = new Func<Decimal, Decimal>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest12(c)"
            );
        }

        [TestMethod]
        public void MinTest13()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { null, 1.0, 2.0, 3.0 }, v => v).Value,
                1.0,
                "MinTest13(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { }, v => v).Value,
                null,
                "MinTest13(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { null }, v => v).Value,
                null,
                "MinTest13(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };
            var f = new Func<Double?, Double?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest13(d)"
            );
        }

        [TestMethod]
        public void MinTest14()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Double[] { 1.0, 2.0, 3.0 }, v => v).Value,
                1.0,
                "MinTest14(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Double[] { }, v => v).Value; }
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };
            var f = new Func<Double, Double>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest14(c)"
            );
        }

        [TestMethod]
        public void MinTest15()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { null, 1f, 2f, 3f }, v => v).Value,
                1f,
                "MinTest15(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { }, v => v).Value,
                null,
                "MinTest15(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { null }, v => v).Value,
                null,
                "MinTest15(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };
            var f = new Func<Single?, Single?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest15(d)"
            );
        }

        [TestMethod]
        public void MinTest16()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Single[] { 1f, 2f, 3f }, v => v).Value,
                1f,
                "MinTest16(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Single[] { }, v => v).Value; }
            );


            var seq = new Single[] { 1f, 2f, 3f };
            var f = new Func<Single, Single>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest16(c)"
            );
        }

        [TestMethod]
        public void MinTest17()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { null, 1, 2, 3 }, v => v).Value,
                1,
                "MinTest17(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { }, v => v).Value,
                null,
                "MinTest17(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { null }, v => v).Value,
                null,
                "MinTest17(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };
            var f = new Func<Int32?, Int32?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest17(d)"
            );
        }

        [TestMethod]
        public void MinTest18()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32[] { 1, 2, 3 }, v => v).Value,
                1,
                "MinTest18(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Int32[] { }, v => v).Value; }
            );

            var seq = new Int32[] { 1, 2, 3 };
            var f = new Func<Int32, Int32>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest18(c)"
            );
        }

        [TestMethod]
        public void MinTest19()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { null, 1L, 2L, 3L }, v => v).Value,
                1L,
                "MinTest19(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { }, v => v).Value,
                null,
                "MinTest19(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { null }, v => v).Value,
                null,
                "MinTest19(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };
            var f = new Func<Int64?, Int64?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest19(d)"
            );
        }

        [TestMethod]
        public void MinTest20()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64[] { 1L, 2L, 3L }, v => v).Value,
                1L,
                "MinTest20(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Int64[] { }, v => v).Value;}
            );

            var seq = new Int64[] { 1L, 2L, 3L };
            var f = new Func<Int64, Int64>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest20(c)"
            );
        }





        [TestMethod]
        public void MinTest21()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { null, 1m, 2m, 3m }, v => ValueProvider.Static(v)).Value,
                1m,
                "MinTest21(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest21(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest21(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };
            var f = new Func<Decimal?, IValueProvider<Decimal?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest21(d)"
            );

        }

        [TestMethod]
        public void MinTest22()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Decimal[] { 1m, 2m, 3m }, v => ValueProvider.Static(v)).Value,
                1m,
                "MinTest22(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Decimal[] { }, v => ValueProvider.Static(v)).Value;}
            );

            var seq = new Decimal[] { 1m, 2m, 3m };
            var f = new Func<Decimal, IValueProvider<Decimal>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest22(c)"
            );

        }

        [TestMethod]
        public void MinTest23()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { null, 1.0, 2.0, 3.0 }, v => ValueProvider.Static(v)).Value,
                1.0,
                "MinTest23(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest23(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Double?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest23(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };
            var f = new Func<Double?, IValueProvider<Double?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest23(d)"
            );
        }

        [TestMethod]
        public void MinTest24()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Double[] { 1.0, 2.0, 3.0 }, v => ValueProvider.Static(v)).Value,
                1.0,
                "MinTest24(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Double[] { }, v => ValueProvider.Static(v)).Value;}
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };
            var f = new Func<Double, IValueProvider<Double>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest24(c)"
            );
        }

        [TestMethod]
        public void MinTest25()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { null, 1f, 2f, 3f }, v => ValueProvider.Static(v)).Value,
                1f,
                "MinTest25(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest25(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Single?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest25(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };
            var f = new Func<Single?, IValueProvider<Single?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest25(d)"
            );
        }

        [TestMethod]
        public void MinTest26()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Single[] { 1f, 2f, 3f }, v => ValueProvider.Static(v)).Value,
                1f,
                "MinTest26(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Single[] { }, v => ValueProvider.Static(v)).Value;}
            );

            var seq = new Single[] { 1f, 2f, 3f };
            var f = new Func<Single, IValueProvider<Single>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest26(c)"
            );
        }

        [TestMethod]
        public void MinTest27()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { null, 1, 2, 3 }, v => ValueProvider.Static(v)).Value,
                1,
                "MinTest27(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest27(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest27(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };
            var f = new Func<Int32?, IValueProvider<Int32?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest27(d)"
            );
        }

        [TestMethod]
        public void MinTest28()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int32[] { 1, 2, 3 }, v => ValueProvider.Static(v)).Value,
                1,
                "MinTest28(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Int32[] { }, v => ValueProvider.Static(v)).Value;}
            );

            var seq = new Int32[] { 1, 2, 3 };
            var f = new Func<Int32, IValueProvider<Int32>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest28(c)"
            );
        }

        [TestMethod]
        public void MinTest29()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { null, 1L, 2L, 3L }, v => ValueProvider.Static(v)).Value,
                1L,
                "MinTest29(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest29(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "MinTest29(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };
            var f = new Func<Int64?, IValueProvider<Int64?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest29(d)"
            );
        }

        [TestMethod]
        public void MinTest30()
        {
            Assert.AreEqual(
                ObservableEnumerable.Min(new Int64[] { 1L, 2L, 3L }, v => ValueProvider.Static(v)).Value,
                1L,
                "MinTest30(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Min(new Int64[] { }, v => ValueProvider.Static(v)).Value;}
            );

            var seq = new Int64[] { 1L, 2L, 3L };
            var f = new Func<Int64, IValueProvider<Int64>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Min(seq, f),
                ObservableEnumerable.Min(seq, f),
                "MinTest30(c)"
            );
        }
    }
}
