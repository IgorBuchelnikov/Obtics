using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Values;
using Obtics;
using OE = Obtics.Collections.ObservableEnumerable;

namespace ObticsUnitTest.Obtics.Values
{
    public partial class ValueProviderTest
    {
        [TestMethod]
        public void CascadeTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(ValueProvider.Static(10)).Cascade().Value,
                10,
                "CascadeTest1(a)"
            );

            Assert.AreEqual(
                ValueProvider.Static((IValueProvider<int>)null).Cascade().OnException((NullReferenceException ex) => 1234).Value,
                1234,
                "CascadeTest1(b)"
            );

            var s = ValueProvider.Static(ValueProvider.Static(10));

            Assert.AreEqual(
                s.Cascade(),
                s.Cascade(),
                "CascadeTest1(c)"
            );
        }

        [TestMethod]
        public void CascadeTest2()
        {
            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static((IEnumerable<int>)new int[] { 1, 2, 3 }).Cascade(),
                    new int[] { 1, 2, 3 }
                ),
                "CascadeTest2(a)"
            );

            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static((IEnumerable<int>)null).Cascade(),
                    new int[] { }
                ),
                "CascadeTest2(b)"
            );

            var s = ValueProvider.Static((IEnumerable<int>)new int[] { 1, 2, 3 });

            Assert.AreEqual(
                s.Cascade(),
                s.Cascade(),
                "CascadeTest2(c)"
            );
        }

        [TestMethod]
        public void CascadeTest3()
        {
            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static((IOrderedEnumerable<int>)OE.OrderBy(new int[] { 3, 2, 1 }, i => i)).Cascade(),
                    new int[] { 1, 2, 3 }
                ),
                "CascadeTest3(a)"
            );

            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static((IOrderedEnumerable<int>)null).Cascade(),
                    new int[] { }
                ),
                "CascadeTest3(b)"
            );

            var s = ValueProvider.Static((IOrderedEnumerable<int>)OE.OrderBy(new int[] { 3, 2, 1 }, i => i));

            Assert.AreEqual(
                s.Cascade(),
                s.Cascade(),
                "CascadeTest3(c)"
            );
        }

        [TestMethod]
        public void CascadeTest4()
        {
            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static(OE.OrderBy(new int[] { 3, 2, 1 }, i => i)).Cascade(),
                    new int[] { 1, 2, 3 }
                ),
                "CascadeTest4(a)"
            );

            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static(OE.OrderBy<int, int>(null, i => i)).Cascade(),
                    new int[] { }
                ),
                "CascadeTest4(b)"
            );

            var s = ValueProvider.Static(OE.OrderBy(new int[] { 3, 2, 1 }, i => i));

            Assert.AreEqual(
                s.Cascade(),
                s.Cascade(),
                "CascadeTest4(c)"
            );
        }

        [TestMethod]
        public void CascadeTest5()
        {
            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static(new int[] { 3, 2, 1 }.GroupBy( i => i).ElementAt(0)).Cascade(),
                    new int[] { 3 }
                ),
                "CascadeTest5(a)"
            );

            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    ValueProvider.Static((IGrouping<int, int>)null).Cascade(),
                    new int[] { }
                ),
                "CascadeTest5(b)"
            );

            var s = ValueProvider.Static(new int[] { 3, 2, 1 }.GroupBy(i => i).ElementAt(0));

            Assert.AreEqual(
                s.Cascade(),
                s.Cascade(),
                "CascadeTest5(c)"
            );
        }
    }
}
