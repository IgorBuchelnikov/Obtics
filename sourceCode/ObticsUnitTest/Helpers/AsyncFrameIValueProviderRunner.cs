using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObticsUnitTest.Helpers
{
    class AsyncFrameIValueProviderRunner<TType>
    {
        TType[] _Items;
        FrameIValueProvider<TType> _Frame;
        bool _Running;
        bool _Stop;

        public AsyncFrameIValueProviderRunner(TType[] items, FrameIValueProvider<TType> frame)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (frame == null)
                throw new ArgumentNullException("frame");

            _Items = items;
            _Frame = frame;
            _Running = false;
            _Stop = false;
            _Frame.Value = items[items.Length - 1];
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
            foreach (TType item in _Items)
                _Frame.Value = item;
        }
    }
}
