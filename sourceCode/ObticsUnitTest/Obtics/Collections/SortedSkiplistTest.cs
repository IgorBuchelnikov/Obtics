using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using OC = Obtics.Collections;

namespace ObticsUnitTest.Obtics.Collections
{
    /// <summary>
    /// Summary description for SkipListTest
    /// </summary>
    [TestClass]
    public class SortedSkiplistTest
    {
        public SortedSkiplistTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        class StringNode : global::Obtics.Collections.SortedSkiplist<StringNode, string>.Node
        {
            public string _String;
        }

        class StringSkipList : global::Obtics.Collections.SortedSkiplist<StringNode, string>
        {
            protected override int Compare(string a, string b)
            { return Comparer<string>.Default.Compare(a, b); }

            protected override string SelectKey(StringNode node)
            { return node._String; }

            public int Add(string str)
            { return this.AddWithIndex(new StringNode { _String = str }); }

            public int Remove(string str, out StringNode dummy)
            {
                int ix = this.FindFirst(str, out dummy);
                if(ix != -1)
                    this.RemoveAt(ix);
                return ix;
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var list = new StringSkipList();
            StringNode dummy;

            Assert.AreEqual(0, list.Add("A"), "First added index should be 0.");
            Assert.AreEqual(1, list.Add("B"), "Second added index should be 1.");
            Assert.AreEqual(2, list.Add("C"), "Third added index should be 2.");
            Assert.AreEqual(3, list.Add("E"), "Fourth added index should be 3.");
            Assert.AreEqual(4, list.Add("F"), "Fifth added index should be 4.");
            Assert.AreEqual(5, list.Add("G"), "Sixth added index should be 5.");
            Assert.AreEqual(6, list.Add("I"), "Seventh added index should be 6.");
            Assert.AreEqual(7, list.Add("J"), "Eight added index should be 7.");
            Assert.AreEqual(8, list.Add("L"), "Nineth added index should be 8.");
            Assert.AreEqual(9, list.Add("M"), "Tenth added index should be 9.");
            Assert.AreEqual(10, list.Add("N"), "Eleveneth added index should be 10.");

            Assert.AreEqual(3, list.Add("D"), "D inserted should be index 3.");
            Assert.AreEqual(7, list.Add("H"), "H inserted should be index 7.");
            Assert.AreEqual(10, list.Add("K"), "K inserted should be index 10.");
            Assert.AreEqual(14, list.Add("O"), "O inserted should be index 14.");

            Assert.AreEqual(0, list.Add("A"), "A double insert should be 0.");
            Assert.AreEqual(0, list.Add("A"), "A tripple insert should be 0.");

            Assert.AreEqual(7, list.Add("F"), "F double insert should be 7.");
            Assert.AreEqual(7, list.Add("F"), "F triple insert should be 7.");

            var sortedList = list.ToList();
            Assert.IsTrue(sortedList.Select(n => n._String).SequenceEqual("AAABCDEFFFGHIJKLMNO".ToCharArray().Select(c => c.ToString())), "Not expected sort order.");

            Assert.AreEqual(18, list.Remove("O", out dummy), "O removed should be 18.");
            Assert.AreEqual(0, list.Remove("A", out dummy), "A removed should be 0.");
            Assert.AreEqual(5, list.Remove("E", out dummy), "E removed should be 5.");
            Assert.AreEqual(8, list.Remove("G", out dummy), "G removed should be 8.");
            Assert.AreEqual(5, list.Remove("F", out dummy), "F removed should be 5.");

            sortedList = list.ToList();
            Assert.IsTrue(sortedList.Select(n => n._String).SequenceEqual("AABCDFFHIJKLMN".ToCharArray().Select(c => c.ToString())), "Not expected sort order.");        
        }

        class IntNode : global::Obtics.Collections.SortedSkiplist<IntNode, int>.Node
        {
            public int _Int;
        }

        class IntSkipList : global::Obtics.Collections.SortedSkiplist<IntNode, int>
        {
            protected override int  Compare(int a, int b)
            { return Comparer<int>.Default.Compare(a,b); }

            protected override int SelectKey(IntNode node)
            { return node._Int; }

            public int Add(int value)
            { return AddWithIndex( new IntNode { _Int = value } ); }

            public int Remove(int value, out IntNode dummy)
            {
                int ix = this.FindFirst(value, out dummy);
                if( ix != -1 )
                    this.RemoveAt(ix);
                return ix;
            }
        }


        [TestMethod]
        public void CorrectnessTest2()
        {
            var list = new IntSkipList();
            var rnd = new Random();

            for (int i = 0; i < 10000; ++i)
                list.Add(rnd.Next(10000));

            Assert.AreEqual(10000, list.Count, "Count should be 10000.");

            int current = 0;

            foreach (var n in list)
            {
                Assert.IsTrue(current <= n._Int, "10000 ints Not sorted properly after insertions.");
                current = n._Int;
            }

            IntNode dummy;

            for (int i = 0; i < 10000; ++i)
                list.Remove(rnd.Next(10000), out dummy);

            current = 0;

            foreach (var n in list)
            {
                Assert.IsTrue(current <= n._Int, "Ints Not sorted properly after deletions.");
                current = n._Int;
            }
        }

    }
}
