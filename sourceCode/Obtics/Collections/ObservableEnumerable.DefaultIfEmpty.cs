using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        private static IEnumerable<TSource> BuildDefaultIfEmpty<TSource>(IEnumerable<TSource> source, IEnumerable<TSource> defaultArray)
        {
            return 
                IsNotEmptyAggregate<TSource>.Create(source)
                    ._Convert( 
                        FuncExtender<IValueProvider<bool>>.Extend(
                            source, 
                            defaultArray, 
                            (c, s, defa) => c.Value ? s : defa 
                        ) 
                    )
                    .Cascade()
            ; 
        }

        /// <summary>
        /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The sequence to return a default value for if it is empty.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> that contains default(<typeparamref name="TSource"/>) if source is empty and <paramref name="source"/> otherwise, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
        { return DefaultIfEmpty(source, default(TSource)); }

        /// <summary>
        /// Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The sequence to return the specified value for if it is empty.</param>
        /// <param name="defaultValue">The value to return if the sequence is empty.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> that contains defaultValue if source is empty and <paramref name="source"/> otherwise, or null when <paramref name="source"/> is null.</returns>
        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        { return BuildDefaultIfEmpty(source, StaticEnumerable<TSource>.Create(defaultValue)); }

        /// <summary>
        /// Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The sequence to return the specified value for if it is empty.</param>
        /// <param name="defaultValue">An <see cref="IValueProvider{T}"/> who's Value property will give the value to return if the sequence is empty.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> that contains defaultValue if source is empty and <paramref name="source"/> otherwise, or null when either <paramref name="source"/> or <paramref name="defaultValue"/> is null.</returns>
        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, IValueProvider<TSource> defaultValue)
        { return BuildDefaultIfEmpty(source, defaultValue.AsEnumerable() ); }

    }
}
