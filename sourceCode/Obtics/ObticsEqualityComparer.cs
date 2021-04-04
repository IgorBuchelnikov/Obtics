using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using Obtics.Configuration;

namespace Obtics
{


#if !FULL_REFLECTION
    public
#else
    internal
#endif
    sealed class FromUntypedComparer<TD> : IEqualityComparer<TD>, IEqualityComparer
    {
        public FromUntypedComparer(IEqualityComparer baseComparer)
        { 
            _BaseComparer = baseComparer;

            if (ObticsEqualityComparer.IsUnpatchedComparer(baseComparer))
                ObticsEqualityComparer.DeclareUnpatched(this);
        }

        IEqualityComparer _BaseComparer;

        #region IEqualityComparer<TD> Members

        public bool Equals(TD x, TD y)
        { return _BaseComparer.Equals(x, y); }

        public int GetHashCode(TD obj)
        { return _BaseComparer.GetHashCode(obj); }

        #endregion
    
        #region IEqualityComparer Members

        public new bool Equals(object x, object y)
        { return _BaseComparer.Equals(x,y); }

        public int  GetHashCode(object obj)
        { return _BaseComparer.GetHashCode(obj); }

        #endregion
    }

    class ClosureEqualityComparer<TC> : IEqualityComparer<TC>, IEqualityComparer 
    {
#if ASSUME_EXECUTION_SCOPE
        class StrongBoxEqualityComparer : IEqualityComparer<object>
        {
            #region IEqualityComparer<object> Members


            public int GetHashCode(object obj)
            {
                var sb = obj as System.Runtime.CompilerServices.IStrongBox;

                return Hasher.CreateFromRef(sb != null ? sb.Value : obj, ObticsEqualityComparer.Default).Value;
            }

            #endregion

            public new bool Equals(object a, object b)
            {
                var sbA = a as System.Runtime.CompilerServices.IStrongBox;
                var sbB = b as System.Runtime.CompilerServices.IStrongBox;

                if (sbA != null && sbB != null)
                    return ObticsEqualityComparer.Default.Equals(sbA.Value, sbB.Value);

                return object.Equals(a, b);
            }

            public static readonly IEqualityComparer<object> Instance = new StrongBoxEqualityComparer();
        }
#endif

        #region IEqualityComparer<Closure> Members

        public bool Equals(TC xobj, TC yobj)
        {
            var x = (System.Runtime.CompilerServices.Closure)(object)xobj;
            var y = (System.Runtime.CompilerServices.Closure)(object)yobj; 

            if (object.ReferenceEquals(x, y))
                return true;

            if (x != null && y != null)
            {
                //Globals and Locals contain StrongBox<> objects. These refer to external variables
                //unfortunately it is not possible to test if these IStrongBox objects refer to the same external variable
                //Comparing the Value properties does not work since different external variables may temporarily hold the same value.
                //Would have been nice: strongBoxA == stringBoxB if they refer to the same external variable. :-(
                //As it is now the below comparison probably only works when Globals and Locals both are empty

                var globalsA = x.Constants;
                var globalsB = y.Constants;
                var localsA = x.Locals;
                var localsB = y.Locals;

                bool res =
                    ((globalsA == null) == (globalsB == null) && (globalsA == null || globalsA.SequenceEqual(globalsB, ObticsEqualityComparer.Default)))
                    && ((localsA == null) == (localsB == null) && (localsA == null || localsA.SequenceEqual(localsB, StrongBoxEqualityComparer.Instance)));

                return res;
            }

            return false;
        }

        public int GetHashCode(TC obj)
        {
            var closure = (System.Runtime.CompilerServices.Closure)(object)obj;
            var hash = Hasher.CreateFromValue(48393829);

            if (closure != null)
            {
                if (closure.Constants != null)
                    hash = closure.Constants.Aggregate(hash, (acc_hash, o) => acc_hash.AddRef(o, ObticsEqualityComparer.Default));

                if (closure.Locals != null)
                    hash = closure.Locals.Aggregate(hash, (acc_hash, o) => acc_hash.AddRef(o, StrongBoxEqualityComparer.Instance));
            }

            return hash.Value;
        }

        #endregion

        #region IEqualityComparer Members

        public new bool Equals(object x, object y)
        { return this.Equals((TC)x, (TC)y) ; }

        public int GetHashCode(object obj)
        { return GetHashCode((TC)obj); }

        #endregion
    }

