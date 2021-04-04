using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static partial class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 a1, T2 a2)
        { return new Tuple<T1, T2>(a1, a2); }
    }

    internal class Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>
    {
        readonly TFirst _First;

        public TFirst First
        { get { return _First; } }

        readonly TSecond _Second;

        public TSecond Second
        { get { return _Second; } }

        public Tuple(TFirst first, TSecond second)
        {
            _First = first;
            _Second = second;
        }

        public override bool Equals(object obj)
        { return this.Equals(obj as Tuple<TFirst, TSecond>); }

        public override int GetHashCode()
        {
            return
                Hasher
                    .CreateFromRef(typeof(Tuple<TFirst, TSecond>))
                    .Add(_First)
                    .Add(_Second);
        }

        public static bool operator ==(Tuple<TFirst, TSecond> a, Tuple<TFirst, TSecond> b)
        { return object.ReferenceEquals(a,b) || !object.ReferenceEquals(a, null) && a.Equals(b); }

        public static bool operator !=(Tuple<TFirst, TSecond> a, Tuple<TFirst, TSecond> b)
        { return !(a == b); }

        #region IEquatable<Tuple<TFirst,TSecond>> Members

        public bool Equals(Tuple<TFirst, TSecond> other)
        { return (object)other != null && ObticsEqualityComparer<TFirst>.Default.Equals(this._First, other._First) && ObticsEqualityComparer<TSecond>.Default.Equals(this._Second, other._Second); }

        #endregion
    }
}
