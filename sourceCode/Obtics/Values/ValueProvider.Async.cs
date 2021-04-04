using Obtics.Values.Transformations;
using Obtics.Async;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Buffers the value of the source <see cref="IValueProvider{TType}"/> and propagates changes asynchronously using a specified work queue.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of the input and result value providers.</typeparam>
        /// <param name="source">The value provider to buffer.</param>
        /// <param name="workQueue">The <see cref="IWorkQueue"/> to use.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/>, that with respect to some asynchronous delay has the same value as <paramref name="source"/>, or null when either <paramref name="source"/> or <paramref name="workQueue"/> is null.</returns>
        public static IValueProvider<TType> Async<TType>(this IValueProvider<TType> source, IWorkQueue workQueue)
        { return _Async(source.Patched(), workQueue).Concrete(); }

        internal static IInternalValueProvider<TType> _Async<TType>(this IInternalValueProvider<TType> source, IWorkQueue workQueue)
        { return BufferTransformation<TType>.Create(source, workQueue); }

        /// <summary>
        /// Buffers the value of the source <see cref="IValueProvider{TType}"/> and propagates changes asynchronously using the default work queue.
        /// </summary>
        /// <typeparam name="TType">The type of the elements of the input and result sequences.</typeparam>
        /// <param name="source">The value provider to buffer.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/>, that with respect to some asynchronous delay has the same value as <paramref name="source"/>, or null when <paramref name="source"/> is null.</returns>
        /// <remarks>The default work queue can be configured using the 'DefaultWorkQueueProvider' attribute of the 'Obtics' configuration section.</remarks>
        /// <seealso cref="WorkQueue.DefaultWorkQueueProvider"/>
        public static IValueProvider<TType> Async<TType>(this IValueProvider<TType> source)
        { return Async(source, WorkQueue.DefaultWorkQueueProvider.GetWorkQueue()); }
    }
}
