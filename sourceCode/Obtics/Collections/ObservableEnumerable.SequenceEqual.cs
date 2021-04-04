using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first sequence to compare.</param>
        /// <param name="second">The second sequence to compare.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property will be true if the two source sequences are of equal element by element according to the default equality comparer for their type and false otherwise, or null when either <paramref name="first"/> or <paramref name="second"/> is null.</returns>
        public static IValueProvider<bool> SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        { return _SequenceEqual(first, second).Concrete(); }

        internal static IInternalValueProvider<bool> _SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        { return _SequenceEqual(first, second, ObticsEqualityComparer<TSource>.Default); }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their elements by using a specified <see cref="IEqualityComparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first sequence to compare.</param>
        /// <param name="second">The second sequence to compare.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TSource}"/> to compare elements with.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/>, whose Value property will be true if the two source sequences are of equal element by element according to the specified <see cref="IEqualityComparer{TSource}"/> and false otherwise, or null when either <paramref name="first"/>, <paramref name="second"/> or <paramref name="comparer"/> is null.</returns>
        public static IValueProvider<bool> SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        { return _SequenceEqual(first, second, comparer).Concrete(); }

        internal static IInternalValueProvider<bool> _SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return               
                    //First comparing the hash. Gives an advantages when the sequences are NOT equal.
                    //hashes are much easier to update when the source sequences change. When the
                    //hashes are not equal then we don't need to be bothered to compare the sequences element by element.
                    ValueProvider._Convert(
                        SequenceHashAggregate<TSource>.Create(first, comparer),
                        SequenceHashAggregate<TSource>.Create(second, comparer),
                        (fsh, ssh) => fsh.Value == ssh.Value
                    )
                    ._And(SequenceEqualsAggregate<TSource>.Create(first, second, comparer));
        }
    
    }
}
