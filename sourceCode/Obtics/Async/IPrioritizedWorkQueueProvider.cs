using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Async
{
    /// <summary>
    /// Interface for an <see cref="IWorkQueue"/> provider that returns multiple
    /// prioritized <see cref="IWorkQueue"/> objects.
    /// </summary>
    /// <remarks>
    /// Any workitems in a work queue of lower priority will only get serviced
    /// when all workitems in higher priority queues have been serviced. In a 
    /// multithreaded environment it is not guaranteed that the higher priority 
    /// work items have completed when a lower priority work item gets serviced.
    /// </remarks>
    public interface IPrioritizedWorkQueueProvider
    {
        /// <summary>
        /// Gives the total number of priority levels provided by this provider.
        /// </summary>
        int Count{ get; }

        /// <summary>
        /// Gives an <see cref="IWorkQueue"/> from the total work queue collection.
        /// </summary>
        /// <param name="priority">Priority of the requested <see cref="IWorkQueue"/>. This ranges from 1 upto and including <see cref="Count"/>.</param>
        /// <returns>The requested <see cref="IWorkQueue"/>.</returns>
        /// <remarks>Work queues aquired with lower priority indexes have lower priority than the ones
        /// aquired with higher priority indexes.</remarks>
        IWorkQueue this[int priority] { get; }
    }
}
