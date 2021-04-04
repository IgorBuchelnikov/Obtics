using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics.Values.Transformations
{
    /// <summary>
    /// An IValueTransformationPipelineFactory is an object that knows how to create a value transformation pipeline out of a 
    /// certain input parameter.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    internal interface IValueTransformationPipelineFactory<TKey,TOut>
    {
        /// <summary>
        /// Create a pipeline based on a given parameter.
        /// </summary>
        /// <param name="key">The input parameter</param>
        /// <returns>a value transformation pipeline </returns>
        /// <remarks>
        /// Typically the result of two separate call to this method with two equal key values
        /// should yield two equal pipelines.
        /// </remarks>
        IValueProvider<TOut> Create(TKey key);
    }

    /// <summary>
    /// Special transformation that helps caching the results of the pipeline factories.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TPG"></typeparam>
    /// <remarks>
    /// In this class the source is not part of the (carrousel-)key. Only the input parameter to the 
    /// pipeline factory is part of the key. The actual type of the factory (with default constructor)
    /// is part of the type identity of the PipelineResultTransformation.
    /// 
    /// The key in these objects will always be a composite key, consisting of a factory delegate and
    /// its arguments. The IValueTransformationPipelineFactory static instance knows how to call this
    /// delegate with its arguments.
    /// </remarks>
    internal sealed class PipelineResultTransformation<TKey, TOut, TPG> : ConvertTransformationBase<TOut, TKey>
        where TPG : IValueTransformationPipelineFactory<TKey, TOut>, new()
        where TKey : class
    {
        public static PipelineResultTransformation<TKey, TOut, TPG> Create(TKey key)
        {
            return Carrousel.Get<PipelineResultTransformation<TKey, TOut, TPG>, TKey>(key, ConstructPRT);
        }

        static TPG _TPG = new TPG();

        static PipelineResultTransformation<TKey, TOut, TPG> ConstructPRT(TKey key)
        {
            var source = _TPG.Create(key).Patched();

            if( source == null )
                return null;

            var res = new PipelineResultTransformation<TKey, TOut, TPG>();
            res._Source = source;
            res.Initialize(key);
            return res;
        }
        
        IInternalValueProvider<TOut> _Source;

        protected override TOut GetValue()
        { return _Source.Value; }

        protected override void SubscribeOnSources()
        { _Source.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Source.UnsubscribeINC(this); }
    }
}
