using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Collections;
using ObticsUnitTest.Helpers;
using Obtics.Values;
using System.Threading;

namespace ObticsUnitTest.Obtics.Collections
{
    /// <summary>
    /// Summary description for ObservableDictionaryTest
    /// </summary>
    [TestClass]
    public class ObservableDictionaryTest
    {
        public ObservableDictionaryTest()
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

        [TestMethod]
        public void AddRemoveItems()
        {
            ObservableDictionary<int, int> dictionary = new ObservableDictionary<int, int>();

            dictionary.Add(10, 20);
            dictionary.Add(11, 21);
            dictionary.Add(12, 22);
            dictionary.Add(13, 23);
            dictionary.Add(14, 24);

            Assert.AreEqual(20, dictionary[10], "key 10 does not give correct result");
            Assert.AreEqual(21, dictionary[11], "key 11 does not give correct result");
            Assert.AreEqual(22, dictionary[12], "key 12 does not give correct result");
            Assert.AreEqual(23, dictionary[13], "key 13 does not give correct result");
            Assert.AreEqual(24, dictionary[14], "key 14 does not give correct result");

            Assert.IsTrue(dictionary.ContainsKey(10), "dictionary should contains key 10");
            Assert.IsTrue(dictionary.ContainsKey(11), "dictionary should contains key 11");
            Assert.IsTrue(dictionary.ContainsKey(12), "dictionary should contains key 12");
            Assert.IsTrue(dictionary.ContainsKey(13), "dictionary should contains key 13");
            Assert.IsTrue(dictionary.ContainsKey(14), "dictionary should contains key 14");

            Assert.IsTrue(dictionary.Values.Contains(20), "dictionary should contains value 20");
            Assert.IsTrue(dictionary.Values.Contains(21), "dictionary should contains value 21");
            Assert.IsTrue(dictionary.Values.Contains(22), "dictionary should contains value 22");
            Assert.IsTrue(dictionary.Values.Contains(23), "dictionary should contains value 23");
            Assert.IsTrue(dictionary.Values.Contains(24), "dictionary should contains value 24");

            Assert.IsFalse(dictionary.ContainsKey(15), "dictionary should not contain key 15");
            
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>(
                new KeyValuePair<int, int>[] {
                    new KeyValuePair<int,int>( 10, 20 ),
                    new KeyValuePair<int,int>( 11, 21 ),
                    new KeyValuePair<int,int>( 12, 22 ),
                    new KeyValuePair<int,int>( 13, 23 ),
                    new KeyValuePair<int,int>( 14, 24 )
                }
            );

            foreach (var kvp in dictionary)
            {
                Assert.IsTrue(list.Contains(kvp), "found kvp not in list.");
                list.Remove(kvp);
            }

            dictionary.Remove(10);
            dictionary.Remove(11);
            dictionary.Remove(13);
            dictionary.Remove(14);

            Assert.AreEqual(22, dictionary[12], "key 12 does not give correct result");

            dictionary.Remove(12);

            Assert.IsFalse(dictionary.ContainsKey(10), "dictionary still contains key 10");
            Assert.IsFalse(dictionary.ContainsKey(11), "dictionary still contains key 11");
            Assert.IsFalse(dictionary.ContainsKey(12), "dictionary still contains key 12");
            Assert.IsFalse(dictionary.ContainsKey(13), "dictionary still contains key 13");
            Assert.IsFalse(dictionary.ContainsKey(14), "dictionary still contains key 14");        
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NoDuplicateKeys()
        {
            ObservableDictionary<int, int> dictionary = new ObservableDictionary<int, int>();

            dictionary.Add(10, 20);
            dictionary.Add(11, 21);
            dictionary.Add(12, 22);
            dictionary.Add(13, 23);
            dictionary.Add(14, 24);

            dictionary.Add(12, 33);        
        }

        [TestMethod]
        public void Observability()
        {
            ObservableDictionary<int, int> dictionary = new ObservableDictionary<int, int>();

            var keys = new CollectionTransformationClientNCC<int>();
            keys.Source = dictionary.Keys;

            var values = new CollectionTransformationClientNCC<int>();
            values.Source = dictionary.Values;

            var kvps = new CollectionTransformationClientNCC<KeyValuePair<int,int>>();
            kvps.Source = dictionary;

            var twelve = new ValueProviderClientNPC<int>();
            twelve.Source = dictionary[ValueProvider.Static(12)].OnException((KeyNotFoundException ex) => 22222);

            var thirteen = new ValueProviderClientNPC<int>();
            thirteen.Source = dictionary[ValueProvider.Static(13)].OnException((KeyNotFoundException ex) => 33333);

            for (int i = 0; i < 1000; i += 2)
                dictionary.Add(i, i + 1);

            for (int i = 0; i < 1000; ++i)
                dictionary[i] = i + 34;

            int readyCount = 3;

            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(
                    delegate
                    {
                        var rnd = new Random();
                        for (int i = 0; i < 1000; ++i)
                            dictionary[rnd.Next(999)] = rnd.Next(100);
                        Interlocked.Decrement(ref readyCount);
                    }
                )
            );

            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(
                    delegate
                    {
                        var rnd = new Random();
                        for (int i = 0; i < 1000; ++i)
                            dictionary.Remove(rnd.Next(900) + 50);
                        Interlocked.Decrement(ref readyCount);
                    }
                )
            );

            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(
                    delegate
                    {
                        var rnd = new Random();
                        for (int i = 0; i < 1000; ++i)
                            dictionary[rnd.Next(999)] = rnd.Next(100);
                        Interlocked.Decrement(ref readyCount);
                    }
                )
            );

            while (readyCount != 0)
                Thread.Sleep(100);

            for (int i = 0; i < 1000; ++i)
                dictionary[i] = i + 10;

            for (int i = 13; i < 1000; ++i)
                dictionary.Remove(i);

            for(int i =0; i < 10; ++i)
                dictionary.Remove(i);

            Assert.AreEqual(33333, thirteen.Buffer, "Key 13 removed from dictionary. Value should be default(int)");
            Assert.AreEqual(22, twelve.Buffer, "Value for key 12 should be 22.");

            Assert.AreEqual(3, keys.Buffer.Count, "Expected 3 values in Keys sequence");
            Assert.AreEqual(3, values.Buffer.Count, "Expected 3 values in Values sequence");
            Assert.AreEqual(3, kvps.Buffer.Count, "Expected 3 values in kvps sequence");

            for (int i = 10; i < 13; ++i)
            {
                Assert.IsTrue(keys.Buffer.Contains(i), "Expected keys to contain" + i.ToString());
                Assert.IsTrue(values.Buffer.Contains(i + 10), "Expected values to contain" + (i + 10).ToString());
                Assert.IsTrue(kvps.Buffer.Contains(new KeyValuePair<int, int>(i, i + 10)), "Expected kvps to contain <" + i.ToString() + "," + (i + 10).ToString() + ">");
            }
        }
    }
}
