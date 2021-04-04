using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObticsUnitTest.Helpers
{
    internal class AsynCollectionTransformationClientRunner<TType>
    {
        CollectionTransformationClient<TType>[] _Clients;
        IEnumerable<TType> _Source;
        bool _Running;
        bool _Stop;

        public AsynCollectionTransformationClientRunner(CollectionTransformationClient<TType>[] clients, IEnumerable<TType> source)
        {
            if (clients == null)
                throw new ArgumentNullException("clients");

            if (source == null)
                throw new ArgumentNullException("source");

            if (clients.Length < 1)
                throw new ArgumentException();

            _Clients = clients;
            _Source = source;
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
                Bind(0);

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

        void Bind(int x)
        {
            if (x < _Clients.Length)
            {
                _Clients[x].Source = _Source;
                Bind(x + 1);
                _Clients[x].Source = null;
                Bind(x + 1);
            }
        }
    }
}
