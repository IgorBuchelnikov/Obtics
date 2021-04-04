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

namespace ObticsUnitTest.Obtics.Collections
{
    
    
    /// <summary>
    ///This is a test class for ObservableEnumerableTest and is intended
    ///to contain all ObservableEnumerableTest Unit Tests
    ///</summary>
    public partial class ObservableEnumerableTest
    {
        [TestMethod]
        public void ConcatTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.Concat(new int[] { 1, 2 }, new int[] { 3, 4 })
                ),
                "ConcatTest1(a)"
            );


            var seq1 = new int[] { 1, 2, 3, 4 };
            var seq2 = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.Concat(seq1, seq2),
                ObservableEnumerable.Concat(seq1, seq2),
                "ConcatTest1(b)"
            );
        }

        [TestMethod]
        public void ConcatTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 2, 3, 4 },
                    ObservableEnumerable.Concat(new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } })
                ),
                "ConcatTest2(a)"
            );

            var seq = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };

            Assert.AreEqual(
                ObservableEnumerable.Concat(seq),
                ObservableEnumerable.Concat(seq),
                "ConcatTest2(b)"
            );
        }

        [TestMethod]
        public void ConcatTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new int[] { 1, 3, 2, 4 },
                    ObservableEnumerable.Concat( System.Linq.Enumerable.GroupBy(  new int[] { 1, 2, 3, 4 }, i => i % 2 ) )
                ),
                "ConcatTest3(a)"
            );

            var seq = System.Linq.Enumerable.GroupBy(new int[] { 1, 2, 3, 4 }, i => i);

            Assert.AreEqual(
                ObservableEnumerable.Concat(seq),
                ObservableEnumerable.Concat(seq),
                "ConcatTest3(b)"
            );
        }
    }
}
