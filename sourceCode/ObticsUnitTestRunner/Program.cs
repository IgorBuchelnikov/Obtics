using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TvdP.UnitTesting;
using System.Text.RegularExpressions;

namespace ObticsUnitTestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var match = args.Length > 0 ? new Regex(args[0]) : null ;

            var assembly = System.Reflection.Assembly.GetAssembly(typeof(ObticsUnitTest.Obtics.Regression));

            foreach (
                var testClass in assembly.GetExportedTypes().Where(type => type.GetCustomAttributes(typeof(TestClassAttribute), false).Length != 0)
            )
            {
                var testMethods = 
                    testClass
                        .GetMethods()
                        .Where(
                            m => 
                                m.GetCustomAttributes(typeof(TestMethodAttribute), false).Length != 0
                                && (match == null || match.IsMatch(testClass.FullName + "." + m.Name))
                        ).ToList();

                if (testMethods.Count != 0)
                {
                    var testInstance = testClass.GetConstructor(Type.EmptyTypes).Invoke(null);

                    //Initialize
                    foreach (
                        var classInitializer in
                            testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(ClassInitializeAttribute), false).Length != 0)
                    )
                        try
                        {
                            classInitializer.Invoke(testInstance, null);
                        }
                        catch (System.Reflection.TargetInvocationException tie)
                        {
                            var ex = tie.InnerException;

                            if (ex != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Class initialization exception: class ");
                                Console.Write(testInstance.GetType().FullName);
                                Console.Write(": exception ");
                                Console.Write(ex.GetType().FullName);
                                Console.Write(": message ");
                                Console.Write(ex.Message);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Enter to continue..");
                                Console.ReadLine();
                                Console.ResetColor();

                                goto resumeClasses;
                            }
                            else
                                throw;                            
                        }

                        


                    foreach (var testMethod in testMethods)
                    {
                        //Initialize
                        foreach (
                            var testInitializer in
                                testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestInitializeAttribute), false).Length != 0)
                        )
                            try
                            {
                                testInitializer.Invoke(testInstance, null);
                            }
                            catch (System.Reflection.TargetInvocationException tie)
                            {
                                var ex = tie.InnerException;

                                if (ex != null)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("Test initialization exception: test ");
                                    Console.Write(testInstance.GetType().FullName + "." + testMethod.Name);
                                    Console.Write(": exception ");
                                    Console.Write(ex.GetType().FullName);
                                    Console.Write(": message ");
                                    Console.Write(ex.Message);
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Enter to continue..");
                                    Console.ReadLine();
                                    Console.ResetColor();

                                    goto resumeClasses;
                                }
                                else
                                    throw;
                            }
                            

                        //Execute test
                        Console.Write(testClass.FullName + "." + testMethod.Name + ":");

                        try
                        {
                            testMethod.Invoke(testInstance, null);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Passed");
                            Console.ResetColor();
                        }
                        catch (System.Reflection.TargetInvocationException tie)
                        {
                            var afe = tie.InnerException as AssertFailedException;

                            if (afe != null)
                            {
                                Console.Write(testClass.FullName + "." + testMethod.Name + ":");
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(afe.Message);
                                Console.CursorTop += 1;

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Enter to continue..");
                                Console.ReadLine();
                                Console.ResetColor();
                            }
                            else
                            {
                                var ex = tie.InnerException;

                                if (ex != null)
                                {
                                    bool expected = false;

                                    foreach (
                                        ExpectedExceptionAttribute eea in
                                        testMethod.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false)
                                    )
                                        if (eea.ExceptionType == ex.GetType() && (String.IsNullOrEmpty(eea.Message) || eea.Message == ex.Message))
                                        {
                                            expected = true;
                                            break;
                                        }

                                    if (expected)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write("Passed");
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write("Exception:");
                                        Console.Write(ex.GetType().FullName);
                                        Console.Write(":");
                                        Console.Write(ex.Message);

                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine("Enter to continue..");
                                        Console.ReadLine();
                                        Console.ResetColor();
                                    }
                                }
                                else
                                    throw;
                            }
                            
                        }


                        Console.WriteLine();

                        //Cleanup
                        foreach (
                            var testCleanup in
                                testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestCleanupAttribute), false).Length != 0)
                        )
                            try
                            {
                                testCleanup.Invoke(testInstance, null);
                            }
                            catch (System.Reflection.TargetInvocationException tie)
                            {
                                var ex = tie.InnerException;

                                if (ex != null)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("Test cleanup exception: test ");
                                    Console.Write(testInstance.GetType().FullName + "." + testMethod.Name);
                                    Console.Write(": exception ");
                                    Console.Write(ex.GetType().FullName);
                                    Console.Write(": message ");
                                    Console.Write(ex.Message);
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Enter to continue..");
                                    Console.ReadLine();
                                    Console.ResetColor();

                                    goto resumeClasses;
                                }
                                else
                                    throw;
                            }

                            

                        if(Console.KeyAvailable)
                        {
                            Console.ReadKey();

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("Paused..");

                            while (!Console.KeyAvailable)
                            {
                                System.Threading.Thread.Sleep(0);
                            }

                            if (Char.ToLower(Console.ReadKey().KeyChar) == 's')
                            {
                                Console.WriteLine("Stopped");
                                Console.ResetColor();
                                goto exit;
                            }

                            Console.WriteLine("Resumed");
                            Console.ResetColor();

                            //Console.ReadKey();
                        }
                    }                    

                    //Cleanup
                    foreach (
                        var classCleaner in
                            testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(ClassCleanupAttribute), false).Length != 0)
                    )
                        try
                        {
                            classCleaner.Invoke(testInstance, null); 
                        }
                        catch (System.Reflection.TargetInvocationException tie)
                        {
                            var ex = tie.InnerException;

                            if (ex != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Class cleanup exception: class ");
                                Console.Write(testInstance.GetType().FullName);
                                Console.Write(": exception ");
                                Console.Write(ex.GetType().FullName);
                                Console.Write(": message ");
                                Console.Write(ex.Message);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Enter to continue..");
                                Console.ReadLine();
                                Console.ResetColor();

                                goto resumeClasses;
                            }
                            else
                                throw;
                        }                                       
                }

            resumeClasses: ;
            }

           exit:

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Done..");
            Console.ResetColor();

            Console.ReadLine();
        }
    }
}