    class DelegateEqualityComparer<TD> : IEqualityComparer<TD>, IEqualityComparer 
    {
        //ASSUME_EXECUTION_SCOPE compiler symbol. When set these methods will tresspass on ExecutionScope and IStrongBox
        //these identities are 'not intended to be used from your code' the exact workings are therefore not clear either
        //Undefine the ASSUME_EXECUTION_SCOPE symbol to stay on the safe side, but many logically equal delegates will not
        //get recognized as such. Giving a very significant performance degradation for the carrousel



        #region IEqualityComparer<TType> Members

        public bool Equals(TD a, TD b)
        {
            var dgtA = (Delegate)(object)a;
            var dgtB = (Delegate)(object)b;

            if (object.ReferenceEquals(dgtA, dgtB))
                return true;

            if (dgtA == null || dgtB == null)
                return false;

#if ASSUME_EXECUTION_SCOPE
            bool res = dgtA.GetType() == dgtB.GetType() && dgtA.Method == dgtB.Method;

            var targetA = dgtA.Target;
            var targetB = dgtB.Target;

            if (res && !object.Equals(targetA, targetB))
            {
                //var scopeA = targetA as System.Runtime.CompilerServices.ExecutionScope;
                //var scopeB = targetB as System.Runtime.CompilerServices.ExecutionScope;

                //if (scopeA != null && scopeB != null)
                //{
                //    //Globals and Locals contain StrongBox<> objects. These refer to external variables
                //    //unfortunately it is not possible to test if these IStrongBox objects refer to the same external variable
                //    //Comparing the Value properties does not work since different external variables may temporarily hold the same value.
                //    //Would have been nice: strongBoxA == stringBoxB if they refer to the same external variable. :-(
                //    //As it is now the below comparison probably only works when Globals and Locals both are empty

                //    var globalsA = scopeA.Globals;
                //    var globalsB = scopeB.Globals;
                //    var localsA = scopeA.Locals;
                //    var localsB = scopeB.Locals;

                //    res =
                //        ((globalsA == null) == (globalsB == null) && (globalsA == null || globalsA.SequenceEqual(globalsB, StrongBoxComparer.Instance)))
                //        && ((localsA == null) == (localsB == null) && (localsA == null || localsA.SequenceEqual(localsB, StrongBoxComparer.Instance)));

                //    targetA = scopeA.Parent;
                //    targetB = scopeB.Parent;
                //}
                //else

                var closureA = targetA as System.Runtime.CompilerServices.Closure;
                var closureB = targetB as System.Runtime.CompilerServices.Closure;

                if (closureA != null && closureB != null)
                    res = ObticsEqualityComparer<System.Runtime.CompilerServices.Closure>.Default.Equals(closureA, closureB);
                else
                    res = ObticsEqualityComparer.Default.Equals(targetA, targetB);
            }


            return res;
#else
                return 
                    dgtA.GetType() == dgtB.GetType() 
                    && dgtA.Method == dgtB.Method 
                    && object.Equals(dgtA.Target,dgtB.Target);
#endif
        }

        public int GetHashCode(TD obj)
        {
            var dgt = (Delegate)(object)obj;

#if ASSUME_EXECUTION_SCOPE

            var hash = Hasher.CreateFromRef(dgt.GetType());//.AddRef(dgt.Method);

            var target = dgt.Target;

            if (target != null)
            {
                //var scope = target as System.Runtime.CompilerServices.ExecutionScope;

                //if (scope != null)
                //{
                //    if (scope.Globals != null)
                //        hash = scope.Globals.Aggregate(hash, (acc_hash, o) => acc_hash.AddRef(o, StrongBoxComparer.Instance));

                //    if (scope.Locals != null)
                //        hash = scope.Locals.Aggregate(hash, (acc_hash, o) => acc_hash.AddRef(o, StrongBoxComparer.Instance));

                //    target = scope.Parent;
                //}
                //else
                var closure = target as System.Runtime.CompilerServices.Closure;

                if (closure != null)
                {
                    hash = hash.AddRef(closure, ObticsEqualityComparer<System.Runtime.CompilerServices.Closure>.Default);
                }
                else
                {
                    hash = hash.AddValue(ObticsEqualityComparer.Default.GetHashCode(target));
                }
            }

            return hash.Value;
#else
                return Hasher.CreateFromRef(dgt.GetType()).AddRef(dgt.Method).AddRef(dgt.Target).Value;
#endif
        }

        #endregion

        #region IEqualityComparer Members

