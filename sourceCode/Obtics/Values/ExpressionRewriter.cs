using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using EOE = Obtics.Collections.ObservableEnumerable;
using OC = Obtics.Collections;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using Obtics.Values.Transformations;

namespace Obtics.Values
{
    /// <summary>
    /// Does the actual heavy lifting when rewriting non-live expressions to live ones.
    /// </summary>
    internal class ExpressionRewriter : System.Linq.Expressions.ExpressionVisitor
    {
        #region TraceabilityTester : Internal class that determines which parts of an expression tree can be tracked for changes

        //Searches the expression tree and build a dictionary with information
        //if an Expression can be traced or not. For eaxample if the tree contains a call
        //to a method that takes a lambda with parameters then the parameters and all that
        //depends on those parameters is NOT traceable since we don't know how many times
        //the lambda will be called. The lambda itself forms a boundary to 'non-traceability'
        //If the contents of the lambda is not traceable but only because of the lambda's own
        //parameters then the lambda itself CAN be regarded as traceable.
        class TraceabilityTester : System.Linq.Expressions.ExpressionVisitor
        {
            //elements with a value of int.MaxValue are traceable. All others are not
            Dictionary<Expression, int> _TraceableExpressions;
            int _Level ;

            public TraceabilityTester(Dictionary<Expression, int> traceableExpressions)
            {
                _TraceableExpressions = traceableExpressions;
            }

            public void Test(Expression exp)
            {
                this.Visit(exp);
            }

            int Traceable(Expression x)
            {
                int traceable;
                return x == null || !_TraceableExpressions.TryGetValue(x, out traceable) ? int.MaxValue : traceable;
            }

            int Min(IEnumerable<int> values)
            { return values.DefaultIfEmpty(int.MaxValue).Min(); }

            int Min(params int[] values)
            { return Min((IEnumerable<int>)values); }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                var res = base.VisitParameter(p);

                if (!_TraceableExpressions.ContainsKey(p))
                    _TraceableExpressions[p] = int.MaxValue;

                return res;
            }

            protected override Expression VisitBinary(BinaryExpression b)
            {
                var res = base.VisitBinary(b);

                _TraceableExpressions[b] = Min(Traceable(b.Left), Traceable(b.Right), Traceable(b.Conversion));

                return res;
            }

            protected override Expression VisitConditional(ConditionalExpression c)
            {
                var res = base.VisitConditional(c);

                _TraceableExpressions[c] = Min(Traceable(c.IfFalse), Traceable(c.IfTrue), Traceable(c.Test));

                return res;
            }

            protected override Expression VisitConstant(ConstantExpression c)
            {
                var res = base.VisitConstant(c);

                _TraceableExpressions[c] = int.MaxValue;

                return res;
            }

            protected override Expression VisitInvocation(InvocationExpression iv)
            {
                var res = base.VisitInvocation(iv);

                _TraceableExpressions[iv] = Min(Traceable(iv.Expression), iv.Arguments == null ? int.MaxValue : Min(iv.Arguments.Select(arg => Traceable(arg))));

                return res;
            }

            protected override Expression VisitLambda<T>(Expression<T> lambda)
            {
                ++_Level;

                foreach(var prm in lambda.Parameters)
                    _TraceableExpressions[prm] = _Level;

                var res = base.VisitLambda(lambda);

                --_Level;

                int childLevel = Traceable(lambda.Body);
                _TraceableExpressions[lambda] = childLevel > _Level ? int.MaxValue : childLevel;

                return res;
            }

            protected override Expression VisitListInit(ListInitExpression init)
            {
                var res = base.VisitListInit(init);

                _TraceableExpressions[init] = Traceable(init.NewExpression);

                return res;
            }

            protected override Expression VisitMember(MemberExpression m)
            {
                var res = base.VisitMember(m);

                _TraceableExpressions[m] = Traceable(m.Expression);

                return res;
            }

            private int MemberBindingLevel(MemberBinding mb)
            {
                switch (mb.BindingType)
                {
                    case MemberBindingType.Assignment:
                        return AssignmenMBLevel((MemberAssignment)mb);
                    case MemberBindingType.ListBinding:
                        return ListMBLevel((MemberListBinding)mb);
                    case MemberBindingType.MemberBinding:
                        return MemberMBLevel((MemberMemberBinding)mb);
                    default:
                        throw new Exception(string.Format("Unhandled memberbinding type: '{0}'", mb.BindingType));
                }
            }

            private int AssignmenMBLevel(MemberAssignment memberAssignment)
            { return Traceable(memberAssignment.Expression); }

            private int ListMBLevel(MemberListBinding memberListBinding)
            { return memberListBinding.Initializers.SelectMany(i => i.Arguments).Select(e => Traceable(e)).DefaultIfEmpty(int.MaxValue).Min(); }

            private int MemberMBLevel(MemberMemberBinding memberMemberBinding)
            { return memberMemberBinding.Bindings.Select(b => MemberBindingLevel(b)).DefaultIfEmpty(int.MaxValue).Min(); }

            protected override Expression VisitMemberInit(MemberInitExpression init)
            {
                var res = base.VisitMemberInit(init);

                _TraceableExpressions[init] = Math.Min(Traceable(init.NewExpression), init.Bindings.Select(b => MemberBindingLevel(b)).DefaultIfEmpty(int.MaxValue).Min());

                return res;
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                var res = base.VisitMethodCall(m);

                _TraceableExpressions[m] = Min(Traceable(m.Object), m.Arguments == null ? int.MaxValue : Min(m.Arguments.Select(arg => Traceable(arg))));

                return res;
            }

            protected override Expression VisitNew(NewExpression nex)
            {
                var res = base.VisitNew(nex);

                _TraceableExpressions[nex] = nex.Arguments == null ? int.MaxValue : Min(nex.Arguments.Select(arg => Traceable(arg)));

                return res;
            }

            protected override Expression VisitNewArray(NewArrayExpression na)
            {
                var res = base.VisitNewArray(na);

                _TraceableExpressions[na] = na.Expressions == null ? int.MaxValue : Min(na.Expressions.Select(exp => Traceable(exp)));

                return res;
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression b)
            {
                var res = base.VisitTypeBinary(b);

                _TraceableExpressions[b] = Traceable(b.Expression);

                return res;
            }

            protected override Expression VisitUnary(UnaryExpression u)
            {
                var res = base.VisitUnary(u);

                _TraceableExpressions[u] = Traceable(u.Operand);

                return res;
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                var res = base.VisitDefault(node);

                _TraceableExpressions[node] = int.MaxValue;

                return res;
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                var res = base.VisitIndex(node);

                _TraceableExpressions[node] = Min(Traceable(node.Object), Min(node.Arguments.Select(arg => Traceable(arg))));

                return res; 
            }


            //Report error when encountering not (yet) supported expression types.

