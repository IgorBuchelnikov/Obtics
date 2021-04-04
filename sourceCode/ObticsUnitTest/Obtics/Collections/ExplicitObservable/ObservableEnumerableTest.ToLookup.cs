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
using Obtics;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        private static void LookupTests(IObservableLookup<char, string> lookup, int infix)
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new char[] { 'A', 'B', 'C' },
                    System.Linq.Enumerable.Select( lookup, kvp => kvp.Key )
                ),
                "ToLookupTest" + infix.ToString() + "(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "AB", "AC", "BD", "CB" },
                    lookup.Concat()
                ),
                "ToLookupTest" + infix.ToString() + "(b)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "AB", "AC" },
                    lookup['A']
                ),
                "ToLookupTest" + infix.ToString() + "(c)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "AB", "AC" },
                    lookup[ValueProvider.Static('A')]
                ),
                "ToLookupTest" + infix.ToString() + "(d)"
            );

            Assert.IsTrue(
                lookup.Contains('C'),
                "ToLookupTest" + infix.ToString() + "(e)"
            );

            Assert.IsFalse(
                lookup.Contains('Q'),
                "ToLookupTest" + infix.ToString() + "(f)"
            );

            Assert.IsTrue(
                lookup.Contains(ValueProvider.Static('C')).Value,
                "ToLookupTest" + infix.ToString() + "(g)"
            );

            Assert.IsFalse(
                lookup.Contains(ValueProvider.Static('Q')).Value,
                "ToLookupTest" + infix.ToString() + "(h)"
            );
        }

        [TestMethod]
        public void ToLookupTest1()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => s[0]),
                1
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel),
                ObservableEnumerable.ToLookup(seq, sel),
                "ToLookupTest1(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest2()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], ObticsEqualityComparer<char>.Default),
                2
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var comp = ObticsEqualityComparer<char>.Default;

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, comp),
                ObservableEnumerable.ToLookup(seq, sel, comp),
                "ToLookupTest2(i)"
            );
        }


        [TestMethod]
        public void ToLookupTest3()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0])),
                3
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel),
                ObservableEnumerable.ToLookup(seq, sel),
                "ToLookupTest3(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest4()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), ObticsEqualityComparer<char>.Default),
                4
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var comp = ObticsEqualityComparer<char>.Default;

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, comp),
                ObservableEnumerable.ToLookup(seq, sel, comp),
                "ToLookupTest4(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest5()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => s),
                5
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es),
                ObservableEnumerable.ToLookup(seq, sel, es),
                "ToLookupTest5(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest6()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => s, ObticsEqualityComparer<char>.Default),
                6
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                "ToLookupTest6(i)"
            );
        }


        [TestMethod]
        public void ToLookupTest7()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => s),
                7
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es),
                ObservableEnumerable.ToLookup(seq, sel, es),
                "ToLookupTest7(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest8()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => s, ObticsEqualityComparer<char>.Default),
                8
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                "ToLookupTest8(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest9()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => ValueProvider.Static(s)),
                9
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es),
                ObservableEnumerable.ToLookup(seq, sel, es),
                "ToLookupTest9(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest10()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => ValueProvider.Static(s), ObticsEqualityComparer<char>.Default),
                10
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                "ToLookupTest10(i)"
            );
        }


        [TestMethod]
        public void ToLookupTest11()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => ValueProvider.Static(s)),
                11
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es),
                ObservableEnumerable.ToLookup(seq, sel, es),
                "ToLookupTest11(i)"
            );
        }

        [TestMethod]
        public void ToLookupTest12()
        {
            LookupTests(
                ObservableEnumerable.ToLookup(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => ValueProvider.Static(s), ObticsEqualityComparer<char>.Default),
                12
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                ObservableEnumerable.ToLookup(seq, sel, es, comp),
                "ToLookupTest12(i)"
            );
        }

    }
}
