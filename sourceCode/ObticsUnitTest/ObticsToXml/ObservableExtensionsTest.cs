using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using System.Xml.Linq;
using System.IO;
using Obtics.Values;
using Obtics.Xml;
using ObticsUnitTest.Helpers;
using System.Collections.ObjectModel;

namespace ObticsUnitTest.ObticsToXml
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

        [TestMethod]
        public void AttributeValueTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), a => a.Value);

            var client = new ValueProviderClientNPC<string>();
            client.Source = attrib1;

            Assert.AreEqual("1", client.Buffer, "Expected '1' initially.");

            var attrib1Obj = document.Root.Attribute("attrib1");
            attrib1Obj.Value = "13";

            Assert.AreEqual("13", client.Buffer, "Expected '13' after change.");
        }

        [TestMethod]
        public void AttributeValueTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), a => (int?)a);

            var client = new ValueProviderClientNPC<int?>();
            client.Source = attrib1;

            Assert.AreEqual(1, client.Buffer, "Expected '1' initially.");

            var attrib1Obj = document.Root.Attribute("attrib1");
            attrib1Obj.Value = "13";

            Assert.AreEqual(13, client.Buffer, "Expected '13' after change.");

            attrib1Obj.Value = "n";

            Assert.AreEqual((int?)null, client.Buffer, "Expected '13' after change.");
        }


        [TestMethod]
        public void AttributeValueTest3()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (int)r);

            var client = new ValueProviderClientNPC<int>();
            client.Source = attrib1;

            Assert.AreEqual(default(int), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10";

            Assert.AreEqual((int)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest4()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (uint?)r);

            var client = new ValueProviderClientNPC<uint?>();
            client.Source = attrib1;

            Assert.AreEqual(default(uint?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10";

            Assert.AreEqual((uint?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest5()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (uint)r);

            var client = new ValueProviderClientNPC<uint>();
            client.Source = attrib1;

            Assert.AreEqual(default(uint), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10";

            Assert.AreEqual((uint)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest6()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (long?)r);

            var client = new ValueProviderClientNPC<long?>();
            client.Source = attrib1;

            Assert.AreEqual(default(long?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10";

            Assert.AreEqual((long?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest7()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (long)r);

            var client = new ValueProviderClientNPC<long>();
            client.Source = attrib1;

            Assert.AreEqual(default(long), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10";

            Assert.AreEqual((long)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }
        [TestMethod]
        public void AttributeValueTest8()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (decimal?)r);

            var client = new ValueProviderClientNPC<decimal?>();
            client.Source = attrib1;

            Assert.AreEqual(default(decimal?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10";

            Assert.AreEqual((decimal?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest9()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (decimal)r);

            var client = new ValueProviderClientNPC<decimal>();
            client.Source = attrib1;

            Assert.AreEqual(default(decimal), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10";

            Assert.AreEqual((decimal)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest10()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (float?)r);

            var client = new ValueProviderClientNPC<float?>();
            client.Source = attrib1;

            Assert.AreEqual(default(float?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10.0";

            Assert.AreEqual((float?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void AttributeValueTest11()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (float)r);

            var client = new ValueProviderClientNPC<float>();
            client.Source = attrib1;

            Assert.AreEqual(default(float), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10.0";

            Assert.AreEqual((float)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void AttributeValueTest12()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (double?)r);

            var client = new ValueProviderClientNPC<double?>();
            client.Source = attrib1;

            Assert.AreEqual(default(double?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10.0";

            Assert.AreEqual((double?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void AttributeValueTest13()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (double)r);

            var client = new ValueProviderClientNPC<double>();
            client.Source = attrib1;

            Assert.AreEqual(default(double), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10.0";

            Assert.AreEqual((double)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void AttributeValueTest14()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (Guid?)r);

            var client = new ValueProviderClientNPC<Guid?>();
            client.Source = attrib1;

            Assert.AreEqual(default(Guid?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10101010101010101010101010101010";

            Assert.AreEqual((Guid?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest15()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (Guid)r);

            var client = new ValueProviderClientNPC<Guid>();
            client.Source = attrib1;

            Assert.AreEqual(default(Guid), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "10101010101010101010101010101010";

            Assert.AreEqual((Guid)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest16()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (TimeSpan?)r);

            var client = new ValueProviderClientNPC<TimeSpan?>();
            client.Source = attrib1;

            Assert.AreEqual(default(TimeSpan?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "PT2H23M";

            Assert.AreEqual((TimeSpan?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest17()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (TimeSpan)r);

            var client = new ValueProviderClientNPC<TimeSpan>();
            client.Source = attrib1;

            Assert.AreEqual(default(TimeSpan), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "PT2H23M";

            Assert.AreEqual((TimeSpan)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest18()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (DateTime?)r);

            var client = new ValueProviderClientNPC<DateTime?>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTime?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "2009-05-25T11:49:10.801+02:00";

            Assert.AreEqual((DateTime?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest19()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (DateTime)r);

            var client = new ValueProviderClientNPC<DateTime>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTime), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "2009-05-25T11:49:10.801+02:00";

            Assert.AreEqual((DateTime)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest20()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (DateTimeOffset?)r);

            var client = new ValueProviderClientNPC<DateTimeOffset?>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTimeOffset?), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "2009-05-25T00:00:00+02:00";

            Assert.AreEqual((DateTimeOffset?)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void AttributeValueTest21()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), r => (DateTimeOffset)r);

            var client = new ValueProviderClientNPC<DateTimeOffset>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTimeOffset), client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Value = "2009-05-25T00:00:00+02:00";

            Assert.AreEqual((DateTimeOffset)document.Root.Attribute("attrib1"), client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void NextAttributeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' attrib3='3' />"));

            var nextAttrib = ExpressionObserver.Execute(document.Root.Attribute("attrib1"), a => a.NextAttribute);

            var client = new ValueProviderClientNPC<XAttribute>();
            client.Source = nextAttrib;

            Assert.AreEqual(document.Root.Attribute("attrib2"), client.Buffer, "Expected correct attribute object initially.");

            var rootObj = document.Root;
            rootObj.Attribute("attrib2").Remove();

            Assert.AreEqual(document.Root.Attribute("attrib3"), client.Buffer, "Expected correct attribute object after change.");

            rootObj.Attribute("attrib3").Remove();

            Assert.AreEqual(null, client.Buffer, "Expected no attribute object after change.");

            rootObj.Add(new XAttribute("attrib4", "4"));

            Assert.AreEqual(document.Root.Attribute("attrib4"), client.Buffer, "Expected correct attribute object after change.");
        }

        [TestMethod]
        public void PreviousAttributeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' attrib3='3' />"));

            var nextAttrib = ExpressionObserver.Execute(document.Root.Attribute("attrib3"), a => a.PreviousAttribute);

            var client = new ValueProviderClientNPC<XAttribute>();
            client.Source = nextAttrib;

            Assert.AreEqual(document.Root.Attribute("attrib2"), client.Buffer, "Expected correct attribute object initially.");

            var rootObj = document.Root;
            rootObj.Attribute("attrib2").Remove();

            Assert.AreEqual(document.Root.Attribute("attrib1"), client.Buffer, "Expected correct attribute object after change.");

            rootObj.Attribute("attrib1").Remove();

            Assert.AreEqual(null, client.Buffer, "Expected no attribute object after change.");

            rootObj.Add(new XAttribute("attrib4", "4"));

            //probably null
            Assert.AreEqual(document.Root.Attribute("attrib3").PreviousAttribute, client.Buffer, "Expected correct attribute object after change.");
        }

        [TestMethod]
        public void NodeDeepEqualsTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B attr1='a' attr2='b'><C attr1='a' attr2='b'/><!-- comment --><C/></B><B attr1='a' attr2='b'><C attr1='a' attr2='b'/><C/></B></A>"));
            XDocument document2 = XDocument.Load(new StringReader("<A><B attr1='a' attr2='b'><C attr1='a' attr2='b'/><!-- comment --><C/></B><B attr1='a' attr2='b'><C attr1='a' attr2='b'/><C/></B></A>"));

            var equalsF = ExpressionObserver.Compile((XNode a, XNode b) => XNode.DeepEquals(a,b));

            Assert.AreEqual(true, equalsF(null, null).Value, "Null nodes should be equal");

            Assert.AreEqual(false, equalsF(null, document).Value, "Null node not equal to not null node.");

            Assert.AreEqual(false, equalsF(document, null).Value, "Null node not equal to not null node.");

            Assert.AreEqual(true, equalsF(document, document2).Value, "Equal documents.");

            var bs = document.Root.Elements().ToArray();
            var equals = equalsF(bs[0], bs[1]);

            var client = new ValueProviderClientNPC<bool>();
            client.Source = equals;

            Assert.AreEqual(true, client.Buffer, "Expected elements equal initially.");
            
            bs[0].Attribute("attr2").Remove();

            Assert.AreEqual(false, client.Buffer, "Expected elements unequal after removing bs[0] attr1.");

            bs[1].Attribute("attr2").Remove();

            Assert.AreEqual(true, client.Buffer, "Expected elements equal after removing bs[1] attr1.");

            bs[0].Add(new XAttribute("attr3", "c"));

            Assert.AreEqual(false, client.Buffer, "Expected elements unequal after adding bs[0] attr3.");

            bs[1].Add(new XAttribute("attr3", "c"));

            Assert.AreEqual(true, client.Buffer, "Expected elements equal after adding bs[1] attr3.");

            bs[0].Add(new XElement("D"));

            Assert.AreEqual(false, client.Buffer, "Expected elements unequal after adding bs[0] elt D.");

            bs[1].Add(new XElement("D"));

            Assert.AreEqual(true, client.Buffer, "Expected elements equal after adding bs[1] elt D.");
        }

        [TestMethod]
        public void ElementsAfterSelfTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><B/><C/><!-- comment --><D/></A>"));

            var allElements = document.Root.Elements().ToArray();
            var elements = ExpressionObserver.Execute((XNode)allElements[0], n => n.ElementsAfterSelf()).Cascade();


            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = elements;

            Assert.IsTrue(allElements[0].ElementsAfterSelf().SequenceEqual(client.Buffer), "Expected correct elements initialy");

            allElements[2].Remove();

            Assert.IsTrue(allElements[0].ElementsAfterSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 1");

            allElements[3].AddBeforeSelf(new XElement("Q"));

            Assert.IsTrue(allElements[0].ElementsAfterSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 2");
        }

        [TestMethod]
        public void ElementsAfterSelfTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><B/><C/><!-- comment --><D/></A>"));

            var allElements = document.Root.Elements().ToArray();
            var elements = ExpressionObserver.Execute((XNode)allElements[0], n => n.ElementsAfterSelf("B")).Cascade();


            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = elements;

            Assert.IsTrue(allElements[0].ElementsAfterSelf("B").SequenceEqual(client.Buffer), "Expected correct elements initialy");

            allElements[1].Remove();

            Assert.IsTrue(allElements[0].ElementsAfterSelf("B").SequenceEqual(client.Buffer), "Expected correct elements after change 1");

            allElements[3].AddBeforeSelf(new XElement("B"));

            Assert.IsTrue(allElements[0].ElementsAfterSelf("B").SequenceEqual(client.Buffer), "Expected correct elements after change 2");
        }

        [TestMethod]
        public void ElementsBeforeSelfTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><B/><C/><!-- comment --><D/></A>"));

            var allElements = document.Root.Elements().ToArray();
            var elements = ExpressionObserver.Execute((XNode)allElements[3], n => n.ElementsBeforeSelf()).Cascade();


            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = elements;

            Assert.IsTrue(allElements[3].ElementsBeforeSelf().SequenceEqual(client.Buffer), "Expected correct elements initialy");

            allElements[2].Remove();

            Assert.IsTrue(allElements[3].ElementsBeforeSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 1");

            allElements[3].AddBeforeSelf(new XElement("Q"));

            Assert.IsTrue(allElements[3].ElementsBeforeSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 2");
        }

        [TestMethod]
        public void ElementsBeforeSelfTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><B/><C/><!-- comment --><D/></A>"));

            var allElements = document.Root.Elements().ToArray();
            var elements = ExpressionObserver.Execute((XNode)allElements[3], n => n.ElementsBeforeSelf("B")).Cascade();


            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = elements;

            Assert.IsTrue(allElements[3].ElementsBeforeSelf("B").SequenceEqual(client.Buffer), "Expected correct elements initialy");

            allElements[1].Remove();

            Assert.IsTrue(allElements[3].ElementsBeforeSelf("B").SequenceEqual(client.Buffer), "Expected correct elements after change 1");

            allElements[3].AddBeforeSelf(new XElement("B"));

            Assert.IsTrue(allElements[3].ElementsBeforeSelf("B").SequenceEqual(client.Buffer), "Expected correct elements after change 2");
        }

        [TestMethod]
        public void NodesAfterSelfTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><B/>A<C/><!-- comment --><D/></A>"));

            var allNodes = document.Root.Nodes().ToArray();
            var elements = ExpressionObserver.Execute((XNode)allNodes[0], n => n.NodesAfterSelf()).Cascade();


            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = elements;

            Assert.IsTrue(allNodes[0].NodesAfterSelf().SequenceEqual(client.Buffer), "Expected correct elements initialy");

            allNodes[2].Remove();

            Assert.IsTrue(allNodes[0].NodesAfterSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 1");

            allNodes[3].AddBeforeSelf(new XElement("Q"));

            Assert.IsTrue(allNodes[0].NodesAfterSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 2");
        }

        [TestMethod]
        public void NodesBeforeSelfTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><B/>A<C/><!-- comment --><D/></A>"));

            var allNodes = document.Root.Nodes().ToArray();
            var elements = ExpressionObserver.Execute((XNode)allNodes[3], n => n.NodesBeforeSelf()).Cascade();


            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = elements;

            Assert.IsTrue(allNodes[3].NodesBeforeSelf().SequenceEqual(client.Buffer), "Expected correct elements initialy");

            allNodes[2].Remove();

            Assert.IsTrue(allNodes[3].NodesBeforeSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 1");

            allNodes[3].AddBeforeSelf(new XElement("Q"));

            Assert.IsTrue(allNodes[3].NodesBeforeSelf().SequenceEqual(client.Buffer), "Expected correct elements after change 2");
        }

        [TestMethod]
        public void NextNodeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D/></A>"));

            var allNodes = document.Root.Nodes().ToArray();
            var nextAttrib = ExpressionObserver.Execute(allNodes[0], n => n.NextNode);

            var client = new ValueProviderClientNPC<XNode>();
            client.Source = nextAttrib;

            Assert.AreEqual(allNodes[0].NextNode, client.Buffer, "Expected correct node object initially.");

            allNodes[1].Remove();

            Assert.AreEqual(allNodes[0].NextNode, client.Buffer, "Expected correct node object after change1.");

            allNodes[2].Remove();

            Assert.AreEqual(null, client.Buffer, "Expected no node object after change2.");

            allNodes[0].AddAfterSelf(new XElement("X"));

            Assert.AreEqual(allNodes[0].NextNode, client.Buffer, "Expected correct node object after change3.");
        }


        [TestMethod]
        public void PreviousNodeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D/></A>"));

            var allNodes = document.Root.Nodes().ToArray();
            var nextAttrib = ExpressionObserver.Execute(allNodes[2], n => n.PreviousNode);

            var client = new ValueProviderClientNPC<XNode>();
            client.Source = nextAttrib;

            Assert.AreEqual(allNodes[2].PreviousNode, client.Buffer, "Expected correct node object initially.");

            allNodes[1].Remove();

            Assert.AreEqual(allNodes[2].PreviousNode, client.Buffer, "Expected correct node object after change1.");

            allNodes[0].Remove();

            Assert.AreEqual(null, client.Buffer, "Expected no node object after change2.");

            allNodes[2].AddBeforeSelf(new XElement("X"));

            Assert.AreEqual(allNodes[2].PreviousNode, client.Buffer, "Expected correct node object after change3.");
        }

        [TestMethod]
        public void DescendantNodesTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E/><!-- comment --></D></A>"));

            var dNodes = ExpressionObserver.Execute(document.Root, r => r.DescendantNodes()).Cascade();

            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = dNodes;

            Assert.IsTrue(document.Root.DescendantNodes().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.DescendantNodes().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(document.Root.DescendantNodes().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantNodesTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E><Q/></E><!-- comment --></D></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodes = ExpressionObserver.Execute(rootElements, r => r.DescendantNodes()).Cascade();

            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = dNodes;

            Assert.IsTrue(rootElements.DescendantNodes().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.DescendantNodes().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.DescendantNodes().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(rootElements.DescendantNodes().SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }

        [TestMethod]
        public void DescendantsTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E/><!-- comment --></D></A>"));

            var dNodes = ExpressionObserver.Execute(document.Root, r => r.Descendants()).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(document.Root.Descendants().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.Descendants().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(document.Root.Descendants().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantsTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E><Q/></E><!-- comment --></D></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodes = ExpressionObserver.Execute(rootElements, r => r.Descendants()).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(rootElements.Descendants().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.Descendants().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.Descendants().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(rootElements.Descendants().SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }

        [TestMethod]
        public void DescendantsTest3()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<B/><!-- comment --></D></A>"));

            var dNodes = ExpressionObserver.Execute(document.Root, r => r.Descendants("B")).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(document.Root.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("B"));

            Assert.IsTrue(document.Root.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantsTest4()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<B>Text<E><B/></E><!-- comment --></B></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodes = ExpressionObserver.Execute(rootElements, r => r.Descendants("B")).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("B").Add(new XElement("B"));

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }

        [TestMethod]
        public void DescendantsTest5()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<B/><!-- comment --></D></A>"));

            var dNodes = ExpressionObserver.Execute(document.Root, r => r.Descendants(ValueProvider.Dynamic("B").Value)).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(document.Root.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("B"));

            Assert.IsTrue(document.Root.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantsTest6()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<B>Text<E><B/></E><!-- comment --></B></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodes = ExpressionObserver.Execute(rootElements, r => r.Descendants(ValueProvider.Dynamic("B").Value)).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("B").Add(new XElement("B"));

            Assert.IsTrue(rootElements.Descendants("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }

        [TestMethod]
        public void ElementTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><C/><D/></A>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Element("C"));

            var client = new ValueProviderClientNPC<XElement>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.Element("C"), client.Buffer, "Expected correct attribute object initially.");

            var rootObj = document.Root;
            rootObj.Element("C").Remove();

            Assert.AreEqual(null, client.Buffer, "Expected null after change.");

            rootObj.Add(new XElement("C"));

            Assert.AreEqual(document.Root.Element("C"), client.Buffer, "Expected correct attribute object after insert.");
        }

        [TestMethod]
        public void ElementsTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><C/><B/><D/></A>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Elements()).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = attrib1;

            Assert.IsTrue(document.Root.Elements().SequenceEqual(client.Buffer), "Expected correct elements initially.");

            var rootObj = document.Root;
            rootObj.Element("B").Remove();

            Assert.IsTrue(document.Root.Elements().SequenceEqual(client.Buffer), "Expected correct elements after removing second.");

            rootObj.Element("C").AddAfterSelf( new XElement("B"));

            Assert.IsTrue(document.Root.Elements().SequenceEqual(client.Buffer), "Expected correct elements after removing second.");
        }

        [TestMethod]
        public void ElementsTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><C/><B/><D/></A>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Elements("B")).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = attrib1;

            Assert.IsTrue(document.Root.Elements("B").SequenceEqual(client.Buffer), "Expected correct attributes initially.");

            var rootObj = document.Root;
            rootObj.Element("B").Remove();

            Assert.IsTrue(document.Root.Elements("B").SequenceEqual(client.Buffer), "Expected correct attributes after removing second.");

            rootObj.Element("C").AddAfterSelf(new XElement("B"));

            Assert.IsTrue(document.Root.Elements("B").SequenceEqual(client.Buffer), "Expected correct attributes after removing second.");
        }

        [TestMethod]
        public void ElementsTest3()
        {
            XDocument document = XDocument.Load(new StringReader("<A><X><B/><C/><B/><D/></X><X><B/><C/><B/><D/></X></A>"));

            var roots = new ObservableCollection<XElement>(document.Root.Elements());

            var attrib1 = ExpressionObserver.Execute(roots, r => r.Elements()).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = attrib1;

            Assert.IsTrue(roots.Elements().SequenceEqual(client.Buffer), "Expected correct elements initially.");

            document.Root.Element("X").Element("B").Remove();

            Assert.IsTrue(roots.Elements().SequenceEqual(client.Buffer), "Expected correct elements after change 1.");

            roots.RemoveAt(0);

            Assert.IsTrue(roots.Elements().SequenceEqual(client.Buffer), "Expected correct elements after change 2.");

            roots.Add(document.Root.Elements().First());
            roots[1].Add(new XElement("B"));

            Assert.IsTrue(roots.Elements().SequenceEqual(client.Buffer), "Expected correct elements after change 3.");
        }

        [TestMethod]
        public void ElementsTest4()
        {
            XDocument document = XDocument.Load(new StringReader("<A><X><B/><C/><B/><D/></X><X><B/><C/><B/><D/></X></A>"));

            var roots = new ObservableCollection<XElement>(document.Root.Elements());

            var attrib1 = ExpressionObserver.Execute(roots, r => r.Elements("B")).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = attrib1;

            Assert.IsTrue(roots.Elements("B").SequenceEqual(client.Buffer), "Expected correct elements initially.");

            document.Root.Element("X").Element("B").Remove();

            Assert.IsTrue(roots.Elements("B").SequenceEqual(client.Buffer), "Expected correct elements after change 1.");

            roots.RemoveAt(0);

            Assert.IsTrue(roots.Elements("B").SequenceEqual(client.Buffer), "Expected correct elements after change 2.");

            roots.Add(document.Root.Elements().First());
            roots[1].Add(new XElement("B"));

            Assert.IsTrue(roots.Elements("B").SequenceEqual(client.Buffer), "Expected correct elements after change 3.");
        }

        [TestMethod]
        public void NodesTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>A<C/><B/><!-- --><D/></A>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Nodes()).Cascade();

            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = attrib1;

            Assert.IsTrue(document.Root.Nodes().SequenceEqual(client.Buffer), "Expected correct elements initially.");

            var rootObj = document.Root;
            rootObj.Element("B").Remove();

            Assert.IsTrue(document.Root.Nodes().SequenceEqual(client.Buffer), "Expected correct elements after removing second.");

            rootObj.Element("C").AddAfterSelf(new XElement("B"));

            Assert.IsTrue(document.Root.Nodes().SequenceEqual(client.Buffer), "Expected correct elements after removing second.");
        }

        [TestMethod]
        public void NodesTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><X><B/>A<C/><B/><D/></X><X><B/><C/>A<B/><D/><!----></X></A>"));

            var roots = new ObservableCollection<XElement>(document.Root.Elements());

            var attrib1 = ExpressionObserver.Execute(roots, r => r.Nodes()).Cascade();

            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = attrib1;

            Assert.IsTrue(roots.Nodes().SequenceEqual(client.Buffer), "Expected correct elements initially.");

            document.Root.Element("X").Nodes().First().Remove();

            Assert.IsTrue(roots.Nodes().SequenceEqual(client.Buffer), "Expected correct elements after change 1.");

            roots.RemoveAt(0);

            Assert.IsTrue(roots.Nodes().SequenceEqual(client.Buffer), "Expected correct elements after change 2.");

            roots.Add(document.Root.Elements().First());
            roots[1].Add(new XComment("B"));

            Assert.IsTrue(roots.Nodes().SequenceEqual(client.Buffer), "Expected correct elements after change 3.");
        }

        [TestMethod]
        public void FirstNodeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D/></A>"));

            var root = document.Root;
            var nextAttrib = ExpressionObserver.Execute(root, n => n.FirstNode);

            var client = new ValueProviderClientNPC<XNode>();
            client.Source = nextAttrib;

            Assert.AreEqual(root.FirstNode, client.Buffer, "Expected correct node object initially.");

            root.FirstNode.Remove();

            Assert.AreEqual(root.FirstNode, client.Buffer, "Expected correct node object after change1.");

            root.LastNode.Remove();

            Assert.AreEqual(root.FirstNode, client.Buffer, "Expected correct node object after change2.");

            root.FirstNode.AddBeforeSelf(new XElement("X"));

            Assert.AreEqual(root.FirstNode, client.Buffer, "Expected correct node object after change3.");
        }

        [TestMethod]
        public void LastNodeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D/></A>"));

            var root = document.Root;
            var nextAttrib = ExpressionObserver.Execute(root, n => n.LastNode);

            var client = new ValueProviderClientNPC<XNode>();
            client.Source = nextAttrib;

            Assert.AreEqual(root.LastNode, client.Buffer, "Expected correct node object initially.");

            root.LastNode.Remove();

            Assert.AreEqual(root.LastNode, client.Buffer, "Expected correct node object after change 1.");

            root.FirstNode.Remove();

            Assert.AreEqual(root.LastNode, client.Buffer, "Expected correct node object after change 2.");

            root.LastNode.AddAfterSelf(new XElement("X"));

            Assert.AreEqual(root.LastNode, client.Buffer, "Expected correct node object after change 3.");
        }

        [TestMethod]
        public void AttributeTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Attribute("attrib1"));

            var client = new ValueProviderClientNPC<XAttribute>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.Attribute("attrib1"), client.Buffer, "Expected correct attribute object initially.");

            var rootObj = document.Root;
            rootObj.RemoveAttributes();

            Assert.AreEqual(null, client.Buffer, "Expected null after change.");

            rootObj.Add(new XAttribute("attrib1", "12"));

            Assert.AreEqual(document.Root.Attribute("attrib1"), client.Buffer, "Expected correct attribute object after insert.");
        }

        [TestMethod]
        public void AttributeTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Attribute(ValueProvider.Dynamic("attrib1").Value));

            var client = new ValueProviderClientNPC<XAttribute>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.Attribute("attrib1"), client.Buffer, "Expected correct attribute object initially.");

            var rootObj = document.Root;
            rootObj.RemoveAttributes();

            Assert.AreEqual(null, client.Buffer, "Expected null after change.");

            rootObj.Add(new XAttribute("attrib1", "12"));

            Assert.AreEqual(document.Root.Attribute("attrib1"), client.Buffer, "Expected correct attribute object after insert.");
        }

        [TestMethod]
        public void AttributesTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Attributes()).Cascade();

            var client = new CollectionTransformationClientSNCC<XAttribute>();
            client.Source = attrib1;

            Assert.IsTrue(new XAttribute[] { document.Root.Attribute("attrib1"), document.Root.Attribute("attrib2") }.SequenceEqual(client.Buffer), "Expected correct attributes initially.");

            var rootObj = document.Root;
            rootObj.Attribute("attrib2").Remove();

            Assert.IsTrue(new XAttribute[] { document.Root.Attribute("attrib1") }.SequenceEqual(client.Buffer), "Expected correct attributes after removing second.");
        }

        [TestMethod]
        public void AttributesTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Attributes("attrib2")).Cascade();

            var client = new CollectionTransformationClientSNCC<XAttribute>();
            client.Source = attrib1;

            Assert.IsTrue(new XAttribute[] { document.Root.Attribute("attrib2") }.SequenceEqual(client.Buffer), "Expected correct attributes initially.");

            var rootObj = document.Root;
            rootObj.Attribute("attrib2").Remove();

            Assert.IsTrue(new XAttribute[] { }.SequenceEqual(client.Buffer), "Expected correct attributes after removing second.");
        }

        [TestMethod]
        public void AttributesTest3()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B attrib1='1' attrib2='2' /><C attrib3='3' attrib4='4' /></A>"));

            var elements = new ObservableCollection<XElement>(document.Root.Elements());
            var attrib1 = ExpressionObserver.Execute(elements, s => s.Attributes()).Cascade();

            var client = new CollectionTransformationClientSNCC<XAttribute>();
            client.Source = attrib1;

            Assert.IsTrue( elements.Attributes().SequenceEqual(client.Buffer), "Expected correct attributes initially.");

            elements.RemoveAt(0);

            Assert.IsTrue(elements.Attributes().SequenceEqual(client.Buffer), "Expected correct attributes after change 1.");

            elements.Add(document.Root.Elements().Last());

            Assert.IsTrue(elements.Attributes().SequenceEqual(client.Buffer), "Expected correct attributes after change 2.");

            elements[0].Add( new XAttribute("attrib5","5") );

            Assert.IsTrue(elements.Attributes().SequenceEqual(client.Buffer), "Expected correct attributes after change 3.");
        }

        [TestMethod]
        public void AttributesTest4()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B attrib1='1' attrib2='2' /><C attrib3='3' attrib4='4' /></A>"));

            var elements = new ObservableCollection<XElement>(document.Root.Elements());
            var attrib1 = ExpressionObserver.Execute(elements, s => s.Attributes("attrib1")).Cascade();

            var client = new CollectionTransformationClientSNCC<XAttribute>();
            client.Source = attrib1;

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes initially.");

            elements.RemoveAt(0);

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes after change 1.");

            elements.Add(document.Root.Elements().Last());

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes after change 2.");

            elements[0].Add(new XAttribute("attrib1", "1"));

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes after change 3.");
        }

        [TestMethod]
        public void AttributesTest5()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Attributes(ValueProvider.Dynamic("attrib2").Value)).Cascade();

            var client = new CollectionTransformationClientSNCC<XAttribute>();
            client.Source = attrib1;

            Assert.IsTrue(new XAttribute[] { document.Root.Attribute("attrib2") }.SequenceEqual(client.Buffer), "Expected correct attributes initially.");

            var rootObj = document.Root;
            rootObj.Attribute("attrib2").Remove();

            Assert.IsTrue(new XAttribute[] { }.SequenceEqual(client.Buffer), "Expected correct attributes after removing second.");
        }

        [TestMethod]
        public void AttributesTest6()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B attrib1='1' attrib2='2' /><C attrib3='3' attrib4='4' /></A>"));

            var elements = new ObservableCollection<XElement>(document.Root.Elements());
            var attrib1 = ExpressionObserver.Execute(elements, s => s.Attributes(ValueProvider.Dynamic("attrib1").Value)).Cascade();

            var client = new CollectionTransformationClientSNCC<XAttribute>();
            client.Source = attrib1;

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes initially.");

            elements.RemoveAt(0);

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes after change 1.");

            elements.Add(document.Root.Elements().Last());

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes after change 2.");

            elements[0].Add(new XAttribute("attrib1", "1"));

            Assert.IsTrue(elements.Attributes("attrib1").SequenceEqual(client.Buffer), "Expected correct attributes after change 3.");
        }

        [TestMethod]
        public void DescendantNodesAndSelfTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E/><!-- comment --></D></A>"));

            var dNodesAndSelf = ExpressionObserver.Execute(document.Root, r => r.DescendantNodesAndSelf()).Cascade();

            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = dNodesAndSelf;

            Assert.IsTrue(document.Root.DescendantNodesAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.DescendantNodesAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(document.Root.DescendantNodesAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantNodesAndSelfTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E><Q/></E><!-- comment --></D></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodesAndSelf = ExpressionObserver.Execute(rootElements, r => r.DescendantNodesAndSelf()).Cascade();

            var client = new CollectionTransformationClientSNCC<XNode>();
            client.Source = dNodesAndSelf;

            Assert.IsTrue(rootElements.DescendantNodesAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.DescendantNodesAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.DescendantNodesAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(rootElements.DescendantNodesAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }


        [TestMethod]
        public void DescendantsAndSelfTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E/><!-- comment --></D></A>"));

            var dNodes = ExpressionObserver.Execute(document.Root, r => r.DescendantsAndSelf()).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(document.Root.DescendantsAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.DescendantsAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(document.Root.DescendantsAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantsAndSelfTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<E><Q/></E><!-- comment --></D></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodes = ExpressionObserver.Execute(rootElements, r => r.DescendantsAndSelf()).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(rootElements.DescendantsAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.DescendantsAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.DescendantsAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("D").Add(new XElement("X"));

            Assert.IsTrue(rootElements.DescendantsAndSelf().SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }

        [TestMethod]
        public void DescendantsAndSelfTest3()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<B/><!-- comment --></D></A>"));

            var dNodes = ExpressionObserver.Execute(document.Root, r => r.DescendantsAndSelf("B")).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(document.Root.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("B"));

            Assert.IsTrue(document.Root.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantsAndSelfTest4()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<B>Text<E><B/></E><!-- comment --></B></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodes = ExpressionObserver.Execute(rootElements, r => r.DescendantsAndSelf("B")).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("B").Add(new XElement("B"));

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }

        [TestMethod]
        public void DescendantsAndSelfTest5()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<D>Text<B/><!-- comment --></D></A>"));

            var dNodes = ExpressionObserver.Execute(document.Root, r => r.DescendantsAndSelf(ValueProvider.Dynamic("B").Value)).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(document.Root.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            document.Root.Elements().First().Remove();

            Assert.IsTrue(document.Root.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            document.Root.Element("D").Add(new XElement("B"));

            Assert.IsTrue(document.Root.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");
        }

        [TestMethod]
        public void DescendantsAndSelfTest6()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/>C<B>Text<E><B/></E><!-- comment --></B></A>"));

            var rootElements = new ObservableCollection<XElement>(document.Root.Elements());

            var dNodes = ExpressionObserver.Execute(rootElements, r => r.DescendantsAndSelf(ValueProvider.Dynamic("B").Value)).Cascade();

            var client = new CollectionTransformationClientSNCC<XElement>();
            client.Source = dNodes;

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes initialy.");

            rootElements.RemoveAt(0);

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 1.");

            rootElements.Add(document.Root.Element("B"));

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 2.");

            document.Root.Element("B").Add(new XElement("B"));

            Assert.IsTrue(rootElements.DescendantsAndSelf("B").SequenceEqual(client.Buffer), "Expected correct nodes after change 3.");
        }

        [TestMethod]
        public void FirstAttributeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A B='base' C='class' />"));

            var root = document.Root;
            var nextAttrib = ExpressionObserver.Execute(root, n => n.FirstAttribute);

            var client = new ValueProviderClientNPC<XAttribute>();
            client.Source = nextAttrib;

            Assert.AreEqual(root.FirstAttribute, client.Buffer, "Expected correct node object initially.");

            root.FirstAttribute.Remove();

            Assert.AreEqual(root.FirstAttribute, client.Buffer, "Expected correct node object after change1.");

            root.LastAttribute.Remove();

            Assert.AreEqual(root.FirstAttribute, client.Buffer, "Expected correct node object after change2.");

            root.Add(new XAttribute("X","x"));

            Assert.AreEqual(root.FirstAttribute, client.Buffer, "Expected correct node object after change3.");
        }

        [TestMethod]
        public void LastAttributeTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A B='base' C='class' />"));

            var root = document.Root;
            var nextAttrib = ExpressionObserver.Execute(root, n => n.LastAttribute);

            var client = new ValueProviderClientNPC<XAttribute>();
            client.Source = nextAttrib;

            Assert.AreEqual(root.LastAttribute, client.Buffer, "Expected correct node object initially.");

            root.LastAttribute.Remove();

            Assert.AreEqual(root.LastAttribute, client.Buffer, "Expected correct node object after change 1.");

            root.FirstAttribute.Remove();

            Assert.AreEqual(root.LastAttribute, client.Buffer, "Expected correct node object after change 2.");

            root.Add(new XAttribute("X", "x"));

            Assert.AreEqual(root.LastAttribute, client.Buffer, "Expected correct node object after change 3.");
        }

        [TestMethod]
        public void HasAttributesTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.HasAttributes);

            var client = new ValueProviderClientNPC<bool>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.HasAttributes, client.Buffer, "Expected correct result initially.");

            document.Root.Attribute("attrib1").Remove();

            Assert.AreEqual(document.Root.HasAttributes, client.Buffer, "Expected correct result after change 1.");

            document.Root.RemoveAttributes();

            Assert.AreEqual(document.Root.HasAttributes, client.Buffer, "Expected correct result after change 2.");

            document.Root.Add(new XAttribute("X", "x"));

            Assert.AreEqual(document.Root.HasAttributes, client.Buffer, "Expected correct result after change 3.");
        }

        [TestMethod]
        public void HasElementsTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><C/></A>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.HasElements);

            var client = new ValueProviderClientNPC<bool>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.HasElements, client.Buffer, "Expected correct result initially.");

            document.Root.Element("B").Remove();

            Assert.AreEqual(document.Root.HasElements, client.Buffer, "Expected correct result after change 1.");

            document.Root.Element("C").Remove();

            Assert.AreEqual(document.Root.HasElements, client.Buffer, "Expected correct result after change 2.");

            document.Root.Add(new XElement("X", "x"));

            Assert.AreEqual(document.Root.HasElements, client.Buffer, "Expected correct result after change 3.");
        }

        [TestMethod]
        public void IsEmptyTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A><B/><C/></A>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.IsEmpty);

            var client = new ValueProviderClientNPC<bool>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.IsEmpty, client.Buffer, "Expected correct result initially.");

            document.Root.Element("B").Remove();

            Assert.AreEqual(document.Root.IsEmpty, client.Buffer, "Expected correct result after change 1.");

            document.Root.RemoveAll();

            Assert.AreEqual(document.Root.IsEmpty, client.Buffer, "Expected correct result after change 2.");

            document.Root.Add(new XAttribute("X", "x"));

            Assert.AreEqual(document.Root.IsEmpty, client.Buffer, "Expected correct result after change 3.");
        }

        [TestMethod]
        public void NameTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Name);

            var client = new ValueProviderClientNPC<XName>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.Name, client.Buffer, "Expected correct result initially.");

            document.Root.Name = "B";

            Assert.AreEqual(document.Root.Name, client.Buffer, "Expected correct result after change 1.");

            document.Root.Name = "C";

            Assert.AreEqual(document.Root.Name, client.Buffer, "Expected correct result after change 2.");
        }

        [TestMethod]
        public void ValueTest1()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => r.Value);

            var client = new ValueProviderClientNPC<string>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root.Value, client.Buffer, "Expected correct result initially.");

            document.Root.Value = "B";

            Assert.AreEqual(document.Root.Value, client.Buffer, "Expected correct result after change 1.");

            document.Root.Add(new XElement("B"));

            Assert.AreEqual(document.Root.Value, client.Buffer, "Expected correct result after change 2.");

            document.Root.RemoveAll();

            Assert.AreEqual(document.Root.Value, client.Buffer, "Expected correct result after change 3.");
        }

        [TestMethod]
        public void ValueTest2()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (int?)r);

            var client = new ValueProviderClientNPC<int?>();
            client.Source = attrib1;

            Assert.AreEqual((int?)null, client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((int?)document.Root, client.Buffer, "Expected correct result after change 1.");

            document.Root.Add(new XElement("X"));

            Assert.AreEqual((int?)document.Root, client.Buffer, "Expected correct result after change 2.");

            document.Root.RemoveAll();

            Assert.AreEqual((int?)null, client.Buffer, "Expected correct result after change 3.");
        }

        [TestMethod]
        public void ValueTest3()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (int)r);

            var client = new ValueProviderClientNPC<int>();
            client.Source = attrib1;

            Assert.AreEqual(default(int), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((int)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest4()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (uint?)r);

            var client = new ValueProviderClientNPC<uint?>();
            client.Source = attrib1;

            Assert.AreEqual(default(uint?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((uint?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest5()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (uint)r);

            var client = new ValueProviderClientNPC<uint>();
            client.Source = attrib1;

            Assert.AreEqual(default(uint), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((uint)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest6()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (long?)r);

            var client = new ValueProviderClientNPC<long?>();
            client.Source = attrib1;

            Assert.AreEqual(default(long?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((long?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest7()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (long)r);

            var client = new ValueProviderClientNPC<long>();
            client.Source = attrib1;

            Assert.AreEqual(default(long), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((long)document.Root, client.Buffer, "Expected correct result after change 1.");
        }
        [TestMethod]
        public void ValueTest8()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (decimal?)r);

            var client = new ValueProviderClientNPC<decimal?>();
            client.Source = attrib1;

            Assert.AreEqual(default(decimal?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((decimal?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest9()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (decimal)r);

            var client = new ValueProviderClientNPC<decimal>();
            client.Source = attrib1;

            Assert.AreEqual(default(decimal), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10";

            Assert.AreEqual((decimal)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest10()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (float?)r);

            var client = new ValueProviderClientNPC<float?>();
            client.Source = attrib1;

            Assert.AreEqual(default(float?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10.0";

            Assert.AreEqual((float?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void ValueTest11()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (float)r);

            var client = new ValueProviderClientNPC<float>();
            client.Source = attrib1;

            Assert.AreEqual(default(float), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10.0";

            Assert.AreEqual((float)document.Root, client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void ValueTest12()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (double?)r);

            var client = new ValueProviderClientNPC<double?>();
            client.Source = attrib1;

            Assert.AreEqual(default(double?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10.0";

            Assert.AreEqual((double?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void ValueTest13()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (double)r);

            var client = new ValueProviderClientNPC<double>();
            client.Source = attrib1;

            Assert.AreEqual(default(double), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10.0";

            Assert.AreEqual((double)document.Root, client.Buffer, "Expected correct result after change 1.");
        }


        [TestMethod]
        public void ValueTest14()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (Guid?)r);

            var client = new ValueProviderClientNPC<Guid?>();
            client.Source = attrib1;

            Assert.AreEqual(default(Guid?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10101010101010101010101010101010";

            Assert.AreEqual((Guid?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest15()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (Guid)r);

            var client = new ValueProviderClientNPC<Guid>();
            client.Source = attrib1;

            Assert.AreEqual(default(Guid), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "10101010101010101010101010101010";

            Assert.AreEqual((Guid)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest16()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (TimeSpan?)r);

            var client = new ValueProviderClientNPC<TimeSpan?>();
            client.Source = attrib1;

            Assert.AreEqual(default(TimeSpan?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "PT2H23M";

            Assert.AreEqual((TimeSpan?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest17()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (TimeSpan)r);

            var client = new ValueProviderClientNPC<TimeSpan>();
            client.Source = attrib1;

            Assert.AreEqual(default(TimeSpan), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "PT2H23M";

            Assert.AreEqual((TimeSpan)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest18()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (DateTime?)r);

            var client = new ValueProviderClientNPC<DateTime?>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTime?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "2009-05-25T11:49:10.801+02:00";

            Assert.AreEqual((DateTime?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest19()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (DateTime)r);

            var client = new ValueProviderClientNPC<DateTime>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTime), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "2009-05-25T11:49:10.801+02:00";

            Assert.AreEqual((DateTime)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest20()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (DateTimeOffset?)r);

            var client = new ValueProviderClientNPC<DateTimeOffset?>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTimeOffset?), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "2009-05-25T00:00:00+02:00";

            Assert.AreEqual((DateTimeOffset?)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void ValueTest21()
        {
            XDocument document = XDocument.Load(new StringReader("<A/>"));

            var attrib1 = ExpressionObserver.Execute(document.Root, r => (DateTimeOffset)r);

            var client = new ValueProviderClientNPC<DateTimeOffset>();
            client.Source = attrib1;

            Assert.AreEqual(default(DateTimeOffset), client.Buffer, "Expected correct result initially.");

            document.Root.Value = "2009-05-25T00:00:00+02:00";

            Assert.AreEqual((DateTimeOffset)document.Root, client.Buffer, "Expected correct result after change 1.");
        }

        [TestMethod]
        public void RootTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2' />"));

            var attrib1 = ExpressionObserver.Execute(document, d => d.Root);

            var client = new ValueProviderClientNPC<XElement>();
            client.Source = attrib1;

            Assert.AreEqual(document.Root, client.Buffer, "Expected correct result initially.");

            document.Root.ReplaceWith(new XElement("C"));

            Assert.AreEqual(document.Root, client.Buffer, "Expected correct result after change 1.");

            document.Root.ReplaceWith(null);

            Assert.AreEqual(document.Root, client.Buffer, "Expected correct result after change 2.");
        }

        [TestMethod]
        public void CommentValueTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2'><!-- comment --></A>"));

            var comment = (XComment)document.Root.Nodes().First();
            var attrib1 = ExpressionObserver.Execute(comment, c => c.Value);

            var client = new ValueProviderClientNPC<string>();
            client.Source = attrib1;

            Assert.AreEqual(comment.Value, client.Buffer, "Expected correct value initially.");

            comment.Value = "B";

            Assert.AreEqual(comment.Value, client.Buffer, "Expected correct value after change 1.");

            comment.Value = "";

            Assert.AreEqual(comment.Value, client.Buffer, "Expected correct value after change 2.");
        }

        [TestMethod]
        public void TextValueTest()
        {
            XDocument document = XDocument.Load(new StringReader("<A attrib1='1' attrib2='2'>Text</A>"));

            var text = (XText)document.Root.Nodes().First();
            var attrib1 = ExpressionObserver.Execute(text, c => c.Value);

            var client = new ValueProviderClientNPC<string>();
            client.Source = attrib1;

            Assert.AreEqual(text.Value, client.Buffer, "Expected correct value initially.");

            text.Value = "B";

            Assert.AreEqual(text.Value, client.Buffer, "Expected correct value after change 1.");

            text.Value = "";

            Assert.AreEqual(text.Value, client.Buffer, "Expected correct value after change 2.");
        }
    }


}
