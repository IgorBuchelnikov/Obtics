using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TvdP.Collections;
using System.Reflection;
using Obtics.Values;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using TvdP.UnitTesting;

namespace ObticsUnitTestRunner_SilverLight
{
    public enum TestResultType
    {
        None = 0,
        Success = 1,
        Undetermined = 2,
        FailedTestClassInstantiation = 3,
        FailedInitialization = 4,
        FailedCleanup = 5,
        Failed = 6,
        FailedExpectedException = 7
    }

    public struct TestResult
    {
        public TestResultType Type;
        public object Message;
    }

    public sealed class TestRunContext
    {
        #region GetTestMethodResult ; gets the last test run result for the given method
        WeakKeyDictionary<MethodInfo, bool, IValueProvider<TestResult>> _TestMethodResultAssociation = new WeakKeyDictionary<MethodInfo, bool, IValueProvider<TestResult>>();

        /// <summary>
        /// gets the last test run result for the given method
        /// </summary>
        /// <param name="testMethod"></param>
        /// <returns></returns>
        public IValueProvider<TestResult> GetTestMethodResult(MethodInfo testMethod)
        {
            return
                _TestMethodResultAssociation.GetOrAdd(
                    testMethod,
                    false,
                    (_, __) => ValueProvider.Dynamic<TestResult>()
                )
            ;
        }
        #endregion

        IEnumerable<Type> GetTestMethodExpectedExceptions(MethodInfo testMethod)
        {
            return 
                testMethod
                    .GetCustomAttributes(typeof(ExpectedExceptionAttribute), false)
                    .OfType<ExpectedExceptionAttribute>()
                    .Select(eea => eea.ExceptionType);
        }

        bool GetTestMethodRequiresUIThread(MethodInfo testMethod)
        {
            return 
                testMethod
                    .GetCustomAttributes(typeof(UIThreadAttribute), false)
                    .Any();
        }

        static void RunTestInitializers(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            foreach (
                var classInitializer in
                    instance.GetType().GetMethods().Where(m => m.GetCustomAttributes(typeof(TestInitializeAttribute), false).Length != 0)
            )
                classInitializer.Invoke(instance, null);
        }

        static void RunTestCleaners(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            foreach (
                var testCleanup in
                    instance.GetType().GetMethods().Where(m => m.GetCustomAttributes(typeof(TestCleanupAttribute), false).Length != 0)
            )
                testCleanup.Invoke(instance, null);
        }