            Exception NotSupported(string name)
            { return new NotSupportedException("Obtics does not (yet) support " + name); }

            protected override Expression VisitBlock(BlockExpression node)
            { throw NotSupported("BlockExpression"); }

            protected override Expression VisitDebugInfo(DebugInfoExpression node)
            { throw NotSupported("DebugInfoExpression"); }

            protected override Expression VisitDynamic(DynamicExpression node)
            { throw NotSupported("DynamicExpression"); }

            protected override Expression VisitExtension(Expression node)
            { throw NotSupported("extension Expression"); }

            protected override Expression VisitGoto(GotoExpression node)
            { throw NotSupported("GotoExpression"); }

            protected override Expression VisitLabel(LabelExpression node)
            { throw NotSupported("LabelExpression"); }

            protected override Expression VisitLoop(LoopExpression node)
            { throw NotSupported("LoopExpression"); }

            protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            { throw NotSupported("RuntimeVariablesExpression"); }

            protected override Expression VisitSwitch(SwitchExpression node)
            { throw NotSupported("SwitchExpression"); }

            protected override Expression VisitTry(TryExpression node)
            { throw NotSupported("TryExpression"); }
        }

        #endregion

        #region Methods : static references to MethodInfo objects used when rewriting the expresson tree.

        //some static references to some method and property reflection information
        //these methods and properties may get invoked in an Expression made observable.

        //MethodInfo of ValueProvider.Property(IValueProvider<TSource> source, PropertyInfo propInfo)
        static MethodInfo PropertyMethodInfo = ((Func<IValueProvider<object>, PropertyInfo, IValueProvider<object>>)ValueProvider.Property<object, object>).Method.GetGenericMethodDefinition();

        static MethodInfo StaticPropertyMethodInfo = ((Func<object, PropertyInfo, IValueProvider<object>>)ValueProvider.Property<object, object>).Method.GetGenericMethodDefinition();

        //MethodInfo of ValueProvider.Static<TType>(TType value)
        static MethodInfo StaticMethodInfo = ((Func<object, IValueProvider<object>>)ValueProvider.Static<object>).Method.GetGenericMethodDefinition();

        // MethodInfos of various ValueProvider.Select overloades
        static MethodInfo SelectInfMethodInfo = ((Func<IValueProvider[], Func<object[], object>, IValueProvider<object>>)ValueProvider.Select<object>).Method.GetGenericMethodDefinition();

        static MethodInfo[] SelectMethodInfos = new MethodInfo[] {
            ((Func<IValueProvider<object>, Func<object, object>, IValueProvider<object>>)ValueProvider.Select<object,object>).Method.GetGenericMethodDefinition(),
            ((Func<IValueProvider<object>, IValueProvider<object>, Func<object, object, object>, IValueProvider<object>>)ValueProvider.Select<object,object,object>).Method.GetGenericMethodDefinition(),
            ((Func<IValueProvider<object>, IValueProvider<object>, IValueProvider<object>, Func<object, object, object, object>, IValueProvider<object>>)ValueProvider.Select<object, object, object, object>).Method.GetGenericMethodDefinition(),
            ((Func<IValueProvider<object>, IValueProvider<object>, IValueProvider<object>, IValueProvider<object>, Func<object, object, object, object, object>, IValueProvider<object>>)ValueProvider.Select<object, object, object, object, object>).Method.GetGenericMethodDefinition()
        };

        static MethodInfo CascadingSelectInfMethodInfo = ((Func<IValueProvider[], Func<object[], IValueProvider<object>>, IValueProvider<object>>)ValueProvider.Select<object>).Method.GetGenericMethodDefinition();

        static MethodInfo[] CascadingSelectMethodInfos = new MethodInfo[] {
            ((Func<IValueProvider<object>, Func<object, IValueProvider<object>>, IValueProvider<object>>)ValueProvider.Select<object,object>).Method.GetGenericMethodDefinition(),
            ((Func<IValueProvider<object>, IValueProvider<object>, Func<object, object, IValueProvider<object>>, IValueProvider<object>>)ValueProvider.Select<object,object,object>).Method.GetGenericMethodDefinition(),
            ((Func<IValueProvider<object>, IValueProvider<object>, IValueProvider<object>, Func<object, object, object, IValueProvider<object>>, IValueProvider<object>>)ValueProvider.Select<object, object, object, object>).Method.GetGenericMethodDefinition(),
            ((Func<IValueProvider<object>, IValueProvider<object>, IValueProvider<object>, IValueProvider<object>, Func<object, object, object, object, IValueProvider<object>>, IValueProvider<object>>)ValueProvider.Select<object, object, object, object, object>).Method.GetGenericMethodDefinition()
        };

        //MethodInfo of ValueProvider.Cascade<TType>(this IValueProvider<IValueProvider<TType>> source)
        static MethodInfo CascadeMethodInfo = ((Func<IValueProvider<IValueProvider<object>>, IValueProvider<object>>)ValueProvider.Cascade<object>).Method.GetGenericMethodDefinition();

        static MethodInfo[] SelectExtMethodInfos = new MethodInfo[] {
            typeof(RewriterHelper).GetMethod("SelectExt1"),
            typeof(RewriterHelper).GetMethod("SelectExt2"),
            typeof(RewriterHelper).GetMethod("SelectExt3"),
            typeof(RewriterHelper).GetMethod("SelectExt4")
        };

        //static MethodInfo SafeCallMethodInfo = typeof(RewriterHelper).GetMethod("SafeCall");

        #endregion

        #region Method maps : mapping information from non-live methods to live methods

        //maps standard iif operator to ValueProvider.IIf methods
        static MapInfo iifMap =
            ExpressionObserverMappingHelper.CreateMap(
                (Func<bool, object, object, IValueProvider<object>>)ValueProvider.IIf<object>, //placeholder
                (Func<IValueProvider<bool>, IValueProvider<object>, IValueProvider<object>, IValueProvider<object>>)ValueProvider.IIf<object>,
                (Func<bool, IValueProvider<object>, IValueProvider<object>, IValueProvider<object>>)ValueProvider.IIf<object>,
                (Func<IValueProvider<bool>, object, IValueProvider<object>, IValueProvider<object>>)ValueProvider.IIf<object>,
                (Func<bool, object, IValueProvider<object>, IValueProvider<object>>)ValueProvider.IIf<object>,
                (Func<IValueProvider<bool>, IValueProvider<object>, object, IValueProvider<object>>)ValueProvider.IIf<object>,
                (Func<bool, IValueProvider<object>, object, IValueProvider<object>>)ValueProvider.IIf<object>,
                (Func<IValueProvider<bool>, object, object, IValueProvider<object>>)ValueProvider.IIf<object>,
                (Func<bool, object, object, IValueProvider<object>>)ValueProvider.IIf<object>
            )
        ;

