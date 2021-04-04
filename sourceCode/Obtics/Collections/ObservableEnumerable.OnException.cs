using System.Collections.Generic;
using Obtics.Collections.Transformations;
using Obtics.Values;
using System;
using System.Collections;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        public static IEnumerable<TSource> OnException<TSource, TException>(this IEnumerable<TSource> source, Func<TException, bool> handler, Func<TException, IEnumerable<TSource>> fallbackCollectionGenerator)
            where TException : Exception
        { return ExceptionTransformation<TSource, TException>.Create(source, handler, fallbackCollectionGenerator); }

        public static IEnumerable<TSource> OnException<TSource, TException>(this IEnumerable<TSource> source, Func<TException, IEnumerable<TSource>> fallbackCollectionGenerator)
            where TException : Exception
        { return ExceptionTransformation<TSource, TException>.Create(source, _ => true, fallbackCollectionGenerator); }

        public static IEnumerable<TSource> OnException<TSource, TException>(this IEnumerable<TSource> source, Func<TException, bool> handler, IEnumerable<TSource> fallbackCollection)
            where TException : Exception
        { return ExceptionTransformation<TSource, TException>.Create(source, handler, _ => fallbackCollection); }

        public static IEnumerable<TSource> OnException<TSource, TException>(this IEnumerable<TSource> source, Func<TException, bool> handler)
            where TException : Exception
        { return ExceptionTransformation<TSource, TException>.Create(source, handler, _ => System.Linq.Enumerable.Empty<TSource>()); }


        public static IEnumerable OnException<TException>(this IEnumerable source, Func<TException, bool> handler, Func<TException, IEnumerable> fallbackCollectionGenerator)
            where TException : Exception
        {
            if (fallbackCollectionGenerator == null)
                return null;

            return ExceptionTransformation<object, TException>.Create(source.Cast<object>(), handler, ex => fallbackCollectionGenerator(ex).Cast<object>()); 
        }

        public static IEnumerable OnException<TException>(this IEnumerable source, Func<TException, IEnumerable> fallbackCollectionGenerator)
            where TException : Exception
        {
            if (fallbackCollectionGenerator == null)
                return null;

            return ExceptionTransformation<object, TException>.Create(source.Cast<object>(), _ => true, ex => fallbackCollectionGenerator(ex).Cast<object>()); 
        }

        public static IEnumerable OnException<TException>(this IEnumerable source, Func<TException, bool> handler, IEnumerable fallbackCollection)
            where TException : Exception
        {
            if (fallbackCollection == null)
                return null;

            return ExceptionTransformation<object, TException>.Create(source.Cast<object>(), handler, _ => fallbackCollection.Cast<object>()); 
        }

        public static IEnumerable OnException<TException>(this IEnumerable source, Func<TException, bool> handler)
            where TException : Exception
        { return ExceptionTransformation<object, TException>.Create(source.Cast<object>(), handler, _ => System.Linq.Enumerable.Empty<object>()); }
    }
}
