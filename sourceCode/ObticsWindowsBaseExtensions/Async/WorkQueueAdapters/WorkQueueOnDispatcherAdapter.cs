using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace Obtics.Async.WorkQueueAdapters
{
    /// <summary>
    /// Creates an <see cref="IWorkQueue"/> based on a given <see cref="Dispatcher"/> .
    /// </summary>
    public static class WorkQueueOnDispatcherAdapter
    {
        class InternalWorkQueueOnDispatcherAdapter : IWorkQueue
        {
            internal InternalWorkQueueOnDispatcherAdapter(Tuple<Dispatcher, DispatcherPriority> prms)
            {
                _Prms = prms;
            }

            readonly Tuple<Dispatcher, DispatcherPriority> _Prms;

            #region IWorkQueue Members

            /// <summary>
            /// Queue a workitem for later execution.
            /// </summary>
            /// <param name="callback">The <see cref="WaitCallback"/> delegate that will be called when the workitem is executed.</param>
            /// <param name="prm">Any extra arguments for the <paramref name="callback"/> delegate.</param>
            /// <remarks>Each queued workitem gets executed once. The order in which the individual queued workitems are executed is undetermined.</remarks>
            public void QueueWorkItem(WaitCallback callback, object prm)
            { _Prms.First.BeginInvoke(_Prms.Second, callback, prm); }

            #endregion
        }

        /// <summary>
        /// Aquires an <see cref="IWorkQueue"/> based on the given <see cref="Dispatcher"/> and <see cref="DispatcherPriority"/>.
        /// </summary>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> to base the <see cref="IWorkQueue"/> on.</param>
        /// <param name="priority">The <see cref="DispatcherPriority"/> to base the <see cref="IWorkQueue"/> on.</param>
        /// <returns>An <see cref="IWorkQueue"/> whose work items get serviced by invokations from <paramref name="dispatcher"/> at priority <paramref name="priority"/>.</returns>
        /// <remarks>Each unique combination yields one <see cref="IWorkQueue"/> and one <see cref="IWorkQueue"/> only.</remarks>
        public static IWorkQueue Get(Dispatcher dispatcher, DispatcherPriority priority)
        { return Carrousel.Get(Tuple.Create(dispatcher, priority), t => new InternalWorkQueueOnDispatcherAdapter(t)); }

        /// <summary>
        /// Aquires an <see cref="IWorkQueue"/> based on the CurrentDispatcher and Normal priority.
        /// </summary>
        /// <returns></returns>
        public static IWorkQueue Get()
        { return Get(Dispatcher.CurrentDispatcher, DispatcherPriority.Normal); }
    }
}
