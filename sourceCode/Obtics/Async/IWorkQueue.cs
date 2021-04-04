using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Obtics.Async
{
    /// <summary>
    /// Interface for a work pool that can be used by Obtics 
    /// </summary>
    public interface IWorkQueue
    {
        /// <summary>
        /// Queue a workitem for later execution.
        /// </summary>
        /// <param name="callback">The <see cref="WaitCallback"/> delegate that will be called when the workitem is executed.</param>
        /// <param name="callbackArgument">Any extra arguments for the <paramref name="callback"/> delegate.</param>
        /// <remarks>Each queued workitem gets executed once. The order in which the individual queued workitems are executed is undetermined.</remarks>
        void QueueWorkItem(WaitCallback callback, object callbackArgument); 
    }
}
