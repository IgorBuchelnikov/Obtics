using System;
using System.Collections.Generic;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Transforms a collection of TIn elements to a collection of TOut elements by
    /// converting each individual source element by means of a converter Function.
    /// </summary>
    /// <typeparam name="TIn">Type of the source elements</typeparam>
    /// <typeparam name="TOut">Type of the outpus elements</typeparam>
    internal sealed class ConvertTransformation<TIn, TOut> : ConvertTransformationBase<TIn, TOut, IInternalEnumerable<TIn>, Tuple<IInternalEnumerable<TIn>, Func<TIn, TOut>>>
    {
        public static ConvertTransformation<TIn, TOut> Create(IEnumerable<TIn> s, Func<TIn, TOut> func)
        {
            var source = s.Patched();

            if (source == null || func == null)
                return null;

            return Carrousel.Get<ConvertTransformation<TIn, TOut>, IInternalEnumerable<TIn>, Func<TIn, TOut>>(source, func);
        }

        public override bool IsMostUnordered
        { get { return _Prms.First.IsMostUnordered; } }

        public override IInternalEnumerable<TOut> UnorderedForm
        { get { return IsMostUnordered ? this : Create( _Prms.First.UnorderedForm, _Prms.Second ); } }

        internal protected override IInternalEnumerable<TIn> Source
        { get { return _Prms.First; } }

        Func<TIn, TOut> Func
        { get { return _Prms.Second; } }

        protected override TOut ConvertValue(TIn value)
        { return Func(value); }
    }
}
