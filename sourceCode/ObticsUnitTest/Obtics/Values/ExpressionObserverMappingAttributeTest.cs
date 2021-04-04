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
using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.Obtics.Values
{
    [TestClass]
    public class ExpressionObserverMappingAttributeTest
    {
        public class Nest1<T1>
        {
            public class Nest2
            {
                public class Nest3<T3>
                {
                    [ExpressionObserverMapping(typeof(ObservableImp))]
                    public static string Methode<TM>(T1 p1, T3 p2, TM p3)
                    { return ""; }

                    [ExpressionObserverMapping(typeof(ObservableImp), "XXX")]
                    public static string Methode2<TM>(T1 p1, T3 p2, TM p3)
                    { return ""; }
                }
            }
        }

        public class ObservableImp
        {
            public static IValueProvider<string> Methode<T1, T3, TM>(IValueProvider<T1> p1, IValueProvider<T3> p2, IValueProvider<TM> p3)
            { return p1.Select(p2, p3, (v1, v2, v3) => v1.ToString() + v2.ToString() + v3.ToString()); }

            public static IValueProvider<string> XXX<T1, T3, TM>(IValueProvider<T1> p1, IValueProvider<T3> p2, IValueProvider<TM> p3)
            { return p1.Select(p2, p3, (v1, v2, v3) => v3.ToString() + v2.ToString() + v1.ToString()); }
        }


        [TestMethod]
        public void CanFindMappingForDeepNestedGenericMethod()
        {
            var intSource = ValueProvider.Dynamic(10);
            var floatSource = ValueProvider.Dynamic(10.0);
            var strSource = ValueProvider.Dynamic("Ape");

            var client = new ValueProviderClientNPC<string>();
            client.Source = ExpressionObserver.Execute(intSource, floatSource, strSource, (ints, ds, ss) => Nest1<int>.Nest2.Nest3<double>.Methode(ints.Value, ds.Value, ss.Value));

            Assert.AreEqual("1010Ape", client.Buffer);

            intSource.Value = 20;
            floatSource.Value = 20.0;
            strSource.Value = "Monkey";

            Assert.AreEqual("2020Monkey", client.Buffer);
        }

        [TestMethod]
        public void CanFindMappingForDeepNestedGenericMethodWithSpecificName()
        {
            var intSource = ValueProvider.Dynamic(10);
            var floatSource = ValueProvider.Dynamic(10.0);
            var strSource = ValueProvider.Dynamic("Ape");

            var client = new ValueProviderClientNPC<string>();
            client.Source = ExpressionObserver.Execute(intSource, floatSource, strSource, (ints, ds, ss) => Nest1<int>.Nest2.Nest3<double>.Methode2(ints.Value, ds.Value, ss.Value));

            Assert.AreEqual("Ape1010", client.Buffer);

            intSource.Value = 20;
            floatSource.Value = 20.0;
            strSource.Value = "Monkey";

            Assert.AreEqual("Monkey2020", client.Buffer);
        }        
    }
}
