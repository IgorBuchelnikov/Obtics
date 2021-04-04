using System;
using System.Collections.Generic;
//using System.Linq;
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

//use the explicit observable object linq
using Obtics.Collections;


namespace Test
{
    //Simple smoke test program for ObservableLINQ
    //Just runs a numer of test queries.
    //It has 23 different tests, just give the number of each test
    //to run as argument to the application
    //Like: Test.exe e 0 17
    //runs tests 0 and 17
    partial class Program
    {
        /// <summary>
        /// Wrap observable property. Helper to get an IValueProvider on an observable property
        /// of a Person object.
        /// </summary>
        static IValueProvider<string> ObserveProperty(Person person, string propertyName)
        { return ValueProvider.Property<Person, string>(person, propertyName); }

        /// <summary>
        /// Returns an IValueProvider&lt;string&gt; for the FirstName property.
        /// </summary>
        static IValueProvider<string> ObserveFirstName(Person person)
        { return ObserveProperty(person, Person.FirstNamePropertyName); }

        /// <summary>
        /// Returns an IValueProvider&lt;string&gt; for the LastName property.
        /// </summary>
        static IValueProvider<string> ObserveLastName(Person person)
        { return ObserveProperty(person, Person.LastNamePropertyName); }


        static void ExplicitMain(string[] args, ObservableCollection<Person> source, List<EnumerableReporter> reporters)
        {
            if (System.Linq.Enumerable.Contains(args, "1"))
            {
                //Select al FirstNames from the person objects.
                //Note that FirstName is observable and in this 
                //way the result collection also follows changes
                //to the values of FirstNames.
                var q = 
                    from p in source 
                    select ObserveFirstName(p) ;

                reporters.Add(new EnumerableReporter(q, "1 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "2"))
            {
                //Select al Persons where Alphabetically FirstName comes after or is same as LastName
                //FirstName and LastName are observable properties. The comparer Func
                //returns a Value Transformation that updates the result when eiter
                //First or LastName changes value
                var q =
                    from i in source
                    where 
                        ObserveFirstName(i).Select( ObserveLastName(i), (fn, ln) => Comparer<string>.Default.Compare(fn, ln) >= 0 )
                    select i;

                reporters.Add(new EnumerableReporter(q, "2 exp:"));
            }


            if (System.Linq.Enumerable.Contains(args, "3"))
            {
                //Select al Persons where Alphabetically FirstName comes before LastName
                //FirstName and LastName are observable properties. The comparer Func
                //returns a Value Transformation that updates the result when eiter
                //First or LastName changes value
                //Note that this is the reverse of Test 2
                var q =
                    from i in source
                    where
                        ObserveFirstName(i).Select(ObserveLastName(i), (fn, ln) => Comparer<string>.Default.Compare(fn, ln) < 0)
                    select i;

                reporters.Add(new EnumerableReporter(q, "3 exp:"));
            }


            if (System.Linq.Enumerable.Contains(args, "4"))
            {
                //Select al Persons and sort them by LastName (observable)
                var q =
                    from i in source
                    orderby ObserveLastName(i)
                    select i;

                reporters.Add(new EnumerableReporter(q, "4 exp:"));
            }


            if (System.Linq.Enumerable.Contains(args, "5"))
            {
                //Select al Persons and sort them by LastName (observable) 
                //and then by FirstName (observable)
                var q =
                    from i in source
                    orderby ObserveLastName(i), ObserveFirstName(i)
                    select i;

                reporters.Add(new EnumerableReporter(q, "5 exp:"));
            }


            if (System.Linq.Enumerable.Contains(args, "6"))
            {
                //Select all FirstNames (observable) and return the distinct values
                var q =
                    (from j in source select ObserveFirstName(j)).Distinct();

                reporters.Add(new EnumerableReporter(q, "6 exp:"));
            }


            if (System.Linq.Enumerable.Contains(args, "7"))
            {
                var q =
                    from j in
                        from i in source
                        select new CObj { V = i }
                    orderby ObserveFirstName(j.V)
                    select j.V;

                reporters.Add(new EnumerableReporter(q, "7 exp:"));
            }

            //7..8..9

            if (System.Linq.Enumerable.Contains(args, "10"))
            {
                //Inner join Person collection with itself on
                //LastName == FirstName
                //Then order by LastName and FirstName of outer collection.
                //Note it is not necessairy to use  in the inner
                //collection. The observable Join is used because the outer collection
                //is specified as observable.
                //Al properties are looked at in an observable way.
                var q =
                    from i in source
                    join j in source on ObserveLastName(i) equals ObserveFirstName(j)
                    orderby ObserveLastName(i), ObserveFirstName(i)
                    select new PPair<Person, Person>(i, j);

                reporters.Add(new EnumerableReporter(q, "10 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "11"))
            {
                //Group Persons by LastName (observable)
                //then Concat all groups
                //Result should then be a sequence Persons grouped by LastName.
                var q = (
                    from i in source
                    group i by ObserveLastName(i) into g
                    select g
                ).Concat();

                reporters.Add(new EnumerableReporter(q, "11 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "12"))
            {
                //Group Persons by FirstName (observable)
                //Filter out those groups where the number of items in the group is at most 1.
                //Convert the remaining groups into Pairs where the First item is the Key (FirstName)
                //and the second item is the element count of the group. (the number of times
                //this FirstName is present in the original Person collection)
                var q =
                    from i in source
                    group i by ObserveFirstName(i) into g
                    where
                        //g.Count().Convert( gc => gc > 1 )
                        ExpressionObserver.Execute(g,gv => gv.Count().Value > 1)
                        //VT.VPFinder.Observe(() => g.Count().Value > 1)()
                    select
                        ExpressionObserver.Execute(g,gv => new PPair<string, int>(gv.Key, gv.Count().Value));
                        //VT.VPFinder.Observe(() => new PPair<string, int>(g.Key, g.Count().Value))();
                        //(IValueProvider<PPair<string, int>>)(g.Count().Convert(gc => new PPair<string, int>(g.Key, gc)));


                reporters.Add(new EnumerableReporter(q, "12 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "13"))
            {
                //Select the subrange from the 2nd untill the 6th element
                var q = source.Skip(2).Take(4);
                reporters.Add(new EnumerableReporter(q, "13 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "14"))
            {
                //Create a collection consisting of the First and Last elements of the source collection.
                //First() and Last() return an IValueProvider. These are then converted to Collections
                //(of exactly 1 element)
                //Concat those collections and filter out these elements with value == null
                var q1 = source.First().AsEnumerable();
                var q2 = source.Last().AsEnumerable();
                var q = from i in q1.Concat(q2) where i != null select i;
                reporters.Add(new EnumerableReporter(q, "14 exp:"));
            }

            //A 'flat' representation of the Person collection. Each static item of this collection
            //will get updated though whenever a FirstName or LastName of a Person changes value.
            //Note that flat is an IObservableEnumerable so we don't need to use 
            //on it to invoke the Observable LINQ methods.
            var makePair = ExpressionObserver.Compile((Person p) => new PPair<string, string>(p.FirstName, p.LastName));

            var flat =
                from i in source
                select makePair(i);

            if (System.Linq.Enumerable.Contains(args, "15"))
            {
                //Select all Pairs from flat where the swapped version (First <-> Second) is not
                //present in the collection
                var q = flat.Except(from i in flat select new PPair<string, string>(i.Second, i.First));
                reporters.Add(new EnumerableReporter(q, "15 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "16"))
            {
                //Select a union of all elements from 'flat' and there swapped versions (First <-> Second)
                var q = flat.Union(from i in flat select new PPair<string, string>(i.Second, i.First));
                reporters.Add(new EnumerableReporter(q, "16 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "17"))
            {
                //Select all alements from 'flat' where the swapped version (First <-> Second) is also
                //present in 'flat'
                var q = flat.Intersect(from i in flat select new PPair<string, string>(i.Second, i.First));
                reporters.Add(new EnumerableReporter(q, "17 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "18"))
            {

                //var q = from i in flat
                //        from j in flat.SkipWhile(m => !m.Equals(i))
                //        where !i.Equals(j)
                //        where new VT.ConvertTransformation<int, bool>(new string[] { i.First, i.Second, j.First, j.Second }.Distinct().Count(), c => c < 3)
                //        select new PPair<Person, Person>(new Person(i.First, i.Second), new Person(j.First, j.Second));

                //Create a collection of Pairs from flat where each item is coupled with each other item.. ONCE
                //of this collection Filter IN only those pairs where the total number of DIFFERENT property values
                //is less than 3
                var q =
                    from p in
                        flat.Select(
                            (i, ix) =>
                                from j in flat.Skip(ix + 1)
                                select new PPair<PPair<string, string>, PPair<string, string>>(i, j)
                        ).Concat()
                    where new string[] { p.First.First, p.First.Second, p.Second.First, p.Second.Second }.Distinct().Count().Value < 3
                    select p;


                reporters.Add(new EnumerableReporter(q, "18 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "19"))
            {
                //GroupJoin source with itself 
                //In each group create new Persons with the FirstName of the outer item 
                //and the LastName of the inner item (observable)
                //Concat al the collections of new Persons
                var f = ExpressionObserver.Compile((Person p1, Person p2) => new Person(p1.FirstName, p2.LastName));

                var q = (
                    from i in source
                    join j in source on ObserveLastName(i) equals ObserveFirstName(j) into s
                    select (
                        from k in s
                        select f(i,k)
                    )
                ).Concat();

                reporters.Add(new EnumerableReporter(q, "19 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "20"))
            {
                //Do same thing as in Test 19 but use 'flat' as Source.
                //create Persons as last 
                var q = (
                    from i in flat
                    join j in flat on i.Second equals j.First into s
                    select (
                        from k in s
                        select new Person(i.First, k.Second)
                    )
                ).Concat() ;

                reporters.Add(new EnumerableReporter(q, "20 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "21"))
            {
                //Create one collection of all First and LastNames (observable)
                //Group them by Name and order these Groups by ItemCount (highest first)
                //Convert the Groups to pairs consisting of the GroupKey (Name) and the ItemCounts. 
                var firstNames = from i in source select ObserveFirstName(i);
                var lastNames = from i in source select ObserveLastName(i);

                var q = from c in firstNames.Concat(lastNames)
                        group c by c into cs
                        orderby cs.Count() descending
                        select cs.Count().Select(v => new PPair<string, int>(cs.Key, v)) ;

                reporters.Add(new EnumerableReporter(q, "21 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "22"))
            {
                //Create a union (distinct items) of Pairs consisting of either First or Second value and the Pair itself.
                //Create a lookup out of these Pair-Pairs using the First value as the lookup key and Second value (the original Pair)
                //as the lookup value.
                //Do a lookup for all Pairs where either the First or Second value is "A";
                var firstNames = from i in flat select new PPair<string, PPair<string, string>>(i.First, i);
                var lastNames = from i in flat select new PPair<string, PPair<string, string>>(i.Second, i);

                var q = firstNames.Union(lastNames).ToLookup(p => p.First, p => p.Second)["A"] ;

                reporters.Add(new EnumerableReporter(q, "22 exp:"));
            }

            if (System.Linq.Enumerable.Contains(args, "23"))
            {
                //var exp = Obtics.Values.Transformations.VPFinder.ObservableExpression<Person, string>((p) => Obtics.Values.Transformations.VPFinder.FuncImp1(p, pa => pa.FirstName + "+" + p.LastName));
                //var exp = VT.VPFinder.ObservableExpression<Person, string>((p) => Obtics.Values.Transformations.VPFinder.FuncImp1(p, pa => new DynamicValueProvider<Person>( pa )).Value.FirstName + "+" + p.LastName);
                //var exp = VT.VPFinder.ObservableExpression<Person, string>(p => new Person[] { p }.First().Value.FirstName + "+" + p.LastName);
                //var exp = VT.VPFinder.ObservableExpression<Person, string>(p => p.FirstName + "+" + p.LastName);
                //Console.Out.WriteLine(exp);
                var func = ExpressionObserver.Compile((Person p) => p.FirstName + "+" + p.LastName);

                var q = from p in source select func(p);
                reporters.Add(new EnumerableReporter(q, "23 exp:"));
            }
        }
    }
}
