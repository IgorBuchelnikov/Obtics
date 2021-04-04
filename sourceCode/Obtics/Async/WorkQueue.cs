using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Async
{
    /// <summary>
    /// Static methods for <see cref="IWorkQueue"/> operations.
    /// </summary>
    public static class WorkQueue
    {
        static IWorkQueueProvider _DefaultWorkQueueProvider = (IWorkQueueProvider)Obtics.Configuration.ObticsConfigurationSection.GetSection().GetDefaultWorkQueueProviderInstance();

        /// <summary>
        /// Returns the default <see cref="IWorkQueueProvider"/> used by optics.
        /// </summary>
        /// <remarks>The actual <see cref="IWorkQueueProvider"/> returned can be configured
        /// using the <see cref="Obtics.Configuration.ObticsConfigurationSection"/> configuration class.</remarks>
        public static IWorkQueueProvider DefaultWorkQueueProvider
        { get { return _DefaultWorkQueueProvider; } }
    }
}
