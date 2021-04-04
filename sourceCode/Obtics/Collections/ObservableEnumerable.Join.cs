using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector)
        { return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, ObticsEqualityComparer<TKey>.Default); }


        #region Join helper methods

        //takes a sequence of inner items, one outer item and a result selector that combines individual inner items and the outer item to a result value.
        //ite returns a sequence of result values.
        static IEnumerable<TResult> _JoinHelper1<TOuter, TInner, TResult>(this IEnumerable<TInner> innerItems, Func<TOuter, TInner, TResult> selector, TOuter outerItem)
        {
            return
                innerItems.Select(
                    selector,
                    outerItem,
                    (innerItem, ressel2, oitm) => ressel2(oitm, innerItem)
                );
        }

        //same, but result selector returns a live result
        static IEnumerable<TResult> _JoinHelper1<TOuter, TInner, TResult>(this IEnumerable<TInner> innerItems, Func<TOuter, TInner, IValueProvider<TResult>> selector, TOuter outerItem)
        {
            return
                innerItems.Select(
                    selector,
                    outerItem,
                    (innerItem, ressel2, oitm) => ressel2(oitm, innerItem)
                );
        }

        //gives a static group dispenser (groups of inner items by key)
        private static BoundGroupFilterDispenser<TInner, TKey> _JoinHelper2<TInner, TKey>(IEnumerable<TInner> inner, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey> comparer)
        {
            return BoundGroupFilterDispenser<TInner, TKey>.Create(
                ConvertToPairsTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );
        }

        //gives a live group dispenser (groups of inner items by key) key selector returns a live result
        private static BoundGroupFilterDispenser<TInner, TKey> _JoinHelper2<TInner, TKey>(IEnumerable<TInner> inner, Func<TInner, IValueProvider<TKey>> innerKeySelector, IEqualityComparer<TKey> comparer)
        {
            return BoundGroupFilterDispenser<TInner, TKey>.Create(
                NotifyVpcTransformation<TInner, TKey>.Create(inner, innerKeySelector),
                comparer
            );
        }

        //_JoinHelper3 combines live or not live outerKeySelector with live or not live result selector
        private static IEnumerable<TResult> _JoinHelper3<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, Func<TOuter, TKey> outerKeySelector, Func<TOuter, TInner, TResult> resultSelector, BoundGroupFilterDispenser<TInner, TKey> groupDispenser)
        {
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        (outerItem, grpdisp, oksel, ressel) =>
                            grpdisp
                                .GetGroup(oksel(outerItem))
                                ._JoinHelper1(ressel, outerItem)
                    )
                    .Concat();
        }

        //_JoinHelper3 combines live or not live outerKeySelector with live or not live result selector
        private static IEnumerable<TResult> _JoinHelper3<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TOuter, TInner, TResult> resultSelector, BoundGroupFilterDispenser<TInner, TKey> groupDispenser)
        {
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        (outerItem, grpdisp, oksel, ressel) =>
                            oksel(outerItem).Patched()
                                ._Select((Func<TKey, System.Linq.IGrouping<TKey, TInner>>)grpdisp.GetGroup)
                                ._Cascade()
                                ._JoinHelper1(ressel, outerItem)
                    )
                    .Concat();
        }

        //_JoinHelper3 combines live or not live outerKeySelector with live or not live result selector
        private static IEnumerable<TResult> _JoinHelper3<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, Func<TOuter, TKey> outerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector, BoundGroupFilterDispenser<TInner, TKey> groupDispenser)
        {
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        (outerItem, grpdisp, oksel, ressel) =>
                            grpdisp
                                .GetGroup(oksel(outerItem))
                                ._JoinHelper1(ressel, outerItem)
                    )
                    .Concat();
        }

        //_JoinHelper3 combines live or not live outerKeySelector with live or not live result selector
        private static IEnumerable<TResult> _JoinHelper3<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector, BoundGroupFilterDispenser<TInner, TKey> groupDispenser)
        {
            return
                groupDispenser == null ? null :
                outer
                    .Select(
                        groupDispenser,
                        outerKeySelector,
                        resultSelector,
                        (outerItem, grpdisp, oksel, ressel) =>
                            oksel(outerItem).Patched()
                                ._Select((Func<TKey, System.Linq.IGrouping<TKey, TInner>>)grpdisp.GetGroup)
                                ._Cascade()
                                ._JoinHelper1(ressel, outerItem)
                    )
                    .Concat();
        }

        #endregion

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null ) 
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }


        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }


        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }


        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }



        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }


        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }


        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys dynamicaly. A given <see cref="IEqualityComparer{TKey}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first, outer sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second, inner sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys return by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence. It returns an <see cref="IValueProvider{TKey}"/> whose <typeparamref name="TKey"/> Value property gives the key for the given element.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements. It returns an <see cref="IValueProvider{TResult}"/> whose <typeparamref name="TResult"/> Value property gives the result for the given elements.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to hash and compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that has elements of type <typeparamref name="TResult"/>, that are obtained by performing an inner join on two sequences, or null when either <paramref name="outer"/>, <paramref name="inner"/>, <paramref name="outerKeySelector"/>, <paramref name="innerKeySelector"/>, <paramref name="resultSelector"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, IValueProvider<TKey>> outerKeySelector, Func<TInner, IValueProvider<TKey>> innerKeySelector, Func<TOuter, TInner, IValueProvider<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector == null || resultSelector == null)
                return null;

            var groupDispenser = _JoinHelper2<TInner, TKey>(inner, innerKeySelector, comparer);

            return _JoinHelper3<TOuter, TInner, TKey, TResult>(outer, outerKeySelector, resultSelector, groupDispenser);
        }
    }
}
