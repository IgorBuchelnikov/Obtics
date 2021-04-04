using System;
using System.Collections.Generic;
using System.Linq;
using Obtics.Values;
using Obtics.Values.Transformations;

namespace Obtics.Collections
{
    //Explicitly observable object linq
    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>An <see cref="IEnumerable{Int}"/> of <see cref="System.Int32"/> that contains a range of sequential integral numbers.</returns>
        public static IEnumerable<int> Range(int start, int count)
        { return Static(Enumerable.Range(start, count)); }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">An <see cref="IValueProvider{Int}"/> of <see cref="System.Int32"/> whose value gives the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>An <see cref="IEnumerable{Int}"/> of <see cref="System.Int32"/> that contains a range of sequential integral numbers, or null when <paramref name="start"/> is null.</returns>
        public static IEnumerable<int> Range(IValueProvider<int> start, int count)
        { return _Range(start.Patched(), ValueProvider._Static(count)); }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">An <see cref="IValueProvider{Int}"/> of <see cref="System.Int32"/> whose value gives the number of sequential integers to generate.</param>
        /// <returns>An <see cref="IEnumerable{Int}"/> of <see cref="System.Int32"/> that contains a range of sequential integral numbers, or null when <paramref name="count"/> is null.</returns>
        public static IEnumerable<int> Range(int start, IValueProvider<int> count)
        { return _Range(ValueProvider._Static(start), count.Patched()); }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">An <see cref="IValueProvider{Int}"/> of <see cref="System.Int32"/> whose value gives the first integer in the sequence.</param>
        /// <param name="count">An <see cref="IValueProvider{Int}"/> of <see cref="System.Int32"/> whose value gives the number of sequential integers to generate.</param>
        /// <returns>An <see cref="IEnumerable{Int}"/> of <see cref="System.Int32"/> that contains a range of sequential integral numbers, or null when either <paramref name="start"/> or <paramref name="count"/> is null.</returns>
        public static IEnumerable<int> Range(IValueProvider<int> start, IValueProvider<int> count)
        { return _Range(start.Patched(), count.Patched()); }

        internal static IEnumerable<int> _Range(IInternalValueProvider<int> start, IInternalValueProvider<int> count)
        { 
            return 
                RangeTransformation.Create(
                    start, 
                    start._Convert(
                        count, 
                        (s, c) => s.Value + c.Value
                    )
                ); 
        }
    }
}
