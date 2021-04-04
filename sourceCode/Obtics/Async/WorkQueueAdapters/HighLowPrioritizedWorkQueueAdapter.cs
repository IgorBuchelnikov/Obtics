using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Obtics.Async.WorkQueueAdapters
{
    /// <summary>
    /// A work queue adapter that provides a high and low priority queue based on a single parent queue.
    /// </summary>
    public static class HighLowPrioritizedWorkQueueAdapter
    {
        class InternalHighLowPrioritizedWorkQueueAdapter : IPrioritizedWorkQueueProvider
        {
            internal InternalHighLowPrioritizedWorkQueueAdapter(IWorkQueue parent)
            {
                _ParentWorkQueue = parent;
                _HighPriorityDispatcher = new InternalDispatcher(this,2);
                _LowPriorityDispatcher = new InternalDispatcher(this,1);
            }

            class InternalDispatcher : IWorkQueue
            {
                internal InternalDispatcher(InternalHighLowPrioritizedWorkQueueAdapter owner, int priority)
                {
                    _Priority = priority;
                    _Owner = owner;
                }

                internal readonly InternalHighLowPrioritizedWorkQueueAdapter _Owner;
                internal readonly int _Priority;

                #region IWorkQueue Members

                public void QueueWorkItem(WaitCallback callback, object prm)
                { _Owner.QueueWorkItem(callback, prm, _Priority); }

                #endregion
            }

            readonly InternalDispatcher _HighPriorityDispatcher;
            readonly InternalDispatcher _LowPriorityDispatcher;
            readonly IWorkQueue _ParentWorkQueue;
            readonly Queue<Tuple<WaitCallback, object>> _LowPriorityQueue = new Queue<Tuple<WaitCallback, object>>();
            int _HighPriorityRunningJobs;


            void QueueWorkItem(WaitCallback callback, object prm, int priority)
            {
                Lock();

                if (priority == 2)
                {
                    _ParentWorkQueue.QueueWorkItem(InternalHighLowPrioritizedWorkQueueAdapter.InternalWaitCallback, Tuple.Create(this, callback, prm, priority));
                    ++_HighPriorityRunningJobs;
                }
                else if (_HighPriorityRunningJobs == 0) //only when all highprio jobs have finshed will we start low prio jobs
                    _ParentWorkQueue.QueueWorkItem(InternalHighLowPrioritizedWorkQueueAdapter.InternalWaitCallback, Tuple.Create(this, callback, prm, priority));
                else
                    _LowPriorityQueue.Enqueue(Tuple.Create(callback, prm));

                Release();
            }

            static void InternalWaitCallback(object prms)
            {
                var runInfo = (Tuple<InternalHighLowPrioritizedWorkQueueAdapter, WaitCallback, object, int>) prms;

                try
                {
                    runInfo.Second(runInfo.Third);
                }
                finally
                {
                    //only when all highprio jobs have finshed will we start low prio jobs
                    if (runInfo.Fourth == 2)
                    {
                        var t = runInfo.First;

                        t.Lock();

                        try
                        {

                            if (--t._HighPriorityRunningJobs == 0)
                                while (t._LowPriorityQueue.Count > 0)
                                {
                                    var data = t._LowPriorityQueue.Dequeue();
                                    t._ParentWorkQueue.QueueWorkItem(InternalHighLowPrioritizedWorkQueueAdapter.InternalWaitCallback, Tuple.Create(t, data.First, data.Second, 1));
                                }
                        }
                        finally
                        { t.Release(); }
                    }
                }
            }


            Int32 _Token;

            void Lock()
            {
                while (Interlocked.Exchange(ref _Token, 1) != 0)
                    Thread.Sleep(0);
            }

            void Release()
            { Interlocked.Exchange(ref _Token, 0); }

            /// <summary>
            /// The total number of prioritized work queues, which will always be 2 for this object.
            /// </summary>
            public int Count { get { return 2; } }

            /// <summary>
            /// Gives an <see cref="IWorkQueue"/> from the total work queue collection.
            /// </summary>
            /// <param name="priority">Priority of the requested <see cref="IWorkQueue"/>. This can be either 1 or 2.</param>
            /// <returns>The requested <see cref="IWorkQueue"/>.</returns>
            /// <remarks>Work queues aquired with lower priority indexes have lower priority than the ones
            /// aquired with higher priority indexes.</remarks>
            public IWorkQueue this[int priority]
            {
                get
                {
                    if (priority < 1 || priority > 2)
                        throw new ArgumentOutOfRangeException("priority");

                    return priority == 1 ? _LowPriorityDispatcher : _HighPriorityDispatcher ;
                }
            }
        }

        /// <summary>
        /// Aquires a high-low prioritized <see cref="IPrioritizedWorkQueueProvider"/> based on a gives parent <see cref="IWorkQueue"/>.
        /// </summary>
        /// <param name="queue">The parent <see cref="IWorkQueue"/> to base the return high-low prioritized <see cref="IPrioritizedWorkQueueProvider"/> on.</param>
        /// <returns>A high-low prioritized <see cref="IPrioritizedWorkQueueProvider"/>.</returns>
        /// <remarks>The same <see cref="IPrioritizedWorkQueueProvider"/> will always get returned for the same parent <see cref="IWorkQueue"/>.</remarks>
        public static IPrioritizedWorkQueueProvider Get(IWorkQueue queue)
        { return Carrousel.Get(queue, q => new InternalHighLowPrioritizedWorkQueueAdapter(q)); }
    }
}
