using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        public static IValueProvider<TOut> OnException<TOut, TException>(this IValueProvider<TOut> source, Func<TException, bool> exceptionHandler, Func<TException, TOut> fallbackValueGenerator)
            where TException : Exception
        { return _OnException(source.Patched(), exceptionHandler, fallbackValueGenerator).Concrete(); }

        public static IValueProvider<TOut> OnException<TOut, TException>(this IValueProvider<TOut> source, Func<TException, TOut> fallbackValueGenerator)
            where TException : Exception
        { return _OnException(source.Patched(), _ => true, fallbackValueGenerator).Concrete(); }

        public static IValueProvider<TOut> OnException<TOut, TException>(this IValueProvider<TOut> source, Func<TException, bool> exceptionHandler, TOut fallbackValue)
            where TException : Exception
        { return _OnException(source.Patched(), exceptionHandler, _ => fallbackValue).Concrete(); }

        internal static IInternalValueProvider<TOut> _OnException<TOut, TException>(this IInternalValueProvider<TOut> source, Func<TException, bool> exceptionHandler, Func<TException, TOut> fallbackValueGenerator)
            where TException : Exception
        { return ExceptionTransformation<TOut, TException>.Create(source, exceptionHandler, fallbackValueGenerator); }



        public static IValueProvider OnException<TException>(this IValueProvider source, Func<TException, bool> exceptionHandler, Func<TException, object> fallbackValueGenerator)
            where TException : Exception
        { return _OnException(source.Patched(), exceptionHandler, fallbackValueGenerator).Concrete(); }

        public static IValueProvider OnException<TException>(this IValueProvider source, Func<TException, object> fallbackValueGenerator)
            where TException : Exception
        { return _OnException(source.Patched(), _ => true, fallbackValueGenerator).Concrete(); }

        public static IValueProvider OnException<TException>(this IValueProvider source, Func<TException, bool> exceptionHandler, object fallbackValue)
            where TException : Exception
        { return _OnException(source.Patched(), exceptionHandler, _ => fallbackValue).Concrete(); }

        internal static IInternalValueProvider _OnException<TException>(this IInternalValueProvider source, Func<TException, bool> exceptionHandler, Func<TException, object> fallbackValueGenerator)
            where TException : Exception
        { return ExceptionTransformation<object, TException>.Create(TypeConvertTransformation<object>.Create(source), exceptionHandler, fallbackValueGenerator); }
    }
}
