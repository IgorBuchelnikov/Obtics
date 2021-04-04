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
        public void AverageTest1()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { null, 1m, 2m, 3m }).Value,
                2m,
                "AverageTest1(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { }).Value,
                null,
                "AverageTest1(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { null }).Value,
                null,
                "AverageTest1(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest1(d)"
            );
        }

        [TestMethod]
        public void AverageTest2()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal[] { 1m, 2m, 3m }).Value,
                2m,
                "AverageTest2(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Decimal[] { }).Value; }
            );

            var seq = new Decimal[] { 1m, 2m, 3m };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest2(c)"
            );
        }

        [TestMethod]
        public void AverageTest3()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { null, 1.0, 2.0, 3.0 }).Value,
                2.0,
                "AverageTest3(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { }).Value,
                null,
                "AverageTest3(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { null }).Value,
                null,
                "AverageTest3(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest3(d)"
            );
        }

        [TestMethod]
        public void AverageTest4()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Double[] { 1.0, 2.0, 3.0 }).Value,
                2.0,
                "AverageTest4(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Double[] { }).Value; }
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest4(c)"
            );
        }

        [TestMethod]
        public void AverageTest5()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { null, 1f, 2f, 3f }).Value,
                2f,
                "AverageTest5(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { }).Value,
                null,
                "AverageTest5(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { null }).Value,
                null,
                "AverageTest5(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest5(d)"
            );
        }

        [TestMethod]
        public void AverageTest6()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Single[] { 1f, 2f, 3f }).Value,
                2f,
                "AverageTest6(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Single[] { }).Value; }
            );

            var seq = new Single[] { 1f, 2f, 3f };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest6(c)"
            );
        }

        [TestMethod]
        public void AverageTest7()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { null, 1, 2, 3 }).Value,
                2.0,
                "AverageTest7(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { }).Value,
                null,
                "AverageTest7(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { null }).Value,
                null,
                "AverageTest7(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest7(d)"
            );
        }

        [TestMethod]
        public void AverageTest8()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32[] { 1, 2, 3 }).Value,
                2.0,
                "AverageTest8(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Int32[] { }).Value; }
            );

            var seq = new Int32[] { 1, 2, 3 };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest8(c)"
            );
        }

        [TestMethod]
        public void AverageTest9()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { null, 1L, 2L, 3L }).Value,
                2.0,
                "AverageTest9(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { }).Value,
                null,
                "AverageTest9(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { null }).Value,
                null,
                "AverageTest9(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest9(d)"
            );
        }

        [TestMethod]
        public void AverageTest10()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64[] { 1L, 2L, 3L }).Value,
                2.0,
                "AverageTest10(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Int64[] { }).Value; }
            );

            var seq = new Int64[] { 1L, 2L, 3L };

            Assert.AreEqual(
                ObservableEnumerable.Average(seq),
                ObservableEnumerable.Average(seq),
                "AverageTest10(c)"
            );
        }



        [TestMethod]
        public void AverageTest11()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { null, 1m, 2m, 3m }, v => v ).Value,
                2m,
                "AverageTest11(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { }, v => v).Value,
                null,
                "AverageTest11(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { null }, v => v).Value,
                null,
                "AverageTest11(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };
            var f = new Func<Decimal?, Decimal?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest11(d)"
            );
        }

        [TestMethod]
        public void AverageTest12()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal[] { 1m, 2m, 3m }, v => v).Value,
                2m,
                "AverageTest12(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Decimal[] { }, v => v).Value; }
            );

            var seq = new Decimal[] { 1m, 2m, 3m };
            var f = new Func<Decimal, Decimal>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest12(c)"
            );
        }

        [TestMethod]
        public void AverageTest13()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { null, 1.0, 2.0, 3.0 }, v => v).Value,
                2.0,
                "AverageTest13(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { }, v => v).Value,
                null,
                "AverageTest13(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { null }, v => v).Value,
                null,
                "AverageTest13(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };
            var f = new Func<Double?, Double?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest13(d)"
            );
        }

        [TestMethod]
        public void AverageTest14()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Double[] { 1.0, 2.0, 3.0 }, v => v).Value,
                2.0,
                "AverageTest14(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Double[] { }, v => v).Value; }
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };
            var f = new Func<Double, Double>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest14(c)"
            );
        }

        [TestMethod]
        public void AverageTest15()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { null, 1f, 2f, 3f }, v => v).Value,
                2f,
                "AverageTest15(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { }, v => v).Value,
                null,
                "AverageTest15(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { null }, v => v).Value,
                null,
                "AverageTest15(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };
            var f = new Func<Single?, Single?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest15(d)"
            );
        }

        [TestMethod]
        public void AverageTest16()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Single[] { 1f, 2f, 3f }, v => v).Value,
                2f,
                "AverageTest16(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Single[] { }, v => v).Value; }
            );

            var seq = new Single[] { 1f, 2f, 3f };
            var f = new Func<Single, Single>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest16(c)"
            );
        }

        [TestMethod]
        public void AverageTest17()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { null, 1, 2, 3 }, v => v).Value,
                2.0,
                "AverageTest17(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { }, v => v).Value,
                null,
                "AverageTest17(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { null }, v => v).Value,
                null,
                "AverageTest17(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };
            var f = new Func<Int32?, Int32?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest17(d)"
            );
        }

        [TestMethod]
        public void AverageTest18()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32[] { 1, 2, 3 }, v => v).Value,
                2.0,
                "AverageTest18(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Int32[] { }, v => v).Value; }
            );

            var seq = new Int32[] { 1, 2, 3 };
            var f = new Func<Int32, Int32>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest18(c)"
            );
        }

        [TestMethod]
        public void AverageTest19()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { null, 1L, 2L, 3L }, v => v).Value,
                2.0,
                "AverageTest19(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { }, v => v).Value,
                null,
                "AverageTest19(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { null }, v => v).Value,
                null,
                "AverageTest19(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };
            var f = new Func<Int64?, Int64?>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest19(d)"
            );
        }

        [TestMethod]
        public void AverageTest20()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64[] { 1L, 2L, 3L }, v => v).Value,
                2.0,
                "AverageTest20(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Int64[] { }, v => v).Value; }
            );

            var seq = new Int64[] { 1L, 2L, 3L };
            var f = new Func<Int64, Int64>(v => v);

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest20(c)"
            );
        }





        [TestMethod]
        public void AverageTest21()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { null, 1m, 2m, 3m }, v => ValueProvider.Static(v)).Value,
                2m,
                "AverageTest21(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest21(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest21(c)"
            );

            var seq = new Decimal?[] { null, 1m, 2m, 3m };
            var f = new Func<Decimal?, IValueProvider<Decimal?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest21(d)"
            );

        }

        [TestMethod]
        public void AverageTest22()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Decimal[] { 1m, 2m, 3m }, v => ValueProvider.Static(v)).Value,
                2m,
                "AverageTest22(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Decimal[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Decimal[] { 1m, 2m, 3m };
            var f = new Func<Decimal, IValueProvider<Decimal>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest22(c)"
            );

        }

        [TestMethod]
        public void AverageTest23()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { null, 1.0, 2.0, 3.0 }, v => ValueProvider.Static(v)).Value,
                2.0,
                "AverageTest23(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest23(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Double?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest23(c)"
            );

            var seq = new Double?[] { null, 1.0, 2.0, 3.0 };
            var f = new Func<Double?, IValueProvider<Double?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest23(d)"
            );
        }

        [TestMethod]
        public void AverageTest24()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Double[] { 1.0, 2.0, 3.0 }, v => ValueProvider.Static(v)).Value,
                2.0,
                "AverageTest24(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Double[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Double[] { 1.0, 2.0, 3.0 };
            var f = new Func<Double, IValueProvider<Double>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest24(c)"
            );
        }

        [TestMethod]
        public void AverageTest25()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { null, 1f, 2f, 3f }, v => ValueProvider.Static(v)).Value,
                2f,
                "AverageTest25(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest25(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Single?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest25(c)"
            );

            var seq = new Single?[] { null, 1f, 2f, 3f };
            var f = new Func<Single?, IValueProvider<Single?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest25(d)"
            );
        }

        [TestMethod]
        public void AverageTest26()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Single[] { 1f, 2f, 3f }, v => ValueProvider.Static(v)).Value,
                2f,
                "AverageTest26(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Single[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Single[] { 1f, 2f, 3f };
            var f = new Func<Single, IValueProvider<Single>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest26(c)"
            );
        }

        [TestMethod]
        public void AverageTest27()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { null, 1, 2, 3 }, v => ValueProvider.Static(v)).Value,
                2.0,
                "AverageTest27(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest27(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest27(c)"
            );

            var seq = new Int32?[] { null, 1, 2, 3 };
            var f = new Func<Int32?, IValueProvider<Int32?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest27(d)"
            );
        }

        [TestMethod]
        public void AverageTest28()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int32[] { 1, 2, 3 }, v => ValueProvider.Static(v)).Value,
                2.0,
                "AverageTest28(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Int32[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Int32[] { 1, 2, 3 };
            var f = new Func<Int32, IValueProvider<Int32>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest28(c)"
            );
        }

        [TestMethod]
        public void AverageTest29()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { null, 1L, 2L, 3L }, v => ValueProvider.Static(v)).Value,
                2.0,
                "AverageTest29(a)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest29(b)"
            );

            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64?[] { null }, v => ValueProvider.Static(v)).Value,
                null,
                "AverageTest29(c)"
            );

            var seq = new Int64?[] { null, 1L, 2L, 3L };
            var f = new Func<Int64?, IValueProvider<Int64?>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest29(d)"
            );
        }

        [TestMethod]
        public void AverageTest30()
        {
            Assert.AreEqual(
                ObservableEnumerable.Average(new Int64[] { 1L, 2L, 3L }, v => ValueProvider.Static(v)).Value,
                2.0,
                "AverageTest30(a)"
            );

            TestHelper.ExpectException<InvalidOperationException>(
                () =>
                { var dummy = ObservableEnumerable.Average(new Int64[] { }, v => ValueProvider.Static(v)).Value; }
            );

            var seq = new Int64[] { 1L, 2L, 3L };
            var f = new Func<Int64, IValueProvider<Int64>>(v => ValueProvider.Static(v));

            Assert.AreEqual(
                ObservableEnumerable.Average(seq, f),
                ObservableEnumerable.Average(seq, f),
                "AverageTest30(c)"
            );
        }
    }
}
