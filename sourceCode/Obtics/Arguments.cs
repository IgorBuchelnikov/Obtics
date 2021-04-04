using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Obtics
{
    internal static class InternalArguments
    {
        [Conditional("VERIFY_INT_ARGS_NOT_NULL")]
        public static void VerifyNotNull<TArg1>(TArg1 arg1, string nameArg1)
            where TArg1 : class
        {
            if (arg1 == null)
                throw new ArgumentNullException(nameArg1);
        }

        [Conditional("VERIFY_INT_ARGS_NOT_NULL")]
        public static void VerifyNotNull<TArg1, TArg2>(
            TArg1 arg1, string nameArg1,
            TArg2 arg2, string nameArg2
        )
            where TArg1 : class
            where TArg2 : class
        {
            VerifyNotNull(arg1, nameArg1);
            VerifyNotNull(arg2, nameArg2);
        }

        [Conditional("VERIFY_INT_ARGS_NOT_NULL")]
        public static void VerifyNotNull<TArg1, TArg2, TArg3>(
            TArg1 arg1, string nameArg1,
            TArg2 arg2, string nameArg2,
            TArg3 arg3, string nameArg3
        )
            where TArg1 : class
            where TArg2 : class
            where TArg3 : class
        {
            VerifyNotNull(arg1, nameArg1);
            VerifyNotNull(arg2, nameArg2);
            VerifyNotNull(arg3, nameArg3);
        }

        [Conditional("VERIFY_INT_ARGS_NOT_NULL")]
        public static void VerifyNotNull<TArg1, TArg2, TArg3, TArg4>(
            TArg1 arg1, string nameArg1,
            TArg2 arg2, string nameArg2,
            TArg3 arg3, string nameArg3,
            TArg4 arg4, string nameArg4
        )
            where TArg1 : class
            where TArg2 : class
            where TArg3 : class
            where TArg4 : class
        {
            VerifyNotNull(arg1, nameArg1);
            VerifyNotNull(arg2, nameArg2);
            VerifyNotNull(arg3, nameArg3);
            VerifyNotNull(arg4, nameArg4);
        }

        [Conditional("VERIFY_INT_ARGS_NOT_NULL")]
        public static void VerifyItemsNotNull<TType>(TType[] items, string ptrn)
            where TType : class
        {
            int i = Array.IndexOf(items, null);

            if( i != -1 )
                throw new ArgumentNullException(String.Format(ptrn, i));
        }
    }
}
