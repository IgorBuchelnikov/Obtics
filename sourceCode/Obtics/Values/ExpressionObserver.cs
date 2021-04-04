using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using TvdP.Collections;
using System.Reflection;

namespace Obtics.Values
{
    /// <summary>
    /// Class with methods to create an observable LambdaExpression out of any LambdaExpression.
    /// </summary>
    /// <remarks>
    /// This class is concerned with rewriting non-observable LamdaExpressions into observable LambdaExpressions.
    /// <para>
    /// A LambdaExpression can be compiled and the result of this compilation is a Lambda function or in short just a lambda. This
    /// lambda can be invoked with 0 or more arguments and it calculates a single result which is the return value of the lambda. A lambda
    /// is called pure if it always returns the same result for any set of arguments. A lamda is not pure if it depends on external
    /// variables. A lambda is 'pseudo pure' if it does not depend on any independent external variables. A variable is independent
    /// if it is not a property of any of the lamda's arguments. For example; if Person is an immutable object then the function 'f'
    /// below is pure. If Person is mutable then the lambda is pseudo pure.
    /// 
    /// <code>
    /// Func&lt;Person,string&gt; f = p => p.FirstName ;
    /// </code>
    /// 
    /// The following code is not pure and not pseudo pure since it depends on the independent external variable 'firstName'
    /// 
    /// <code>
    /// bool firstName;
    /// Func&lt;Person,string&gt; f = p => firstName ? p.FirstName : p.LastName ;
    /// </code>
    /// </para>
    /// 
    /// <para>
    /// A non-observable LambdaExpression is an Expression that when compiled would calculate an intended result from a set of
    /// input parameters. This calculation is one-time. In the case of non-pure lamda's, to get an updated result, the lambda needs to be called again and
    /// it is upto the client to determine when it needs to be called. 
    /// </para>
    /// <para>
    /// An Observable LambdaExpression is an Expression that, when compiled, would return a ValueProvider. The Value of the ValueProvider 
    /// would contain the intended result. Whenever a change is detected in any of parameters passed to the Lambda the ValueProvider will
    /// send a change notification and the Value property of the ValueProvider will contain the new result. So, in the case of non-pure
    /// lambda's the client is relieved of the task to determine when the result needs to be recalculated. Also; in the case that all mutable properties
    /// accessed are themselves observable and any functions accessed within the lambda are pure then the observable variant of
    /// a pseudo pure non-observable lamda will be pure. So the following code will write 'true' to the console.
    /// 
    /// <code>
    /// Func&lt;Person,string&gt; f = ExpressionObserver&lt;Person&gt;.Compile( p => p.FirstName ) ;
    /// Person p = new Person("Glenn","Miller");
    /// var first = f(p);
    /// p.FirstName = "John";
    /// var second = f(p);
    /// Console.Out.WriteLine(object.Equals(first,second));
    /// </code>
    /// </para>
    /// <para>
    /// ExpressionObserver contains three sets of static methods:
    /// 
    /// <list type="bullet">
    /// <item>
    /// <term>Rewrite</term>
    /// <description>
    /// These methods take a non-observable LambdaExpression and return a new Obserable LambdaExpression. When
    /// compiled the new Observable LambdaExpression generates an Observable Lamda. This Observable Lambda has a
    /// ValueProvider as results who's Value property will generate the same result as the original compiled 
    /// LabdaExpression would.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Compile</term>
    /// <description>
    /// These methods take a non-observable LambdaExpression and call a Rewrite method with it. 
    /// After that it compiles the result and return the resulting Lambda function. These methods
    /// use a cache. If they detect that they have already Rewritten and Compiled a previous but
    /// computationaly equal LambdaExpression than they will return the previously compiled Lambda function.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Execute</term>
    /// <description>
    /// These methods take a non-observable LambdaExpression and call a Compile method with it.
    /// After that they execute the resulting Lambda function with arguments that are also passed to the
    /// Execute method and return the resulting IValueProvider.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// ExpressionObserver translates a non-observable LambdaExpression into an observable one by searching
    /// for all property references within the LambdaExpression and registering for change notifications
    /// on these properties. It does this in a cascading fashion. That means that the expression as below.
    /// <code>
    /// Expression&lt;Func&lt;Person,int&gt;&gt; f = p => p.FirstName.Length;
    /// </code>
    /// Is translated into something like
    /// <code>
    /// Expression&lt;Func&lt;Person,IValueProvider&lt;int&gt;&gt;&gt; f = p => ValueProvider.Property(ValueProvider.Property(ValueProvider.Static(p), "FirstName"), "Length");
    /// </code>
    /// </para>
    /// </remarks>
    public static class ExpressionObserver
    {
        #region Rewrite

