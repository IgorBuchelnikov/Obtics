using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    /// <summary>
    /// Wrapper for Arrays. This wrapper compares the Arrays elements for equality
    /// when compared with another wrapper
    /// 
    /// This is not part of ObticsEqualityComparer since arrays are not immutable objects. This means that standard two array instances, even
    /// though they may have the same contents, should be regarded as different.
    /// </summary>
    /// <typeparam name="TType">Type of the array elements</typeparam>
    internal struct ArrayStructuredEqualityWrapper<TType> : IEquatable<ArrayStructuredEqualityWrapper<TType>>
    {
        readonly TType[] _Array;

        public TType[] Array
        { get { return _Array; } }

        public ArrayStructuredEqualityWrapper(TType[] array)
        { _Array = array; }

        public override int GetHashCode()
        { 
            var hasher = Hasher.CreateFromRef(typeof(ArrayStructuredEqualityWrapper<TType>));
            var comparer = ObticsEqualityComparer<TType>.Default;

            for (int i = 0, end = _Array.Length; i != end; ++i)
                hasher = hasher.Add(_Array[i], comparer);

            return hasher.Value;
        }

        public override bool Equals(object obj)
        { return obj is ArrayStructuredEqualityWrapper<TType> && this.Equals((ArrayStructuredEqualityWrapper<TType>)obj); }

        public bool Equals(ArrayStructuredEqualityWrapper<TType> other)
        { 
            if (object.ReferenceEquals(_Array, other._Array))
                return true;

            int end1 = _Array.Length, end2 = other._Array.Length;

            if (end1 != end2)
                return false;

            var comparer = ObticsEqualityComparer<TType>.Default;

            for (int i = 0; i != end1; ++i)
                if (!comparer.Equals(_Array[i], other._Array[i]))
                    return false;

            return true;
        }
    }
}