        //maps standard coalesce operator to ValueProvider.Coalesce
        static MapInfo coalesceMap =
            ExpressionObserverMappingHelper.CreateMap(
                (Func<object, object, IValueProvider<object>>)ValueProvider.Coalesce<object>, //placeholder
                (Func<IValueProvider<object>, IValueProvider<object>, IValueProvider<object>>)ValueProvider.Coalesce<object>,
                (Func<object, IValueProvider<object>, IValueProvider<object>>)ValueProvider.Coalesce<object>,
                (Func<IValueProvider<object>, object, IValueProvider<object>>)ValueProvider.Coalesce<object>,
                (Func<object, object, IValueProvider<object>>)ValueProvider.Coalesce<object>
            )
        ;

        static MapInfo coalesceNullableMap =
            ExpressionObserverMappingHelper.CreateMap(
                (Func<bool?, bool, IValueProvider<bool>>)ValueProvider.Coalesce<bool>, //placeholder
                (Func<IValueProvider<bool?>, IValueProvider<bool>, IValueProvider<bool>>)ValueProvider.Coalesce<bool>,
                (Func<bool?, IValueProvider<bool>, IValueProvider<bool>>)ValueProvider.Coalesce<bool>,
                (Func<IValueProvider<bool?>, bool, IValueProvider<bool>>)ValueProvider.Coalesce<bool>,
                (Func<bool?, bool, IValueProvider<bool>>)ValueProvider.Coalesce<bool>
            )
        ;

        //maps standard and (AndAlso) operator to ValueProvider.Coalesce
        static MapInfo andMap =
            ExpressionObserverMappingHelper.CreateMap(
                (Func<bool, bool, IValueProvider<bool>>)ValueProvider.And, //placeholder
                (Func<IValueProvider<bool>, IValueProvider<bool>, IValueProvider<bool>>)ValueProvider.And,
                (Func<bool, IValueProvider<bool>, IValueProvider<bool>>)ValueProvider.And,
                (Func<IValueProvider<bool>, bool, IValueProvider<bool>>)ValueProvider.And,
                (Func<bool, bool, IValueProvider<bool>>)ValueProvider.And
            )
        ;

        //maps standard or (OrElse) operator to ValueProvider.Or
        static MapInfo orMap =
            ExpressionObserverMappingHelper.CreateMap(
                (Func<bool, bool, IValueProvider<bool>>)ValueProvider.Or, //placeholder
                (Func<IValueProvider<bool>, IValueProvider<bool>, IValueProvider<bool>>)ValueProvider.Or,
                (Func<bool, IValueProvider<bool>, IValueProvider<bool>>)ValueProvider.Or,
                (Func<IValueProvider<bool>, bool, IValueProvider<bool>>)ValueProvider.Or,
                (Func<bool, bool, IValueProvider<bool>>)ValueProvider.Or
            )
        ;

        #endregion

        //In the process of navigating the tree a VPFinder can invoke child VPFinders.
        //To maintain a shared (new)parameter count the VPFinders share an object of this class
        sealed class ParamCountContainer
        { 
            public int _Count;
            public string Next()
            { return "_" + ++_Count; }
        }

        //IValueProvider<T> sources for the current expression
        List<KeyValuePair<Expression,ParameterExpression>> _Sources;

        //Dictionary with traceablility information. Not traceable expressions will be copied as is.
        Dictionary<Expression,int> _TraceableExpressions;

        //To count the number of newly generated parameters
        ParamCountContainer _ParamCountContainer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="traceableExpressions"></param>
        /// <param name="methodMappings"></param>
        /// <param name="pcc"></param>
        private ExpressionRewriter(Dictionary<Expression, int> traceableExpressions, ParamCountContainer pcc)
        {
            _TraceableExpressions = traceableExpressions;
            _ParamCountContainer = pcc;
        }


        #region Rewriting : Observable dependency detection and rewriting

        /// <summary>
        /// Determine if the given MemberInfo represents the Value property of an IValueProvider.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private static bool MemberIsValueProviderValueProperty(MemberInfo memberInfo)
        {
            var propInfo = memberInfo as PropertyInfo;

            if (propInfo == null || propInfo.Name != SIValueProvider.ValuePropertyName)
                return false;

            var declaringType = propInfo.DeclaringType;
            return declaringType == typeof(IValueProvider) || declaringType.IsGenericType && declaringType.GetGenericTypeDefinition() == typeof(IValueProvider<>);
        }

        /// <summary>
        /// Determine if the give Expression represents a call to the ValueProvider.Static method; 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static bool IsStaticMethodCall(Expression exp, out Expression arg)
        {
            if (exp.NodeType == ExpressionType.Call)
            {
                var objectExpressionCall = (MethodCallExpression)exp;
                var objectExpressionCall_Method = objectExpressionCall.Method;

                if (
                    objectExpressionCall_Method.IsGenericMethod
                    && objectExpressionCall_Method.GetGenericMethodDefinition() == StaticMethodInfo
                )
                {
                    arg = objectExpressionCall.Arguments[0];
                    return true;
                }
            }

            arg = null;
            return false;
        }

        //Registers a new IValueProvider<T> source for this expression and generates a parameter expression
        //refering to it.
        //The source will function as an argument for a ValueProvider.Select() method call and the 
        //parameter as parameter for its convert lambda. The expression will become the body of the lambda 
        //The returned ParameterExpression will be of type T
        //
        //ex MUST be of type IValueProvider<T>
        //the returned ParameterExpression will be of type T
        ParameterExpression GetVPReference(Expression ex)
        {
            var sources = _Sources;

            if (sources == null)
                _Sources = sources = new List<KeyValuePair<Expression, ParameterExpression>>( );

            var comparer = ObticsEqualityComparer<Expression>.Default;

            foreach (var kvp in sources)
                if (comparer.Equals(kvp.Key, ex))
                    return kvp.Value;

            var newKvp = new KeyValuePair<Expression, ParameterExpression>(ex, CreateParameter(ex.Type.GetGenericArguments()[0]));

            sources.Add(newKvp);

            return newKvp.Value;
        }

        //Creates a new ParameterExpression with unique name.
        ParameterExpression CreateParameter(Type m_Expression_Type)
        { return Expression.Parameter(m_Expression_Type, "_" + (_ParamCountContainer._Count++).ToString(CultureInfo.InvariantCulture)); }

        //Creates a new ExpressionRewriter with same tracability information and paramters counter.
        //Use to create deeper reactive rewrites
        ExpressionRewriter NextExpressionWriter()
        { return new ExpressionRewriter(this._TraceableExpressions, this._ParamCountContainer); }

        #region MakeConvertCall : creates final convert call using all sources. After calling this method this ExpressionRewriter instance should not be used anymore

