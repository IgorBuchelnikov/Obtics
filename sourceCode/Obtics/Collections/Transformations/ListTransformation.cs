using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using SL = System.Linq;
using Obtics.Collections.Transformations;
using System.Diagnostics;


namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Serves as a 'Cap' on a transformation pipeline. It hides the transformation internals (by not exposing the source)
    /// and provides buffered access to transformation contents.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    internal sealed class ListTransformation<TType> : ListTransformationBase<TType, IInternalEnumerable<TType>, IInternalEnumerable<TType>>
    {
        public static ListTransformation<TType> Create(IEnumerable<TType> source)
        {
            var s = source.Patched();

            return s == null ? null : Carrousel.Get<ListTransformation<TType>, IInternalEnumerable<TType>>(s);
        }

        public override IInternalEnumerable<TType> UnorderedForm
        { get { return _Prms; } }

        protected override IInternalEnumerable<TType> Source
        { get { return _Prms; } }
    }
}
