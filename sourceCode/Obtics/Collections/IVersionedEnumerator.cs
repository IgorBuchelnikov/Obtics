using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Collections;

namespace Obtics.Collections
{
    /// <summary>
    /// Override of the <see cref="IEnumerator"/> interface that provides version information about the sequence being enumerated.
    /// </summary>
    public interface IVersionedEnumerator : IEnumerator
    {
        /// <summary>
        /// The <see cref="VersionNumber"/> of the current enumeration. This should not change during the lifetime of this 
        /// <see cref="IVersionedEnumerator"/> object.
        /// </summary>
        VersionNumber ContentVersion{ get; } 
    }

    /// <summary>
    /// Typed override of the <see cref="IVersionedEnumerator"/> interface.
    /// </summary>
    /// <typeparam name="TType">Type of the elements of the enumerated sequence.</typeparam>
    public interface IVersionedEnumerator<TType> : IVersionedEnumerator, IEnumerator<TType>
    {}

    /// <summary>
    /// Static class with static helper methods for <see cref="IVersionedEnumerator"/> and <see cref="IVersionedEnumerator{TType}"/>.
    /// </summary>
    public static class VersionedEnumerator
    {
        sealed class EnumeratorWrapper<TType> : IVersionedEnumerator<TType>
        {
            internal EnumeratorWrapper(IEnumerator<TType> innerEnumerator, VersionNumber versionNumber )
            {
                _InnerEnumerator = innerEnumerator;
                _VersionNumber = versionNumber;
            }

            readonly IEnumerator<TType> _InnerEnumerator;
            readonly VersionNumber _VersionNumber;

            #region IVersionedEnumerator Members

            public VersionNumber ContentVersion
            { get { return _VersionNumber; } }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            { get { return ((IEnumerator)_InnerEnumerator).Current; } }

            public bool MoveNext()
            { return _InnerEnumerator.MoveNext(); }

            public void Reset()
            { _InnerEnumerator.Reset(); }

            #endregion

            #region IEnumerator<TType> Members

            public TType Current
            { get { return _InnerEnumerator.Current; } }

            #endregion

            #region IDisposable Members

            public void Dispose()
            { _InnerEnumerator.Dispose(); }

            #endregion
        }

        /// <summary>
        /// Constructs an <see cref="IVersionedEnumerator{TType}"/> out of an <see cref="IEnumerator{TType}"/> and a <see cref="VersionNumber"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the elements returned by the sequence.</typeparam>
        /// <param name="innerEnumerator">The <see cref="IEnumerator{TType}"/> or <typeparamref name="TType"/> that will be the source of the elements for the resulting <see cref="IVersionedEnumerator{TType}"/> of <typeparamref name="TType"/>.</param>
        /// <param name="version">The <see cref="VersionNumber"/> of the resulting <see cref="IVersionedEnumerator{TType}"/> of <typeparamref name="TType"/>.</param>
        /// <returns>An <see cref="IVersionedEnumerator{TType}"/> of <typeparamref name="TType"/> whose elements are provided by <paramref name="innerEnumerator"/> and whose ContentVersion is provided by <paramref name="version"/>.</returns>
        public static IVersionedEnumerator<TType> WithContentVersion<TType>(this IEnumerator<TType> innerEnumerator, VersionNumber version)
        {
            if (innerEnumerator == null)
                throw new ArgumentNullException("innerEnumerator");

            return new EnumeratorWrapper<TType>(innerEnumerator, version); 
        }

        /// <summary>
        /// Constructs an <see cref="IVersionedEnumerator{TType}"/> out of an <see cref="IEnumerable{TType}"/> and a <see cref="VersionNumber"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the elements returned by the sequence.</typeparam>
        /// <param name="innerEnumerable">The <see cref="IEnumerable{TType}"/> or <typeparamref name="TType"/> that will be the source of the elements for the resulting <see cref="IVersionedEnumerator{TType}"/> of <typeparamref name="TType"/>.</param>
        /// <param name="version">The <see cref="VersionNumber"/> of the resulting <see cref="IVersionedEnumerator{TType}"/> of <typeparamref name="TType"/>.</param>
        /// <returns>An <see cref="IVersionedEnumerator{TType}"/> of <typeparamref name="TType"/> whose elements are provided by <paramref name="innerEnumerable"/> and whose ContentVersion is provided by <paramref name="version"/>.</returns>
        public static IVersionedEnumerator<TType> WithContentVersion<TType>(IEnumerable<TType> innerEnumerable, VersionNumber version)
        { return WithContentVersion(innerEnumerable.GetEnumerator(), version); }
    }

    /// <summary>
    /// Static information and methods about the <see cref="IVersionedEnumerator{TType}"/> and <see cref="IVersionedEnumerator"/> interfaces.
    /// </summary>
    internal static class SIVersionedEnumerator
    {
        /// <summary>
        /// Name of the <see cref="IVersionedEnumerator"/>.ContentVersion property. 
        /// </summary>
        public const string ContentVersionPropertyName = "ContentVersion";
    }
}
