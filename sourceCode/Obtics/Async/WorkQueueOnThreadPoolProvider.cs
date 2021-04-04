using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Async.WorkQueueAdapters;

namespace Obtics.Async
{
    /// <summary>
    /// <see cref="IWorkQueueProvider"/> that provides an <see cref="IWorkQueue"/> based on <see cref="System.Threading.ThreadPool"/>.
    /// </summary>
    public class WorkQueueOnThreadPoolProvider : IWorkQueueProvider, IPrioritizedWorkQueueProvider
    {
        static IPrioritizedWorkQueueProvider _Adapter;

        static IPrioritizedWorkQueueProvider Adapter
        {
            get
            {
                if (_Adapter == null)
                {
                    _Adapter =
                        HighLowPrioritizedWorkQueueAdapter.Get(
                            WorkQueueOnThreadPoolAdapter.Get()
                        );
                }

                return _Adapter;
            }
        }

        #region IWorkQueueProvider Members

        /// <summary>
        /// Gets the default <see cref="IWorkQueue"/> for this adapter.
        /// </summary>
        /// <returns>The default <see cref="IWorkQueue"/> for this adapter.</returns>
        /// <remarks>The returned <see cref="IWorkQueue"/> is the work queue with priority == 2; The highest priority.</remarks>
        public IWorkQueue GetWorkQueue()
        { return Adapter[2]; }

        #endregion

        #region IPrioritizedWorkQueueProvider Members

        /// <summary>
        /// Gives the total number of prioritized <see cref="IWorkQueue"/> objects provided by this <see cref="IPrioritizedWorkQueueProvider"/>.
        /// </summary>
        /// <remarks>This number is 2 for this provider.</remarks>
        public int Count
        { get { return Adapter.Count; } }

        /// <summary>
        /// Returns an <see cref="IWorkQueue"/> with specified priority. 
        /// </summary>
        /// <param name="priority">Priority of the <see cref="IWorkQueue"/> to get. This priority can be 1 or 2.</param>
        /// <returns>The requested <see cref="IWorkQueue"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The given <paramref name="priority"/> is other than 1 or 2.</exception>
        /// <remarks>This priority can be 1 or 2.</remarks>
        public IWorkQueue this[int priority]
        { get { return Adapter[priority]; } }

        #endregion
    }
}
