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
using System.Windows;
using System.Windows.Threading;

namespace ObticsUnitTest.Obtics.Values
{
    public partial class ValueProviderTest
    {
        public class DepObj : DependencyObject
        {
            public int MyProperty
            {
                get { return (int)GetValue(MyPropertyProperty); }
                set { SetValue(MyPropertyProperty, value); }
            }

            // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MyPropertyProperty =
                DependencyProperty.Register("MyProperty", typeof(int), typeof(DepObj), new PropertyMetadata(17));

            DepObj() { }

            public static DepObj Create()
            {
#if SILVERLIGHT
                DepObj obj = null;
                new DispatcherSynchronizationContext(Deployment.Current.Dispatcher).Send(delegate(object state) { obj = new DepObj(); }, null);
                return obj;
#else
                return new DepObj(); 
#endif
            }
        }

#if !SILVERLIGHT
        [TestMethod]
        public void PropertyTest1()
        {
            Assert.AreEqual(
                ValueProvider.Static(DepObj.Create()).Property<DepObj, int>(DepObj.MyPropertyProperty).Value,
                17,
                "PropertyTest1(a)"
            );

            var s = ValueProvider.Static(DepObj.Create());

            Assert.AreEqual(
                s.Property<DepObj, int>(DepObj.MyPropertyProperty),
                s.Property<DepObj, int>(DepObj.MyPropertyProperty),
                "PropertyTest1(b)"
            );
        }
#endif

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void PropertyTest2()
        {
            Assert.AreEqual(
                ValueProvider.Static(DepObj.Create()).Property<DepObj, int>(typeof(DepObj).GetProperty("MyProperty")).Value,
                17,
                "PropertyTest2(a)"
            );

            var s = ValueProvider.Static(DepObj.Create());
            var propInfo = typeof(DepObj).GetProperty("MyProperty");

            Assert.AreEqual(
                s.Property<DepObj, int>(propInfo),
                s.Property<DepObj, int>(propInfo),
                "PropertyTest2(b)"
            );
        }

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void PropertyTest3()
        {
            Assert.AreEqual(
                ValueProvider.Static(DepObj.Create()).Property<DepObj, int>("MyProperty").Value,
                17,
                "PropertyTest3(a)"
            );

            var s = ValueProvider.Static(DepObj.Create());

            Assert.AreEqual(
                s.Property<DepObj, int>("MyProperty"),
                s.Property<DepObj, int>("MyProperty"),
                "PropertyTest3(b)"
            );
        }

#if !SILVERLIGHT
        [TestMethod]
        public void PropertyTest4()
        {
            Assert.AreEqual(
                ValueProviderWindowsBaseExtensions.Property<DepObj, int>(DepObj.Create(), DepObj.MyPropertyProperty).Value,
                17,
                "PropertyTest4(a)"
            );

            var s = DepObj.Create();

            Assert.AreEqual(
                ValueProviderWindowsBaseExtensions.Property<DepObj, int>(s, DepObj.MyPropertyProperty),
                ValueProviderWindowsBaseExtensions.Property<DepObj, int>(s, DepObj.MyPropertyProperty),
                "PropertyTest4(b)"
            );
        }
#endif

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void PropertyTest5()
        {
            Assert.AreEqual(
                ValueProvider.Property<DepObj, int>(DepObj.Create(), typeof(DepObj).GetProperty("MyProperty")).Value,
                17,
                "PropertyTest5(a)"
            );

            var s = DepObj.Create();
            var propInfo = typeof(DepObj).GetProperty("MyProperty");

            Assert.AreEqual(
                ValueProvider.Property<DepObj, int>(s, propInfo),
                ValueProvider.Property<DepObj, int>(s, propInfo),
                "PropertyTest5(b)"
            );
        }

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void PropertyTest6()
        {
            Assert.AreEqual(
                ValueProvider.Property<DepObj, int>(DepObj.Create(), "MyProperty").Value,
                17,
                "PropertyTest6(a)"
            );

            var s = DepObj.Create();

            Assert.AreEqual(
                ValueProvider.Property<DepObj, int>(s, "MyProperty"),
                ValueProvider.Property<DepObj, int>(s, "MyProperty"),
                "PropertyTest6(b)"
            );
        }

#if !SILVERLIGHT
        [TestMethod]
        public void PropertyTest7()
        {
            Assert.AreEqual(
                ValueProvider.Static(DepObj.Create()).DependencyProperties().Get<int>(DepObj.MyPropertyProperty).Value,
                17,
                "PropertyTest1(a)"
            );

            var s = ValueProvider.Static(DepObj.Create());

            Assert.AreEqual(
                s.Property<DepObj, int>(DepObj.MyPropertyProperty),
                s.Property<DepObj, int>(DepObj.MyPropertyProperty),
                "PropertyTest1(b)"
            );
        }
#endif

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void PropertyTest8()
        {
            Assert.AreEqual(
                ValueProvider.Static(DepObj.Create()).Properties().Get<int>(typeof(DepObj).GetProperty("MyProperty")).Value,
                17,
                "PropertyTest2(a)"
            );

            var s = ValueProvider.Static(DepObj.Create());
            var propInfo = typeof(DepObj).GetProperty("MyProperty");

            Assert.AreEqual(
                s.Property<DepObj, int>(propInfo),
                s.Property<DepObj, int>(propInfo),
                "PropertyTest2(b)"
            );
        }

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void PropertyTest9()
        {
            Assert.AreEqual(
                ValueProvider.Static(DepObj.Create()).Properties().Get<int>("MyProperty").Value,
                17,
                "PropertyTest3(a)"
            );

            var s = ValueProvider.Static(DepObj.Create());

            Assert.AreEqual(
                s.Property<DepObj, int>("MyProperty"),
                s.Property<DepObj, int>("MyProperty"),
                "PropertyTest3(b)"
            );
        }

#if !SILVERLIGHT
        [TestMethod]
        public void PropertyTest10()
        {
            Assert.AreEqual(
                ValueProviderWindowsBaseExtensions.DependencyProperties(DepObj.Create()).Get<int>(DepObj.MyPropertyProperty).Value,
                17,
                "PropertyTest4(a)"
            );

            var s = DepObj.Create();

            Assert.AreEqual(
                ValueProviderWindowsBaseExtensions.Property<DepObj, int>(s, DepObj.MyPropertyProperty),
                ValueProviderWindowsBaseExtensions.Property<DepObj, int>(s, DepObj.MyPropertyProperty),
                "PropertyTest4(b)"
            );
        }
#endif

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void PropertyTest11()
        {
            Assert.AreEqual(
                ValueProvider.Properties(DepObj.Create()).Get<int>(typeof(DepObj).GetProperty("MyProperty")).Value,
                17,
                "PropertyTest5(a)"
            );

            var s = DepObj.Create();
            var propInfo = typeof(DepObj).GetProperty("MyProperty");

            Assert.AreEqual(
                ValueProvider.Property<DepObj, int>(s, propInfo),
                ValueProvider.Property<DepObj, int>(s, propInfo),
                "PropertyTest5(b)"
            );
        }

#if !SILVERLIGHT
        [TestMethod]
        public void PropertyTest12()
        {
            Assert.AreEqual(
                ValueProviderWindowsBaseExtensions.DependencyProperties(DepObj.Create()).Get<int>("MyProperty").Value,
                17,
                "PropertyTest6(a)"
            );

            var s = DepObj.Create();

            Assert.AreEqual(
                ValueProvider.Property<DepObj, int>(s, "MyProperty"),
                ValueProvider.Property<DepObj, int>(s, "MyProperty"),
                "PropertyTest6(b)"
            );
        }
#endif
    }
}
