using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Obtics.Async.WorkQueueAdapters
{
    /// <summary>
    /// Creates an <see cref="IWorkQueue"/> based on <see cref="System.Threading.ThreadPool"/>.
    /// </summary>
    public static class WorkQueueOnThreadPoolAdapter
    {
        class InternalWorkQueueOnThreadPoolAdapter : IWorkQueue
        {
            #region IWorkQueue Members

            /// <summary>
            /// Queue a workitem for later execution.
            /// </summary>
            /// <param name="callback">The <see cref="WaitCallback"/> delegate that will be called when the workitem is executed.</param>
            /// <param name="prm">Any extra arguments for the <paramref name="callback"/> delegate.</param>
            /// <remarks>Each queued workitem gets executed once. The order in which the individual queued workitems are executed is undetermined.</remarks>
            public void QueueWorkItem(WaitCallback callback, object prm)
            { System.Threading.ThreadPool.QueueUserWorkItem(callback, prm); }

            #endregion
        }

        static IWorkQueue _Instance = new InternalWorkQueueOnThreadPoolAdapter();

        /// <summary>
        /// Gets the <see cref="IWorkQueue"/>.
        /// </summary>
        /// <returns>The <see cref="IWorkQueue"/>.</returns>
        public static IWorkQueue Get()
        { return _Instance; }
    }
}
