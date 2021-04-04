using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static class Comparer
    {
        class InvertedComparer<TKey> : IComparer<TKey>
        {
            public InvertedComparer(IComparer<TKey> originalComparer)
            { _OriginalComparer = originalComparer; }

            IComparer<TKey> _OriginalComparer;

            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            { return -_OriginalComparer.Compare(x, y); }

            #endregion

            public override bool Equals(object obj)
            { return obj is InvertedComparer<TKey> && ObticsEqualityComparer<IComparer<TKey>>.Default.Equals(((InvertedComparer<TKey>)obj)._OriginalComparer, _OriginalComparer); }

            public override int GetHashCode()
            { return Hasher.CreateFromRef(typeof(InvertedComparer<TKey>)).AddRef(_OriginalComparer, ObticsEqualityComparer<IComparer<TKey>>.Default); }

            public static InvertedComparer<TKey> _Default = new InvertedComparer<TKey>(Comparer<TKey>.Default);
        }

        /// <summary>
        /// Combines two comparers to form one composite comparer.
        /// The first comparer has highest priority.
        /// </summary>
        /// <typeparam name="TKey1">Type of first value to compare.</typeparam>
        /// <typeparam name="TKey2">Type of second value to compare.</typeparam>
        class CombinedComparer<TKey1, TKey2> : IComparer<Tuple<TKey1, TKey2>>
        {
            readonly IComparer<TKey1> _First;
            readonly IComparer<TKey2> _Second;

            public CombinedComparer(IComparer<TKey1> first, IComparer<TKey2> second)
            {
                _First = first;
                _Second = second;
            }

            #region IComparer<TType> Members

            public int Compare(Tuple<TKey1, TKey2> x, Tuple<TKey1, TKey2> y)
            {
                int c = _First.Compare(x.First, y.First);

                if (c == 0)
                    c = _Second.Compare(x.Second, y.Second);

                return c;
            }

            #endregion

            public override bool Equals(object obj)
            {
                var other = obj as CombinedComparer<TKey1, TKey2>;
                return other != null && ObticsEqualityComparer<IComparer<TKey1>>.Default.Equals(other._First, _First) && ObticsEqualityComparer<IComparer<TKey2>>.Default.Equals(other._Second, _Second);
            }

            public override int GetHashCode()
            { return Hasher.CreateFromRef(typeof(CombinedComparer<TKey1, TKey2>)).AddRef(_First, ObticsEqualityComparer<IComparer<TKey1>>.Default).AddRef(_Second, ObticsEqualityComparer<IComparer<TKey2>>.Default); } 
        }

        public static IComparer<TKey> Inverted<TKey>(this IComparer<TKey> original)
        { return new InvertedComparer<TKey>(original); }

        public static IComparer<Tuple<TKey1, TKey2>> Tupled<TKey1, TKey2>(this IComparer<TKey1> c1, IComparer<TKey2> c2)
        { return new CombinedComparer<TKey1, TKey2>(c1, c2); }

        public static IComparer<TKey> DefaultInverted<TKey>() { return InvertedComparer<TKey>._Default; }    
    }

}
