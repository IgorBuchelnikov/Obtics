using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections;

namespace Obtics
{
    //FuncExtender is a mechanism to parameterize lambda functions in such a way
    //that if the same lamba is parameterized twice with the same parameters then
    //the two results are equal.
    //
    //Unfortunately in .Net methods a such:
    //
    //    static Func<int, int> Generate(int x)
    //    { return i => x + i; }
    //
    //will yield false always:
    //
    //  object.Equals(Generate(10),Generate(10))
    //
    //This is .. unfortunate
    //
    //FuncExtender solves the problem like this
    //
    //    static Func<int, int> Generate(int x)
    //    { return FuncExtender<int>.Extend(x, (i,xv) => xv + i); }
    //
    //Now this will yield true.. always
    //
    //  object.Equals(Generate(10),Generate(10))
    //
    //The paramterized lambda given to FuncExtender should be static.
    //it should contain no free variables!
    //
    //In Obtics this will prevent the Carrousel from getting beaten to much
    //and that Transformation methods called with the same (equivalent) arguments
    //will always yield an equivalent result.
    //

    internal static class FuncExtender1
    {
        public static Func<TArg1, TResult> Extend<TArg1, TSeed, TResult>(TSeed seed, Func<TArg1, TSeed, TResult> func)
        { return Carrousel.Get(func, seed, t => (Func<TArg1, TResult>)( arg1 => t.First(arg1, t.Second) ) ); }

        public static Func<TArg1, TArg2, TResult> Extend<TArg1, TArg2, TSeed, TResult>(TSeed seed, Func<TArg1, TArg2, TSeed, TResult> func)
        { return Carrousel.Get(func, seed, t => (Func<TArg1, TArg2, TResult>)( (arg1, arg2) => t.First(arg1, arg2, t.Second) ) ); }

        public static Func<TArg1, TArg2, TArg3, TResult> Extend<TArg1, TArg2, TArg3, TSeed, TResult>(TSeed seed, Func<TArg1, TArg2, TArg3, TSeed, TResult> func)
        { return Carrousel.Get(func, seed, t => (Func<TArg1, TArg2, TArg3, TResult>)((arg1, arg2, arg3) => t.First(arg1, arg2, arg3, t.Second))); }

        public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Extend<TArg1, TArg2, TArg3, TArg4, TSeed, TResult>(TSeed seed, Func<TArg1, TArg2, TArg3, TArg4, TSeed, TResult> func)
        { return Carrousel.Get(func, seed, t => (Func<TArg1, TArg2, TArg3, TArg4, TResult>)((arg1, arg2, arg3, arg4) => t.First(arg1, arg2, arg3, arg4, t.Second))); }
    }

    internal static class FuncExtender2
    {
        public static Func<TArg1, TResult> Extend<TArg1, TSeed1, TSeed2, TResult>(TSeed1 seed1, TSeed2 seed2, Func<TArg1, TSeed1, TSeed2, TResult> func)
        { return Carrousel.Get(func, seed1, seed2, t => (Func<TArg1, TResult>)(arg1 => t.First(arg1, t.Second, t.Third))); }

        public static Func<TArg1, TArg2, TResult> Extend<TArg1, TArg2, TSeed1, TSeed2, TResult>(TSeed1 seed1, TSeed2 seed2, Func<TArg1, TArg2, TSeed1, TSeed2, TResult> func)
        { return Carrousel.Get(func, seed1, seed2, t => (Func<TArg1, TArg2, TResult>)((arg1, arg2) => t.First(arg1, arg2, t.Second, t.Third))); }
    }

    internal static class FuncExtender3
    {
        public static Func<TArg1, TResult> Extend<TArg1, TSeed1, TSeed2, TSeed3, TResult>(TSeed1 seed1, TSeed2 seed2, TSeed3 seed3, Func<TArg1, TSeed1, TSeed2, TSeed3, TResult> func)
        { return Carrousel.Get(func, seed1, seed2, seed3, t => (Func<TArg1, TResult>)(arg1 => t.First(arg1, t.Second, t.Third, t.Fourth))); }
    }

    internal static class FuncExtender<TArg1>
    {
        public static Func<TArg1, TResult> Extend<TSeed, TResult>(TSeed seed, Func<TArg1, TSeed, TResult> func)
        { return FuncExtender1.Extend<TArg1, TSeed, TResult>(seed, func); }

        public static Func<TArg1, TResult> Extend<TSeed1, TSeed2, TResult>(TSeed1 seed1, TSeed2 seed2, Func<TArg1, TSeed1, TSeed2, TResult> func)
        { return FuncExtender2.Extend<TArg1, TSeed1, TSeed2, TResult>(seed1, seed2, func); }

        public static Func<TArg1, TResult> Extend<TSeed1, TSeed2, TSeed3, TResult>(TSeed1 seed1, TSeed2 seed2, TSeed3 seed3, Func<TArg1, TSeed1, TSeed2, TSeed3, TResult> func)
        { return FuncExtender3.Extend<TArg1, TSeed1, TSeed2, TSeed3, TResult>(seed1, seed2, seed3, func); }
    }

    internal static class FuncExtender<TArg1, TArg2>
    {
        public static Func<TArg1, TArg2, TResult> Extend<TSeed, TResult>(TSeed seed, Func<TArg1, TArg2, TSeed, TResult> func)
        { return FuncExtender1.Extend<TArg1, TArg2, TSeed, TResult>(seed, func); }

        public static Func<TArg1, TArg2, TResult> Extend<TSeed1, TSeed2, TResult>(TSeed1 seed1, TSeed2 seed2, Func<TArg1, TArg2, TSeed1, TSeed2, TResult> func)
        { return FuncExtender2.Extend<TArg1, TArg2, TSeed1, TSeed2, TResult>(seed1, seed2, func); }
    }

    internal static class FuncExtender<TArg1, TArg2, TArg3>
    {
        public static Func<TArg1, TArg2, TArg3, TResult> Extend<TSeed, TResult>(TSeed seed, Func<TArg1, TArg2, TArg3, TSeed, TResult> func)
        { return FuncExtender1.Extend<TArg1, TArg2, TArg3, TSeed, TResult>(seed, func); }
    }

    internal static class FuncExtender<TArg1, TArg2, TArg3, TArg4>
    {
        public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Extend<TSeed, TResult>(TSeed seed, Func<TArg1, TArg2, TArg3, TArg4, TSeed, TResult> func)
        { return FuncExtender1.Extend<TArg1, TArg2, TArg3, TArg4, TSeed, TResult>(seed, func); }
    }

}