        public new bool Equals(object x, object y)
        { return Equals((TD)x, (TD)y); }

        public int GetHashCode(object obj)
        { return GetHashCode((TD)obj); }

        #endregion
    }

    class FallbackEqualityComparer : IEqualityComparer<object>, IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            return 
                object.ReferenceEquals(x, y) ? true :
                x != null ? x.Equals(y) :
                y != null ? y.Equals(x) :
                false
            ;
        }

        public int GetHashCode(object obj)
        { return obj == null ? 48393489 : obj.GetHashCode(); }
    }


    /// <summary>
    /// This class forms a fix for very strange behaviour when comparing delegates for equality.
    /// 
    /// Consider the following main function in a console application:
    /// 
    ///        static void Main(string[] args)
    ///        {
    ///            Func&lt;int, int&gt; shallowFunc1 = i =&gt; i + 10;
    ///            Func&lt;int, int&gt; shallowFunc2 = i =&gt; i + 10;
    ///
    ///            Console.Out.WriteLine("shallowFunc1 == shallowFunc2:" + EqualityComparer&lt;Func&lt;int, int&gt;&gt;.Default.Equals(shallowFunc1, shallowFunc2).ToString());
    ///            Console.Out.WriteLine("shallowFunc1#:" + EqualityComparer&lt;Func&lt;int, int&gt;&gt;.Default.GetHashCode(shallowFunc1).ToString());
    ///            Console.Out.WriteLine("shallowFunc2#:" + EqualityComparer&lt;Func&lt;int, int&gt;&gt;.Default.GetHashCode(shallowFunc2).ToString());
    ///
    ///            Func&lt;Func&lt;int, int&gt;&gt; deepFunc = () =&gt; i =&gt; i + 10;
    ///
    ///            Console.Out.WriteLine("deepFunc() == deepFunc():" + EqualityComparer&lt;Func&lt;int, int&gt;&gt;.Default.Equals(deepFunc(), deepFunc()).ToString());
    ///            Console.Out.WriteLine("deepFunc()#:" + EqualityComparer&lt;Func&lt;int, int&gt;&gt;.Default.GetHashCode(deepFunc()).ToString());
    ///
    ///            Expression&lt;Func&lt;Func&lt;int, int&gt;&gt;&gt; deepExp = () =&gt; i =&gt; i + 10;
    ///            var deepFunc2 = deepExp.Compile();
    ///
    ///            Console.Out.WriteLine("deepFunc2() == deepFunc2():" + EqualityComparer&lt;Func&lt;int, int&gt;&gt;.Default.Equals(deepFunc2(), deepFunc2()).ToString());
    ///            Console.Out.WriteLine("deepFunc2()#:" + EqualityComparer&lt;Func&lt;int, int&gt;&gt;.Default.GetHashCode(deepFunc2()).ToString());
    ///
    ///            Console.In.ReadLine();
    ///        }
    ///}
    ///
    /// This is what I get for output:
    ///
    ///
    /// shallowFunc1 == shallowFunc2:False
    /// shallowFunc1#:1915136
    /// shallowFunc2#:1915136
    /// deepFunc() == deepFunc():True
    /// deepFunc()#:1915136
    /// deepFunc2() == deepFunc2():False
    /// deepFunc2()#:1915136
    ///
    /// All hashcodes are equal! This class forms a work arround for that.
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    /// <typeparam name="TType"></typeparam>
#if !FULL_REFLECTION
    public 
#else
    internal
