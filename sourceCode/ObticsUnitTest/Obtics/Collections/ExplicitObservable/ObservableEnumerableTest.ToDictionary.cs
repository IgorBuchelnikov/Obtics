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
using System.Collections.ObjectModel;
using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        private static void DictionaryTests(IObservableDictionary<char, string> dict, int infix)
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new char[] { 'A', 'B', 'C' },
                    dict.Keys
                ),
                "ToDictionaryTest" + infix.ToString() + "(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new string[] { "AB", "BD", "CB" },
                    dict.Values
                ),
                "ToDictionaryTest" + infix.ToString() + "(b)"
            );

            Assert.AreEqual(
                dict['B'],
                "BD",
                "ToDictionaryTest" + infix.ToString() + "(c)"
            );

            Assert.AreEqual(
                dict[ValueProvider.Static('B')].Value,
                "BD",
                "ToDictionaryTest" + infix.ToString() + "(d)"
            );

            Assert.IsTrue(
                dict.ContainsKey('C'),
                "ToDictionaryTest" + infix.ToString() + "(e)"
            );

            Assert.IsFalse(
                dict.ContainsKey('Q'),
                "ToDictionaryTest" + infix.ToString() + "(f)"
            );

            Assert.IsTrue(
                dict.ContainsKey(ValueProvider.Static('C')).Value,
                "ToDictionaryTest" + infix.ToString() + "(g)"
            );

            Assert.IsFalse(
                dict.ContainsKey(ValueProvider.Static('Q')).Value,
                "ToDictionaryTest" + infix.ToString() + "(h)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest1()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => s[0]),
                1
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel),
                ObservableEnumerable.ToDictionary(seq, sel),
                "ToDictionaryTest1(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest2()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], ObticsEqualityComparer<char>.Default),
                2
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var comp = ObticsEqualityComparer<char>.Default;

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, comp),
                ObservableEnumerable.ToDictionary(seq, sel, comp),
                "ToDictionaryTest2(i)"
            );
        }


        [TestMethod]
        public void ToDictionaryTest3()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0])),
                3
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel),
                ObservableEnumerable.ToDictionary(seq, sel),
                "ToDictionaryTest3(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest4()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), ObticsEqualityComparer<char>.Default),
                4
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var comp = ObticsEqualityComparer<char>.Default;

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, comp),
                ObservableEnumerable.ToDictionary(seq, sel, comp),
                "ToDictionaryTest4(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest5()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => s),
                5
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es),
                ObservableEnumerable.ToDictionary(seq, sel, es),
                "ToDictionaryTest5(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest6()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => s, ObticsEqualityComparer<char>.Default),
                6
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                "ToDictionaryTest6(i)"
            );
        }


        [TestMethod]
        public void ToDictionaryTest7()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => s),
                7
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es),
                ObservableEnumerable.ToDictionary(seq, sel, es),
                "ToDictionaryTest7(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest8()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => s, ObticsEqualityComparer<char>.Default),
                8
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, string>(s => s);

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                "ToDictionaryTest8(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest9()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => ValueProvider.Static(s)),
                9
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es),
                ObservableEnumerable.ToDictionary(seq, sel, es),
                "ToDictionaryTest9(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest10()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => s[0], s => ValueProvider.Static(s), ObticsEqualityComparer<char>.Default),
                10
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, char>(s => s[0]);
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                "ToDictionaryTest10(i)"
            );
        }


        [TestMethod]
        public void ToDictionaryTest11()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => ValueProvider.Static(s)),
                11
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es),
                ObservableEnumerable.ToDictionary(seq, sel, es),
                "ToDictionaryTest11(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryTest12()
        {
            DictionaryTests(
                ObservableEnumerable.ToDictionary(new string[] { "AB", "BD", "AC", "CB" }, s => ValueProvider.Static(s[0]), s => ValueProvider.Static(s), ObticsEqualityComparer<char>.Default),
                12
            );

            var seq = new string[] { "AB", "BD", "AC", "CB" };
            var sel = new Func<string, IValueProvider<char>>(s => ValueProvider.Static(s[0]));
            var comp = ObticsEqualityComparer<char>.Default;
            var es = new Func<string, IValueProvider<string>>(s => ValueProvider.Static(s));

            Assert.AreEqual(
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                ObservableEnumerable.ToDictionary(seq, sel, es, comp),
                "ToDictionaryTest12(i)"
            );
        }

        [TestMethod]
        public void ToDictionaryWithReturnPathTest1()
        {
            var source = new ObservableCollection<KeyValuePair<string, int>>();

            var dict = ObservableEnumerable.ToDictionary(
                source,
                kvp => kvp.Key,
                kvp => kvp.Value,
                new DictionaryReturnPathFactory<string, int>
                {
                    Add = (d, k, v) => source.Add(new KeyValuePair<string, int>(k, v)),
                    RemoveWithValueCheck = (d, k, v) => source.Remove(new KeyValuePair<string, int>(k, v)),
                    IsSynchronized = d => ((ICollection)source).IsSynchronized,
                    SyncRoot = d => ((ICollection)source).SyncRoot
                }.CreateReturnPath()
            );

            dict.Add("10", 10);
            dict.Add("11", 27);
            dict.Add("12", 12);
            dict["13"] = 13;
            dict["11"] = 11;
            dict.Remove("12");

            Assert.AreEqual(10, dict["10"], "ToDictionaryWithReturnPathTest1(a)");
            Assert.AreEqual(11, dict["11"], "ToDictionaryWithReturnPathTest1(b)");
            Assert.IsFalse(dict.Remove(new KeyValuePair<string, int>("13", 26)), "ToDictionaryWithReturnPathTest1(c)");
            Assert.AreEqual(13, dict["13"], "ToDictionaryWithReturnPathTest1(d)");
            Assert.IsFalse(dict.ContainsKey("12"), "ToDictionaryWithReturnPathTest1(e)");

            dict.Clear();

            Assert.AreEqual(0, dict.Count, "ToDictionaryWithReturnPathTest1(f)");
        }

        [TestMethod]
        public void ToDictionaryWithReturnPathTest2()
        {
            var source = new ObservableCollection<KeyValuePair<string, int>>();
            var ir = ValueProvider.Dynamic(false);

            var dict = ObservableEnumerable.ToDictionary(
                source,
                kvp => kvp.Key,
                kvp => kvp.Value,
                new DictionaryReturnPathFactory<string, int>
                {
                    IsReadOnly = d => ir
                }.CreateReturnPath()
            );

            var client = new ValueProviderClientNPC<bool>();
            client.Source = ValueProvider.Properties(dict).Get<bool>("IsReadOnly");

            Assert.IsFalse(client.Buffer, "ToDictionaryWithReturnPathTest2(a)");

            ir.Value = true;

            Assert.IsTrue(client.Buffer, "ToDictionaryWithReturnPathTest2(b)");

            ir.Value = false;

            Assert.IsFalse(client.Buffer, "ToDictionaryWithReturnPathTest2(c)");
        }

        [TestMethod]
        public void ToDictionaryWithReturnPathTest3()
        {
            var source = new ObservableCollection<KeyValuePair<string, int>>();

            var dict =
                source.ToDictionaryFactory(
                    source,
                    kvp => kvp.Key,
                    kvp => kvp.Value
                )
                    .Add((d, s, k, v) => s.Add(new KeyValuePair<string, int>(k, v)))
                    .RemoveWithValueCheck((d, s, k, v) => s.Remove(new KeyValuePair<string, int>(k, v)))
                    .IsSynchronized((d, s) => ((ICollection)s).IsSynchronized)
                    .SyncRoot((d, s) => ((ICollection)source).SyncRoot)
                    .Create()
            ;

            dict.Add("10", 10);
            dict.Add("11", 27);
            dict.Add("12", 12);
            dict["13"] = 13;
            dict["11"] = 11;
            dict.Remove("12");

            Assert.AreEqual(10, dict["10"], "ToDictionaryWithReturnPathTest1(a)");
            Assert.AreEqual(11, dict["11"], "ToDictionaryWithReturnPathTest1(b)");
            Assert.IsFalse(dict.Remove(new KeyValuePair<string, int>("13", 26)), "ToDictionaryWithReturnPathTest1(c)");
            Assert.AreEqual(13, dict["13"], "ToDictionaryWithReturnPathTest1(d)");
            Assert.IsFalse(dict.ContainsKey("12"), "ToDictionaryWithReturnPathTest1(e)");

            dict.Clear();

            Assert.AreEqual(0, dict.Count, "ToDictionaryWithReturnPathTest1(f)");
        }
    
    }
}
