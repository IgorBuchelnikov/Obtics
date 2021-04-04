using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Values;

namespace CustomMapping
{
    using System.Linq;

    public static partial class MyEnumerable
    {
        struct Pair<T, K> : IComparable<Pair<T, K>>
        {
            public T _T;
            public K _K;

            #region IComparable<Pair<T,K>> Members

            public int CompareTo(Pair<T, K> other)
            { return Comparer<K>.Default.Compare(_K, other._K); }

            #endregion
        }

        //WhithMax returns the element of the sequence that has a max value of something
        //An alternative approach would be to sort the sequence by that something and return 
        //the first item. It would be a waste though to sort an entire sequence just to get
        //the top element.
        //
        // seq.WithMax( p => p.Birthdate )
        //
        // could also be written as
        //
        // seq.OrderByDescending( p => p.Birthdate ).First() //wasteful! O(sort) = n log n where O(max) = n
        //
        // Use ExpressionObserverMapping to register a mapping from the non-observable variant
        // to the observable variants. ExpressionObserver recognizes this attribute and uses
        // the mapping to create live expressions.
        [ExpressionObserverMapping(typeof(MyEnumerable.Observable))]
        public static T ElementWithMax<T, K>(this IEnumerable<T> sequence, Func<T, K> keySelector)
        { return sequence.Select(t => new Pair<T, K> { _T = t, _K = keySelector(t) }).Max()._T; }

        //Not as generic and generaly usefull as ElementWithMax but for example:
        [ExpressionObserverMapping(typeof(MyEnumerable.Observable))]
        public static IEnumerable<Person> StartingWith(this IEnumerable<Person> sequence, string c)
        { return sequence.Where(p => p.Name.StartsWith(c)); }


        public static partial class Observable
        {
            //target methods for StartingWith. Note that return types are same as source method.
            //the returned instances though will be implicitly observable by implementing INotifyCollectionChanged. 
            public static IEnumerable<Person> StartingWith(IEnumerable<Person> sequence, string c)
            { return _StartingWith1F(sequence, c).Cascade(); }


            public static IEnumerable<Person> StartingWith(IEnumerable<Person> sequence, IValueProvider<string> c)
            { return c.Select(cv => _StartingWith1F(sequence, cv)).Cascade(); }

            static Func<IEnumerable<Person>, string, IValueProvider<IEnumerable<Person>>> _StartingWith1F =
                ExpressionObserver.Compile(
                    (IEnumerable<Person> sequence, string c) =>
                        sequence.Where(p => p.Name.StartsWith(c, StringComparison.InvariantCultureIgnoreCase))
                );
        }
    }
}

namespace CustomMapping
{
    using Obtics.Collections;

    public static partial class MyEnumerable
    {
        //partialy defined here so we can use convenient syntax with Obtics.Collections replacing System.Linq
        public static partial class Observable
        {
            //Target methods are public static and have same name as the source method.
         
            //paramters can be mapped like: TPrm p => IValueProvider<TPrm> p
            //or: Func<..,TResult> => Func<..,IValueProvider<TResult>>
            //
            //Note that if a lambda parameter gets successfuly mapped it's body, including the lambda's arguments
            //will be completely analysed for observable entities.
            //
            //The return type can be either the same as or derived of the type of the source member, or an IValueProvider of that type.
            //All target methods must return the same result type. 

            //This variation has reduced reactivity but gains in performance (fewer things to track).
            //this version will be chosen by the ExpressionObserver when it can't create a non-trivial
            //observable lambda parameter (keySelector)
            public static IValueProvider<T> ElementWithMax<T, K>(IEnumerable<T> sequence, Func<T, K> keySelector)
            { return sequence.Select(keySelector, (t,ks) => new Pair<T, K> { _T = t, _K = ks(t) }).Max().Select(p => p._T); }

            //This variation is the most reactive at the expense of performance. IT will be chosen when the
            //ExpressionObserver can create a non-trivialy observable lambda parameter (keySelector).
            //
            //This is the fallback mapping. It's mappeable parameters covers the mappable parameters of all other
            //target methods (trivial; only one other target method with 0 mappable parameters). There must always 
            //be one such target method in the given set of target methods.
            public static IValueProvider<T> ElementWithMax<T, K>(IEnumerable<T> sequence, Func<T, IValueProvider<K>> keySelector)
            { return sequence.Select(keySelector, (t,ks) => ks(t).Select( ValueProvider.Static(t), (k, t2) => new Pair<T, K> { _T = t2, _K = k })).Max().Select(p => p._T); }
        }
    }
}

