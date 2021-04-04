using System.Collections.Generic;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Returns the input typed as <see cref="IEnumerable{TType}"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of source</typeparam>
        /// <param name="source">The sequence to type as <see cref="IEnumerable{TType}"/>.</param>
        /// <returns>The input sequence typed as <see cref="IEnumerable{TType}"/>, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TType> AsEnumerable<TType>(this IEnumerable<TType> source)
        { return source; }
    }
}
