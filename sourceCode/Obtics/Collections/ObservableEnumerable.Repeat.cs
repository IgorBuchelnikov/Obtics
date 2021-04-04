
using System.Collections.Generic;
using Obtics.Values;
namespace Obtics.Collections
{
    //Explicitly observable object linq

    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
        /// <param name="element">An <see cref="IValueProvider{TResult}"/> whose Value property gives the value to be repeated.</param>
        /// <param name="count">An <see cref="IValueProvider{Int}"/> of <see cref="System.Int32"/> whose value gives the number of times to repeat the value in the generated sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that contains a repeated value, or null when either <paramref name="element"/> or <paramref name="count"/> is null.</returns>
        public static IEnumerable<TResult> Repeat<TResult>(IValueProvider<TResult> element, IValueProvider<int> count)
        {
            if (element == null || count == null)
                return null;

            return 
                element.Convert(
                    FuncExtender<IValueProvider<TResult>>.Extend(
                        Range(0, count), 
                        (e, r) => 
                            r.Select(e.Value, (v, ev) => ev)
                    )
                )
                .Cascade()
            ;
        }

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
        /// <param name="element">The value to be repeated.</param>
        /// <param name="count">An <see cref="IValueProvider{Int}"/> of <see cref="System.Int32"/> whose value gives the number of times to repeat the value in the generated sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that contains a repeated value, or null when <paramref name="count"/> is null.</returns>
        public static IEnumerable<TResult> Repeat<TResult>(TResult element, IValueProvider<int> count)
        {
            if (count == null)
                return null;

            return Range(0, count).Select(element, (v, ev) => ev);
        }

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
        /// <param name="element">An <see cref="IValueProvider{TResult}"/> whose Value property gives the value to be repeated.</param>
        /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that contains a repeated value, or null when <paramref name="element"/> is null.</returns>
        public static IEnumerable<TResult> Repeat<TResult>(IValueProvider<TResult> element, int count)
        {
            if (element == null)
                return null;

            return
                element.Convert(
                    FuncExtender<IValueProvider<TResult>>.Extend(
                        Range(0, count),
                        (e, r) => r.Select(e.Value, (v, ev) => ev)
                    )
                )
                .Cascade()
            ;
        }

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
        /// <param name="element">The value to be repeated.</param>
        /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> that contains a repeated value.</returns>
        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
        { return Static(System.Linq.Enumerable.Repeat(element,System.Math.Abs(count))); }
    }
}
