using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObticsUnitTest.Helpers
{
    internal class AsyncFrameIEnumerableRunner<TType>
    {
        TType[] _Items;
        FrameIEnumerable<TType> _Frame;
        bool _Running;
        bool _Stop;


        public AsyncFrameIEnumerableRunner(TType[] items, FrameIEnumerable<TType> frame)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (frame == null)
                throw new ArgumentNullException("frame");

            if (items.Length < 13)
                throw new ArgumentException();

            _Items = items;
            _Frame = frame;
            _Running = false;
            _Stop = false;
        }

        public void Start()
        {
            lock (this)
            {
                _Stop = false;

                if (!_Running)
                {
                    _Running = true;
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Loop));
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                _Stop = true;
            }
        }

        public bool IsRunning
        {
            get
            {
                lock (this)
                    return _Running;
            }
        }

        void Loop(object dummy)
        {
            while (true)
            {
                Iteration();

                lock (this)
                {
                    if (_Stop)
                    {
                        _Running = false;
                        return;
                    }
                }
            }
        }

        void Iteration()
        {
            var list = new List<TType>( _Items );

            _Frame.ResetItems(new List<TType>());
            _Frame.AddItems(list.GetRange(0, 3), 0); //0,1,2
            _Frame.AddItems(list.GetRange(6, 3), 3); //0,1,2,6,7,8
            _Frame.AddItems(list.GetRange(3, 3), 3); //0,1,2,3,4,5,6,7,8
            _Frame.ReplaceItems(list.GetRange(9, 2), 3, 2); //0,1,9,10,5,6,7,8
            _Frame.ReplaceItems(list.GetRange(11, 2), 1, 7); //0,1,9,10,5,6,7,11,12
            _Frame.MoveItems(2, 3, 0); //9,10,5,0,1,6,7,11,12
            _Frame.MoveItems(4, 5, 2); //9,10,7,11,12,5,0,1,6
            _Frame.MoveItems(2, 0, 7); //1,6,9,10,7,11,12,5,0
            _Frame.RemoveItems(1, 5); //1,6,9,10,7,12,5,0
            _Frame.ReplaceItems(list.GetRange(0, 3), 2, 0); //0,1,2,9,10,7,12,5,0
            _Frame.RemoveItems(2, 5); //0,1,2,9,10,5,0
            _Frame.ReplaceItems(list.GetRange(3, 2), 1, 6);//0,1,2,9,10,5,3,4
            _Frame.MoveItems(1, 7, 5); //0,1,2,9,10,3,4,5
            _Frame.RemoveItems(2, 3); //0,1,2,3,4,5
            _Frame.AddItems(list.GetRange(6, 6), 0); //6,7,8,9,10,11,0,1,2,3,4,5
            _Frame.RemoveItems(2, 4);//6,7,8,9,0,1,2,3,4,5
            _Frame.MoveItems(6, 0, 4);//0,1,2,3,4,5,6,7,8,9
        }

    }
}
