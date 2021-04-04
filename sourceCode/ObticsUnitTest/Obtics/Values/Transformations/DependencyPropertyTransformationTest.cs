using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using System.ComponentModel;
using ObticsUnitTest.Helpers;
using Obtics.Values.Transformations;
using Obtics.Values;
using Obtics;
using System.Windows;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for DependencyPropertyTransformationTest
    /// </summary>
    [TestClass]
    public class DependencyPropertyTransformationTest
    {
        public DependencyPropertyTransformationTest()
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

#if !SILVERLIGHT

        public class ObjectClass : DependencyObject
        {
            public int Waarde
            {
                get { return (int)GetValue(WaardeProperty); }
                set { SetValue(WaardeProperty, value); }
            }

            // Using a DependencyProperty as the backing store for Waarde.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty WaardeProperty =
                DependencyProperty.Register("Waarde", typeof(int), typeof(ObjectClass), new PropertyMetadata(0));

            public int Value
            {
                get { return (int)GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(int), typeof(ObjectClass), new PropertyMetadata(0));

            ObjectClass() { }

            public static ObjectClass Create()
            {
                return new ObjectClass(); 
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var source = ValueProvider.Static( ObjectClass.Create() );

            source.Value.Waarde = 10;

            var client = new ValueProviderClientNPC<int>();
            client.Source = DependencyPropertyTransformation<ObjectClass, int>.Create(source.Patched(), ObjectClass.WaardeProperty);

            Assert.AreEqual<int>(client.Buffer, 10, "new DependencyPropertyTransformation<ObjectClass,int>(source, ObjectClass.WaardeProperty ) (0): initialy the result value is not correct.");

            source.Value.Waarde = 13;

            Assert.AreEqual<int>(client.Buffer, 13, "new DependencyPropertyTransformation<ObjectClass,int>(source, ObjectClass.WaardeProperty ) (1): after change to source property, result value is not correct.");

            source.Value.Waarde = 8;

            Assert.AreEqual<int>(client.Buffer, 8, "new DependencyPropertyTransformation<ObjectClass,int>(source, ObjectClass.WaardeProperty ) (2): after change to source property, result value is not correct.");
        }        

        class DeterministicEventRegistrationRunner1 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<ObjectClass, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<ObjectClass> frame)
            {
                return DependencyPropertyTransformation<ObjectClass, int>.Create(frame.Patched(), ObjectClass.WaardeProperty).OnException((NullReferenceException ex)=>9876);
            }

            public override string Prefix
            {
                get { return "new DependencyPropertyTransformation<ObjectClass, int>(frame, ObjectClass.WaardeProperty)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest1()
        {
            (new DeterministicEventRegistrationRunner1()).Run();
        }

        //class DeterministicEventRegistrationRunner2 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<IValueProvider<int>, int>
        //{
        //    protected override IValueProvider<int> Create(FrameIValueProviderNPC<IValueProvider<int>> frame)
        //    {
        //        return new DependencyPropertyTransformation<IValueProvider<int>, int>(frame, SIValueProvider.ValuePropertyName);
        //    }

        //    public override string Prefix
        //    {
        //        get { return "new DependencyPropertyTransformation<IValueProvider<int>, int>(frame, SIValueProvider.ValuePropertyName)"; }
        //    }
        //}

        //[TestMethod]
        //public void DeterministicEventRegistrationTest2()
        //{
        //    (new DeterministicEventRegistrationRunner2()).Run();
        //}


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(DependencyPropertyTransformation<ObjectClass, int>.Create(null, ObjectClass.WaardeProperty),"Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_dependencyProperty()
        {
            Assert.IsNull(DependencyPropertyTransformation<ObjectClass, int>.Create(ValueProvider.Static(ObjectClass.Create()).Patched(), (DependencyProperty)null), "Should return null when dependencyProperty is null.");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<ObjectClass>)ValueProvider.Static(ObjectClass.Create()),
                (IValueProvider<ObjectClass>)ValueProvider.Static(ObjectClass.Create()),
                ObjectClass.WaardeProperty, ObjectClass.ValueProperty,
                (f, p) => DependencyPropertyTransformation<ObjectClass, int>.Create(f.Patched(), p),
                "DependencyPropertyTransformation<ObjectClass, int>"
            );
        }
#endif

    }
}
