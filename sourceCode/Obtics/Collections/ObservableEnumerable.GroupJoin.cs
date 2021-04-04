using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using CT = Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys and groups the results dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector)
        { return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys and groups the results. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null )
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                ConvertToPairsTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        ( outerItem, grpdisp, oksel, ressel ) => 
                            (IValueProvider<TResult>)CT.Aggregate<TInner, TResult>.Create(
                                (IVersionedEnumerable<TInner>)grpdisp.GetGroup(oksel(outerItem)),
                                FuncExtender<IEnumerable<TInner>>.Extend( ressel, outerItem, ( innerGroup, ressel2, oitm ) => ressel2(oitm, innerGroup) )
                            )
                    );
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                ConvertToPairsTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        ( outerItem, grpdisp, oksel, ressel ) => 
                            (IValueProvider<TResult>)CT.Aggregate<TInner, TResult>.Create(
                                (IVersionedEnumerable<TInner>) 
                                    oksel(outerItem).Patched()
                                        ._Select((Func<TKey, System.Linq.IGrouping<TKey, TInner>>)grpdisp.GetGroup)
                                        ._Cascade(),
                                FuncExtender<IEnumerable<TInner>>.Extend( ressel, outerItem, ( innerGroup, ressel2, oitm ) => ressel2(oitm, innerGroup) )
                            )
                    );
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                NotifyVpcTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        ( outerItem, grpdisp, oksel, ressel ) => 
                            (IValueProvider<TResult>)CT.Aggregate<TInner, TResult>.Create(
                                (IVersionedEnumerable<TInner>)grpdisp.GetGroup(oksel(outerItem)),
                                FuncExtender<IEnumerable<TInner>>.Extend(ressel, outerItem, (innerGroup, ressel2, oitm) => ressel2(oitm, innerGroup))
                        )
                    );
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                NotifyVpcTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        ( outerItem, grpdisp, oksel, ressel ) => 
                            (IValueProvider<TResult>)CT.Aggregate<TInner, TResult>.Create(
                                (IVersionedEnumerable<TInner>)
                                    oksel(outerItem).Patched()
                                        ._Select((Func<TKey,System.Linq.IGrouping<TKey,TInner>>)grpdisp.GetGroup)
                                        ._Cascade(),
                                FuncExtender<IEnumerable<TInner>>.Extend(ressel, outerItem, (innerGroup, ressel2, oitm) => ressel2(oitm, innerGroup))
                            )
                    );
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys and groups the results dynamicaly. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                ConvertToPairsTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            //in this case we assume that the resultSelector is aware of changes in inner selection.
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        ( outerItem, grpdisp, oksel, ressel ) => 
                            ressel(
                                outerItem,
                                grpdisp.GetGroup(oksel(outerItem))
                        )
                    );
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results dynamicaly. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                ConvertToPairsTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            //in this case we assume that the resultSelector is aware of changes in inner selection.
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        (outerItem, grpdisp, oksel, ressel) =>
                            ressel(
                                outerItem,
                                oksel(outerItem).Patched()
                                    ._Select((Func<TKey,System.Linq.IGrouping<TKey,TInner>>)grpdisp.GetGroup)
                                    ._Cascade()                        
                            )
                    );
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results dynamicaly. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                NotifyVpcTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            //in this case we assume that the resultSelector is aware of changes in inner selection.
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        (outerItem, grpdisp, oksel, ressel) =>
                            ressel(
                                outerItem,
                                grpdisp.GetGroup(oksel(outerItem))
                            )
                    );
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys dynamicaly and groups the results dynamicaly. Key values are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. This returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key to join by for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence. It resturns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the grouping.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains elements of type <typeparamref name="TResult"/> that are obtained by performing a grouped join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = BoundGroupFilterDispenser<TInner, TKey>.Create(
                NotifyVpcTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );

            //in this case we assume that the resultSelector is aware of changes in inner selection.
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        (outerItem, grpdisp, oksel, ressel) =>
                            ressel(
                                outerItem,
                                oksel(outerItem).Patched()
                                    ._Select((Func<TKey, System.Linq.IGrouping<TKey, TInner>>)grpdisp.GetGroup)
                                    ._Cascade()
                            )
                    );
        }
    }
}
