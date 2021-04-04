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
        public void DefaultIfEmptyTest1()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new long[] { 0L },
                    ObservableEnumerable.DefaultIfEmpty(new long[] { })
                ),
                "DefaultIfEmptyTest1(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new long[] { 2L },
                    ObservableEnumerable.DefaultIfEmpty(new long[] { 2L })
                ),
                "DefaultIfEmptyTest1(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.DefaultIfEmpty(seq),
                ObservableEnumerable.DefaultIfEmpty(seq),
                "DefaultIfEmptyTest1(c)"
            );
        }

        [TestMethod]
        public void DefaultIfEmptyTest2()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new long[] { 1L },
                    ObservableEnumerable.DefaultIfEmpty(new long[] { }, 1L)
                ),
                "DefaultIfEmptyTest2(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new long[] { 2L },
                    ObservableEnumerable.DefaultIfEmpty(new long[] { 2L }, 1L)
                ),
                "DefaultIfEmptyTest2(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.DefaultIfEmpty(seq, 3),
                ObservableEnumerable.DefaultIfEmpty(seq, 3),
                "DefaultIfEmptyTest2(c)"
            );
        }

        [TestMethod]
        public void DefaultIfEmptyTest3()
        {
            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new long[] { 1L },
                    ObservableEnumerable.DefaultIfEmpty(new long[] { }, ValueProvider.Static(1L))
                ),
                "DefaultIfEmptyTest3(a)"
            );

            Assert.IsTrue(
                System.Linq.Enumerable.SequenceEqual(
                    new long[] { 2L },
                    ObservableEnumerable.DefaultIfEmpty(new long[] { 2L }, ValueProvider.Static(1L))
                ),
                "DefaultIfEmptyTest3(b)"
            );

            var seq = new int[] { 1, 2, 3, 4 };

            Assert.AreEqual(
                ObservableEnumerable.DefaultIfEmpty(seq, ValueProvider.Static(3)),
                ObservableEnumerable.DefaultIfEmpty(seq, ValueProvider.Static(3)),
                "DefaultIfEmptyTest3(c)"
            );
        }
    }
}
