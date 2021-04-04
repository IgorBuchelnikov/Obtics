using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Collections
{
    internal class WeightedSkiplist<N> : IList<N> where N : WeightedSkiplist<N>.Node
    {
        internal struct Noderef
        {
            internal N _Node;
            internal int _Distance;
            internal int _Weight;
        }

        internal class Node
        {
            internal Noderef[] _Neighbours;
        }

        Noderef[] _First = new Noderef[32];
        int _Count;
        int _Weight;
        int _MaxHeight;

        [ThreadStatic]
        static Random _Randomizer ;

        static Random Randomizer
        { get { return _Randomizer ?? (_Randomizer = new Random()); } }

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

        public struct IndexAndWeight
        {
            public int _Index;
            public int _Weight;
        }

        IndexAndWeight DistanceToEnd(Noderef[] currentNB, int currentDistance, int currentWeight)
        {
            var testedRef = currentNB[currentNB.Length - 1];

            if (testedRef._Node == null)
                return new IndexAndWeight { _Index = currentDistance + testedRef._Distance, _Weight = currentWeight + testedRef._Weight };
            else
                return DistanceToEnd(testedRef._Node._Neighbours, currentDistance + testedRef._Distance, currentWeight + testedRef._Weight);
        }

        public int IndexOf(N node)
        {
            if (node._Neighbours == null)
                throw new ArgumentException();

            return _Count - DistanceToEnd(node._Neighbours, 0, 0)._Index;
        }

        public int WeightAt(N node)
        {
            if (node._Neighbours == null)
                throw new ArgumentException();

            return _Weight - DistanceToEnd(node._Neighbours, 0, 0)._Weight;
        }

        public IndexAndWeight IndexOfAndWeightAt(N node)
        {
            if (node._Neighbours == null)
                throw new ArgumentException();

            var distanceToEnd = DistanceToEnd(node._Neighbours, 0, 0);

            return new IndexAndWeight { _Weight = _Weight - distanceToEnd._Weight, _Index = _Count - distanceToEnd._Index };
        }

        int Insert(N node, int offset, int weight, Noderef[] currentNeighbours, int currentHeight)
        {
            var testedRef = currentNeighbours[currentHeight];


            if (testedRef._Distance < offset)
            {
                return Insert(node, offset - testedRef._Distance, weight, testedRef._Node._Neighbours, currentHeight) + testedRef._Weight;
            }
            else if (currentHeight >= node._Neighbours.Length)
            {
                currentNeighbours[currentHeight] = new Noderef { _Node = testedRef._Node, _Distance = testedRef._Distance + 1, _Weight = testedRef._Weight + weight };

                return Insert(node, offset, weight, currentNeighbours, currentHeight - 1);
            }
            else
            {
                var weightTillInsertPoint = currentHeight != 0 ? Insert(node, offset, weight, currentNeighbours, currentHeight - 1) : 0;

                node._Neighbours[currentHeight] = new Noderef { _Node = testedRef._Node, _Distance = testedRef._Distance - offset + 1, _Weight = testedRef._Weight - weightTillInsertPoint };

                currentNeighbours[currentHeight] = new Noderef { _Node = node, _Distance = offset, _Weight = weightTillInsertPoint + weight };

                return weightTillInsertPoint;
            }
        }

        public void Insert(int index, N node, int weight)
        {
            if (index > _Count || index < 0)
                throw new IndexOutOfRangeException();

            CreateNeighbourRefs(node);

            Insert(node, index + 1, weight, _First, _MaxHeight - 1);

            ++_Count;
            _Weight += weight;
        }

        public void Insert(int index, N node)
        { Insert(index, node, 0); }

        private void CreateNeighbourRefs(N node)
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
                    _First[i] = new Noderef { _Node = null, _Distance = _Count + 1, _Weight = _Weight };

                _MaxHeight = height;
            }

            node._Neighbours = new Noderef[height];
        }

        public struct WeightInfo
        {
            public N Node;
            public int WeightOffset;
            public int Weight;
            public int NodeIndex;
        }

        WeightInfo AtWeightOffset(int weightOffset, Noderef[] currentNB, int currentHeight, int currentDistance, int currentWeight)
        {
            var testedRef = currentNB[currentHeight];

            if(testedRef._Weight < weightOffset)
                return AtWeightOffset(weightOffset - testedRef._Weight, testedRef._Node._Neighbours, currentHeight, currentDistance + testedRef._Distance, currentWeight + testedRef._Weight);
            else if (currentHeight == 0)
            {
                return
                    new WeightInfo
                    {
                        Node = testedRef._Node,
                        WeightOffset = currentWeight + testedRef._Weight,
                        Weight = testedRef._Weight,
                        NodeIndex = currentDistance + 1
                    }
                ;
            }
            else
                return AtWeightOffset(weightOffset, currentNB, currentHeight - 1, currentDistance, currentWeight);
        }

        public WeightInfo AtWeightOffset(int weightOffset)
        {
            if (weightOffset < 0 || weightOffset > _Weight)
                throw new ArgumentException("weightOffset");

            if (_Count > 0)
                return AtWeightOffset(weightOffset, _First, _MaxHeight - 1, -1, 0);
            else
                return
                    new WeightInfo
                    {
                        Weight = _Weight,
                        WeightOffset = _Weight
                    }
                ;
        }

        int Remove(int offset, Noderef[] currentNB, int currentHeight)
        {
            var testedRef = currentNB[currentHeight];

            if (testedRef._Distance < offset)
                return Remove(offset - testedRef._Distance, testedRef._Node._Neighbours, currentHeight);
            else if (testedRef._Distance > offset)
            {
                var removedWeight = Remove(offset, currentNB, currentHeight - 1);

                currentNB[currentHeight] = new Noderef { _Node = testedRef._Node, _Distance = testedRef._Distance - 1, _Weight = testedRef._Weight - removedWeight };

                return removedWeight;
            }
            else
            {
                var removedRef = testedRef._Node._Neighbours[currentHeight];
                var removedWeight = currentHeight > 0 ? Remove(offset, currentNB, currentHeight - 1) : testedRef._Weight;

                currentNB[currentHeight] = new Noderef { _Node = removedRef._Node, _Distance = testedRef._Distance + removedRef._Distance - 1, _Weight = testedRef._Weight + removedRef._Weight - removedWeight };

                return removedWeight;
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= _Count || index < 0)
                throw new IndexOutOfRangeException();

            _Weight -= Remove(index + 1, _First, _MaxHeight - 1);

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

        int IncreaseWeightAt(int offset, Noderef[] currentNeighbours, int currentHeight, int increase)
        {
            var testedRef = currentNeighbours[currentHeight];

            if (testedRef._Distance < offset)
                return IncreaseWeightAt(offset - testedRef._Distance, testedRef._Node._Neighbours, currentHeight, increase);
            else
            {
                currentNeighbours[currentHeight]._Weight += increase;
                return currentHeight > 0 ? IncreaseWeightAt(offset, currentNeighbours, currentHeight - 1, increase) : currentNeighbours[currentHeight]._Weight;
            }
        }

        public void IncreaseWeightAt(int index, int ammount)
        {
            if (index > _Count || index < 0)
                throw new IndexOutOfRangeException();

            if(_Count > 0)
                IncreaseWeightAt(index + 1, _First, _MaxHeight - 1, ammount);

            _Weight += ammount;
        }

        int SetWeightAt(int offset, Noderef[] currentNeighbours, int currentHeight, int weight)
        {
            var testedRef = currentNeighbours[currentHeight];

            if (testedRef._Distance < offset)
                return SetWeightAt(offset - testedRef._Distance, testedRef._Node._Neighbours, currentHeight, weight);
            else
            {
                var weightDiff = currentHeight > 0 ? SetWeightAt(offset, currentNeighbours, currentHeight - 1, weight) : (weight - testedRef._Weight);
                currentNeighbours[currentHeight]._Weight += weightDiff;
                return weightDiff;
            }
        }

        public void SetWeightAt(int index, int weight)
        {
            if (index > _Count || index < 0)
                throw new IndexOutOfRangeException();

            if (_Count > 0)
                _Weight += SetWeightAt(index + 1, _First, _MaxHeight - 1, weight);
            else
                _Weight = weight;
        }

        #region ICollection<N> Members

        public void Add(N item)
        { Insert(_Count, item); }

        public void Add(N item, int weight)
        { Insert(_Count, item, weight); }

        public void Clear()
        {
            _First = new Noderef[32];
            _Count = 0;
            _Weight = 0;
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

        public int Weight
        { get { return _Weight; } }

        public bool IsReadOnly
        { get { return false; } }

        public bool Remove(N item)
        { throw new NotImplementedException(); }

        #endregion

    }
}
