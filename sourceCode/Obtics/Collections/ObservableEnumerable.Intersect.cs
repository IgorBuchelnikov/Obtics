using System;
using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{TSource}"/> whose distinct elements that also appear in <paramref name="second"/> will be returned.</param>
        /// <param name="second">An <see cref="IEnumerable{TSource}"/> whose distinct elements that also appear in the <paramref name="first"/> sequence will be returned.</param>
        /// <returns>A sequence, that contains the elements that form the set intersection of two sequences, or null when either <paramref name="first"/> or <paramref name="second"/> is null.</returns>
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        { return Intersect(first, second, ObticsEqualityComparer<TSource>.Default); }

        /// <summary>
        /// Produces the set intersection of two sequences by using the specified <see cref="IEqualityComparer{TSource}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{TSource}"/> whose distinct elements that also appear in <paramref name="second"/> will be returned.</param>
        /// <param name="second">An <see cref="IEnumerable{TSource}"/> whose distinct elements that also appear in the <paramref name="first"/> sequence will be returned.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TSource}"/> to compare values.</param>
        /// <returns>A sequence, that contains the elements that form the set intersection of two sequences, or null when either <paramref name="first"/>, <paramref name="second"/> or <paramref name="comparer"/> is null.</returns>
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            //var dispenser = BoundContainsDispenser<TSource, TSource>.Create(second, item => item, comparer);

            var dispenser = UnorderedBoundGroupFilterDispenser<TSource, TSource>.Create(second.Select(s => Tuple.Create(s, s)), comparer);

            return dispenser == null ? null : first.Where(FuncExtender<TSource>.Extend(dispenser, (firstItem, disp) => (IValueProvider<bool>)disp.GetGroup(firstItem)._Any())).Distinct();
        }
    }
}
