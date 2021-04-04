using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Values.Transformations;
using Obtics;
using Obtics.Values;
using System.ComponentModel;

namespace ObticsUnitTest.Obtics.Values.Transformations
{
    /// <summary>
    /// Summary description for ConvertTransformationBaseTest
    /// </summary>
    [TestClass]
    public class ConvertTransformationBaseTest
    {
        [TestMethod]
        public void CorrectValueInInitialStateTest()
        {
            var frame = new FrameIValueProviderNPC<string>();
            frame.IsReadOnly = true;

            //plain value
            frame.SetValue("777");

            var trf = UnarySelectTransformation<string, int>.Create(frame.Patched(), s => int.Parse(s));

            Assert.AreEqual(777, trf.Value, "Expected correct result in Initial State");

            //Invalid input (convert lambda raises exception)
            frame.SetValue("Aap");

            try
            {
                var res = trf.Value;
                Assert.Fail("Expected FormatException; not " + res.ToString());
            }
            catch (FormatException) { }
            catch { Assert.Fail("Expected FormatException"); }

            //source raises exception
            frame.SetExceptionValue(new NullReferenceException());

            try
            {
                var res = trf.Value;
                Assert.Fail("Expected FormatException; not " + res.ToString());
            }
            catch (NullReferenceException) { }
            catch { Assert.Fail("Expected NullReferenceException"); }

            //reset to normal value
            frame.SetValue("999");

            Assert.AreEqual(999, trf.Value, "Expected correct result in Initial State");
        }

        [TestMethod]
        public void IndirectClientRegistrationTest()
        {
            var frame = new FrameIValueProviderNPC<string>();
            frame.IsReadOnly = true;

            //plain value
            frame.SetValue("777");

            var trf = UnarySelectTransformation<string, int>.Create(frame.Patched(), s => int.Parse(s));

            Assert.AreEqual(0, frame.PropertyChangedClientsCount, "Expected no change registrations initialy.");

            PropertyChangedEventHandler handler = delegate(object sender, System.ComponentModel.PropertyChangedEventArgs e) { };

            ((INotifyPropertyChanged)trf.Concrete()).PropertyChanged += handler;

            //Test removed. On adding a property changed handler the value will be probed.
            //Too many clients, even official ones (Silverlight) might first aquire the value and
            //after register for changes.
            //This probing will lead to a popery changed registration on the source.
            //
            //Assert.AreEqual(0, frame.PropertyChangedClientsCount, "Expected no change registrations after only registered.");

            Assert.AreEqual(777, trf.Value, "Expected correct value in clients state.");

            Assert.AreEqual(1, frame.PropertyChangedClientsCount, "Expected 1 change registrations after only registered.");

            ((INotifyPropertyChanged)trf.Concrete()).PropertyChanged -= handler;

            Assert.AreEqual(0, frame.PropertyChangedClientsCount, "Expected no change registrations after removing only registration.");

            INotifyPropertyChanged[] variations = new INotifyPropertyChanged[100];

            for (var i = 0; i < 100; ++i)
            {
                var hold = i;
                variations[i] = (INotifyPropertyChanged)trf.Select(v => v + hold);
                (variations[i]).PropertyChanged += handler;
            }

            //Assert.AreEqual(0, frame.PropertyChangedClientsCount, "Expected no change registrations after only 100 times registered.");

            Assert.AreEqual(777, trf.Value, "Expected correct value in clients state.");

            for (var i = 0; i < 100; ++i)
                Assert.AreEqual(777 + i, ((IValueProvider<int>)variations[i]).Value, "Expected correct value from cient.");

            Assert.AreEqual(1, frame.PropertyChangedClientsCount, "Expected 1 change registrations after value retrieved.");

            int getValueCalledCount = 0;

            EventHandler getValueCalledHandler = delegate(object sender, EventArgs args)
            { getValueCalledCount += 1; };

            frame.GetValueCalled += getValueCalledHandler;

            Assert.AreEqual(777, trf.Value, "Expected correct value in clients state.");

            Assert.AreEqual(0, getValueCalledCount, "Value niet meerdere keren ophalen als state = Cached.");

            frame.SetValue("666");

            Assert.AreEqual(666, trf.Value, "Expected correct value after change.");
            Assert.AreEqual(666, trf.Value, "Expected correct value after change.");
            Assert.AreEqual(666, trf.Value, "Expected correct value after change.");

            Assert.AreEqual(1, getValueCalledCount, "Value niet meerdere keren ophalen als state = Cached.");

            for (var i = 0; i < 100; ++i)
                (variations[i]).PropertyChanged -= handler;

            Assert.AreEqual(0, frame.PropertyChangedClientsCount, "Expected 0 change registrations after all registrations removed.");

            Assert.AreEqual(666, trf.Value, "Expected correct value after change.");
            Assert.AreEqual(666, trf.Value, "Expected correct value after change.");

            Assert.AreEqual(3, getValueCalledCount, "Value always retrieved when in inital state.");
        }

        [TestMethod]
        public void HidingTest()
        {
            var frame = new FrameIValueProviderNPC<string>();
            frame.IsReadOnly = true;

            //plain value
            frame.SetValue("777");

            var trf = UnarySelectTransformation<string, int>.Create(frame.Patched(), s => int.Parse(s));

            var changedEventCount = 0;

            PropertyChangedEventHandler handler = delegate(object sender, System.ComponentModel.PropertyChangedEventArgs e) { ++changedEventCount; };

            ((INotifyPropertyChanged)trf.Concrete()).PropertyChanged += handler;

            frame.SetValue("666");

            //Test removed. On adding a property changed handler the enumerator will be probed.
            //Too many clients, even official ones (Silverlight), might first aquire the enumerator and
            //after register for changes.
            //This probing will lead to a change registration on the source.
            //
            //Assert.AreEqual(0, changedEventCount, "Expect no change events when only registered and not read value.");

            Assert.AreEqual(666, trf.Value, "Expected correct result");

            frame.SetValue("555");

            Assert.AreEqual(2, changedEventCount, "Expect change events when registered and read value.");

            frame.SetValue("444");
            frame.SetValue("333");
            frame.SetValue("222");

            Assert.AreEqual(2, changedEventCount, "Expect no more change events after receiving change event and not reading result.");

            Assert.AreEqual(222, trf.Value, "Expected correct result");

            frame.SetValue("666");

            Assert.AreEqual(3, changedEventCount, "Expect one change event again after reading reasult.");

            Assert.AreEqual(666, trf.Value, "Expected correct result");

            frame.SetValue("444");
        
        }

    }
}
