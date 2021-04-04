using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static partial class Tuple
    {
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 a1, T2 a2, T3 a3)
        { return new Tuple<T1, T2, T3>(a1, a2, a3); }
    }

    internal class Tuple<TFirst, TSecond, TThird> : IEquatable<Tuple<TFirst, TSecond, TThird>>
    {
        readonly TFirst _First;

        public TFirst First
        { get { return _First; } }

        readonly TSecond _Second;

        public TSecond Second
        { get { return _Second; } }

        readonly TThird _Third;

        public TThird Third
        { get { return _Third; } }

        public Tuple(TFirst first, TSecond second, TThird third)
        {
            _First = first;
            _Second = second;
            _Third = third;
        }

        public override bool Equals(object obj)
        { return this.Equals(obj as Tuple<TFirst, TSecond, TThird>); }

        public override int GetHashCode()
        {
            return
                Hasher
                    .CreateFromRef(typeof(Tuple<TFirst, TSecond, TThird>))
                    .Add(_First)
                    .Add(_Second)
                    .Add(_Third);
        }

        public static bool operator ==(Tuple<TFirst, TSecond, TThird> a, Tuple<TFirst, TSecond, TThird> b)
        { return object.ReferenceEquals(a, b) || !object.ReferenceEquals(a, null) && a.Equals(b); }

        public static bool operator !=(Tuple<TFirst, TSecond, TThird> a, Tuple<TFirst, TSecond, TThird> b)
        { return !(a == b); }

        #region IEquatable<Tuple<TFirst,TSecond,TThird>> Members

        public bool Equals(Tuple<TFirst, TSecond, TThird> other)
        {
            return
                (object)other != null 
                && ObticsEqualityComparer<TFirst>.Default.Equals(this._First, other._First)
                && ObticsEqualityComparer<TSecond>.Default.Equals(this._Second, other._Second)
                && ObticsEqualityComparer<TThird>.Default.Equals(this._Third, other._Third);
        }

        #endregion
    }
}
