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
    [TestClass]
    public class WeightedSkiplistTest
    {
        class Node : OC.WeightedSkiplist<Node>.Node
        {
            public int _Value;
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var skipList = new OC.WeightedSkiplist<Node>();

            var node1 = new Node { _Value = 1 };
            var node2 = new Node { _Value = 2 };
            var node3 = new Node { _Value = 3 };
            var node4 = new Node { _Value = 4 };
            var node5 = new Node { _Value = 5 };
            var node6 = new Node { _Value = 6 };

            skipList.Insert(0, node1, 13);
            skipList.Insert(1, node4, 17);
            skipList.Insert(2, node6, 11);
            skipList.Insert(1, node3, 7);
            skipList.Insert(1, node2, 19);
            skipList.Insert(4, node5, 5);

            Assert.AreEqual(6, skipList.Count, "list contains 6 items");
            Assert.AreEqual(72, skipList.Weight, "skiplist total weight is 72");

            Assert.AreEqual(0, skipList.IndexOf(node1), "Node1 at pos 0");
            Assert.AreEqual(1, skipList.IndexOf(node2), "Node2 at pos 1");
            Assert.AreEqual(2, skipList.IndexOf(node3), "Node3 at pos 2");
            Assert.AreEqual(3, skipList.IndexOf(node4), "Node4 at pos 3");
            Assert.AreEqual(4, skipList.IndexOf(node5), "Node5 at pos 4");
            Assert.AreEqual(5, skipList.IndexOf(node6), "Node6 at pos 5");

            Assert.AreEqual(13, skipList.WeightAt(node1), "Node1 with weight 13");
            Assert.AreEqual(32, skipList.WeightAt(node2), "Node2 with weight 32");
            Assert.AreEqual(39, skipList.WeightAt(node3), "Node3 with weight 39");
            Assert.AreEqual(56, skipList.WeightAt(node4), "Node4 with weight 56");
            Assert.AreEqual(61, skipList.WeightAt(node5), "Node5 with weight 61");
            Assert.AreEqual(72, skipList.WeightAt(node6), "Node6 with weight 72");

            skipList.RemoveAt(4);
            skipList.RemoveAt(2);

            Assert.AreEqual(4, skipList.Count, "list contains 4 items");
            Assert.AreEqual(60, skipList.Weight, "skiplist total weight is 60");

            Assert.AreEqual(0, skipList.IndexOf(node1), "Node1 at pos 0");
            Assert.AreEqual(1, skipList.IndexOf(node2), "Node2 at pos 1");
            Assert.AreEqual(2, skipList.IndexOf(node4), "Node4 at pos 2");
            Assert.AreEqual(3, skipList.IndexOf(node6), "Node6 at pos 3");

            Assert.AreEqual(13, skipList.WeightAt(node1), "Node1 with weight 13");
            Assert.AreEqual(32, skipList.WeightAt(node2), "Node2 with weight 32");
            Assert.AreEqual(49, skipList.WeightAt(node4), "Node3 with weight 49");
            Assert.AreEqual(60, skipList.WeightAt(node6), "Node4 with weight 60");

            skipList.SetWeightAt(0, 4);
            skipList.SetWeightAt(2, 4);

            Assert.AreEqual(38, skipList.Weight, "skiplist total weight is 38");

            Assert.AreEqual(4, skipList.WeightAt(node1), "Node1 with weight 4");
            Assert.AreEqual(23, skipList.WeightAt(node2), "Node2 with weight 23");
            Assert.AreEqual(27, skipList.WeightAt(node4), "Node3 with weight 27");
            Assert.AreEqual(38, skipList.WeightAt(node6), "Node4 with weight 38");
        
        }
    }
}