#endif
    class ObticsEqualityComparer<TType>
    {
        static IEqualityComparer<TType> _Default;

        public static IEqualityComparer<TType> Default { get { return _Default; } }

        static bool _IsPatched;

        /// <summary>
        /// Gives true if an extraordinary equality comparer has been specified for this type.
        /// This means we can not trust the standard equality comparer of the type. (otherwise
        /// such an extraordinary equality comparer would not have been needed)
        /// </summary>
        public static bool IsPatched { get { return _IsPatched; } }

        static object GetTypedComparer(IEqualityComparer comparer, Type compareeType)
        {
            var specificComparerType = typeof(IEqualityComparer<>).MakeGenericType(compareeType);

            if (specificComparerType.IsAssignableFrom(comparer.GetType()))
                return comparer;

            return
                Activator.CreateInstance(
                    typeof(FromUntypedComparer<>).MakeGenericType(compareeType),
                    comparer
                )
            ;
        }

        static ObticsEqualityComparer()
        {
            IEqualityComparer untypedComparer = ObticsEqualityComparer.GetEqualityComparer(typeof(TType));

            _Default = (IEqualityComparer<TType>)GetTypedComparer(untypedComparer, typeof(TType));
            _IsPatched = !ObticsEqualityComparer.IsUnpatchedComparer(untypedComparer);
        }
    }

    internal static class ObticsEqualityComparer
    {
        static System.Runtime.CompilerServices.ConditionalWeakTable<IEqualityComparer, object> _UnpatchedTokenMap = new System.Runtime.CompilerServices.ConditionalWeakTable<IEqualityComparer, object>();

        public static  bool IsUnpatchedComparer(IEqualityComparer comparer)
        { 
            object dummy;
            return _UnpatchedTokenMap.TryGetValue(comparer, out dummy);
        }

        public static void DeclareUnpatched(IEqualityComparer comparer)
        { _UnpatchedTokenMap.GetOrCreateValue(comparer); }

        static IEqualityComparer GetDefaultEqualityComparer(Type compareeType)
        {
            IEqualityComparer comparer;

            if (typeof(Delegate).IsAssignableFrom(compareeType))
                comparer = new DelegateEqualityComparer<Delegate>();
            else if (typeof(Expression).IsAssignableFrom(compareeType))
                comparer = new ExpressionEqualityComparer<Expression>();
            else if (typeof(System.Runtime.CompilerServices.Closure).IsAssignableFrom(compareeType))
                comparer = new ClosureEqualityComparer<System.Runtime.CompilerServices.Closure>();
            else
                comparer = new FallbackEqualityComparer();

            DeclareUnpatched(comparer);

            return comparer;
        }

        class AttributeProvider : IEqualityComparerProvider
        {
            #region IEqualityComparerProvider Members

            public IEqualityComparer GetEqualityComparer(Type compareeType)
            {
                var attribute = (ObticsEqualityComparerAttribute)Attribute.GetCustomAttribute(compareeType, typeof(ObticsEqualityComparerAttribute));

                return attribute != null ? (IEqualityComparer)Activator.CreateInstance(attribute.EqualityComparerType) : null;
            }

            #endregion
        }

#if !SILVERLIGHT
        static ProviderFinder<IEqualityComparerProvider, Type, IEqualityComparer> _ProviderFinder =
            new ProviderFinder<IEqualityComparerProvider, Type, IEqualityComparer>(
                Obtics.Configuration.ObticsConfigurationSection
                    .GetSection()
                    .EqualityComparers
                    .Cast<EqualityComparerConfigurationElement>()
                    .Select(ecce => ecce.EqualityComparerProviderInstance)
                    .Concat( Enumerable.Repeat((IEqualityComparerProvider)new AttributeProvider(), 1) )
                    .ToArray(),
                (provider, compareeType) => provider.GetEqualityComparer(compareeType),
                GetDefaultEqualityComparer,
                Obtics.Configuration.ObticsConfigurationSection.GetSection().EnableRegistriationThroughReflection
            )
        ;
#else
        static ProviderFinder<IEqualityComparerProvider, Type, IEqualityComparer> _ProviderFinder =
            new ProviderFinder<IEqualityComparerProvider, Type, IEqualityComparer>(
                new IEqualityComparerProvider[] { new AttributeProvider() } ,
                (provider, compareeType) => provider.GetEqualityComparer(compareeType),
                GetDefaultEqualityComparer,
                true
            )
        ;
#endif

        public static IEqualityComparer GetEqualityComparer(Type type)
        {
            {
                //try to find via attribute
                var attribute = (ObticsEqualityComparerAttribute)Attribute.GetCustomAttribute(type, typeof(ObticsEqualityComparerAttribute));

                if (attribute != null)
                {
                    return
                        (IEqualityComparer)Activator.CreateInstance(attribute.EqualityComparerType);
                }
            }

            return _ProviderFinder.GetValue(type); 
        }

        internal class ObjectComparer : IEqualityComparer<object>, IEqualityComparer
        {
            #region IEqualityComparer<object> Members

            public new bool Equals(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null || y == null)
                    return false;

                return GetEqualityComparer(x.GetType()).Equals(x, y);
            }

            public int GetHashCode(object obj)
            {
                if (obj == null)
                    return 94839849;

                return GetEqualityComparer(obj.GetType()).GetHashCode(obj);
            }

            #endregion
        }

        static ObjectComparer _Default = new ObjectComparer();

        internal static ObjectComparer Default { get { return _Default; } }
    }
}
