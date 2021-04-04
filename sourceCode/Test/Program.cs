using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics;
using Obtics.Collections;
using Obtics.Values;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using CT = Obtics.Collections.Transformations;
using System.ComponentModel;
using VT = Obtics.Values.Transformations;

namespace Test
{
    //Simple smoke test program for ObservableLINQ
    //Just runs a numer of test queries.
    //It has 23 different tests, just give the number of each test
    //to run as argument to the application
    //The tests are implemented using either explicit or implicit observable object linq
    //Like: Test.exe i e 0 17
    //runs tests 0 and 17
    partial class Program
    {
        static void Main(string[] args)
        {
            var source = new ObservableCollection<Person>();

            List<EnumerableReporter> reporters = new List<EnumerableReporter>();

            if (System.Linq.Enumerable.Contains(args, "0"))
            {
                //Just follow source directly
                var reporter0 = new EnumerableReporter(source, "0:");
            }

            ExplicitMain(args, source, reporters);


            //Now start manipulating source. 
            //All the Transformations should follow these changes.
            Console.Out.WriteLine("------------ add [A:B] ----------------------");
            source.Add(new Person("A", "B"));
            Console.Out.WriteLine("------------ add [A:C] ----------------------");
            source.Add(new Person("A", "C"));
            Console.Out.WriteLine("------------ add [D:A] ----------------------");
            source.Add(new Person("D", "A"));
            Console.Out.WriteLine("------------ add [B:A] ----------------------");
            source.Add(new Person("B", "A"));
            Console.Out.WriteLine("------------ add [D:C] ----------------------");
            source.Add(new Person("D", "C"));
            Console.Out.WriteLine("------------ add [C:A] ----------------------");
            source.Add(new Person("C", "A"));
            Console.Out.WriteLine("------------ insert [C:C] at 5 --------------");
            source.Insert(5, new Person("C", "C"));
            Console.Out.WriteLine("------------ insert [C:D] at 3 --------------");
            source.Insert(3, new Person("C", "D"));
            Console.Out.WriteLine("------------ move [D:A] 2 -> 0 --------------");
            source.Move(2, 0);
            Console.Out.WriteLine("------------ move [D:A] 0 -> 5 --------------");
            source.Move(0, 5);
            Console.Out.WriteLine("------------ move [D:C] 4 -> 7 --------------");
            source.Move(4, 7);
            Console.Out.WriteLine("------------ replace 0 [A:B] with [D:B] -----");
            source[0] = new Person("D", "B");
            Console.Out.WriteLine("------------ replace 5 [C:C] with [A:A] -----");
            source[5] = new Person("A", "A");
            Console.Out.WriteLine("------------ replace 7 [D:C] with [B:A] -----");
            source[7] = new Person("B", "A");
            Console.Out.WriteLine("----------------------------");
            Console.Out.WriteLine("--> ValueProvider changes");
            Console.Out.WriteLine("");
            Console.Out.WriteLine("------------- change 0 [D:B->C] -------------");
            source[0].LastName = "C";
            Console.Out.WriteLine("------------- change 1 [A:C->D] -------------");
            source[1].LastName = "D";
            Console.Out.WriteLine("------------- change 5 [A->A:A]--------------");
            source[5].FirstName = "A";
            Console.Out.WriteLine("------------- change 3 [B->A:A] -------------");
            source[3].FirstName = "A";
            Console.Out.WriteLine("------------- change 2 [C->B:D] -------------");
            source[2].FirstName = "B";
            Console.Out.WriteLine("");
            Console.Out.WriteLine("<-- ValueProvider changes");
            Console.Out.WriteLine("------------- remove 7 [B:A] ----------------");
            source.RemoveAt(source.Count - 1);
            Console.Out.WriteLine("------------- remove 0 [D:C] ----------------");
            source.RemoveAt(0);
            Console.Out.WriteLine("------------- remove 4 [A:A] ----------------");
            source.RemoveAt(4);
            Console.Out.WriteLine("------------- remove 2 [A:A] ----------------");
            source.RemoveAt(2);
            Console.Out.WriteLine("---------------------------------------------");

            Console.In.ReadLine();
        }
    }
}