        //Returns an observable variation of a given LambdaExpression

        /// <summary>
        /// Rewrites a LambdaExpression to a new reactive LambdaExpression that will return an IValueProvider;
        /// </summary>
        /// <param name="original">The original, presumably not-observable LambdaExpression</param>
        /// <returns>
        /// Reactive LambdaExpression which in turn will return an IValueProvider of the original return type.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static LambdaExpression Rewrite(LambdaExpression original)
        { return ExpressionRewriter.Rewrite(original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<IValueProvider<TResult>>> Rewrite<TResult>(Expression<Func<TResult>> original)
        { return (Expression<Func<IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }

        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression (<see cref="Expression{Func}"/>). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, IValueProvider<TResult>>> Rewrite<TPrm1, TResult>(Expression<Func<TPrm1, TResult>> original)
        { return (Expression<Func<TPrm1, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }

        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TResult>(Expression<Func<TPrm1, TPrm2, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }

        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }

        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm14">Type of the thirteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm14">Type of the thirteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm15">Type of the fifteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }


        /// <summary>
        /// Rewrites a LambdaExpression that returns <typeparamref name="TResult"/> to a LambdaExpression that will return an <see cref="IValueProvider{TResult}"/>.
        /// This <see cref="IValueProvider{TResult}"/> will be reactive to changes of observable dependencies of the original LambdaExpression. 
        /// </summary>
        /// <typeparam name="TPrm1">Type of the first argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm8">Type of the seventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm14">Type of the thirteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm15">Type of the fifteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TPrm16">Type of the sixteenth argument to be passed to the <paramref name="original"/> LambdaExpression and the result LambdaExpression</typeparam>
        /// <typeparam name="TResult">Type of the result returned by the <paramref name="original"/> LambdaExpression (when compiled) and type of the <see cref="IValueProvider{TResult}.Value"/> property of the <see cref="IValueProvider{TResult}"/> returned by the rewritten LambdaExpression</typeparam>
        /// <param name="original">The original, presumably not-observable, LambdaExpression ((<see cref="Expression{Func}"/>)). The return value of this LambdaExpression when compiled should be <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A LambdaExpression which when compiled will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this LambdaExpression form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.       
        /// </remarks>
        public static Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, IValueProvider<TResult>>> Rewrite<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, TResult>> original)
        { return (Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, IValueProvider<TResult>>>)Rewrite((LambdaExpression)original); }

        #endregion

        #region Compile

        //Gives a compiled observable variation of a LambdaExpression.
        //The compiled result is stored in a cache.

        //This cache stores its keys (Expressions) with strong references. This is neccessairy to have Transformation pipeline comparison
        //work correctly. The compiled versions of two 'equal' Expression trees need to be equal too a.k. be the same instance.
        //By having strong references to the source Expressions it will always be possible to find a source expression as
        //long as there is a reference to the compiled version.
        static Cache<Expression, Delegate> _CompilationCache = new Cache<Expression, Delegate>(ObticsEqualityComparer<Expression>.Default);

        /// <summary>
        /// Rewrites a <see cref="LambdaExpression"/> to a new reactive <see cref="LambdaExpression"/> that will return an IValueProvider and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="LambdaExpression"/></param>
        /// <returns>
        /// Reactive <see cref="Delegate"/> which in turn will return an IValueProvider of the original return type
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="LambdaExpression"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="LambdaExpression"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="LambdaExpression"/> is computationaly equal to a previously compiled <see cref="LambdaExpression"/>
        /// then they will return the cached <see cref="Delegate"/>.
        /// </remarks>
        public static Delegate Compile(LambdaExpression original)
        {
            if (original == null) return null;

            Delegate f;

            if (!_CompilationCache.TryGetItem(original, out f))
                f = _CompilationCache.GetOldest(original, Rewrite(original).Compile());

            return f;
        }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="Func{IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{IValueProvider}"/>.
        /// </remarks>
        public static Func<IValueProvider<TResult>> Compile<TResult>(Expression<Func<TResult>> original)
        { return ((Func<IValueProvider<TResult>>)Compile((LambdaExpression)original)); }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the argument for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, IValueProvider<TResult>> Compile<TPrm1, TResult>(Expression<Func<TPrm1, TResult>> original)
        { return ((Func<TPrm1, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TResult>(Expression<Func<TPrm1, TPrm2, TResult>> original)
        { return ((Func<TPrm1, TPrm2, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm14">Type of the fourteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm14">Type of the fourteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm15">Type of the fifteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/> and compiles the result.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm8">Type of the eight argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm14">Type of the fourteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm15">Type of the fifteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm16">Type of the sixteenth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <returns>
        /// Reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> which in turn will return an <see cref="IValueProvider{TResult}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously rewritten and compiled <see cref="Expression{Func}"/>
        /// then they will return the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// </remarks>
        public static Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, IValueProvider<TResult>> Compile<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, TResult>(Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, TResult>> original)
        { return ((Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, IValueProvider<TResult>>)Compile((LambdaExpression)original)); }

        #endregion

        #region Execute

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it.
        /// </summary>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached <see cref="Func{IValueProvider}"/> will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TResult>(Expression<Func<TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)();
        }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>.
        /// </summary>
        /// <param name="prm1">The argument to be passed to the compiled reactive <see cref="Func{TPrm1,TResult}"/> to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached <see cref="Func{TPrm1,IValueProvider}"/> will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TResult>(TPrm1 prm1, Expression<Func<TPrm1, TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)(prm1);
        }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/> and <paramref name="prm2"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TResult}"/> to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TResult}"/> to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached <see cref="Func{TPrm1,TPrm2,IValueProvider}"/> will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TResult>(TPrm1 prm1, TPrm2 prm2, Expression<Func<TPrm1, TPrm2, TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2);
        }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/> and <paramref name="prm3"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TResult}"/> to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TResult}"/> to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TResult}"/> to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached <see cref="Func{TPrm1,TPrm2,TPrm3,IValueProvider}"/> will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TResult>(TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, Expression<Func<TPrm1, TPrm2, TPrm3, TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3);
        }

        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,TResult}"/> to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,TResult}"/> to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,TResult}"/> to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,TResult}"/> to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/> will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TResult>(TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TResult>(TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TResult>(TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TResult>(TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TResult>> original)
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm10">The tenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9, TPrm10 prm10,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9, prm10);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm10">The tenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm11">The eleventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9, TPrm10 prm10, TPrm11 prm11,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9, prm10, prm11);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm10">The tenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm11">The eleventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm12">The twelveth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9, TPrm10 prm10, TPrm11 prm11, TPrm12 prm12,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9, prm10, prm11, prm12);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm10">The tenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm11">The eleventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm12">The twelveth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm13">The thirteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9, TPrm10 prm10, TPrm11 prm11, TPrm12 prm12, TPrm13 prm13,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9, prm10, prm11, prm12, prm13);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm10">The tenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm11">The eleventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm12">The twelveth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm13">The thirteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm14">The fourteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm14">Type of the fourteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9, TPrm10 prm10, TPrm11 prm11, TPrm12 prm12, TPrm13 prm13, TPrm14 prm14,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9, prm10, prm11, prm12, prm13, prm14);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm10">The tenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm11">The eleventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm12">The twelveth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm13">The thirteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm14">The fourteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm15">The fifteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm14">Type of the fourteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm15">Type of the fifteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9, TPrm10 prm10, TPrm11 prm11, TPrm12 prm12, TPrm13 prm13, TPrm14 prm14, TPrm15 prm15,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9, prm10, prm11, prm12, prm13, prm14, prm15);
        }


        /// <summary>
        /// Rewrites a <see cref="Expression{Func}"/> to a new reactive <see cref="Expression{Func}"/> that will return an <see cref="IValueProvider{TResult}"/>, compiles the result and executes it with the given <paramref name="prm1"/>, <paramref name="prm2"/>, <paramref name="prm3"/> and <paramref name="prm4"/>.
        /// </summary>
        /// <param name="prm1">The first argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm2">The second argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm3">The third argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm4">The fourth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm5">The fifth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm6">The sixth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm7">The seventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm8">The eigth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm9">The nineth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm10">The tenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm11">The eleventh argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm12">The twelveth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm13">The thirteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm14">The fourteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm15">The fifteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="prm16">The sixteenth argument to be passed to the compiled reactive lambda function to get the final result.</param>
        /// <param name="original">The original, presumably not-observable <see cref="Expression{Func}"/></param>
        /// <returns>
        /// Reactive <see cref="IValueProvider{TResult}"/> or null returned by the compiled <see cref="Func{TPrm1,TPrm2,TPrm3,TPrm4,IValueProvider}"/>.
        /// If <paramref name="original"/> equals null then null will be returned instead.
        /// </returns>
        /// <typeparam name="TPrm1">Type of the first argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm2">Type of the second argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm3">Type of the third argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm4">Type of the fourth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm5">Type of the fifth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm6">Type of the sixth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm7">Type of the seventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm8">Type of the eigth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm9">Type of the nineth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm10">Type of the tenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm11">Type of the eleventh argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm12">Type of the twelveth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm13">Type of the thirteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm14">Type of the fourteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm15">Type of the fifteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TPrm16">Type of the sixteenth argument to <paramref name="original"/> and the resulting lambda function.</typeparam>
        /// <typeparam name="TResult">Type of the return value for <paramref name="original"/>.</typeparam>
        /// <remarks>
        /// This method will try to rewrite the original Expression to a form that will yield a maximum reactive value transformation.
        /// The new expression will try to listen for changes on all property references in the original expression. Lambda functions
        /// enclosed in this <see cref="Expression{Func}"/> form a limit to the reactiveness of the result. Any values that depend
        /// on an argument of an inner lambda function will not get tracked for changes. The changes can not be tracked because this
        /// inner lambda will be consumed my some function and the rewriteer can not guess how this function will use that lambda.
        /// More importantly it doesn't know how many times the lambda will be called by that function.
        /// Note that many Implicitly observable collection transformations (Observable LINQ) will consume lambda functions
        /// but they will rewrite their own lambda's.
        /// 
        /// The resulting <see cref="Expression{Func}"/> will not always be compiled. The Compile methods maintain a cache and if
        /// the detect that a given original <see cref="Expression{Func}"/> is computationaly equal to a previously compiled <see cref="Expression{Func}"/>
        /// then the cached lambda function will be executed.
        /// </remarks>
        public static IValueProvider<TResult> Execute<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, TResult>(
            TPrm1 prm1, TPrm2 prm2, TPrm3 prm3, TPrm4 prm4, TPrm5 prm5, TPrm6 prm6, TPrm7 prm7, TPrm8 prm8, TPrm9 prm9, TPrm10 prm10, TPrm11 prm11, TPrm12 prm12, TPrm13 prm13, TPrm14 prm14, TPrm15 prm15, TPrm16 prm16,
            Expression<Func<TPrm1, TPrm2, TPrm3, TPrm4, TPrm5, TPrm6, TPrm7, TPrm8, TPrm9, TPrm10, TPrm11, TPrm12, TPrm13, TPrm14, TPrm15, TPrm16, TResult>> original
        )
        {
            if (original == null) return null;
            return Compile(original)(prm1, prm2, prm3, prm4, prm5, prm6, prm7, prm8, prm9, prm10, prm11, prm12, prm13, prm14, prm15, prm16);
        }

        #endregion

        #region Unobserve

        /// <summary>
        /// This expression shields the arguments to a given lambda from rewriting when used in an observed expression.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TOut">Type of the return value of <paramref name="lambda"/>.</typeparam>
        /// <param name="in1">First argument to <paramref name="lambda"/>.</param>
        /// <param name="lambda">The lambda (expression) whose components that are directly dependent on it's arguments will be rewritten by <see cref="ExpressionObserver"/>.</param>
        /// <returns>The value returned by <paramref name="lambda"/>.</returns>
        /// <remarks>
        /// ExpressionObserver never rewrites arguments of inner lambda functions unless they lambda is an argument to a special function (Enumerable member functions).
        /// 
        /// This method is an easy way to inject an inner lambda in an observed expression. Note that only those parts of the inner lambda that are directly dependend on one of the lambda arguments, are protected
        /// from rewriting.
        /// </remarks>
        public static TOut Unobserve<TIn1, TOut>(TIn1 in1, Func<TIn1, TOut> lambda) { return lambda(in1); }

        /// <summary>
        /// This expression shields the arguments to a given lambda from rewriting when used in an observed expression.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TOut">Type of the return value of <paramref name="lambda"/>.</typeparam>
        /// <param name="in1">First argument to <paramref name="lambda"/>.</param>
        /// <param name="in2">Second argument to <paramref name="lambda"/>.</param>
        /// <param name="lambda">The lambda (expression) whose components that are directly dependent on it's arguments will be rewritten by <see cref="ExpressionObserver"/>.</param>
        /// <returns>The value returned by <paramref name="lambda"/>.</returns>
        /// <remarks>
        /// ExpressionObserver never rewrites arguments of inner lambda functions unless they lambda is an argument to a special function (Enumerable member functions).
        /// 
        /// This method is an easy way to inject an inner lambda in an observed expression. Note that only those parts of the inner lambda that are directly dependend on one of the lambda arguments, are protected
        /// from rewriting.
        /// </remarks>
        public static TOut Unobserve<TIn1, TIn2, TOut>(TIn1 in1, TIn2 in2, Func<TIn1, TIn2, TOut> lambda) { return lambda(in1, in2); }

        /// <summary>
        /// This expression shields the arguments to a given lambda from rewriting when used in an observed expression.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TIn3">Type of the third argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TOut">Type of the return value of <paramref name="lambda"/>.</typeparam>
        /// <param name="in1">First argument to <paramref name="lambda"/>.</param>
        /// <param name="in2">Second argument to <paramref name="lambda"/>.</param>
        /// <param name="in3">Third argument to <paramref name="lambda"/>.</param>
        /// <param name="lambda">The lambda (expression) whose components that are directly dependent on it's arguments will be rewritten by <see cref="ExpressionObserver"/>.</param>
        /// <returns>The value returned by <paramref name="lambda"/>.</returns>
        /// <remarks>
        /// ExpressionObserver never rewrites arguments of inner lambda functions unless they lambda is an argument to a special function (Enumerable member functions).
        /// 
        /// This method is an easy way to inject an inner lambda in an observed expression. Note that only those parts of the inner lambda that are directly dependend on one of the lambda arguments, are protected
        /// from rewriting.
        /// </remarks>
        public static TOut Unobserve<TIn1, TIn2, TIn3, TOut>(TIn1 in1, TIn2 in2, TIn3 in3, Func<TIn1, TIn2, TIn3, TOut> lambda) { return lambda(in1, in2, in3); }

        /// <summary>
        /// This expression shields the arguments to a given lambda from rewriting when used in an observed expression.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TIn3">Type of the third argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TIn4">Type of the fourth argument to <paramref name="lambda"/>.</typeparam>
        /// <typeparam name="TOut">Type of the return value of <paramref name="lambda"/>.</typeparam>
        /// <param name="in1">First argument to <paramref name="lambda"/>.</param>
        /// <param name="in2">Second argument to <paramref name="lambda"/>.</param>
        /// <param name="in3">Third argument to <paramref name="lambda"/>.</param>
        /// <param name="in4">Fourth argument to <paramref name="lambda"/>.</param>
        /// <param name="lambda">The lambda (expression) whose components that are directly dependent on it's arguments will be rewritten by <see cref="ExpressionObserver"/>.</param>
        /// <returns>The value returned by <paramref name="lambda"/>.</returns>
        /// <remarks>
        /// ExpressionObserver never rewrites arguments of inner lambda functions unless they lambda is an argument to a special function (Enumerable member functions).
        /// 
        /// This method is an easy way to inject an inner lambda in an observed expression. Note that only those parts of the inner lambda that are directly dependend on one of the lambda arguments, are protected
        /// from rewriting.
        /// </remarks>
        public static TOut Unobserve<TIn1, TIn2, TIn3, TIn4, TOut>(TIn1 in1, TIn2 in2, TIn3 in3, TIn4 in4, Func<TIn1, TIn2, TIn3, TIn4, TOut> lambda) { return lambda(in1, in2, in3, in4); }

        #endregion
    }
}
