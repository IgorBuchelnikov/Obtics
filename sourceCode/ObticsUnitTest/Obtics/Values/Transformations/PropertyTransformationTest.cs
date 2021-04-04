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

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for PropertyTransformationTest
    /// </summary>
    [TestClass]
    public class PropertyTransformationTest
    {
        public PropertyTransformationTest()
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

        public class ObjectClass : INotifyPropertyChanged
        {
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            int _Waarde;

            public const string WaardePropertyName = "Waarde";

            public int Waarde 
            {
                get { return _Waarde; }
                set
                {
                    if (value != _Waarde)
                    {
                        _Waarde = value;

                        if (PropertyChanged != null)
                            PropertyChanged(this, new PropertyChangedEventArgs(WaardePropertyName));
                    }
                }
            }
        }

        [TestMethod]
        public void CorrectnessTest()
        {
            var source = ValueProvider.Static( new ObjectClass() );

            source.Value.Waarde = 10;

            var client = new ValueProviderClientNPC<int>();
            client.Source = PropertyTransformation<ObjectClass, int>.Create(source.Patched(), ObjectClass.WaardePropertyName);

            Assert.AreEqual<int>(client.Buffer, 10, "new PropertyTransformation<ObjectClass,int>(source, ObjectClass.WaardePropertyName ) (0): initialy the result value is not correct.");

            source.Value.Waarde = 13;

            Assert.AreEqual<int>(client.Buffer, 13, "new PropertyTransformation<ObjectClass,int>(source, ObjectClass.WaardePropertyName ) (1): after change to source property, result value is not correct.");

            source.Value.Waarde = 8;

            Assert.AreEqual<int>(client.Buffer, 8, "new PropertyTransformation<ObjectClass,int>(source, ObjectClass.WaardePropertyName ) (2): after change to source property, result value is not correct.");

            client.Source = PropertyTransformation<ObjectClass, int>.Create(source.Patched(), typeof(ObjectClass).GetProperty(ObjectClass.WaardePropertyName));

            Assert.AreEqual<int>(client.Buffer, 8, "new PropertyTransformation<ObjectClass, int>(source, typeof(ObjectClass).GetProperty(ObjectClass.WaardePropertyName)) (3): initialy the result value is not correct.");

            source.Value.Waarde = 13;

            Assert.AreEqual<int>(client.Buffer, 13, "new PropertyTransformation<ObjectClass, int>(source, typeof(ObjectClass).GetProperty(ObjectClass.WaardePropertyName)) (4): initialy the result value is not correct.");
        }

        class DeterministicEventRegistrationRunner1 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return PropertyTransformation<IValueProvider<int>, int>.Create(ValueProvider.Static((IValueProvider<int>)frame).Patched(), SIValueProvider.ValuePropertyName);
            }

            public override string Prefix
            {
                get { return "new PropertyTransformation<IValueProvider<int>, int>(frame, SIValueProvider.ValuePropertyName)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest1()
        {
            (new DeterministicEventRegistrationRunner1()).Run();
        }

        class DeterministicEventRegistrationRunner2 : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<IValueProvider<int>, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<IValueProvider<int>> frame)
            {
                return PropertyTransformation<IValueProvider<int>, int>.Create(frame.Patched(), SIValueProvider.ValuePropertyName).OnException((NullReferenceException ex)=>9876);
            }

            public override string Prefix
            {
                get { return "new PropertyTransformation<IValueProvider<int>, int>(frame, SIValueProvider.ValuePropertyName)"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest2()
        {
            (new DeterministicEventRegistrationRunner2()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source1()
        {
            Assert.IsNull(PropertyTransformation<IValueProvider<int>, int>.Create(null, SIValueProvider.ValuePropertyName), "Should return null when source is null");
        }

        [TestMethod]
        public void ArgumentsCheck_source2()
        {
            Assert.IsNull(PropertyTransformation<IValueProvider<int>, int>.Create(null, typeof(IValueProvider<int>).GetProperty(SIValueProvider.ValuePropertyName)), "Should return null when source is null");
        }

        [TestMethod]
        public void ArgumentsCheck_propInfo1()
        {
            Assert.IsNull(PropertyTransformation<IValueProvider<int>, int>.Create(ValueProvider.Static(ValueProvider.Static(10)).Patched(), (System.Reflection.PropertyInfo)null), "Should return null when propInfo is null");
        }

        [TestMethod]
        public void ArgumentsCheck_propInfo2()
        {
            try
            {
                var r = PropertyTransformation<object, int>.Create(ValueProvider.Static<object>(10).Patched(), typeof(IValueProvider<int>).GetProperty(SIValueProvider.ValuePropertyName));
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void ArgumentsCheck_propInfo3()
        {
            Assert.IsNull(PropertyTransformation<IValueProvider<int>, float>.Create(ValueProvider.Static(ValueProvider.Static(10)).Patched(), (System.Reflection.PropertyInfo)null), "Should return null when propInfo is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_propertyName()
        {
            Assert.IsNull(PropertyTransformation<IValueProvider<int>, int>.Create(ValueProvider.Static(ValueProvider.Static(10)).Patched(), (string)null), "Should return null when propName is null.");
        }


        public class TwoProp
        {
            public bool True { get { return true; } }
            public bool False { get { return false; } }
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<TwoProp>)ValueProvider.Static(new TwoProp()),
                (IValueProvider<TwoProp>)ValueProvider.Static(new TwoProp()),
                "True", "False",
                (f, p) => PropertyTransformation<TwoProp, bool>.Create(f.Patched(), p),
                "PropertyTransformation<TwoProp, bool>"
            );
        }

    }
}
