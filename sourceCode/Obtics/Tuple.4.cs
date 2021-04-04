using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static partial class Tuple
    {
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 a1, T2 a2, T3 a3, T4 a4)
        { return new Tuple<T1, T2, T3, T4>(a1, a2, a3, a4); }
    }

    internal class Tuple<TFirst, TSecond, TThird, TFourth> : IEquatable<Tuple<TFirst, TSecond, TThird, TFourth>>
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

        readonly TFourth _Fourth;

        public TFourth Fourth
        { get { return _Fourth; } }

        public Tuple(TFirst first, TSecond second, TThird third, TFourth fourth)
        {
            _First = first;
            _Second = second;
            _Third = third;
            _Fourth = fourth;
        }

        public override bool Equals(object obj)
        { return this.Equals(obj as Tuple<TFirst, TSecond, TThird, TFourth>); }

        public override int GetHashCode()
        {
            return
                Hasher
                    .CreateFromRef(typeof(Tuple<TFirst, TSecond, TThird, TFourth>))
                    .Add(_First)
                    .Add(_Second)
                    .Add(_Third)
                    .Add(_Fourth);
        }

        public static bool operator ==(Tuple<TFirst, TSecond, TThird, TFourth> a, Tuple<TFirst, TSecond, TThird, TFourth> b)
        { return object.ReferenceEquals(a, b) || !object.ReferenceEquals(a, null) && a.Equals(b); }

        public static bool operator !=(Tuple<TFirst, TSecond, TThird, TFourth> a, Tuple<TFirst, TSecond, TThird, TFourth> b)
        { return !(a == b); }

        #region IEquatable<Tuple<TFirst,TSecond,TThird, TFourth>> Members

        public bool Equals(Tuple<TFirst, TSecond, TThird, TFourth> other)
        {
            return
                (object)other != null
                && ObticsEqualityComparer<TFirst>.Default.Equals(this._First, other._First)
                && ObticsEqualityComparer<TSecond>.Default.Equals(this._Second, other._Second)
                && ObticsEqualityComparer<TThird>.Default.Equals(this._Third, other._Third)
                && ObticsEqualityComparer<TFourth>.Default.Equals(this._Fourth, other._Fourth);
        }

        #endregion
    }
}