        /// <summary>
        /// gets the total list of test methods for the given assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns> 
        public static IEnumerable<MethodInfo> GetTestMethods(Assembly assembly)
        { 
            return 
                assembly
                    .GetExportedTypes()
                    .Where(type => type.GetCustomAttributes(typeof(TestClassAttribute), false).Length != 0)
                    .SelectMany(type => type.GetMethods())
                    .Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), false).Length != 0);
        }

        #region WorkTime ; The continualy updated ammount time the current run took.
        IValueProvider<TimeSpan> _WorkTime = ValueProvider.Dynamic(TimeSpan.Zero);

        /// <summary>
        /// The continualy updated ammount time the current run took.
        /// </summary>
        public IValueProvider<TimeSpan> WorkTime { get { return _WorkTime.Async(); } }
        #endregion

        #region Progress ; The factor of completion 0 -> 1 of the current run
        IValueProvider<double> _Progress = ValueProvider.Dynamic(0.0);

        /// <summary>
        /// The factor of completion 0 -> 1 of the current run
        /// </summary>
        public IValueProvider<double> Progress { get { return _Progress.Async(); } }
        #endregion

        #region Running ; If there is a run running
        IValueProvider<bool> _Running = ValueProvider.Dynamic(false);

        /// <summary>
        /// If there is a run running. Can't start a new run when there is.
        /// </summary>
        public IValueProvider<bool> Running { get { return _Running.Async(); } }
        #endregion

        #region Cancel ; Set to true to cancel a current Run.
        IValueProvider<bool> _Cancel = ValueProvider.Dynamic(false);

        /// <summary>
        /// Set true to cancel a current Run.
        /// </summary>
        public IValueProvider<bool> Cancel { get { return _Cancel.Async(); } }
        #endregion

        #region CurrentlyExecutingTestMethod ; 
        IValueProvider<MethodInfo> _CurrentlyExecutingTestMethod = ValueProvider.Dynamic<MethodInfo>();

        /// <summary>
        /// The method currently executing.
        /// </summary>
        public IValueProvider<MethodInfo> CurrentlyExecutingTestMethod { get { return _CurrentlyExecutingTestMethod.Async(); } }
        #endregion

        /// <summary>
        /// Start a new test run.
        /// </summary>
        /// <param name="methods"></param>
        public void Run(IEnumerable<MethodInfo> methods)
        {
            if (!Running.Value)
            {
                //Set Running and Clear cancel flag.
                Cancel.Value = false;
                Running.Value = true;

                System.Threading.ThreadPool.QueueUserWorkItem(
                    new System.Threading.WaitCallback(Run_BackgroundProcess),
                    methods.ToList()
                );
            }
        }

        private void Run_BackgroundProcess(object testMethodsObject)
        {
            try
            {
                var testMethods = (List<MethodInfo>)testMethodsObject;
                var currentIndex = 0;
                Progress.Value = 0.0;
                WorkTime.Value = TimeSpan.Zero;
                DateTime startTime = DateTime.Now;

                foreach (var testMethod in testMethods)
                {
                    if (Cancel.Value)
                        break;

                    var testClass = testMethod.DeclaringType;
                    var testResult = GetTestMethodResult(testMethod);
                    CurrentlyExecutingTestMethod.Value = testMethod;

                    object testInstance = null;

                    try
                    {
                        testInstance = testClass.GetConstructor(Type.EmptyTypes).Invoke(null);
                    }
                    catch (Exception ex)
                    {
                        testResult.Value = new TestResult() { Type = TestResultType.FailedTestClassInstantiation, Message = ex };
                        continue;
                    }

                    try
                    {
                        RunTestInitializers(testInstance);
                    }
                    catch (Exception ex)
                    {
                        testResult.Value = new TestResult() { Type = TestResultType.FailedInitialization, Message = ex };
                        continue;
                    }

                    try
                    {
                        //Switch to UI thread if needed
                        if (GetTestMethodRequiresUIThread(testMethod) && !Deployment.Current.Dispatcher.CheckAccess())
                            new System.Windows.Threading.DispatcherSynchronizationContext(Deployment.Current.Dispatcher).Send(delegate(object state) { TestMethodInvoke(testMethod, testResult, testInstance); }, null);
                        else
                            TestMethodInvoke(testMethod, testResult, testInstance);
                    }
                    finally
                    {
                        try
                        {
                            RunTestCleaners(testInstance);
                        }
                        catch (Exception ex)
                        {
                            testResult.Value = new TestResult() { Type = TestResultType.FailedCleanup, Message = ex };
                        }
                    }

                    Progress.Value = (double)(++currentIndex) / (double)testMethods.Count;
                    WorkTime.Value = DateTime.Now - startTime;
                }

                CurrentlyExecutingTestMethod.Value = null;
                Progress.Value = 1.0;
                WorkTime.Value = DateTime.Now - startTime;
            }
            finally
            {
                //When done clear Running and Cancel flags
                Running.Value = false;
                Cancel.Value = false;
            }

        }

        private void TestMethodInvoke(MethodInfo testMethod, IValueProvider<TestResult> testResult, object testInstance)
        {
            try
            {
                testMethod.Invoke(testInstance, null);

                if (GetTestMethodExpectedExceptions(testMethod).Count() > 0)
                    testResult.Value = new TestResult() { Type = TestResultType.FailedExpectedException, Message = "Expected exception." };
                else
                    testResult.Value = new TestResult() { Type = TestResultType.Success, Message = null };
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                var afe = tie.InnerException as AssertFailedException;

                if (afe != null)
                    testResult.Value = new TestResult() { Type = TestResultType.Failed, Message = afe };
                else
                {
                    var ex = tie.InnerException;

                    if (ex != null)
                    {
                        if (GetTestMethodExpectedExceptions(testMethod).FirstOrDefault(expectedType => expectedType == ex.GetType()) != null)
                            testResult.Value = new TestResult() { Type = TestResultType.Success, Message = null };
                        else
                            testResult.Value = new TestResult() { Type = TestResultType.Failed, Message = ex };
                    }
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Cancel a current test run.
        /// </summary>
        public void CancelRun()
        {
            if (Running.Value)
                Cancel.Value = true;                
        }
    }
}
