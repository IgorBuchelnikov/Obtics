
using System.Collections.Generic;
using System.Collections;
using Obtics.Collections.Transformations;
using System.Collections.Specialized;
using TvdP.Collections;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Obtics.Collections
{
    //Explicitly observable object linq

    /// <summary>
    /// Provides a set of static methods for reactively querying objects that implement <see cref="System.Collections.Generic.IEnumerable{TElement}"/>.
    /// In most cases the results of these methods are observable.
    /// </summary>
    /// <remarks>
    /// <para>The methods in this class provide an implementation of the standard query operators for querying data sources that implement <see cref="System.Collections.Generic.IEnumerable{TElement}"/>. The standard query operators are general purpose methods that follow the LINQ pattern and enable you to express traversal, filter, and projection operations over data in any .NET-based programming language.</para>
    /// <para>The majority of the methods in this class are defined as extension methods that extend <see cref="System.Collections.Generic.IEnumerable{TElement}"/>. This means they can be called like an instance method on any object that implements <see cref="System.Collections.Generic.IEnumerable{TElement}"/>.</para>
    /// <para>Methods that are used in a query that returns a sequence of values do not consume the target data until the query object is enumerated. This is known as deferred execution.</para>
    /// <para>Most methods can take either a lambda function that returns an explicit result or a lambda function that returns a dynamic value provider of the result.
    /// In the first case the result of the method is static in regard to the result of the lambda and in the later case the result of the method will be reactive to updates of the value provider.
    /// These allows the developer to determine exactly what changes the result should be reactive to.</para>
    /// <para>The methods of this class can conflict with the methods in <see cref="System.Linq.Enumerable"/>. When both the System.Linq and Obtics.Collections namespaces are added
    /// to the namespace list of your source file, inline query syntax statements on IEnumerables will lead to ambiguity erros. </para>
    /// <para>There has been chosen not to make use of
    /// a token type (say IObservableEnumerable) to keep the ObservableEnumerable as interchangeable as possible with the Enumerable methods. An IObservableEnumerable type
    /// could have been used to disambiguate between <see cref="System.Linq.Enumerable"/> methods and <see cref="ObservableEnumerable"/> methods, but would have caused
    /// problems when working with composed sequence types.</para>
    /// <para>The GroupBy method for example returns an IEnumerable&gt;IGrouping&gt;,&lt;&lt; type of object. Now if we were to use a token type then the result of
    /// the ObservableEnumerable variation would have been something lige IObservableEnumerable&gt;IObservableGrouping&gt;,&lt;&lt;. The problem is that this type can not be
    /// converted to IEnumerable&gt;IGrouping&gt;,&lt;&lt; by a simple up-cast. This would prevent the two GroupBy implementations from being interchangable.</para>
    /// </remarks>
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Indicates if the give sequence is in it's most unordered form or not.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns>boolean indicating that the current sequence is the most unordered that can be found.</returns>
        /// <remarks>
        /// Sequences that implement IInternalEnumerable are queried explicitly
        /// if they are most unordered or not. Any other sequences are regarded as
        /// 'MostUnordered', meaning that there is no form that is less ordered than
        /// the given sequence.
        /// 
        /// An unordered sequence has the potential to be cheaper since less information needs to be carried.
        /// 
        /// Ordered sequences are valid input for unordered transformations. The other way
        /// arroung is not true.
        /// </remarks>
        internal static bool IsMostUnordered(this IInternalEnumerable enumerable)
        {
            return enumerable == null || enumerable.IsMostUnordered ;
        }


        internal static IInternalEnumerable<T> Unordered<T>(this IInternalEnumerable<T> s)
        { return s == null ? null : s.UnorderedForm; }

        /// <summary>
        /// Shorthand for .Patched().Unordered()
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        internal static IInternalEnumerable<TElement> PatchedUnordered<TElement>(this IEnumerable<TElement> enumerable)
        {
            var p = enumerable.Patched();
            return p == null ? null : p.UnorderedForm; 
        }

        /// <summary>
        /// Converts the given sequence to a patched sequence that uses the internal event mechanism.
        /// </summary>
        /// <typeparam name="TElement">Type of the elements of the sequence.</typeparam>
        /// <param name="source">The source sequence to convert.</param>
        /// <returns></returns>
        internal static IInternalEnumerable<TElement> Patched<TElement>(this IEnumerable<TElement> source)
        { return (IInternalEnumerable<TElement>)Obtics.Collections.ObservableEnumerable.OfType<TElement>(source); }

        /// <summary>
        /// Converts an <see cref="IEnumerable"/> sequence to an <see cref="IVersionedEnumerable"/> and / or
        /// an <see cref="INotifyCollectionChanged"/> to an <see cref="INotifyChanged"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>
        /// The result needs to insert change order numbers. In a single threaded environment this is easy.
        /// We just make sure that our patch object is the ONLY obtics object listeningfor changes on the source.
        /// That means we can never get out of sync and we can just count the change events.
        /// 
        /// In a multithreaded environment it is a little bit more complicated. New change events may arrive when our
        /// patch obect is still processing the last event. Sequence and change events may therefore go out of sync.
        /// We will need to maintain a snapshot copy of the contents of our source.
        /// </remarks>
        internal static IInternalEnumerable Patched(this IEnumerable source)
        {
            //Obtics collection transformation elements all work with IVersionedEnumerable and INotifyChanged (internal) interfaces.
            //This method is concerned with translating IEnumerable (or any type of object using custom adapters)
            //objects to IVersionedEnumerable and INotifyCollectionChanged, INotifyPropertyChanged to INotifyChanged.

            if (source == null)
                return null;

            var patched = source as IInternalEnumerable;

            if (patched == null)
            {
                var stage1 = CollectionAdapterProvider.GetAdapter(source);

                patched = stage1 as IInternalEnumerable ?? CollectionAdapterProvider.GetAdapter(stage1) as IInternalEnumerable;
            }

            return patched; 
        }
    }
}
