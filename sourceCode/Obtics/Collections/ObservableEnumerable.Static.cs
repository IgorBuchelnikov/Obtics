using System.Collections.Generic;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Creates an unchanging shallow copy of a sequence as it is the moment the method is invoked.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The source to create a copy of.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/>, that contains the same elements as <paramref name="source"/> at the moment this method was invoked, or null when source is null.</returns>
        public static IEnumerable<TSource> Static<TSource>(this IEnumerable<TSource> source)
        { return StaticEnumerable<TSource>.Create(source); }

        /// <summary>
        /// Creates a static sequence from a number of given elements or an array of element
        /// </summary>
        /// <typeparam name="TSource">Type of the sequence elements.</typeparam>
        /// <param name="source">The (array of) elements to create the sequence with.</param>
        /// <returns>A static sequence.</returns>
        /// <remarks>
        /// The sequence returned by this method differs from an array in that two of these sequence instances
        /// will be equal if all elements one for one are equal. Two different array instances are never equal.
        /// </remarks>
        public static IEnumerable<TSource> Static<TSource>(params TSource[] source)
        { return StaticEnumerable<TSource>.Create(source); } 
    }
}
