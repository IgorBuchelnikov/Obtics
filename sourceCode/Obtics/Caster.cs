using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections;
using System.Linq.Expressions;
using TvdP.Collections;
using System.Security;
using Obtics.Configuration;
using System.Security.Permissions;

namespace Obtics
{
    internal static class Caster<TOut>
    {
        /// <summary>
        /// Static bool field if the TOut type is nullable
        /// </summary>
        static bool _IsNullable = typeof(TOut).IsClass || typeof(TOut).IsInterface || (typeof(TOut).IsGenericType && typeof(TOut).GetGenericTypeDefinition() == typeof(Nullable<>));

        /// <summary>
        /// Cache for compiled converter functions
        /// </summary>
        static readonly Cache<Type, Func<object, Tuple<TOut, bool>>> Cache = new Cache<Type, Func<object, Tuple<TOut, bool>>>();

        /// <summary>
        /// Cast a given object to TOut.
        /// </summary>
        /// <param name="castee">The object to cast or null.</param>
        /// <returns>A tuple where the first value is the cast result and the second is a boolean that indicates if the cast is supported.</returns>
        public static Tuple<TOut, bool> Cast(object castee)
        {
            if (castee == null)
                return Tuple.Create(default(TOut),_IsNullable);

            Type cType = castee.GetType();
            Func<object, Tuple<TOut, bool>> cvtFunc = null;
            var cache = Cache;

            //try get converter function from cache.
            if( !cache.TryGetItem(cType, out cvtFunc ) )
            {
                //If it can not be found; Try to create a converter function function from a lambda expression.
                try
                {
#if !SILVERLIGHT
                    var assertFlag = ObticsConfigurationSection.AssertReflectionPermissionFlag;

                    //check if we can raise reflection permissions
                    if (assertFlag.HasValue)
                        new ReflectionPermission(assertFlag.Value).Assert();

                    try
                    {
#endif
                        var inParam = Expression.Parameter(typeof(object), "castee");
                        var compiled =
                            (Func<object, TOut>)
                                Expression.Lambda(
                                    Expression.Convert(
                                        Expression.Convert(inParam, cType),
                                        typeof(TOut)
                                    ),
                                    inParam
                                )
                                .Compile();

                        cvtFunc = new Func<object, Tuple<TOut, bool>>(o => Tuple.Create(compiled(o), true));
#if !SILVERLIGHT
                    }
                    finally
                    {
                        if (assertFlag.HasValue)
                            CodeAccessPermission.RevertAssert();
                    }
#endif
                }
                catch(SecurityException)
                {
                    //Trying to use a converter we don't have access to
                }
                catch (InvalidOperationException)
                {
                    //no conversion operator/path could be found. The cast is not supported.
                }

                //store it in the cache.
                cvtFunc = cache.GetOldest(cType, cvtFunc ?? new Func<object, Tuple<TOut, bool>>( o => Tuple.Create( default(TOut), false) ) );
            }

            return cvtFunc(castee);
        }
    }
}
