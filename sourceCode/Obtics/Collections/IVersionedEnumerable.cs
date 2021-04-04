using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Collections;

namespace Obtics.Collections
{
    /// <summary>
    /// IEnumerable override that also publishes version information.
    /// </summary>
    /// <remarks>
    /// When a class implements IVersionedEnumerable and also implements collection change notification
    /// it is required to do this via <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>. Additionaly the CollectionChanged events need
    /// to pass an <see cref="OrderedNotifyCollectionChangedEventArgs"/> object as event argument.
    /// </remarks>
    public interface IVersionedEnumerable : IEnumerable
    {
        /// <summary>
        /// GetEnumerator override that returns an <see cref="IVersionedEnumerator"/>.
        /// </summary>
        /// <returns>An <see cref="IVersionedEnumerator"/> for this sequence.</returns>
        new IVersionedEnumerator GetEnumerator();
    }

    /// <summary>
    /// A typed override of the <see cref="IVersionedEnumerable"/> interface.
    /// </summary>
    /// <typeparam name="TType">The type of the elements of the sequence.</typeparam>
    /// <remarks>
    /// When a class implements IVersionedEnumerable and also implements collection change notification
    /// it is required to do this via <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>. Additionaly the CollectionChanged events need
    /// to pass an <see cref="OrderedNotifyCollectionChangedEventArgs"/> object as event argument.
    /// </remarks>
    public interface IVersionedEnumerable<TType> : IVersionedEnumerable, IEnumerable<TType>
    {
        /// <summary>
        /// GetEnumerator override that returns an <see cref="IVersionedEnumerator{TType}"/>.
        /// </summary>
        /// <returns>An <see cref="IVersionedEnumerator{TType}"/> for this sequence.</returns>
        new IVersionedEnumerator<TType> GetEnumerator();
    }
}
