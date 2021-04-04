using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Values;
using Obtics.Data;
using System.Data;
using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.ObticsToDataSet
{
    /// <summary>
    /// Summary description for ObservableExtensionsTest
    /// </summary>
    [TestClass]
    public class ObservableExtensionsTest
    {
        public ObservableExtensionsTest()
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

 
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }


        void ManipulateDataSet1(DataSet1 dataSet, Action<int> check)
        {
            var table = dataSet.Table1;

            dataSet.Clear();
            dataSet.AcceptChanges();

            check(1);

            for (int i = 0; i < 100; ++i)
                table.AddTable1Row(i, i.ToString(), (decimal)i);

            check(2);

            dataSet.RejectChanges();

            check(3);

            for (int i = 0; i < 100; ++i)
                table.AddTable1Row(i, i.ToString(), (decimal)i);

            check(4);

            for (int i = 20; i < 30; ++i)
                table[i].Delete();

            check(5);

            dataSet.AcceptChanges();

            check(6);

            table.Rows.RemoveAt(5);
            table.Rows.RemoveAt(6);

            check(7);

            dataSet.RejectChanges();

            check(8);

            table[7].Column3 = 4000;
            table[8].Column3 = 4001;

            check(9);

            table[7].Column2 = "4000";
            table[8].Column2 = "4001";

            check(10);

            table[7].Column1 = 4000;
            table[8].Column1 = 4001;

            check(11);

            dataSet.RejectChanges();

            check(12);
            table[7].Column3 = 4000;
            table[8].Column3 = 4001;

            check(13);

            table[7].Column2 = "4000";
            table[8].Column2 = "4001";

            check(14);

            table[7].Column1 = 4000;
            table[8].Column1 = 4001;

            check(15);

            dataSet.AcceptChanges();

            check(16);
        }

        [TestMethod]
        public void DataRowRowStateTest()
        {
            var dataSet = new DataSet();
            var testTable = dataSet.Tables.Add("TestTable");
            testTable.Columns.Add("First", typeof(string));
            testTable.Columns.Add("Second", typeof(int));

            var row = testTable.NewRow();
            var rowState = ExpressionObserver.Execute(row, r => r.RowState);

            var client = new ValueProviderClientNPC<DataRowState>();
            client.Source = rowState;

            testTable.Rows.Add(row);

            Assert.AreEqual(DataRowState.Added, client.Buffer, "RowState not updated");
        }

        public void DataRowRowStateTest2()
        {
            var dataSet1 = new DataSet1();

            var rowStates = ExpressionObserver.Execute(() => dataSet1.Table1.Select( tr => tr.RowState )).Cascade();

            var client2 = new CollectionTransformationClientSNCC<DataRowState>();
            client2.Source = rowStates;

            ManipulateDataSet1(
                dataSet1,
                (x) =>
                    Assert.IsTrue(
                        dataSet1.Table1.Select(tr => tr.RowState).SequenceEqual(client2.Buffer),
                        "Equality failed at test " + x.ToString()
                    )
            );
        }

        [TestMethod]
        public void DataRowItemTest()
        {
            var dataSet = new DataSet();
            var testTable = dataSet.Tables.Add("TestTable");
            testTable.Columns.Add("First", typeof(string));
            testTable.Columns.Add("Second", typeof(int));

            var row = testTable.Rows.Add("A", 1);
            var rowColumn2 = ExpressionObserver.Execute(row, r => r[1]);

            var client = new ValueProviderClientNPC<object>();
            client.Source = rowColumn2;

            row[1] = 101;

            Assert.AreEqual(row[1], client.Buffer, "Row column 2 not updated");
        }

        [TestMethod]
        public void DataRowItemTest2()
        {
            var dataSet1 = new DataSet1();

            var rowStates = ExpressionObserver.Execute(() => dataSet1.Table1.Select(tr => tr[dataSet1.Table1.Column2Column])).Cascade();

            var client2 = new CollectionTransformationClientSNCC<object>();
            client2.Source = rowStates;

            ManipulateDataSet1(
                dataSet1,
                (x) =>
                    Assert.IsTrue(
                        dataSet1.Table1.Select(tr => tr[dataSet1.Table1.Column2Column]).SequenceEqual(client2.Buffer),
                        "Equality failed at test " + x.ToString()
                    )
            );
        }

        [TestMethod]
        public void DataRowFieldTest()
        {
            var dataSet = new DataSet();
            var testTable = dataSet.Tables.Add("TestTable");
            testTable.Columns.Add("First", typeof(string));
            testTable.Columns.Add("Second", typeof(int));

            var row = testTable.Rows.Add("A", 1);
            var rowColumn2 = ExpressionObserver.Execute(row, r => r.Field<int>(1));

            var client = new ValueProviderClientNPC<int>();
            client.Source = rowColumn2;

            row[1] = 101;

            Assert.AreEqual(row[1], client.Buffer, "Row column 2 not updated");
        }

        [TestMethod]
        public void DataRowFieldTest2()
        {
            var dataSet1 = new DataSet1();

            var rowStates = ExpressionObserver.Execute(() => dataSet1.Table1.Select(tr => tr.Field<int>(0))).Cascade();

            var client2 = new CollectionTransformationClientSNCC<int>();
            client2.Source = rowStates;

            ManipulateDataSet1(
                dataSet1,
                (x) =>
                    Assert.IsTrue(
                        dataSet1.Table1.Select(tr => tr.Field<int>(0)).SequenceEqual(client2.Buffer),
                        "Equality failed at test " + x.ToString()
                    )
            );
        }

        [TestMethod]
        public void DataRowItemTest3()
        {
            var dataSet = new DataSet1();
            var testTable = dataSet.Table1;

            var row = testTable.NewTable1Row();
            row.Column1 = 1;
            row.Column2 = "a";
            testTable.Rows.Add(row);
            var rowColumn2 = ExpressionObserver.Execute(row, r => r.Column2);

            var client = new ValueProviderClientNPC<string>();
            client.Source = rowColumn2;

            row.Column2 = "b";

            Assert.AreEqual(row.Column2, client.Buffer, "Row column 2 not updated");
        }

        [TestMethod]
        public void DataRowItemTest4()
        {
            var dataSet1 = new DataSet1();

            var rowStates = ExpressionObserver.Execute(() => dataSet1.Table1.Select(tr => tr.Column2)).Cascade();

            var client2 = new CollectionTransformationClientSNCC<string>();
            client2.Source = rowStates;

            ManipulateDataSet1(
                dataSet1,
                (x) =>
                    Assert.IsTrue(
                        dataSet1.Table1.Select(tr => tr.Column2).SequenceEqual(client2.Buffer),
                        "Equality failed at test " + x.ToString()
                    )
            );
        }

        [TestMethod]
        public void DataTableRowsTest()
        {
            var dataSet1 = new DataSet1();

            var rowStates = ExpressionObserver.Execute(dataSet1, ds => ds.Tables["Table1"].Rows.OfType<DataRow>()).Cascade();

            var client2 = new CollectionTransformationClientSNCC<DataRow>();
            client2.Source = rowStates;

            ManipulateDataSet1(
                dataSet1,
                (x) =>
                    Assert.IsTrue(
                        dataSet1.Tables["Table1"].Rows.OfType<DataRow>().SequenceEqual(client2.Buffer),
                        "Equality failed at test " + x.ToString()
                    )
            );
        }

        [TestMethod]
        public void DataTableRowsTest2()
        {
            var dataSet1 = new DataSet1();

            var rowStates = ExpressionObserver.Execute(dataSet1, ds => ds.Table1.AsEnumerable()).Cascade();

            var client2 = new CollectionTransformationClientSNCC<DataSet1.Table1Row>();
            client2.Source = rowStates;

            ManipulateDataSet1(
                dataSet1,
                (x) =>
                    Assert.IsTrue(
                        dataSet1.Table1.AsEnumerable().SequenceEqual(client2.Buffer),
                        "Equality failed at test " + x.ToString()
                    )
            );
        }
    }
}
