using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static partial class Tuple
    {
        public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        { return new Tuple<T1, T2, T3, T4, T5, T6>(a1, a2, a3, a4, a5, a6); }
    }

    internal class Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> : IEquatable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>>
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

        readonly TSixth _Sixth;

        public TSixth Sixth
        { get { return _Sixth; } }

        public Tuple(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth)
        {
            _First = first;
            _Second = second;
            _Third = third;
            _Fourth = fourth;
            _Fifth = fifth;
            _Sixth = sixth;
        }

        public override bool Equals(object obj)
        { return this.Equals(obj as Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>); }

        public override int GetHashCode()
        {
            return
                Hasher
                    .CreateFromRef(typeof(Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>))
                    .Add(_First)
                    .Add(_Second)
                    .Add(_Third)
                    .Add(_Fourth)
                    .Add(_Fifth)
                    .Add(_Sixth);
        }

        public static bool operator ==(Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> a, Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> b)
        { return object.ReferenceEquals(a, b) || !object.ReferenceEquals(a, null) && a.Equals(b); }

        public static bool operator !=(Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> a, Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> b)
        { return !(a == b); }

        #region IEquatable<Tuple<TFirst,TSecond,TThird, TFourth, TFifth, TSixth>> Members

        public bool Equals(Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> other)
        {
            return
                (object)other != null
                && ObticsEqualityComparer<TFirst>.Default.Equals(this._First, other._First)
                && ObticsEqualityComparer<TSecond>.Default.Equals(this._Second, other._Second)
                && ObticsEqualityComparer<TThird>.Default.Equals(this._Third, other._Third)
                && ObticsEqualityComparer<TFourth>.Default.Equals(this._Fourth, other._Fourth)
                && ObticsEqualityComparer<TFifth>.Default.Equals(this._Fifth, other._Fifth)
                && ObticsEqualityComparer<TSixth>.Default.Equals(this._Sixth, other._Sixth);
        }

        #endregion
    }
}