        Expression MakeConvertCall(Expression nextExp, bool cascading)
        {
            if (!cascading && nextExp.NodeType == ExpressionType.MemberAccess)
            {
                var nextExpMA = (MemberExpression)nextExp;

                if (MemberIsValueProviderValueProperty(nextExpMA.Member))
                {
                    nextExp = nextExpMA.Expression;
                    cascading = true;
                }
            }

            var sources = this._Sources;
            var sources_Count = sources == null ? 0 : sources.Count;
            var nextExp_Type = nextExp.Type;

            if (sources_Count == 0)
            {
                //result does not depend on ANY observable sources
                return
                    cascading ?
                        nextExp
                    :
                        Expression.Call(StaticMethodInfo.MakeGenericMethod(nextExp_Type), nextExp);
            }
            else if (sources_Count == 1 && nextExp == sources[0].Value)
            {
                //if we are about to build s.Select(x=>x)
                //we may just as well return s directly when not cascading or s.Cascade() when cascading                
                return
                    cascading ?
                        Expression.Call(
                                CascadeMethodInfo.MakeGenericMethod(nextExp_Type),
                                sources[0].Key
                            )
                    :
                        sources[0].Key;
            }
            else
                return
                    sources_Count > 4 ?
                        MakeUnlimitedConvertCall(nextExp, cascading)
                    :
                        MakeLimitedConvertCall(nextExp, cascading);
        }

        /// <summary>
        /// Build a Call to ObservableEnumerable.Convert version that takes an array ('infinit' number) of ValuProviders
        /// </summary>
        /// <param name="nextExp">The select method body.</param>
        /// <param name="cascading">If true the body returns an IValueProvider we can pass through directly; If false the body returns the value we have to create an IValueProvider for.</param>
        /// <returns>An expression that gives an IValueProvider with the value.</returns>
        Expression MakeUnlimitedConvertCall(Expression nextExp, bool cascading)
        {
            var innerParam = CreateParameter(typeof(object[]));

            var nextExp_Type = nextExp.Type;
            var resultType = cascading ? nextExp_Type.GetGenericArguments()[0] : nextExp_Type;
            var sources = this._Sources;
            var sources_Count = sources.Count;

            //Chain Sources and Parameters                
            for (int sIx = 0, step = 0; (step = Math.Min(sources_Count - sIx, 4)) > 0; sIx += step)
            {
                var funcImpTypeArgs = new Type[step + 1];
                var prmsRange = new ParameterExpression[step];

                for (int i = 0; i != step; ++i)
                {
                    var kvp = sources[sIx + i];
                    funcImpTypeArgs[i] = kvp.Key.Type.GetGenericArguments()[0];
                    prmsRange[i] = kvp.Value;
                }

                funcImpTypeArgs[step] = nextExp_Type;

                nextExp = Expression.Call(
                    SelectExtMethodInfos[step - 1].MakeGenericMethod(funcImpTypeArgs),
                    innerParam,
                    Expression.Constant(sIx),
                    Expression.Lambda(
                        nextExp,
                        prmsRange
                    )
                );
            }

            return Expression.Call(
                (cascading ? CascadingSelectInfMethodInfo : SelectInfMethodInfo).MakeGenericMethod(resultType),
                Expression.NewArrayInit(
                    typeof(IValueProvider),
                    sources.Select(kvp => kvp.Key)
                ),
                Expression.Lambda(
                    nextExp,
                    innerParam
                )
            );
        }

        /// <summary>
        /// Build a Call to ObservableEnumerable.Convert version that takes a limited (1-4) number of IValueProvider arguments.
        /// </summary>
        /// <param name="nextExp">The select method body.</param>
        /// <param name="cascading">If true the body returns an IValueProvider we can pass through directly; If false the body returns the value we have to create an IValueProvider for.</param>
        /// <returns>An expression that gives an IValueProvider with the value.</returns>
        Expression MakeLimitedConvertCall(Expression nextExp, bool cascading)
        {
            var sources = this._Sources;
            int sources_Count = sources.Count;

            var selectMethodTypeArgs = new Type[sources_Count + 1];
            var selectMethodArgs = new Expression[sources_Count + 1];
            var nextExp_Type = nextExp.Type;

            for (int i = 0; i < sources_Count; ++i)
                selectMethodTypeArgs[i] = (selectMethodArgs[i] = sources[i].Key).Type.GetGenericArguments()[0];

            selectMethodTypeArgs[sources_Count] = cascading ? nextExp_Type.GetGenericArguments()[0] : nextExp_Type;

            selectMethodArgs[sources_Count] = Expression.Lambda(
                nextExp,
                sources.Select(kvp => kvp.Value).ToArray()
            );

            return Expression.Call(
                (cascading ? CascadingSelectMethodInfos : SelectMethodInfos)[sources_Count - 1].MakeGenericMethod(selectMethodTypeArgs),
                selectMethodArgs
            );
        }

        #endregion

        /// <summary>
        /// Take the top level expression. Walk down the tree to find observable dependencies.
        /// These dependencies will be rewritten as IValueProviders. In the toplevel expression
        /// we will replace these dependencies by references to the Value properties of 
        /// IValueProvider arguments. This top level expression with the observable dependencies replaced
        /// will become the body of a lambda expression. This lambda expression and the rewritten
        /// dependencies (IValueProviders) will become the arguments to a call to ValueProvider.Convert.
        /// This Convert will give us an IValueProvider yielding the original result of our toplevel expression
        /// as the Value of this IValueProvider.
        /// 
        /// It is evident that the observable dependencies themselves will be rewritten using a recursive
        /// invocation of this method.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        Expression Observe(Expression ex)
        {
            var nextFinder = NextExpressionWriter();
            var nextExp = nextFinder.Visit(ex);

            return nextFinder.MakeConvertCall(nextExp, false);
        }

