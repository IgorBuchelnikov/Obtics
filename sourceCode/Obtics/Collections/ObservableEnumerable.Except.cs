using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Produces the set difference of two sequences by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{TSource}"/> whose elements that are not also in second will be returned.</param>
        /// <param name="second">An <see cref="IEnumerable{TSource}"/> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
        /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        { return Except(first, second, ObticsEqualityComparer<TSource>.Default); }

        /// <summary>
        /// Produces the set difference of two sequences by using the specified <see cref="IEqualityComparer{TSource}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{TSource}"/> whose elements that are not also in second will be returned.</param>
        /// <param name="second">An <see cref="IEnumerable{TSource}"/> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TSource}"/> to compare values.</param>
        /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            var dispenser = UnorderedBoundGroupFilterDispenser<TSource, TSource>.Create(second.Select(s => Tuple.Create(s, s)), comparer);

            return dispenser == null ? null : first.Where(FuncExtender<TSource>.Extend(dispenser, (firstItem, disp) => (IValueProvider<bool>)disp.GetGroup(firstItem)._Any()._Invert()));
        }
    }
}
