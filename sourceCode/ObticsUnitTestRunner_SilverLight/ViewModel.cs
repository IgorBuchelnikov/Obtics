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
using System.Collections.Generic;
using System.Linq;
using Obtics.Values;
using Obtics;
using System.Text.RegularExpressions;
using TvdP.Collections;

namespace ObticsUnitTestRunner_SilverLight
{
    public sealed class ViewModel
    {
        public ViewModel(TestRunContext ctxt, Assembly assmbly)
        {
            _Context = ctxt;
            _Assembly = assmbly;
        }

        #region Context ; The 'model' we work on
        TestRunContext _Context;

        public TestRunContext Context { get { return _Context; } }
        #endregion

        #region Assembly ; The assembly with test classes and methods we work on
        Assembly _Assembly;

        public Assembly Assembly { get { return _Assembly; } }
        #endregion

        #region FilterExpression ; the Regular expression (as string) we filter the test methods with
        IValueProvider<string> _FilterExpression = ValueProvider.Dynamic<string>("^.*(?<!ConcurrencyTest.?)$");

        public IValueProvider<string> FilterExpression { get { return _FilterExpression; } }
        #endregion

        #region FilterRegex ; Regex form of the FilterExpression
        static KeyValuePair<Regex,bool> ToRegex(string expression)
        {
            try
            {
                return new KeyValuePair<Regex,bool>( new Regex(expression), true);
            }
            catch
            {
                return new KeyValuePair<Regex,bool>( new Regex(".*"), false);
            }
        }

        //use Async here to prevent repeated interpreting of the regex string. (Async buffers the result)
        IValueProvider<KeyValuePair<Regex,bool>> FilterRegex
        { get { return FilterExpression.Select(exp => ToRegex(exp)).Async(); } }
        #endregion

        #region GetTestMethodSelectedState ; set/unset to select or deselect a test method.
        WeakKeyDictionary<MethodInfo, bool, IValueProvider<bool>> _TestMethodSelectedState = new WeakKeyDictionary<MethodInfo, bool, IValueProvider<bool>>();

        public IValueProvider<bool> GetTestMethodSelectedState(MethodInfo method)
        {
            return
                _TestMethodSelectedState.GetOrAdd(
                    method,
                    false,
                    (_, __) => ValueProvider.Dynamic(false)
                )
            ;
        }
        #endregion

        #region VisibleMethods ; test methods to display in grid (as TestMethodVM)
        public IEnumerable<TestMethodVM> VisibleMethods
        { get { return Obtics.Values.ValueProvider.Cascade(_VisibleMethodsF(this)); }  }

        static Func<ViewModel, Obtics.Values.IValueProvider<IEnumerable<TestMethodVM>>> _VisibleMethodsF =
            Obtics.Values.ExpressionObserver.Compile(
                (ViewModel t) =>
                    TestRunContext.GetTestMethods(t.Assembly)
                    .Select(mInfo => TestMethodVM.Create(mInfo, t))
                    .Where(mvm => mvm != null && t.FilterRegex.Value.Key.IsMatch(mvm.FullName))
            );
        #endregion

        #region CurrentlyRunningMethod ; as TestMethodVM
        //Top off with Concrete to allow binding from Silverlight XAML
        public IValueProvider<TestMethodVM> CurrentlyRunningMethod
        { 
            get 
            { 
                return 
                    Context
                        .CurrentlyExecutingTestMethod
                        .Select(m => TestMethodVM.Create(m, this))
                        .Async(); 
            }
        }
        #endregion

        #region AllSelected ; boolean set/unset to select/deselect all visible test methods.
        public IValueProvider<bool> AllSelected
        { 
            get 
            {
                return _AllSelectedF(this).ReturnPath(
                    v =>
                    {
                        foreach (var vm in VisibleMethods)
                            vm.Selected.Value = v;
                    }
                ); 
            } 
        } 

        static Func<ViewModel, IValueProvider<bool>> _AllSelectedF =
            ExpressionObserver.Compile(
                (ViewModel t) =>
                    t.VisibleMethods.All(vm => vm.Selected.Value)
            );
        #endregion

        //Progress on the current run
        public IValueProvider<double> Progress { get { return Context.Progress; } }

        //true/false if run button needs to be enabled/disabled
        public IValueProvider<bool> RunButtonEnabled { get { return Context.Running.Invert(); } }

        //true/false if cancel button needs to be enabled/disabled
        public IValueProvider<bool> CancelButtonEnabled { get { return Context.Running; } }

        //Visibility of progress panel
        public IValueProvider<Visibility> ProgressPanelVisibility { get { return Context.Running.Select(r => r ? Visibility.Visible : Visibility.Collapsed); } }

        //Total time taken by current run.
        public IValueProvider<TimeSpan> RunTime { get { return Context.WorkTime; } }

        #region PassedCount ; total number of visible test methods that have passed during their last run.
        public IValueProvider<int> PassedCount
        { get { return _PassedCountF(this).Async(); } }

        static Func<ViewModel, IValueProvider<int>> _PassedCountF =
            ExpressionObserver.Compile(
                (ViewModel t) =>
                    t.VisibleMethods
                        .Where(m => t.Context.GetTestMethodResult(m.TestMethod).Value.Type == TestResultType.Success)
                        .Count()
            );
        #endregion

        #region FailedCount ; total number of visible test methods that have failed during their last run.
        public IValueProvider<int> FailedCount
        { get { return _FailedCountF(this).Async(); } }

        static TestResultType[] _FailedTypes = new TestResultType[] { 
            TestResultType.Failed, 
            TestResultType.FailedCleanup, 
            TestResultType.FailedExpectedException, 
            TestResultType.FailedInitialization, 
            TestResultType.FailedTestClassInstantiation 
        };

        static Func<ViewModel, IValueProvider<int>> _FailedCountF =
            ExpressionObserver.Compile(
                (ViewModel t) =>
                    t.VisibleMethods
                        .Where(m => Array.IndexOf(_FailedTypes, t.Context.GetTestMethodResult(m.TestMethod).Value.Type) != -1 )
                        .Count()
            );
        #endregion

        #region Run method; call to start a run with the currently selected visible test methods
        internal void Run()
        {
            var methods = from mvm in VisibleMethods where mvm.Selected.Value select mvm.TestMethod;
            Context.Run(methods);
        }
        #endregion

        #region Cancal method; call to cancel a currently running test run
        internal void Cancel()
        {
            Context.CancelRun();
        }
        #endregion
    }
}
