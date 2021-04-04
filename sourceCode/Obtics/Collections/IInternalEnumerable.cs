using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Collections
{
    /// <summary>
    /// Internal IEnumerableInterface
    /// Gives information about the ordering quality of a sequence and direct association with INotifyChanged
    /// </summary>
    /// <remarks>
    /// Usualy sequences ar ordered. Every element in the sequence has a particular index in the sequence
    /// and can be identified by this index. Element in the sequence are returned in ascending index order
    /// and the indexes are contiguous.
    /// 
    /// This index information is extra information and therefore an extra weight to carry when doing transformations. 
    /// For many transformations this extra index information is less relevant or totally irrelevant.
    /// 
    /// Sorting and any aggregate that does not depend on the order of elements (Min,Max,Sum,Average,All,Any,Contains,Count,LongCount).
    /// 
    /// These transformations can request an 'unordered form' of their original sources. If possible this unordered form
    /// can ignore any order information in the source and therefore be lighter.
    /// 
    /// The ordered form of a Where clause for example always needs to maintain a mapping with result items and indexes. Adding
    /// and removing of items from this mapping always require a lot of work on top of the memory overhead.
    /// Un unordered form can do away with this mapping and therefore consumes less memory and has less overhead. 
    /// </remarks>
    internal interface IInternalEnumerable : IVersionedEnumerable, INotifyChanged
    {
        /// <summary>
        /// Returns the unordered form of this sequence.
        /// </summary>
        /// <remarks>
        /// If IsMostUnordered is true this property should return this object. If IsMostUnordered
        /// is false it should return a different, unordered object.
        /// </remarks>
        IInternalEnumerable UnorderedForm { get; }

        /// <summary>
        /// If this sequence object is unordered.
        /// </summary>
        bool IsMostUnordered { get; }

        /// <summary>
        /// If the enumerator of this sequence can always be safely enumerated (no exeptions will be raised during enumeration)
        /// </summary>
        bool HasSafeEnumerator { get; }
    }

    /// <summary>
    /// Typed <see cref="IInternalEnumerable"/> interface.
    /// </summary>
    /// <typeparam name="TType">Type of the sequence elements.</typeparam>
    internal interface IInternalEnumerable<TType> : IInternalEnumerable, IVersionedEnumerable<TType>
    {
        /// <summary>
        /// Returns the unordered form of this sequence.
        /// </summary>
        new IInternalEnumerable<TType> UnorderedForm { get; }
    }
}
