using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Obtics.Collections
{
    internal abstract class SortedSkiplist<N,K> : IList<N> where N : SortedSkiplist<N,K>.Node
    {
        internal struct Noderef
        {
            internal N _Node;
            internal int _Distance;
        }

        internal class Node
        {
            internal Noderef[] _Neighbours;
        }

        protected Noderef[] _First = new Noderef[32];
        protected int _Count;
        protected int _MaxHeight;

        [ThreadStatic]
        static Random _Randomizer;

        static Random Randomizer
        { get { return _Randomizer ?? (_Randomizer = new Random()); } }

        protected abstract K SelectKey(N node);
        protected abstract int Compare(K first, K second);

        #region IEnumerable<N> Members

        public IEnumerator<N> GetEnumerator()
        {
            for (var currentRef = _First[0]; currentRef._Node != null; currentRef = currentRef._Node._Neighbours[0])
                yield return currentRef._Node;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        int DistanceToEnd(Noderef[] currentNB, int currentDistance)
        {
            var testedRef = currentNB[currentNB.Length - 1];

            if (testedRef._Node == null)
                return currentDistance + testedRef._Distance;
            else
                return DistanceToEnd(testedRef._Node._Neighbours, currentDistance + testedRef._Distance);
        }

        public int IndexOf(N node)
        {
            if (node._Neighbours == null)
                throw new ArgumentException();

            return _Count - DistanceToEnd(node._Neighbours, 0);
        }

        public void Insert(int index, N node)
        {
            throw new NotSupportedException();
        }

        void RemoveAt(int offset, Noderef[] currentNB, int currentHeight)
        {
            var testedRef = currentNB[currentHeight];

            if (testedRef._Distance < offset)
                RemoveAt(offset - testedRef._Distance, testedRef._Node._Neighbours, currentHeight);
            else if (testedRef._Distance > offset)
            {
                currentNB[currentHeight] = new Noderef { _Node = testedRef._Node, _Distance = testedRef._Distance - 1 };

                RemoveAt(offset, currentNB, currentHeight - 1);
            }
            else
            {
                var removedRef = testedRef._Node._Neighbours[currentHeight];

                currentNB[currentHeight] = new Noderef { _Node = removedRef._Node, _Distance = testedRef._Distance + removedRef._Distance - 1 };

                if (currentHeight > 0)
                    RemoveAt(offset, currentNB, currentHeight - 1);
                //TEST:
                else
                    testedRef._Node._Neighbours = null;
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= _Count || index < 0)
                throw new IndexOutOfRangeException();

            RemoveAt(index + 1, _First, _MaxHeight - 1);

            --_Count;

            while (_MaxHeight > 0 && _First[_MaxHeight - 1]._Node == null)
                --_MaxHeight;
        }

        N GetItem(int offset, Noderef[] refs, int currentHeight)
        {
            var testedRef = refs[currentHeight];

            if (testedRef._Node == null || testedRef._Distance > offset)
                return GetItem(offset, refs, currentHeight - 1);
            else if (testedRef._Distance < offset)
                return GetItem(offset - testedRef._Distance, testedRef._Node._Neighbours, currentHeight);
            else
                return testedRef._Node;
        }

        public N this[int index]
        {
            get
            {
                if (index < 0 || index >= _Count)
                    throw new IndexOutOfRangeException();

                return GetItem(index + 1, _First, _MaxHeight - 1);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        int FindFirst(K key, Noderef[] currentNeighbours, int currentHeight, out N node)
        {
            var testedRef = currentNeighbours[currentHeight];

            int comp = testedRef._Node == null ? 1 : Compare(SelectKey(testedRef._Node), key);

            if (comp < 0)
                return FindFirst(key, testedRef._Node._Neighbours, currentHeight, out node) + testedRef._Distance;
            else if (testedRef._Distance == 1)
            {
                node = comp == 0 ? testedRef._Node : null;
                return 1;
            }
            else
                return FindFirst(key, currentNeighbours, currentHeight - 1, out node);
        }

        public int FindFirst(K key, out N value)
        {
            if (_MaxHeight == 0)
            {
                value = null;
                return -1;
            }

            var ix = FindFirst(key, _First, _MaxHeight - 1, out value);

            return value == null ? -1 : ix - 1;
        }

        public int Find(K key, Func<N, bool> test, out N value)
        {
            N node;
            int x = FindFirst(key, out node);
            if (node != null)
            {
                do
                {
                    if (test(node))
                    {
                        value = node;
                        return x;
                    }
                        
                    node = node._Neighbours[0]._Node;
                    x += 1;
                }
                while (node != null && Compare(key, SelectKey(node)) == 0);
            }

            value = null;
            return -1;
        }

        public IEnumerator<N> FindAll(K key)
        {
            N node;
            FindFirst(key, out node);
            if (node != null)
            {
                do
                {
                    yield return node;
                    node = node._Neighbours[0]._Node;
                }
                while (node != null && Compare(key, SelectKey(node)) == 0);
            }
        }

        #region ICollection<N> Members

        int Add(N node, K key, Noderef[] currentNeighbours, int currentHeight)
        {
            var testedRef = currentNeighbours[currentHeight];

            if (testedRef._Node != null && Compare(SelectKey(testedRef._Node), key) < 0)
                return Add(node, key, testedRef._Node._Neighbours, currentHeight) + testedRef._Distance;
            else if (currentHeight >= node._Neighbours.Length)
            {
                currentNeighbours[currentHeight]._Distance += 1;
                return Add(node, key, currentNeighbours, currentHeight - 1);
            }
            else
            {
                int distance = currentHeight == 0 ? 1 : Add(node, key, currentNeighbours, currentHeight - 1);

                node._Neighbours[currentHeight] = new Noderef { _Node = testedRef._Node, _Distance = testedRef._Distance - distance + 1 };
                currentNeighbours[currentHeight] = new Noderef { _Node = node, _Distance = distance };

                return distance;
            }
        }

        public int AddWithIndex(N item)
        {
            var rnd = Randomizer.Next();

            var height = 1;

            for (; height < 32; ++height)
            {
                if ((rnd & 1) == 0)
                    break;

                rnd >>= 1;
            }

            if (height > _MaxHeight)
            {
                for (int i = _MaxHeight; i < height; ++i)
                    _First[i] = new Noderef { _Node = null, _Distance = _Count + 1 };

                _MaxHeight = height;
            }

            item._Neighbours = new Noderef[height];

            int ix = Add(item, SelectKey(item), _First, _MaxHeight - 1);

            ++_Count;

            return ix -1;
        }

        public void Add(N item)
        { AddWithIndex(item); }


        public void Clear()
        {
            _First = new Noderef[32];
            _Count = 0;
            _MaxHeight = 0;
        }

        public bool Contains(N item)
        { throw new NotSupportedException(); }

        public void CopyTo(N[] array, int arrayIndex)
        {
            foreach (var n in this)
                array[arrayIndex++] = n;
        }

        public int Count
        { get { return _Count; } }

        public bool IsReadOnly
        { get { return false; } }

        public bool Remove(N item)
        {
            RemoveWithIndex(item);
            return true;
        }

        public int RemoveWithIndex(N item)
        {
            int index = IndexOf(item);
            RemoveAt(index);
            return index;
        }

        #endregion

    }
}
