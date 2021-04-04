using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    /// <summary>
    /// Struct to help creating a good hash
    /// </summary>
    internal struct Hasher
    {
        public Hasher(int seed)
        { unchecked { _Value = seed; } }

        int _Value;

        public int Value
        { get { unchecked { return _Value; } } }

        public static Hasher Create<TType>(TType value)
        { return new Hasher(!typeof(TType).IsValueType && value == null ? 83927458 : ObticsEqualityComparer<TType>.Default.GetHashCode(value)); }

        public static Hasher Create<TType>(TType value, IEqualityComparer<TType> comparer)
        { return new Hasher(!typeof(TType).IsValueType && value == null ? 83927458 : comparer.GetHashCode(value)); }

        public static Hasher CreateFromRef<TType>(TType value) where TType : class
        { return new Hasher(value == null ? 83927458 : ObticsEqualityComparer<TType>.Default.GetHashCode(value)); }

        public static Hasher CreateFromRef<TType>(TType value, IEqualityComparer<TType> comparer) where TType : class
        { return new Hasher(value == null ? 83927458 : comparer.GetHashCode(value)); }

        public static Hasher CreateFromHash(int value)
        { return new Hasher(value); }

        public static Hasher CreateFromValue<TType>(TType value) where TType : struct
        { return new Hasher(ObticsEqualityComparer<TType>.Default.GetHashCode(value)); }

        public static Hasher CreateFromValue<TType>(TType value, IEqualityComparer<TType> comparer) where TType : struct
        { return new Hasher(comparer.GetHashCode(value)); }

        public static Int32 Rehash(Int32 hash)
        {
            unchecked
            {
                ulong m = (ulong)hash * 0x00000000e85791a6UL;
                return (Int32)(m ^ (m >> 32) ^ 0x00000000827a092bUL);
            }
        }

        public Hasher Add(Hasher b)
        {
            unchecked
            {
                return new Hasher(Rehash(_Value + b._Value));
            }
        }

        public Hasher Add<TType>(TType value)
        { return Add(Create(value)); }

        public Hasher Add<TType>(TType value, IEqualityComparer<TType> comparer)
        { return Add(Create(value, comparer)); }

        public Hasher AddRef<TType>(TType value) where TType : class
        { return Add(CreateFromRef(value)); }

        public Hasher AddRef<TType>(TType value, IEqualityComparer<TType> comparer) where TType : class
        { return Add(CreateFromRef(value, comparer)); }

        public Hasher AddValue<TType>(TType value) where TType : struct
        { return Add(CreateFromValue(value)); }

        public Hasher AddValue<TType>(TType value, IEqualityComparer<TType> comparer) where TType : struct
        { return Add(CreateFromValue(value, comparer)); }

        public static implicit operator int(Hasher h)
        { return h.Value; }    
    }
}