        //Rewrite arguments to a mapped method.
        Expression[] MapMethodArguments(MapInfo mapping, IEnumerable<Expression> orgMethodCallArgs, IEnumerable<Type> orgMethodCallArgTypes, out int mapIndex)
        {
            var parameterMap = mapping.Parameters;
            var parameterMap_Length = parameterMap.Length;
            Expression[] res = new Expression[parameterMap_Length];

            var mappingList = new List<int>();

            using (var orgMethodCallArgTypesEnumerator = orgMethodCallArgTypes.GetEnumerator())
            {
                using (var orgMethodCallArgsEnumerator = orgMethodCallArgs.GetEnumerator())
                {
                    for (int i = 0; i != parameterMap_Length; ++i)
                    {
                        orgMethodCallArgsEnumerator.MoveNext();
                        orgMethodCallArgTypesEnumerator.MoveNext();

                        var argType = orgMethodCallArgTypesEnumerator.Current;
                        var arg = orgMethodCallArgsEnumerator.Current;

                        switch (parameterMap[i])
                        {
                            case ParameterMapInfo.Direct:
                                res[i] = Visit(arg);
                                break;
                            case ParameterMapInfo.ObservableLambda:
                                //ObservableLanbda, the mapped argument takes a lambda delegate but the
                                //return type of the lambda is an IValueProvider of the original return type.
                                //If the actual argument is a LambdaExpression then Rewrite it and use it directly.
                                //If the lambda delegate is passed indirectly, treat the original argument
                                //as if it is a Direct mapped argument and bluntly convert the result
                                //to 'observable' by passing it through a call to a lambdaMapper method.
                                if (arg.NodeType == ExpressionType.Lambda)
                                {
                                    var internalLambda = (LambdaExpression)Visit(arg); //extract external dependencies first.

                                    var lambdaBody = ObserveLambdaBody(internalLambda);

                                    Expression staticArg;

                                    if (IsStaticMethodCall(lambdaBody, out staticArg))
                                        lambdaBody = staticArg;
                                    else
                                        mappingList.Add(i);

                                    res[i] = Expression.Lambda(lambdaBody, internalLambda.Parameters.ToArray());
                                }
                                else
                                    res[i] = Visit(arg);

                                break;
                            case ParameterMapInfo.ObservableValue:
                                {
                                    //ObservableValue, the mapped argument is an IValueProvider of the original
                                    //argument type. Make it observable and use the result directly.

                                    Expression fixedArg;
                                    
                                    //if arg is not exactly the required type; cast it.
                                    if (arg.Type == argType)
                                        fixedArg = arg;
                                    else
                                    {
                                        fixedArg = Expression.Convert(arg, argType);
                                        _TraceableExpressions[fixedArg] = _TraceableExpressions[arg];
                                    }

                                    var observableArg = Observe(fixedArg);

                                    Expression staticArg;

                                    if (IsStaticMethodCall(observableArg, out staticArg))
                                        observableArg = staticArg;
                                    else
                                        mappingList.Add(i);

                                    res[i] = observableArg;
                                }
                                break;
                            default:
                                throw new Exception(string.Format("Unhandled ParameterMapInfo type: '{0}'", parameterMap[i]));
                        }
                    }
                }
            }

            //find best match
            var bestMatch = 
                mapping.TargetMethods
                    .Where(t => !mappingList.Except(t.Second).Any()) //all mapped paramters are covered by the target method
                    .OrderBy(t => t.Second.Length) //the ones with the least extra mappable parameters on top
                    .First(); //take the first of those.

            //patch any extra mappable parameters
            for (int i = 0, end = bestMatch.Second.Length; i < end; ++i)
            {
                var mappableIndex = bestMatch.Second[i];

                if (!mappingList.Contains(mappableIndex))
                {
                    //index mappable for our chosen target method but not mapped yet.. patch it
                    switch (parameterMap[mappableIndex])
                    {
                        case ParameterMapInfo.ObservableLambda :
                            {
                                var oldLambda = res[mappableIndex];

                                if( oldLambda.NodeType == ExpressionType.Lambda )
                                {
                                    //lambda expression
                                    var oldLambdaExpression = (LambdaExpression)oldLambda;
                                    res[mappableIndex] = 
                                        Expression.Lambda(
                                            Expression.Call(
                                                StaticMethodInfo.MakeGenericMethod( oldLambdaExpression.Body.Type ),
                                                oldLambdaExpression.Body
                                            ),
                                            oldLambdaExpression.Parameters.ToArray()
                                        );                                                
                                }
                                else
                                {
                                    //expression must return delegate type
                                    var dgtType = oldLambda.Type;
                                    var invokeMethodInfo = dgtType.GetMethod("Invoke");

                                    var lambdaPrms = invokeMethodInfo.GetParameters().Select( prmInfo => Expression.Parameter( prmInfo.ParameterType, this._ParamCountContainer.Next() ) ).ToArray();

                                    res[mappableIndex] =
                                        Expression.Lambda(
                                            Expression.Call(
                                                StaticMethodInfo.MakeGenericMethod( invokeMethodInfo.ReturnType ),
                                                Expression.Invoke(
                                                    oldLambda,
                                                    lambdaPrms
                                                )
                                            ),
                                            lambdaPrms
                                        );
                                }
                            }
                            break;

                        case ParameterMapInfo.ObservableValue:
                            {
                                var oldValue = res[mappableIndex];

                                res[mappableIndex] =
                                    Expression.Call(
                                        StaticMethodInfo.MakeGenericMethod(oldValue.Type),
                                        oldValue
                                    );
                            }
                            break;
                        default:
                            throw new Exception("Unexpected mapping type.");
                    }
                }
            }

            mapIndex = Array.IndexOf(mapping.TargetMethods, bestMatch);

            return res;
        }

        //Expression visitors:

