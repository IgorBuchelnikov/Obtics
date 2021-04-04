using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Obtics.Collections.Transformations;
using TvdP.Collections;
using SL = System.Linq;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        static Cache<Tuple<Type, Type>, bool> _IsUpcastCache = new Cache<Tuple<Type, Type>, bool>();

        /// <summary>
        /// Helper method shared by Cast and OfType. Checks if the element cast is a simple upcast or not.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns>The upcasted sequence or null if it isn't an upcast.</returns>
        static IEnumerable<TResult> UpCast<TResult>(this IVersionedEnumerable source)
        {
            if (source == null)
                return null;

            var correct = source as IVersionedEnumerable<TResult>;

            if (correct != null)
                return correct;

            if(typeof(TResult)==typeof(object))
                return TypeConvertTransformation<TResult>.Create(source);

            bool isUpcast;

            var sourceType = source.GetType();

            var key = Tuple.Create(sourceType, typeof(TResult));

            if (!_IsUpcastCache.TryGetItem(key, out isUpcast))
            {
                isUpcast = 
                    _IsUpcastCache.GetOldest(
                        key,
                        SL.Enumerable.FirstOrDefault(
                            sourceType.GetInterfaces(),
                            type => 
                                type.IsGenericType 
                                && type.GetGenericTypeDefinition() == typeof(IVersionedEnumerable<>)
                                && typeof(TResult).IsAssignableFrom(type.GetGenericArguments()[0])
                            ) != null 
                    );
            }

            if (isUpcast)
                return TypeConvertTransformation<TResult>.Create(source);

            return null;
        }

        /// <summary>
        /// Converts the elements of an <see cref="IEnumerable"/> to the specified type, leaving out the elements for which a conversion path can not be found.
        /// </summary>
        /// <typeparam name="TResult">The type to convert the elements of source to.</typeparam>
        /// <param name="source">The <see cref="IEnumerable"/> that contains the elements to be converted.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/>, that contains each convertable element of
        ///     the source sequence converted to the specified type, or null if <paramref name="source"/> is null.</returns>
        /// <remarks>
        /// Obtics transformations in principle never raise exceptions for the same reasons
        /// that eventhandlers should never raise exceptions.
        /// When no conversion path can be found to cast an item to the intended type the 
        /// item is simply left out of the result collection (like <see cref="OfType"/>).
        /// When a path can be found then this path should never raise an exception. This 
        /// transformation will not handle such exceptions since it knows nothing about them
        /// and will let them escalate. This in turn means that the transformation pipeline will be
        /// left in an inconsistent state. If it is
        /// possible for such a path to raise an exception then this is not the method to use.
        /// In such a case it would be better to use <see cref="Where"/> and <see cref="Select"/> and check the conversion.
        /// </remarks>
        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            var seqSource = source.Patched();

            return 
                seqSource == null ? null :
                UpCast<TResult>(seqSource) ??
                    (seqSource as IEnumerable<object> ?? TypeConvertTransformation<object>.Create(seqSource))
                        .Select((Func<object,Tuple<TResult,bool>>)Caster<TResult>.Cast)
                        .Where(t => t.Second)
                        .Select(t => t.First);
        }
    }
}
