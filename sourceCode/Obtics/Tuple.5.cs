using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static partial class Tuple
    {
        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        { return new Tuple<T1, T2, T3, T4, T5>(a1, a2, a3, a4, a5); }
    }

    internal class Tuple<TFirst, TSecond, TThird, TFourth, TFifth> : IEquatable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>>
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

        readonly TFifth _Fifth;

        public TFifth Fifth
        { get { return _Fifth; } }

        public Tuple(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth)
        {
            _First = first;
            _Second = second;
            _Third = third;
            _Fourth = fourth;
            _Fifth = fifth;
        }

        public override bool Equals(object obj)
        { return this.Equals(obj as Tuple<TFirst, TSecond, TThird, TFourth, TFifth>); }

        public override int GetHashCode()
        {
            return
                Hasher
                    .CreateFromRef(typeof(Tuple<TFirst, TSecond, TThird, TFourth, TFifth>))
                    .Add(_First)
                    .Add(_Second)
                    .Add(_Third)
                    .Add(_Fourth)
                    .Add(_Fifth);
        }

        public static bool operator ==(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> a, Tuple<TFirst, TSecond, TThird, TFourth, TFifth> b)
        { return object.ReferenceEquals(a, b) || !object.ReferenceEquals(a, null) && a.Equals(b); }

        public static bool operator !=(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> a, Tuple<TFirst, TSecond, TThird, TFourth, TFifth> b)
        { return !(a == b); }

        #region IEquatable<Tuple<TFirst,TSecond,TThird, TFourth, TFifth>> Members

        public bool Equals(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> other)
        {
            return
                (object)other != null
                && ObticsEqualityComparer<TFirst>.Default.Equals(this._First, other._First)
                && ObticsEqualityComparer<TSecond>.Default.Equals(this._Second, other._Second)
                && ObticsEqualityComparer<TThird>.Default.Equals(this._Third, other._Third)
                && ObticsEqualityComparer<TFourth>.Default.Equals(this._Fourth, other._Fourth)
                && ObticsEqualityComparer<TFifth>.Default.Equals(this._Fifth, other._Fifth);
        }

        #endregion
    }
}