        //Member access is where core reactiveness comes in. Objects may give change notifications on properties
        //So replace property access with calls to ValueProvider.Property which return IValueProviders<T>
        protected override Expression VisitMember(MemberExpression m)
        {
            var m_Member = m.Member;
            var m_Expression = m.Expression;

            if (
                _TraceableExpressions[m] == int.MaxValue
                && m_Member != null
            )
            {
                MapInfo mapping;

                if (FindMemberMapping(m_Member, out mapping))
                {
                    int mapIndex;
                    var nextFinder = mapping.ResultDirectlyObservable ? this : NextExpressionWriter();
                    var parameters =
                        nextFinder.MapMethodArguments(
                            mapping,
                            //if static then Object is null; no arguments. If instance method add 'this' as argument
                            m_Expression == null ? new Expression[0] : new Expression[] { m_Expression },
                            m_Expression == null ? Type.EmptyTypes : new Type[] { m_Member.DeclaringType },
                            out mapIndex
                        );

                    var targetMethod = mapping.TargetMethods[mapIndex].First;

                    if (mapping.GenericPrmsCount != 0)
                    {
                        //find type args from generic declaring type
                        targetMethod = targetMethod.MakeGenericMethod(m_Member.DeclaringType.GetGenericArguments());
                    }

                    var targetMethodCall =
                        Expression.Call(
                            targetMethod,
                            parameters
                        );

                    return
                        !mapping.ResultDirectlyObservable ? GetVPReference(nextFinder.MakeConvertCall(targetMethodCall, true)) :
                        mapping.NeedsUpcast ? (Expression)Expression.Convert(targetMethodCall, m.Type) :
                        targetMethodCall;
                }


                if (m_Expression != null)
                {
                    var m_Expression_Type = m_Expression.Type;

                    if (!m_Expression_Type.IsValueType)
                    {
                        var m_Type = m.Type;

                        if (m_Member.MemberType == System.Reflection.MemberTypes.Property)
                        {




                            if ( //check if type can possibly give change notifications
                                m_Expression_Type.IsSealed
                                && !typeof(INotifyPropertyChanged).IsAssignableFrom(m_Expression_Type)
#if SILVERLIGHT
                                && !typeof(System.Windows.DependencyObject).IsAssignableFrom(m_Expression_Type)
#else
                                && !TypeDescriptor.GetProperties(m_Member.DeclaringType).Find(m_Member.Name, false).SupportsChangeEvents
#endif

                            )
                            {
                                //No; it can not generate change events that we know of.
                                
                                return base.VisitMember(m);

                                // the below code serves to shield agains null reference exceptions
                                // should have proper exception handling now so that below code is not needed
                                //
                                // TODO:Expensive
                                // translate x.f to x == null ? default(fType) : x.f 
                                //var target = Visit(m_Expression);
                                //var lambdaParam = CreateParameter(m_Expression_Type);

                                //return
                                //    Expression.Call(
                                //        SafeCallMethodInfo.MakeGenericMethod(m_Expression_Type, m_Type),
                                //        target,
                                //        Expression.Lambda(
                                //            Expression.Property(
                                //                lambdaParam,
                                //                (PropertyInfo)m_Member
                                //            ),
                                //            lambdaParam
                                //        )
                                //    );
                            }

                            //Observe will return an IValueProvider yielding our object as value. So, if our object itself
                            //is traceable (like in a chain of member accesses a.b.c) the changing of our object
                            //can be traced.
                            var objectExpression = Observe(m_Expression);

                            Expression vpSource = null;

                            if (m_Member == typeof(IValueProvider<>).MakeGenericType(m_Type).GetProperty(SIValueProvider.ValuePropertyName))
                            {
                                //It appears that we want the value of a ValueProvider.
                                //The type of objectExpression must be IValueProvider<IValueProvider<T>>
                                //we can reduce this using a ValueProvider.Cascade call instead of refering to the
                                //Value property.
                                //This Cascade will be a Source for our expression and we take the Value of the
                                //corresponding parameter (of the lambda that our expression will become).

                                //But! if objectExpression is a call to ValueProvider.Static then we can return
                                //the argument of that call instead.
                                if (!IsStaticMethodCall(objectExpression, out vpSource))
                                {
                                    vpSource =
                                        Expression.Call(
                                            CascadeMethodInfo.MakeGenericMethod(m_Type),
                                            objectExpression
                                        );
                                }
                            }
                            else
                            {
                                Expression innerObjectExpression;
                                MethodInfo propertyMethodInfo;

                                //if objectExpression is call to static use value for static and StaticPropertyMethodInfo
                                if (IsStaticMethodCall(objectExpression, out innerObjectExpression))
                                {
                                    objectExpression = innerObjectExpression;
                                    propertyMethodInfo = StaticPropertyMethodInfo;
                                }
                                else
                                    propertyMethodInfo = PropertyMethodInfo;

                                //Ok objectExpression is an IValueProvider<O>. we use ValueProvider.Property
                                //to get an IValueProvider<T> that returns the value of member.
                                //This will become a Source for our expression and we take the Value of the
                                //corresponding parameter (of the lambda that our expression will become).
                                vpSource =
                                    Expression.Call(
                                        propertyMethodInfo.MakeGenericMethod(
                                            m_Expression_Type,
                                            m_Type
                                        ),
                                        objectExpression,
                                        Expression.Constant(
                                            m_Member,
                                            typeof(PropertyInfo)
                                        )
                                    );
                            }

                            return GetVPReference(vpSource);
                        }
                        else
                        {
                            return base.VisitMember(m);

                            // below code serves to shield agains null ref exceptions
                            // since proper exception handling now shouldn't need it anymore
                            // TODO:Expensive
                            // translate x.f to x == null ? default(fType) : x.f 
                            //var target = Visit(m_Expression);
                            //var lambdaParam = CreateParameter(m_Expression_Type);

                            //return
                            //    Expression.Call(
                            //        SafeCallMethodInfo.MakeGenericMethod(m_Expression_Type, m_Type),
                            //        target,
                            //        Expression.Lambda(
                            //            Expression.Field(
                            //                lambdaParam,
                            //                (FieldInfo)m_Member
                            //            ),
                            //            lambdaParam
                            //        )
                            //    );
                        }
                    }
                }
            }

            return base.VisitMember(m);
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            if (_TraceableExpressions[c] == int.MaxValue)
            {
                var nextFinder = NextExpressionWriter();
                int mapIndex;

                var parameters =
                    nextFinder.MapMethodArguments(
                        iifMap,
                        new Expression[] { 
                            c.Test, 
                            c.IfTrue,
                            c.IfFalse,
                        },
                        new Type[] { typeof(bool), c.Type, c.Type },
                        out mapIndex
                    );

                Expression call;

                if (mapIndex != 7)
                    call =
                        Expression.Call(
                            iifMap.TargetMethods[mapIndex].First.MakeGenericMethod(c.Type),
                            parameters
                        );
                else
                    call = Expression.Condition(parameters[0], parameters[1], parameters[2]);

                Expression convert =
                    nextFinder.MakeConvertCall(
                        call,
                        mapIndex != 7
                    );

                Expression staticArg;

                return IsStaticMethodCall(convert, out staticArg) ? staticArg : GetVPReference(convert);
            }

            return base.VisitConditional(c);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (_TraceableExpressions[b] == int.MaxValue )
            {
                MapInfo mapping;

                if (b.Method != null && FindMemberMapping(b.Method, out mapping))
                {
                    return MapMethod(b.Method, ref mapping, new Expression[] { b.Left, b.Right }, b.Type);
                }

                if (b.Method == null)
                {
                    var bNodeType = b.NodeType;

                    MapInfo map;

                    var leftType = b.Left.Type;
                    var rightType = b.Right.Type;

                    switch (bNodeType)
                    {
                        case ExpressionType.AndAlso:
                            map = andMap;
                            break;
                        case ExpressionType.Coalesce:
                            map = leftType.IsGenericType && leftType.GetGenericTypeDefinition() == typeof(Nullable<>) && leftType != rightType ? coalesceNullableMap : coalesceMap;
                            break;
                        case ExpressionType.OrElse:
                            map = orMap;
                            break;
                        default:
                            map = null;
                            break;
                    }

                    if (map != null)
                    {
                        var nextFinder = NextExpressionWriter();
                        int mapIndex;
                        var b_Type = b.Type;
                        var b_Left = b.Left;
                        var b_Right = b.Right;

                        var parameters =
                            nextFinder.MapMethodArguments(
                                map,
                                new Expression[] { 
                                    b_Left,
                                    b_Right,
                                },
                                new[] { leftType, rightType },
                                out mapIndex
                            );

                        Expression call;

                        if (mapIndex != 3)
                        {
                            var targetMethod = map.TargetMethods[mapIndex].First;

                            if (map.GenericPrmsCount != 0)
                                targetMethod = targetMethod.MakeGenericMethod(b_Type);

                            call =
                                Expression.Call(
                                    targetMethod,
                                    parameters
                                );
                        }
                        else
                        {
                            var prms0 = parameters[0];
                            var prms1 = parameters[1];

                            switch (bNodeType)
                            {
                                case ExpressionType.AndAlso:
                                    call = Expression.AndAlso(prms0, prms1);
                                    break;
                                case ExpressionType.Coalesce:
                                    call = Expression.Coalesce(prms0, prms1);
                                    break;
                                case ExpressionType.OrElse:
                                    call = Expression.OrElse(prms0, prms1);
                                    break;
                                default:
                                    call = null;
                                    break;
                            }
                        }

                        Expression convert =
                            nextFinder.MakeConvertCall(
                                call,
                                mapIndex != 3
                            );

                        Expression staticArg;

                        return IsStaticMethodCall(convert, out staticArg) ? staticArg : GetVPReference(convert);
                    }
                }
            }
           
            return base.VisitBinary(b);
        }

