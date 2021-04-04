using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using System.Linq;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>A sequence of <see cref="IGrouping{TKey,TElement}"/>, where each <see cref="IGrouping{TKey,TElement}"/> object contains a sequence of objects and a key, or null if either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        { return GroupBy(source, keySelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. This method returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element with.</param>
        /// <returns>An sequence of <see cref="IGrouping{TKey,TElement}"/>, where each <see cref="IGrouping{TKey,TElement}"/> object contains a sequence of objects and a key, or null if either <paramref name="source"/> or <paramref name="keySelector"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector)
        { return GroupBy(source, keySelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        { return GroupBy(source, keySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        { return GroupBy(source, keySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, IValueProvider<TResult>> resultSelector)
        { return GroupBy(source, keySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, IValueProvider<TResult>> resultSelector)
        { return GroupBy(source, keySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        { return GroupBy(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and projects the elements for each group by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector)
        { return GroupBy(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return GroupBy(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and projects the elements for each group by using a specified function dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector)
        { return GroupBy(source, keySelector, elementSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key. The elements of each group are projected by using a specified function dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function dynamicaly.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector)
        { return GroupBy(source, keySelector, elementSelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of <see cref="IGrouping{TKey,TElement}"/>, where each <see cref="IGrouping{TKey,TElement}"/> object contains a sequence of objects and a key, or null if either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (keySelector == null)
                return null;

            return
                Distinct(
                    GroupFilterConverterTransformation<TSource, TKey>.Create(
                        (IVersionedEnumerable<Tuple<TSource,TKey>>)Select(source, keySelector, (item, ks) => Tuple.Create(item, ks(item))),
                        comparer
                    )
                );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and compares the keys by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. This method returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element with.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An sequence of <see cref="IGrouping{TKey,TElement}"/>, where each <see cref="IGrouping{TKey,TElement}"/> object contains a sequence of objects and a key, or null if either <paramref name="source"/>, <paramref name="keySelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            return 
                Distinct(
                    GroupFilterConverterTransformation<TSource, TKey>.Create(
                        NotifyVpcTransformation<TSource, TKey>.Create(source, keySelector),
                        comparer
                    )
                );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>,<paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                Select(
                    GroupBy(source, keySelector, comparer),
                    resultSelector,
                    ( grouping, rs ) => rs(grouping.Key, grouping)
                );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>,<paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                Select(
                    GroupBy(source, keySelector, comparer),
                    resultSelector,
                    ( grouping, rs ) => rs(grouping.Key, grouping)
                );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>,<paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                Select(
                    GroupBy(source, keySelector, comparer),
                    resultSelector,
                    ( grouping, rs ) => rs(grouping.Key, grouping)
                );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>,<paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                Select(
                    GroupBy(source, keySelector, comparer),
                    resultSelector,
                    ( grouping, rs ) => rs(grouping.Key, grouping)
                );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return 
                GroupBy(source, keySelector, comparer)
                    .Select(
                        elementSelector,
                        ( grouping, elsel ) => 
                            (IGrouping<TKey, TElement>)GroupingTransformation<TKey, TElement>.Create(
                                (IVersionedEnumerable<TElement>)grouping.Select(elsel), 
                                grouping.Key
                            )
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and projects the elements for each group by using a specified function. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return 
                GroupBy(source, keySelector, comparer)
                    .Select(         
                        elementSelector,
                        ( grouping, elsel ) => 
                            (IGrouping<TKey, TElement>)GroupingTransformation<TKey, TElement>.Create(
                                (IVersionedEnumerable<TElement>)grouping.Select(elsel), 
                                grouping.Key
                            )
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return 
                GroupBy(source, keySelector, comparer)
                    .Select(
                        elementSelector,
                        ( grouping, elsel ) => 
                            (IGrouping<TKey, TElement>)GroupingTransformation<TKey, TElement>.Create(
                                (IVersionedEnumerable<TElement>)grouping.Select(elsel), 
                                grouping.Key
                            )
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and projects the elements for each group by using a specified function dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An IEnumerable&lt;<see cref="IGrouping{TKey, TElement}"/>&gt;, where each <see cref="IGrouping{TKey, TElement}"/> object contains a collection of objects of type <typeparamref name="TElement"/> and a key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return 
                GroupBy(source, keySelector, comparer)
                    .Select(
                        elementSelector,
                        ( grouping, elsel) => 
                            (IGrouping<TKey, TElement>)GroupingTransformation<TKey, TElement>.Create(
                                (IVersionedEnumerable<TElement>)grouping.Select(elsel), 
                                grouping.Key
                            )
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key. The elements of each group are projected by using a specified function. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key. The elements of each group are projected by using a specified function dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key selector function dynamicaly and creates a result value from each group and its key dynamicaly. The elements of each group are projected by using a specified function dynamicaly. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the <see cref="IGrouping{TKey,TElement}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to group the given element by.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the <see cref="IGrouping{TKey,TElement}"/>. It returns an <see cref="IValueProvider{TElement}"/> whose <typeparamref name="TElement"/> Value property gives the projection of the given source element.</param>
        /// <param name="resultSelector">A function to create a result value from each group. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given group.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>A sequence of elements of type <typeparamref name="TResult"/>, where each element represents a projection over a group and its key, or null when either <paramref name="source"/>, <paramref name="keySelector"/>, <paramref name="elementSelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, IValueProvider<TKey>> keySelector, Func<TSource, IValueProvider<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (resultSelector == null)
                return null;

            return
                GroupBy(source, keySelector, elementSelector, comparer)
                    .Select(
                        resultSelector,
                        (elementGrouping, ressel) => ressel(elementGrouping.Key, elementGrouping)
                    );
        }
    }
}
