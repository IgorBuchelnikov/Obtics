using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Obtics.Collections
{
    /// <summary>
    /// An interface for objects that can adapt an object of an unknown type of sequence to
    /// a form Obtics can understand. This adapted form should implement IVersionedEnumerable at least
    /// and preferably implement <see cref="INotifyCollectionChanged"/> for observability. This
    /// <see cref="INotifyCollectionChanged"/> implementation should provide <see cref="OrderedNotifyCollectionChangedEventArgs"/>
    /// for its event arguments.
    /// </summary>
    public interface ICollectionAdapter
    {        
        /// <summary>
        /// Adapts an unknown collection to a form Obtics can understand.
        /// </summary>
        /// <param name="collection">The unkown collection to adapt.</param>
        /// <returns>An <see cref="IVersionedEnumerable"/> implementation that wraps <paramref name="collection"/>.
        /// It should preferably implement <see cref="INotifyCollectionChanged"/> for observability but this is not required. This
        /// <see cref="INotifyCollectionChanged"/> implementation should provide <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        /// for its event arguments.</returns>
        IVersionedEnumerable AdaptCollection(object collection);
    }
}
