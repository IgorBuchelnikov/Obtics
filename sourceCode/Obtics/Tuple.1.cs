using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static partial class Tuple
    {
        public static Tuple<T1> Create<T1>(T1 a1)
        { return new Tuple<T1>(a1); }
    }

    internal class Tuple<TFirst> : IEquatable<Tuple<TFirst>>
    {
        readonly TFirst _First;

        public TFirst First
        { get { return _First; } }

        public Tuple(TFirst first)
        { _First = first; }

        public override bool Equals(object obj)
        { return this.Equals(obj as Tuple<TFirst>); }

        public override int GetHashCode()
        {
            return
                Hasher
                    .CreateFromRef(typeof(Tuple<TFirst>))
                    .Add(_First);
        }

        public static bool operator ==(Tuple<TFirst> a, Tuple<TFirst> b)
        { return object.ReferenceEquals(a, b) || !object.ReferenceEquals(a, null) && a.Equals(b); }

        public static bool operator !=(Tuple<TFirst> a, Tuple<TFirst> b)
        { return !(a == b); }

        #region IEquatable<Tuple<TFirst>> Members

        public bool Equals(Tuple<TFirst> other)
        { return (object)other != null && ObticsEqualityComparer<TFirst>.Default.Equals(this._First, other._First); }

        #endregion
    }
}