        //Not all convert operations a cross hierarchy conversions (casts)
        protected override Expression VisitUnary(UnaryExpression u)
        {
            var u_NodeType = u.NodeType;
            var u_Operand = u.Operand;
            var u_Type = u.Type;

            if (
                _TraceableExpressions[u] == int.MaxValue &&
                (u_NodeType == ExpressionType.Convert || u_NodeType == ExpressionType.ConvertChecked)
                )
            {
                MapInfo mapping;

                if(u.Method != null && FindMemberMapping(u.Method, out mapping))
                {
                    return MapMethod(u.Method, ref mapping, new Expression[] { u.Operand }, u.Type);
                }

                if (
                    u.Method == null
                    && !u_Operand.Type.IsValueType
                    && !u_Type.IsValueType
                )
                {
                    //map '(T)x' to 'x as T' if type of x and T are not value types. 
                    return Expression.TypeAs(base.Visit(u_Operand), u_Type);
                }
            }

            return base.VisitUnary(u);
        }


        class LambdaInvocationExpander : System.Linq.Expressions.ExpressionVisitor
        {
            Dictionary<ParameterExpression, Expression> _ParameterMap;

            protected override Expression VisitParameter(ParameterExpression p)
            {
                Expression mappedExpression;
               
                return _ParameterMap.TryGetValue(p, out mappedExpression) ? mappedExpression : base.VisitParameter(p);
            }

            /// <summary>
            /// For invocation of lambda expression. Replaces all parameters with arguments directly
            /// and return body of lambda expression. 
            /// 
            /// so (A.v,B)((a, b) => a + b * a) becomes A.v + B * A.v
            /// 
            /// </summary>
            /// <param name="iv"></param>
            /// <returns></returns>
            public static Expression ExpandLambdaBody(InvocationExpression iv)
            {
                //iv must be invocation of lambdaexpression.
                var invokedLambda = (LambdaExpression)iv.Expression;
                var invokedLambda_Parameters = invokedLambda.Parameters;
                var iv_Arguments = iv.Arguments;

                Dictionary<ParameterExpression, Expression> parameterMap = new Dictionary<ParameterExpression, Expression>();

                for (int i = 0, end = iv_Arguments.Count; i < end; ++i)
                    parameterMap.Add(invokedLambda_Parameters[i], iv_Arguments[i]);

                var expander = new LambdaInvocationExpander { _ParameterMap = parameterMap };
                return expander.Visit(invokedLambda.Body);
            }
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            if (_TraceableExpressions[iv] == int.MaxValue && iv.Expression.NodeType == ExpressionType.Lambda)
            {
                //invocation of lambda expression. Expand. (replace all parameters with arguments and use body directly)
                var expandedBody = LambdaInvocationExpander.ExpandLambdaBody(iv);

                if (iv.Type != expandedBody.Type)
                {
                    //patch if body not equal type as invocation.
                    expandedBody = Expression.Convert(expandedBody, iv.Type);
                }

                //replaced part of expression tree. Reevaluate tracability for that part.
                new TraceabilityTester(_TraceableExpressions).Test(expandedBody);

                return Visit(expandedBody);
            }

            return base.VisitInvocation(iv);
        }

        protected override Expression VisitMethodCall(MethodCallExpression orgMethodCall)
        {
            var orgMethodCall_Method = orgMethodCall.Method;

            if (_TraceableExpressions[orgMethodCall] == int.MaxValue && orgMethodCall_Method != null)
            {
                //See if we can map System.Linq.Enumerable methods to equivalent observable
                //Obtics.Collections.Implicit.ObservableEnumerable methods

                MapInfo mapping;

                if( FindMemberMapping(orgMethodCall_Method, out mapping) )
                {
                    var arguments = orgMethodCall.Object == null ? orgMethodCall.Arguments : new Expression[] { orgMethodCall.Object }.Concat(orgMethodCall.Arguments);
                    var resultType = orgMethodCall.Type;

                    return MapMethod(orgMethodCall_Method, ref mapping, arguments, resultType);
                }

                // below code servs to protecd agains null ref exceptions
                // since have better exception handling now.. shouldn't need this
                // TODO:Expensive
                // check if target can be null
                //if (
                //    orgMethodCall.Object != null
                //    && !orgMethodCall.Object.Type.IsValueType
                //)
                //{
                //    //map x.f() to SafeCall(m,_m => _m.f()); SafeCall verifies if x is null and returns a default value if so.
                //    var visitedMethodCall = (MethodCallExpression)base.VisitMethodCall(orgMethodCall);
                //    var visitedMethodCall_Object = visitedMethodCall.Object;
                //    var visitedMethodCall_Object_Type = visitedMethodCall_Object.Type;
                //    var lambdaParam = CreateParameter(visitedMethodCall_Object_Type);

                //    return
                //        Expression.Call(
                //            SafeCallMethodInfo.MakeGenericMethod(visitedMethodCall_Object_Type, visitedMethodCall.Type),
                //            visitedMethodCall_Object,
                //            Expression.Lambda(
                //                Expression.Call(
                //                    lambdaParam,
                //                    visitedMethodCall.Method,
                //                    visitedMethodCall.Arguments
                //                ),
                //                lambdaParam
                //            )
                //        );
                //}
            }

            return base.VisitMethodCall(orgMethodCall);
        }

