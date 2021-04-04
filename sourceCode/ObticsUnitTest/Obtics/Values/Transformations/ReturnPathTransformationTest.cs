using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Values.Transformations;
using ObticsUnitTest.Helpers;
using Obtics;
using Obtics.Values;
using System.ComponentModel;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for ReturnPathTransformationTest
    /// </summary>
    [TestClass]
    public class ReturnPathTransformationTest
    {
        public ReturnPathTransformationTest()
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

        [TestMethod]
        public void ConcurrencyTest1()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new string[] { "0", "1", "2", "9", "10", "6", "3", "3", "2", "1", "5", "5" },
                "5",
                frame => 
                    ReturnPathTransformationExplicitReadOnlyCheck<string>.Create(
                        frame.Patched(), 
                        ValueProvider.Static(false).Patched(), 
                        delegate(string value, bool isReadOnly) { }
                    ),
                "ReturnPathTransformationExplicitReadOnlyCheck<string>.Create(frame, ValueProvider.Static(false), delegate(string value) { })"
            );
        }

        [TestMethod]
        public void ConcurrencyTest2()
        {
            AsyncTestRunnerForValueTransformation.Run(
                new bool[] { true, false, true, false,false, true, true, false, true },
                true,
                frame => 
                    PropertyTransformation<IValueProvider, bool>.Create(
                        ValueProvider.Static<IValueProvider>(
                            ReturnPathTransformationExplicitReadOnlyCheck<bool>.Create(
                                ValueProvider.Static(true).Patched(), 
                                frame.Patched(), 
                                delegate(bool value, bool isReadOnly) { }
                            )
                            .Concrete()
                        )
                        .Patched(), 
                        SIValueProvider.IsReadOnlyPropertyName
                    ),
                "PropertyTransformation<IValueProvider<bool>, bool>.Create(ValueProvider.Static<IValueProvider>(ReturnPathTransformationExplicitReadOnlyCheck<bool>.Create(ValueProvider.Static(true), frame, delegate(bool value) { })), SIValueProvider.IsReadOnlyPropertyName)"
            );
        }
        
        [TestMethod]
        public void CorrectnessTest()
        {
            var sequence = new int[] { 1, 2, 3, 4, 1, 3, 1 };

            var frame0 = new FrameIValueProviderNPC<int>();
            frame0.IsReadOnly = true;

            frame0.SetValue(sequence[0]);

            var client = new ValueProviderClientNPC<int>();
            client.Source = ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(frame0.Patched(), ValueProvider.Static(false).Patched(), delegate(int value, bool isReadOnly) { });

            Assert.AreEqual<int>(client.Buffer, sequence[0], "new ReturnPathTransformationExplicitReadOnlyCheck<int>(frame0) (0): initialy the result value is not correct.");

            for (int i = 1, end = sequence.Length; i < end; ++i)
            {
                frame0.SetValue(sequence[i]);

                Assert.AreEqual<int>(sequence[i], client.Buffer, "new ReturnPathTransformationExplicitReadOnlyCheck<int>(frame0) (" + i.ToString() + "): after changes to source the result value is not correct.");
            }
        }

        class DeterministicEventRegistrationRunner : ValueProviderDeterministicEventRegistrationRunnerForValueProvider<int, int>
        {
            protected override IValueProvider<int> Create(FrameIValueProviderNPC<int> frame)
            {
                return ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(frame.Patched(), ValueProvider.Static(false).Patched(), delegate(int value, bool isReadOnly) { });
            }

            public override string Prefix
            {
                get { return "new ReturnPathTransformationExplicitReadOnlyCheck<int>(frame, ValueProvider.Static(false), delegate(int value) { })"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            (new DeterministicEventRegistrationRunner()).Run();
        }


        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(null, ValueProvider.Static(false).Patched(), delegate(int value, bool isReadOnly) { }), "Should return null when sourc is null");
        }

        [TestMethod]
        public void ArgumentsCheck_setValueAction()
        {
            Assert.IsNull(ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(ValueProvider.Static(10).Patched(), ValueProvider.Static(false).Patched(), null), "Should return null when setValueAction is null");
        }

        [TestMethod]
        public void ArgumentsCheck_isReadOnly()
        {
            Assert.IsNotNull(ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(ValueProvider.Static(10).Patched(), null, delegate(int value, bool isReadOnly) { }), "Should NOT return null when isReadOnly is null");
        }


        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                (IValueProvider<int>)ValueProvider.Static(23), ValueProvider.Static(2),
                ValueProvider.Static(true),ValueProvider.Static(false),
                (Action<int, bool>)delegate(int value, bool isReadOnly) { }, (Action<int, bool>)delegate(int value, bool isReadOnly) { },
                (s, iro, sva) => ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(s.Patched(), iro.Patched(), sva),
                "ReturnPathTransformationExplicitReadOnlyCheck<int>"
            );
        }

        [TestMethod]
        public void ValueSetterTest()
        {
            var x = 10;
            var t = ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(ValueProvider.Static(11).Patched(), ValueProvider.Static(false).Patched(), delegate(int v, bool isReadOnly) { x = v; });

            ((IValueProvider<int>)t).Value = 3;

            Assert.AreEqual(3, x, "Expected setValueAction delegate to be called whenever Value property setter was called.");

            t = ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(ValueProvider.Static(11).Patched(), ValueProvider.Static(true).Patched(), delegate(int v, bool isReadOnly) { x = v; });

            ((IValueProvider<int>)t).Value = 4;

            Assert.AreEqual(4, x, "Expected setValueAction delegate to be called whenever Value property setter was called, even if IsReadOnly is true.");
        }

        [TestMethod]
        public void IsReadOnlyTest()
        {
            var x = ValueProvider.Dynamic(false);
            var t = ReturnPathTransformationExplicitReadOnlyCheck<int>.Create(ValueProvider.Static(11).Patched(), x.Patched(), delegate(int v, bool isReadOnly) { });

            Assert.AreEqual(false, ((IValueProvider)t).IsReadOnly, "Expected initial value to be false.");

            x.Value = true;

            Assert.AreEqual(true, ((IValueProvider)t).IsReadOnly, "Expected value to follow isReadOnly value provider.");

            var propName = string.Empty;

            ((INotifyPropertyChanged)t.Concrete()).PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                propName = args.PropertyName;
            };

            x.Value = false;

            Assert.AreEqual(SIValueProvider.IsReadOnlyPropertyName, propName, "Expected prop changed event to be raised with propname 'IsReadOnly'");
        }
    }
}
