using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Values.Transformations
{
    internal sealed class CachedTransformation<TType> : CachedTransformationBase<TType, IInternalValueProvider<TType>>
    {
        #region Bitflags
        const Int32 HaveBufferMask = 1 << (ConvertTransformationBase<TType, IInternalValueProvider<TType>>.BitFlagIndexEnd + 0);


        #endregion

        #region Constructor

        /// <summary>
        /// returns an existing BufferTransformation with given parameters or constructs a new one 
        /// </summary>
        /// <param name="source">Source <see cref="IValueProvider{TType}"/>.</param>
        /// <param name="workQueue">An <see cref="IWorkQueue"/> object we queue or jobs in for delayed processing.</param>
        /// <returns>The found or newly created <see cref="BufferTransformation{TType}"/>.</returns>
        public static CachedTransformation<TType> Create(IInternalValueProvider<TType> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<CachedTransformation<TType>, IInternalValueProvider<TType>>(source);
        }

        #endregion

        protected override void SubscribeOnSources()
        { _Prms.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.UnsubscribeINC(this); }

        protected override void InitializeBuffer()
        { _Buffer = _Prms.Value; }

        protected override void ClearBuffer()
        { _Buffer = default(TType); }

        protected override TType GetValueFromBuffer()
        { return _Buffer; }

        protected override TType GetValueDirect()
        { return _Prms.Value; }

        TType _Buffer;
    }
}