        private Expression MapMethod(MethodInfo orgMethodCall_Method, ref MapInfo mapping, IEnumerable<Expression> arguments, Type resultType)
        {
                    int mapIndex;
                    var nextFinder = mapping.ResultDirectlyObservable ? this : NextExpressionWriter();
                    var parameters = 
                        nextFinder.MapMethodArguments(
                            mapping, 
                            //if static then Object is null; use only arguments. If instance method add 'this' as first argument
                            arguments,
                            ( orgMethodCall_Method.IsStatic ? Type.EmptyTypes : new Type[] { orgMethodCall_Method.DeclaringType } ).Concat( orgMethodCall_Method.GetParameters().Select(pInfo => pInfo.ParameterType) ),
                            out mapIndex
                        );

                    var targetMethod = mapping.TargetMethods[mapIndex].First;

                    if (mapping.GenericPrmsCount != 0)
                    {
                        //find type args. 
                        var i = mapping.GenericPrmsCount;
                        IEnumerable<Type> argsSeq;

                        //First from generic(?) method
                        if (orgMethodCall_Method.IsGenericMethod)
                        {
                            var typeArgs = orgMethodCall_Method.GetGenericArguments();
                            i -= typeArgs.Length;
                            argsSeq = typeArgs;
                        }
                        else
                            argsSeq = Enumerable.Empty<Type>();

                        //Then from generic(?) declaring type
                        if (i != 0)
                        {
                            var typeArgs = orgMethodCall_Method.DeclaringType.GetGenericArguments();
                            argsSeq = typeArgs.Concat(argsSeq);
                        }

                        targetMethod = targetMethod.MakeGenericMethod(argsSeq.ToArray());
                    }

                    var targetMethodCall =
                        Expression.Call(
                            targetMethod,
                            parameters
                        );

                    if (mapping.ResultDirectlyObservable)
                    {
                        //The targetmethod returns a value
                        return mapping.NeedsUpcast ? (Expression)Expression.Convert(targetMethodCall, resultType) : targetMethodCall;
                    }
                    else
                    {
                        //The targetmethod returns a value provider.
                        return
                            GetVPReference(
                                nextFinder.MakeConvertCall(
                                    targetMethodCall,
                                    true
                                )
                            );
                    }
        }


        private bool FindMemberMapping(MemberInfo orgMember, out MapInfo mapping)
        {

            //first search method directly
            if (ExpressionObserverMappingHelper.TryGetMapping(orgMember, out mapping))
                return true;

            //elseif is generic try generic definition
            var orgMethod = orgMember as MethodInfo;

            if (orgMethod != null && orgMethod.IsGenericMethod && ExpressionObserverMappingHelper.TryGetMapping(orgMethod.GetGenericMethodDefinition(), out mapping))
                return true;

            //elseif is member of generic type try declaration in generic type definition ( find via MetadataToken )
            if (orgMember.DeclaringType.IsGenericType)
            {
                var declaredMembers =
                    orgMember.DeclaringType
                        .GetGenericTypeDefinition()
                        .FindMembers(
                            orgMember.MemberType,
                            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public,
                            (MemberFilter)delegate(MemberInfo mInfo, object obj) { return mInfo.MetadataToken == (int)obj; },
                            orgMember.MetadataToken
                        );

                if (declaredMembers.Length > 0) //if not then member must have been protected or private
                    return ExpressionObserverMappingHelper.TryGetMapping(declaredMembers[0], out mapping);
            }

            return false;
        }

        #endregion

        static Expression ObserveLambdaBody(LambdaExpression ex)
        {
            var traceableExpressions = new Dictionary<Expression, int>();

            if( ex.Parameters != null )
                foreach (var prm in ex.Parameters)
                    traceableExpressions[prm] = int.MaxValue;

            //in case return type of lambda is not return type of body
            //we needto do an explicit cast since implicit cast of IValueProvider<Derived> -> IValueProvider<Base>
            //is not possible
            var body = ex.Body;
            var lambdaReturnType = ex.Type.GetMethod("Invoke").ReturnType; 

            if (body.Type != lambdaReturnType)
                body = Expression.Convert(body, lambdaReturnType);

            new TraceabilityTester(traceableExpressions).Test(body);

            try
            {
                var nextFinder = new ExpressionRewriter(traceableExpressions, new ParamCountContainer());
                var nextExp = nextFinder.Visit(body);

                return nextFinder.MakeConvertCall(nextExp, false);
            }
            catch (ArgumentException argEx)
            {
                //We expect this exception only when rewriting expression with
                //call to Enumerable.ToList() and ToDictionary()
                throw new RewriteException(Resources.RewriteExceptionMessage, argEx);
            }
        }

        //cache the results of the pipeline factories.
        //The result of a successful rewrite and compilation is not a value transformation pipeline but
        //rather a delegate (object) that knows how to create a value transformation pipeline from certain
        //input parameters (factory) .
        //It is not uncommon that this process of creating a pipeline (or finding a previously created one)
        //is an expensive process.
        //
        //In this section we cap the created factory of with a caching object (Carrousel). 

        static MethodInfo[] Cachers = { 
            typeof(RewriterHelper).GetMethod("Cacher0"),
            typeof(RewriterHelper).GetMethod("Cacher1"),
            typeof(RewriterHelper).GetMethod("Cacher2"),
            typeof(RewriterHelper).GetMethod("Cacher3"),
            typeof(RewriterHelper).GetMethod("Cacher4"),
            typeof(RewriterHelper).GetMethod("Cacher5"),
            typeof(RewriterHelper).GetMethod("Cacher6"),
            typeof(RewriterHelper).GetMethod("Cacher7"),
            typeof(RewriterHelper).GetMethod("Cacher8"),
            typeof(RewriterHelper).GetMethod("Cacher9"),
            typeof(RewriterHelper).GetMethod("Cacher10"),
            typeof(RewriterHelper).GetMethod("Cacher11"),
            typeof(RewriterHelper).GetMethod("Cacher12"),
            typeof(RewriterHelper).GetMethod("Cacher13"),
            typeof(RewriterHelper).GetMethod("Cacher14"),
            typeof(RewriterHelper).GetMethod("Cacher15"),
            typeof(RewriterHelper).GetMethod("Cacher16")
        };


        public static LambdaExpression Rewrite(LambdaExpression original)
        {
            if (original == null) return null;

            var originalParameters = original.Parameters.ToArray();

            if (originalParameters.Length >= Cachers.Length)
                throw new ArgumentException("Can not rewrite lambda expressions with more than 16 parameters.");

            var innerLambda = Expression.Lambda(ExpressionRewriter.ObserveLambdaBody(original), originalParameters);

            var outerParameters = originalParameters.Select(prm => Expression.Parameter(prm.Type, "_" + prm.Name)).ToArray();

            //wrap the lambda in a call to a caching function
            return
                Expression.Lambda(
                    Expression.Call(
                        Cachers[outerParameters.Length].MakeGenericMethod(outerParameters.Select(prm => prm.Type).Concat(new Type[] { original.Type.GetMethod("Invoke").ReturnType }).ToArray()),
                        ( new Expression[] { innerLambda } ).Concat( outerParameters.Cast<Expression>() ).ToArray()
                    ),
                    outerParameters
                );
        }

    }
}
