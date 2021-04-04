using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif



namespace ObticsUnitTest.Helpers
{
    public static class EqualityRunner
    {
        public static void Run<Type>(Func<Type> builder, string prefix )
        {
            var a = builder();
            var b = builder();

            Assert.IsTrue(a.Equals(a), prefix + " (1) object should be equal to itself.");
            Assert.IsTrue(a.Equals(b), prefix + " (2) objects created with equal arguments should be equal.");
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), prefix + " (3) objects created with equal arguments should have same hash.");
            Assert.IsFalse(a.Equals(null), prefix + " (4) object should not be equal to null.");
            Assert.IsFalse(a.Equals(string.Empty), prefix + " (5) object should not be equal to object of different type.");
        }

        public static void Run<TArg1, Type>(TArg1 arg1a, TArg1 arg1b, Func<TArg1, Type> builder, string prefix)
        {
            var a = builder(arg1a);
            var b = builder(arg1a);
            var c = builder(arg1b);

            Assert.IsTrue(a.Equals(a), prefix + " (1) object should be equal to itself.");
            Assert.IsTrue(a.Equals(b), prefix + " (2) objects created with equal arguments should be equal.");
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), prefix + " (3) objects created with equal arguments should have same hash.");
            Assert.IsFalse(a.Equals(null), prefix + " (4) object should not be equal to null.");
            Assert.IsFalse(a.Equals(string.Empty), prefix + " (5) object should not be equal to object of different type.");
            Assert.IsFalse(a.Equals(c), prefix + " (6) objects created from different arguments should not be equal.");
        }

        public static void Run<TArg1, TArg2, Type>(TArg1 arg1a, TArg1 arg1b, TArg2 arg2a, TArg2 arg2b, Func<TArg1, TArg2, Type> builder, string prefix)
        {
            var a = builder(arg1a, arg2a);
            var b = builder(arg1a, arg2a);
            var c = builder(arg1b, arg2a);
            var d = builder(arg1a, arg2b);
            var e = builder(arg1b, arg2b);

            Assert.IsTrue(a.Equals(a), prefix + " (1) object should be equal to itself.");
            Assert.IsTrue(a.Equals(b), prefix + " (2) objects created with equal arguments should be equal.");
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), prefix + " (3) objects created with equal arguments should have same hash.");
            Assert.IsFalse(a.Equals(null), prefix + " (4) object should not be equal to null.");
            Assert.IsFalse(a.Equals(string.Empty), prefix + " (5) object should not be equal to object of different type.");
            Assert.IsFalse(a.Equals(c), prefix + " (6) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(d), prefix + " (7) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(e), prefix + " (8) objects created from different arguments should not be equal.");
        }

        public static void Run<TArg1, TArg2, TArg3, Type>(TArg1 arg1a, TArg1 arg1b, TArg2 arg2a, TArg2 arg2b, TArg3 arg3a, TArg3 arg3b, Func<TArg1, TArg2, TArg3, Type> builder, string prefix)
        {
            var a = builder(arg1a, arg2a, arg3a);
            var b = builder(arg1a, arg2a, arg3a);
            var c = builder(arg1b, arg2a, arg3a);
            var d = builder(arg1a, arg2b, arg3a);
            var e = builder(arg1a, arg2a, arg3b);
            var f = builder(arg1b, arg2b, arg3b);

            Assert.IsTrue(a.Equals(a), prefix + " (1) object should be equal to itself.");
            Assert.IsTrue(a.Equals(b), prefix + " (2) objects created with equal arguments should be equal.");
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), prefix + " (3) objects created with equal arguments should have same hash.");
            Assert.IsFalse(a.Equals(null), prefix + " (4) object should not be equal to null.");
            Assert.IsFalse(a.Equals(string.Empty), prefix + " (5) object should not be equal to object of different type.");
            Assert.IsFalse(a.Equals(c), prefix + " (6) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(d), prefix + " (7) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(e), prefix + " (8) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(f), prefix + " (9) objects created from different arguments should not be equal.");
        }

        public static void Run<TArg1, TArg2, TArg3, TArg4, Type>(TArg1 arg1a, TArg1 arg1b, TArg2 arg2a, TArg2 arg2b, TArg3 arg3a, TArg3 arg3b, TArg4 arg4a, TArg4 arg4b, Func<TArg1, TArg2, TArg3, TArg4, Type> builder, string prefix)
        {
            var a = builder(arg1a, arg2a, arg3a, arg4a);
            var b = builder(arg1a, arg2a, arg3a, arg4a);
            var c = builder(arg1b, arg2a, arg3a, arg4a);
            var d = builder(arg1a, arg2b, arg3a, arg4a);
            var e = builder(arg1a, arg2a, arg3b, arg4a);
            var f = builder(arg1a, arg2a, arg3a, arg4b);
            var g = builder(arg1b, arg2b, arg3b, arg4b);

            Assert.IsTrue(a.Equals(a), prefix + " (1) object should be equal to itself.");
            Assert.IsTrue(a.Equals(b), prefix + " (2) objects created with equal arguments should be equal.");
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), prefix + " (3) objects created with equal arguments should have same hash.");
            Assert.IsFalse(a.Equals(null), prefix + " (4) object should not be equal to null.");
            Assert.IsFalse(a.Equals(string.Empty), prefix + " (5) object should not be equal to object of different type.");
            Assert.IsFalse(a.Equals(c), prefix + " (6) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(d), prefix + " (7) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(e), prefix + " (8) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(f), prefix + " (9) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(g), prefix + " (10) objects created from different arguments should not be equal.");
        }

        public delegate Type Func<TArg1, TArg2, TArg3, TArg4, TArg5, Type>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

        public static void Run<TArg1, TArg2, TArg3, TArg4, TArg5, Type>(TArg1 arg1a, TArg1 arg1b, TArg2 arg2a, TArg2 arg2b, TArg3 arg3a, TArg3 arg3b, TArg4 arg4a, TArg4 arg4b, TArg5 arg5a, TArg5 arg5b, Func<TArg1, TArg2, TArg3, TArg4, TArg5, Type> builder, string prefix)
        {
            var a = builder(arg1a, arg2a, arg3a, arg4a, arg5a);
            var b = builder(arg1a, arg2a, arg3a, arg4a, arg5a);
            var c = builder(arg1b, arg2a, arg3a, arg4a, arg5a);
            var d = builder(arg1a, arg2b, arg3a, arg4a, arg5a);
            var e = builder(arg1a, arg2a, arg3b, arg4a, arg5a);
            var f = builder(arg1a, arg2a, arg3a, arg4b, arg5a);
            var g = builder(arg1a, arg2a, arg3a, arg4a, arg5b);
            var h = builder(arg1b, arg2b, arg3b, arg4b, arg5b);

            Assert.IsTrue(a.Equals(a), prefix + " (1) object should be equal to itself.");
            Assert.IsTrue(a.Equals(b), prefix + " (2) objects created with equal arguments should be equal.");
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), prefix + " (3) objects created with equal arguments should have same hash.");
            Assert.IsFalse(a.Equals(null), prefix + " (4) object should not be equal to null.");
            Assert.IsFalse(a.Equals(string.Empty), prefix + " (5) object should not be equal to object of different type.");
            Assert.IsFalse(a.Equals(c), prefix + " (6) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(d), prefix + " (7) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(e), prefix + " (8) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(f), prefix + " (9) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(g), prefix + " (10) objects created from different arguments should not be equal.");
            Assert.IsFalse(a.Equals(h), prefix + " (11) objects created from different arguments should not be equal.");
        }
    }
}
