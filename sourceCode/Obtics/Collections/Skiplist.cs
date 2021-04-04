using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Collections
{
    class Skiplist<N> : IList<N> where N : Skiplist<N>.Node
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

        Noderef[] _First = new Noderef[32];
        int _Count;
        int _MaxHeight;

        [ThreadStatic]
        static Random _Randomizer;

        static Random Randomizer
        { get{ return _Randomizer ?? (_Randomizer = new Random()); } }

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

        void Insert(N node, int offset, Noderef[] currentNeighbours, int currentHeight)
        {
            var testedRef = currentNeighbours[currentHeight];

            if (testedRef._Distance < offset)
                Insert(node, offset - testedRef._Distance,testedRef._Node._Neighbours, currentHeight);
            else if (currentHeight >= node._Neighbours.Length)
            {
                currentNeighbours[currentHeight]._Distance += 1;
                Insert(node, offset, currentNeighbours, currentHeight - 1);
            }
            else
            {
                node._Neighbours[currentHeight] = new Noderef { _Node = testedRef._Node, _Distance = testedRef._Distance - offset + 1 };
                currentNeighbours[currentHeight] = new Noderef { _Node = node, _Distance = offset };

                if (currentHeight != 0)
                    Insert(node, offset, currentNeighbours, currentHeight - 1);
            }
        }

        public void Insert(int index, N node)
        {
            if (index > _Count || index < 0)
                throw new IndexOutOfRangeException();

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

            node._Neighbours = new Noderef[height];

            Insert(node, index + 1, _First, _MaxHeight - 1);

            ++_Count;
        }


        void Remove(int offset, Noderef[] currentNB, int currentHeight)
        {
            var testedRef = currentNB[currentHeight];

            if (testedRef._Distance < offset)
                Remove(offset - testedRef._Distance, testedRef._Node._Neighbours, currentHeight);
            else if (testedRef._Distance > offset)
            {                
                currentNB[currentHeight] = new Noderef { _Node = testedRef._Node, _Distance = testedRef._Distance - 1 };

                Remove(offset, currentNB, currentHeight - 1);
            }
            else
            {
                var removedRef = testedRef._Node._Neighbours[currentHeight];

                currentNB[currentHeight] = new Noderef { _Node = removedRef._Node, _Distance = testedRef._Distance + removedRef._Distance - 1 };

                if (currentHeight > 0)
                    Remove(offset, currentNB, currentHeight - 1);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= _Count || index < 0)
                throw new IndexOutOfRangeException();

            Remove(index + 1, _First, _MaxHeight - 1);

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
                RemoveAt(index);
                Insert(index, value);
            }
        }


        #region ICollection<N> Members

        public void Add(N item)
        { Insert(_Count, item); }

        public void Clear()
        {
            _First = new Noderef[32];
            _Count = 0;
            _MaxHeight = 0;
        }

        public bool Contains(N item)
        {
            return Enumerable.Contains(this, item);
        }

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
        { throw new NotImplementedException(); }

        #endregion

    }

    class LookupHybridList<N> : IList<N> where N : Skiplist<N>.Node
    {
        IList<N> _Implementation = new List<N>();

        #region IList<N> Members

        public int IndexOf(N item)
        {
            return _Implementation.IndexOf(item);
        }

        //treshold value depends heavily on how list is used.
        //purely inserts and deletes has an optimum treshold of about 2000
        //doing 2 lookups (IndexOf) for each insert and delete combination
        //already gives an optimum of about 200. 
        //Here 1000 +/- 200 is choosen. Optimization may still be needed

        void TestGrow(int ammount)
        {
            var count = _Implementation.Count;

            if (count > 1200 && count - ammount <= 1200 && _Implementation is List<N>)
            {
                var newList = new Skiplist<N>();

                foreach (var node in _Implementation)
                    newList.Add(node);

                _Implementation = newList;
            }
        }

        void TestShrink(int ammount)
        {
            var count = _Implementation.Count;

            if (count < 800 && count + ammount >= 800 && _Implementation is Skiplist<N>)
            {
                var newList = new List<N>();

                newList.AddRange(_Implementation);

                _Implementation = newList;
            }
        }

        public void Insert(int index, N item)
        {
            _Implementation.Insert(index, item);
            TestGrow(1);
        }

        void InsertRange(int index, IEnumerable<N> items)
        {
            var itemsList = new List<N>(items);

            var impAsList = _Implementation as List<N>;

            if (impAsList != null)
                impAsList.InsertRange(index, itemsList);
            else
                for (int i = 0, end = itemsList.Count; i < end; ++i)
                    _Implementation.Insert(index + i, itemsList[i]);

            TestGrow(itemsList.Count);
        }

        public void RemoveAt(int index)
        {
            _Implementation.RemoveAt(index);
            TestShrink(1);
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            if (index + count > Count)
                throw new ArgumentOutOfRangeException("count");

            var impAsList = _Implementation as List<N>;

            if (impAsList != null)
                impAsList.RemoveRange(index, count);
            else
                for (int i = 0; i < count; ++i)
                    _Implementation.RemoveAt(index);

            TestShrink(count);
        }

        public N this[int index]
        {
            get
            {
                return _Implementation[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<N> Members

        public void Add(N item)
        {
            _Implementation.Add(item);
            TestGrow(1);
        }

        public void Clear()
        {
            _Implementation = new List<N>();
        }

        public bool Contains(N item)
        {
            return _Implementation.Contains(item);
        }

        public void CopyTo(N[] array, int arrayIndex)
        { _Implementation.CopyTo(array, arrayIndex); }

        public int Count
        { get { return _Implementation.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        public bool Remove(N item)
        { throw new NotImplementedException(); }

        #endregion

        #region IEnumerable<N> Members

        public IEnumerator<N> GetEnumerator()
        { return _Implementation.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return ((System.Collections.IEnumerable)_Implementation).GetEnumerator(); }

        #endregion
    }

    class ValueHybridList<V> : IList<V> 
    {
        class ValueSkiplist : Skiplist<ValueSkiplistNode<V>>, IList<V>
        {
            #region IList<V> Members

            int IList<V>.IndexOf(V item)
            {
                var x = 0;
                var c = ObticsEqualityComparer<V>.Default;

                foreach (var n in this)
                    if (c.Equals(n.Value,item))
                        return x;
                    else
                        ++x;

                return -1;
            }

            void IList<V>.Insert(int index, V item)
            {
                this.Insert(index, new ValueSkiplistNode<V> { Value = item });
            }

            void IList<V>.RemoveAt(int index)
            {
                this.RemoveAt(index);
            }

            V IList<V>.this[int index]
            {
                get
                {
                    return this[index].Value;
                }
                set
                {
                    this[index].Value = value;
                }
            }

            #endregion

            #region ICollection<V> Members

            void ICollection<V>.Add(V item)
            {
                this.Add(new ValueSkiplistNode<V> { Value = item });
            }

            void ICollection<V>.Clear()
            {
                this.Clear();
            }

            bool ICollection<V>.Contains(V item)
            {
                return ((IList<V>)this).IndexOf(item) != -1;
            }

            void ICollection<V>.CopyTo(V[] array, int arrayIndex)
            {
                Enumerable.ToList(Enumerable.Select(this, (ValueSkiplistNode<V> n) => n.Value)).CopyTo(array, arrayIndex);
            }

            int ICollection<V>.Count
            {
                get { return this.Count; }
            }

            bool ICollection<V>.IsReadOnly
            {
                get { return this.IsReadOnly; }
            }

            bool ICollection<V>.Remove(V item)
            {
                var index = ((IList<V>)this).IndexOf(item);

                if (index != -1)
                {
                    this.RemoveAt(index);
                    return true;
                }
                else
                    return false;
            }

            #endregion

            #region IEnumerable<V> Members

            IEnumerator<V> IEnumerable<V>.GetEnumerator()
            {
                foreach (var n in this)
                    yield return n.Value;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<V>)this).GetEnumerator();
            }

            #endregion
        }

        IList<V> _Implementation = new List<V>();

        #region IList<V> Members

        public int IndexOf(V item)
        {
            return _Implementation.IndexOf(item);
        }

        //treshold value depends heavily on how list is used.
        //purely inserts and deletes has an optimum treshold of about 3400

        void TestGrow(int ammount)
        {
            var count = _Implementation.Count;

            if (count > 3400 && count - ammount <= 3400 && _Implementation is List<V>)
            {
                IList<V> newList = new ValueSkiplist();

                foreach (var node in _Implementation)
                    newList.Add(node);

                _Implementation = newList;
            }
        }

        void TestShrink(int ammount)
        {
            var count = _Implementation.Count;

            if (count < 3000 && count + ammount >= 3000 && _Implementation is ValueSkiplist)
            {
                var newList = new List<V>();

                newList.AddRange(_Implementation);

                _Implementation = newList;
            }
        }

        public void Insert(int index, V item)
        {
            _Implementation.Insert(index, item);
            TestGrow(1);
        }

        public void InsertRange(int index, IEnumerable<V> items)
        {
            var itemsList = new List<V>(items);

            var impAsList = _Implementation as List<V>;

            if (impAsList != null)
                impAsList.InsertRange(index, itemsList);
            else
                for (int i = 0, end = itemsList.Count; i < end; ++i)
                    _Implementation.Insert(index + i, itemsList[i]);

            TestGrow(itemsList.Count);
        }

        public void RemoveAt(int index)
        {
            _Implementation.RemoveAt(index);
            TestShrink(1);
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            if (index + count > Count)
                throw new ArgumentOutOfRangeException("count");

            var impAsList = _Implementation as List<V>;

            if (impAsList != null)
                impAsList.RemoveRange(index, count);
            else
                for (int i = 0; i < count; ++i)
                    _Implementation.RemoveAt(index);

            TestShrink(count);
        }

        public V this[int index]
        {
            get
            {
                return _Implementation[index];
            }
            set
            {
                _Implementation[index] = value;
            }
        }

        #endregion

        #region ICollection<N> Members

        public void Add(V item)
        {
            _Implementation.Add(item);
            TestGrow(1);
        }

        public void Clear()
        {
            _Implementation = new List<V>();
        }

        public bool Contains(V item)
        {
            return _Implementation.Contains(item);
        }

        public void CopyTo(V[] array, int arrayIndex)
        { _Implementation.CopyTo(array, arrayIndex); }

        public int Count
        { get { return _Implementation.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        public bool Remove(V item)
        { throw new NotImplementedException(); }

        #endregion

        #region IEnumerable<V> Members

        public IEnumerator<V> GetEnumerator()
        { return _Implementation.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return ((System.Collections.IEnumerable)_Implementation).GetEnumerator(); }

        #endregion
    }

    class ValueSkiplistNode<V> : Skiplist<ValueSkiplistNode<V>>.Node
    {
        public V Value;
    }
}
