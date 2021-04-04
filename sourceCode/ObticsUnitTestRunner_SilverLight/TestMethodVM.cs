using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using Obtics.Values;
using Obtics;
using System.Collections.Generic;
using System.Linq;

namespace ObticsUnitTestRunner_SilverLight
{
    /// <summary>
    /// View model of test method
    /// </summary>
    public sealed class TestMethodVM : IEquatable<TestMethodVM>
    {
        TestMethodVM(MethodInfo testMethod, ViewModel vm)
        { _TestMethod = testMethod; _VM = vm; }

        static public TestMethodVM Create(MethodInfo testMethod, ViewModel vm)
        {
            if (testMethod == null || vm == null)
                return null;

            return new TestMethodVM(testMethod, vm);
        }

        #region TestMethod ; the test method encapsulated by this TestMethodVM
        MethodInfo _TestMethod;

        /// <summary>
        /// the test method encapsulated by this TestMethodVM
        /// </summary>
        public MethodInfo TestMethod { get { return _TestMethod; } }
        #endregion

        #region VM ; our parent ViewModel object
        ViewModel _VM;

        /// <summary>
        /// our parent ViewModel object
        /// </summary>
        public ViewModel VM { get{ return _VM; } }
        #endregion

        /// <summary>
        /// name of the test method
        /// </summary>
        public string Name { get { return _TestMethod.Name; } }

        /// <summary>
        /// full name of the test method
        /// </summary>
        public string FullName { get { return _TestMethod.DeclaringType.FullName + "." + Name; } }

        #region Result ; result of the last run of our test method (as string)
        /// <summary>
        /// result of the last run of our test method (as string)
        /// </summary>
        public IValueProvider<string> Result
        { get { return _ResultF(this).Async(); } }

        static string[] _ResultStrings = {
             "",
             "Passed",
             "Undetermined",
             "Instantiation failed",
             "Initialization failed",
             "Cleanup failed",
             "Failed",
             "Expected exception"
         };
      
        static Func<TestMethodVM, IValueProvider<string>> _ResultF =
            ExpressionObserver.Compile(
                (TestMethodVM t) =>
                    t.VM.Context.CurrentlyExecutingTestMethod.Value == t.TestMethod ? "Running ..." :
                    _ResultStrings[(int)t.VM.Context.GetTestMethodResult(t.TestMethod).Value.Type]
            );
        #endregion

        #region Message ; message of the last run of our test method

        /// <summary>
        /// message of the last run of our test method
        /// </summary>
        public IValueProvider<object> Message
        { get { return _MessageF(this).Async(); } }

        static Func<TestMethodVM, IValueProvider<object>> _MessageF =
            ExpressionObserver.Compile(
                (TestMethodVM t) =>
                    t.VM.Context.CurrentlyExecutingTestMethod.Value == t.TestMethod ? null :
                    t.VM.Context.GetTestMethodResult(t.TestMethod).Value.Message
            );
        #endregion

        /// <summary>
        /// If our method is selected or not.
        /// </summary>
        public IValueProvider<bool> Selected { get { return _VM.GetTestMethodSelectedState(_TestMethod); } }

        #region IEquatable<TestMethodVM> Members

        public bool Equals(TestMethodVM other)
        { return other != null && _TestMethod == other._TestMethod && _VM == other._VM; }

        public override bool Equals(object obj)
        { return Equals(obj as TestMethodVM); }

        public override int GetHashCode()
        { return 2093480293 ^ _TestMethod.GetHashCode() ^ _VM.GetHashCode(); }

        #endregion
    }
}
