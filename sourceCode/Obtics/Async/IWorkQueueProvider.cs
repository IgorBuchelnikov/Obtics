using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Async
{
    /// <summary>
    /// Interface for an <see cref="IWorkQueue"/> provider.
    /// </summary>
    public interface IWorkQueueProvider
    {
        /// <summary>
        /// Gets a work queue
        /// </summary>
        /// <returns>An <see cref="IWorkQueue"/> implementation that can be used to queue workitems for delayed processing.</returns>
        /// <remarks>This method should be thread safe.
        /// Depending on the context each call may return a different <see cref="IWorkQueue"/> object. In a multithreaded environment,
        /// for example, the returned work queue may be a different one for each thread.
        /// </remarks>
        IWorkQueue GetWorkQueue();
    }
}
