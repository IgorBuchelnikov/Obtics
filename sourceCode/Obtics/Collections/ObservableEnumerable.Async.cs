
using Obtics.Collections.Transformations;
using System.Collections.Generic;
using Obtics.Async;
namespace Obtics.Collections
{
    //Explicitly observable object linq

    public static partial class ObservableEnumerable
    {
        /// <summary>
        /// Buffers contents of the source <see cref="IEnumerable{TType}"/> and propagates changes asynchronously using a specified work queue.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of the input and result sequences.</typeparam>
        /// <param name="source">The sequence to buffer.</param>
        /// <param name="workQueue">The <see cref="IWorkQueue"/> to use.</param>
        /// <returns>An <see cref="IEnumerable{TType}"/>, that with respect to some asynchronous delay has the same contents as <paramref name="source"/>, or null when either <paramref name="source"/> or <paramref name="workQueue"/> is null.</returns>
        public static IEnumerable<TType> Async<TType>(this IEnumerable<TType> source, IWorkQueue workQueue)
        { return AsyncTransformation<TType>.Create(source, workQueue); }

        /// <summary>
        /// Buffers contents of the source <see cref="IEnumerable{TType}"/> and propagates changes asynchronously using the default work queue.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of the input and result sequences.</typeparam>
        /// <param name="source">The sequence to buffer.</param>
        /// <returns>An <see cref="IEnumerable{TType}"/>, that with respect to some asynchronous delay has the same contents as <paramref name="source"/>, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>The default work queue can be configured using the 'DefaultWorkQueueProvider' attribute of the 'Obtics' configuration section.</remarks>
        /// <seealso cref="WorkQueue.DefaultWorkQueueProvider"/>
        public static IEnumerable<TType> Async<TType>(this IEnumerable<TType> source)
        { return Async(source, WorkQueue.DefaultWorkQueueProvider.GetWorkQueue()); }
    }
}
