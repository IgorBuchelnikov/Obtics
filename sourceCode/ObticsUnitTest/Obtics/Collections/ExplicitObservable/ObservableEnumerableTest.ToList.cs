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
        [TestMethod]
        public void ToListTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.ToList(new int[] { 1, 2, 3, 4 })
                ),
                "ToListTest1(a)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.ToList(seq),
                ObservableEnumerable.ToList(seq),
                "ToListTest1(b)"
            );
        }

        [TestMethod]
        public void ToListReturnPathTest1()
        {
            ObservableCollection<int> source = new ObservableCollection<int>();

            var list = ObservableEnumerable.ToList(
                source,
                new ListReturnPathFactory<int>
                {
                    Add = (l, v) => source.Add(v),
                    Remove = (l, v) => source.Remove(v),
                    SyncRoot = l => ((ICollection)source).SyncRoot,
                    IsSynchronized = l => ((ICollection)source).IsSynchronized
                }.CreateReturnPath()
            );

            list.Add(10);
            list.Add(11);
            list.Add(12);
            list.Add(13);

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 10, 11, 12, 13 },
                    list
                ),
                "ToToListReturnPathTest1ListTest1(a)"
            );

            list.Remove(11);
            list.Remove(13);

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 10, 12 },
                    list
                ),
                "ToToListReturnPathTest1ListTest1(b)"
            );

            list.Clear();

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { },
                    list
                ),
                "ToToListReturnPathTest1ListTest1(c)"
            );

            try
            {
                list.Insert(0, 10);

                Assert.Fail("ToToListReturnPathTest1ListTest1(d)");
            }
            catch (NotSupportedException)
            { }

            try
            {
                list.RemoveAt(0);

                Assert.Fail("ToToListReturnPathTest1ListTest1(e)");
            }
            catch (NotSupportedException)
            { }

            try
            {
                list[0] = 10;

                Assert.Fail("ToToListReturnPathTest1ListTest1(f)");
            }
            catch (NotSupportedException)
            { }
        }

        [TestMethod]
        public void ToListReturnPathTest2()
        {
            ObservableCollection<int> source = new ObservableCollection<int>();

            var list = ObservableEnumerable.ToList(
                source,
                new ListReturnPathFactory<int>
                {
                    Insert = (l, i, v) => source.Insert(i,v),
                    RemoveAt = (l, i) => source.RemoveAt(i),
                    ReplaceAt = (l, i, v) => source[i] = v,
                    SyncRoot = l => ((ICollection)source).SyncRoot,
                    IsSynchronized = l => ((ICollection)source).IsSynchronized
                }.CreateReturnPath()
            );

            list.Add(10);
            list.Add(13);
            list.Insert(1,11);
            list.Insert(2,12);

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 10, 11, 12, 13 },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(a)"
            );

            list.Remove(11);
            list.RemoveAt(2);

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 10, 12 },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(b)"
            );

            list[0] = 13;
            list[1] = 13;

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 13, 13 },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(c)"
            );

            list.Clear();

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(d)"
            );
        }

        [TestMethod]
        public void ToListReturnPathTest3()
        {
            var source = new ObservableCollection<string>();
            var ir = ValueProvider.Dynamic(false);

            var dict = ObservableEnumerable.ToList(
                source,
                new ListReturnPathFactory<string>
                {
                    IsReadOnly = d => ir
                }.CreateReturnPath()
            );

            var client = new ValueProviderClientNPC<bool>();
            client.Source = ValueProvider.Properties(dict).Get<bool>("IsReadOnly");

            Assert.IsFalse(client.Buffer, "ToListReturnPathTest3(a)");

            ir.Value = true;

            Assert.IsTrue(client.Buffer, "ToListReturnPathTest3(b)");

            ir.Value = false;

            Assert.IsFalse(client.Buffer, "ToListReturnPathTest3(c)");
        }

        [TestMethod]
        public void ToListReturnPathTest4()
        {
            ObservableCollection<int> source = new ObservableCollection<int>();

            var list =
                ListReturnPath.ToListFactory(
                    source,
                    source
                )
                    .Insert((l, p, i, v) => p.Insert(i, v))
                    .RemoveAt((l, p, i) => p.RemoveAt(i))
                    .ReplaceAt((l, p, i, v) => p[i] = v)
                    .SyncRoot((l,p) => ((ICollection)p).SyncRoot)
                    .IsSynchronized((l,p) => ((ICollection)p).IsSynchronized)
                    .Create();

            list.Add(10);
            list.Add(13);
            list.Insert(1, 11);
            list.Insert(2, 12);

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 10, 11, 12, 13 },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(a)"
            );

            list.Remove(11);
            list.RemoveAt(2);

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 10, 12 },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(b)"
            );

            list[0] = 13;
            list[1] = 13;

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 13, 13 },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(c)"
            );

            list.Clear();

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { },
                    list
                ),
                "ToToListReturnPathTest1ListTest2(d)"
            );
        }

    }
}
