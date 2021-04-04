using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Async;

namespace Obtics.Values.Transformations
{
    /// <summary>
    /// Provides asynchronous processing of <see cref="IValueProvider{TType}"/> change propagation.
    /// </summary>
    /// <typeparam name="TType">Type of the Value property</typeparam>
    /// <remarks>
    /// Provides asynchronous processing by using an <see cref="IWorkQueue"/>. The existing value is buffered
    /// and a received change notification from the source is queued in the <see cref="IWorkQueue"/>.
    /// Only when the <see cref="IWorkQueue"/> returns will the change be propagated to our clients.
    /// </remarks>
    internal sealed class BufferTransformation<TType> : ConvertTransformationBase<TType, Tuple<IInternalValueProvider<TType>, IWorkQueue>>
    {
        #region Bitflags

        const Int32 PSVPCNIsPendingMask = 1 << (ConvertTransformationBase<TType, Tuple<IInternalValueProvider<TType>, IWorkQueue>>.BitFlagIndexEnd + 0);
        const Int32 PSIRPCNIsPendingMask = 1 << (ConvertTransformationBase<TType, Tuple<IInternalValueProvider<TType>, IWorkQueue>>.BitFlagIndexEnd + 1);

        #endregion

        #region Constructor

        /// <summary>
        /// returns an existing BufferTransformation with given parameters or constructs a new one 
        /// </summary>
        /// <param name="source">Source <see cref="IValueProvider{TType}"/>.</param>
        /// <param name="workQueue">An <see cref="IWorkQueue"/> object we queue or jobs in for delayed processing.</param>
        /// <returns>The found or newly created <see cref="BufferTransformation{TType}"/>.</returns>
        public static BufferTransformation<TType> Create(IInternalValueProvider<TType> source, IWorkQueue workQueue)
        {
            if (source == null || workQueue == null)
                return null;

            return Carrousel.Get<BufferTransformation<TType>, IInternalValueProvider<TType>, IWorkQueue>(source, workQueue);
        }

        #endregion

#if PARALLEL
        internal override void Initialize(Tuple<IInternalValueProvider<TType>, IWorkQueue> prms)
        {
            base.Initialize(prms);
            FlagsState flags;
            GetFlags(out flags);
            SetParallelizationForbidden(ref flags, true);
            Commit(ref flags);
        }
#endif

        /// <summary>
        /// PSVPCNIsPending (P rocess S ource V alue P roperty C hanged N otification)
        /// True when a job has been scheduled to PSVPCN.
        /// </summary>
        bool GetPSVPCNIsPending(ref FlagsState flags)
        { return flags.GetBitFlag(PSVPCNIsPendingMask); }

        bool SetPSVPCNIsPending(ref FlagsState flags, bool value)
        { return flags.SetBitFlag(PSVPCNIsPendingMask, value); }


        static void PSVPCNCallback(object me)
        { ((BufferTransformation<TType>)me).PSVPCNCallback(); }

        void PSVPCNCallback()
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags));

            SetPSVPCNIsPending(ref flags, false);

            if (!Commit(ref flags))
                goto retry;

            base.SourceChangeEvent(_Prms.First);
        }

        protected override void SourceChangeEvent(object sender)
        {
            DelayedActionRegistry.Register(
                () =>
                {
                    FlagsState flags;

                retry:

                    while (!GetFlags(out flags)) ;

                    if (!GetPSVPCNIsPending(ref flags))
                    {
                        if (!Lock(ref flags))
                            goto retry;

                        _Prms.Second.QueueWorkItem(PSVPCNCallback, this);

                        SetPSVPCNIsPending(ref flags, true);

                        Commit(ref flags);
                    }
                }
            );
        }

        bool GetPSIRPCNIsPending(ref FlagsState flags)
        { return flags.GetBitFlag(PSIRPCNIsPendingMask); }

        bool SetPSIRPCNIsPending(ref FlagsState flags, bool value)
        { return flags.SetBitFlag(PSIRPCNIsPendingMask, value); }

        static void PSIRPCNCallback(object me)
        { ((BufferTransformation<TType>)me).PSIRPCNCallback(); }

        void PSIRPCNCallback()
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            SetPSIRPCNIsPending(ref flags, false);

            if (!Commit(ref flags))
                goto retry;

            SendMessage(ref flags, INCEventArgs.IsReadOnlyChanged());
        }

        protected override void SourceIsReadOnlyEvent(object sender)
        {
            FlagsState flags;

        retry:

            while (!GetFlags(out flags)) ;

            if (!GetPSIRPCNIsPending(ref flags))
            {
                if (!Lock(ref flags))
                    goto retry;

                _Prms.Second.QueueWorkItem(PSIRPCNCallback, this);

                SetPSIRPCNIsPending(ref flags, true);

                Commit(ref flags);
            }
        }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        protected override TType GetValue()
        { return _Prms.First.Value; }

        protected override void SetValue(TType value)
        { _Prms.First.Value = value; }

        protected override bool GetIsReadOnly()
        { return _Prms.First.IsReadOnly; }
    }
}
