using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Collections;

namespace Obtics
{
    internal class ExpressionEqualityComparer<TE> : IEqualityComparer<TE>, IEqualityComparer
    {
        #region Equality methods

        static bool Visit(Expression expA, Expression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            if ( object.ReferenceEquals(expA, expB) )
                return true;

            if (expA == null || expB == null || expA.NodeType != expB.NodeType || expA.Type != expB.Type)
                return false;
           
            switch (expA.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)expA, (UnaryExpression)expB, pi);

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)expA, (BinaryExpression)expB, pi);

                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)expA, (TypeBinaryExpression)expB, pi);

                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)expA, (ConditionalExpression)expB, pi);

                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)expA, (ConstantExpression)expB);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)expA, (ParameterExpression)expB, pi);

                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)expA, (MemberExpression)expB, pi);

                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)expA, (MethodCallExpression)expB, pi);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)expA, (LambdaExpression)expB, pi);

                case ExpressionType.New:
                    return VisitNew((NewExpression)expA, (NewExpression)expB, pi);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)expA, (NewArrayExpression)expB, pi);

                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)expA, (InvocationExpression)expB, pi);

                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)expA, (MemberInitExpression)expB, pi);

                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)expA, (ListInitExpression)expB, pi);

                case ExpressionType.Default:
                    return VisitDefault((DefaultExpression)expA, (DefaultExpression)expB, pi);

                case ExpressionType.Index:
                    return VisitIndex((IndexExpression)expA, (IndexExpression)expB, pi);

                default:
                    throw new NotSupportedException(string.Format("Not supported expression type: '{0}'", expA.NodeType));
            }
        }

        private static bool VisitIndex(IndexExpression iA, IndexExpression iB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                iA.Indexer == iB.Indexer
                && VisitExpressions(iA.Arguments, iB.Arguments, pi)
                && Visit(iA.Object, iB.Object, pi)
            ;
        }

        private static bool VisitDefault(DefaultExpression deA, DefaultExpression deB, Dictionary<ParameterExpression, ParameterExpression> pi)
        { return true; }

        static bool VisitMemberBinding(MemberBinding mbA, MemberBinding mbB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            if (object.ReferenceEquals(mbA, mbB))
                return true;

            if (mbA == null || mbB == null || mbA.BindingType != mbB.BindingType || mbA.Member != mbB.Member)
                return false;

            switch (mbA.BindingType)
            {
                case MemberBindingType.Assignment :
                    return VisitAssignmenMB((MemberAssignment)mbA, (MemberAssignment)mbB, pi);
                case MemberBindingType.ListBinding :
                    return VisitListMB((MemberListBinding)mbA, (MemberListBinding)mbB, pi);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMB((MemberMemberBinding)mbA, (MemberMemberBinding)mbB, pi);
                default:
                    throw new Exception(string.Format("Unhandled memberbinding type: '{0}'", mbA.BindingType));
            }
        }

        private static bool VisitAssignmenMB(MemberAssignment mbA, MemberAssignment mbB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return Visit(mbA.Expression,mbB.Expression,pi);
        }

        private static bool VisitListMB(MemberListBinding mbA, MemberListBinding mbB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return VisitInitializers(mbA.Initializers, mbB.Initializers, pi);
        }

        private static bool VisitMemberMB(MemberMemberBinding mbA, MemberMemberBinding mbB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return VisitMemberBindings( mbA.Bindings, mbB.Bindings, pi);
        }

        static private bool VisitExpressions<ExpType>(ReadOnlyCollection<ExpType> expsA, ReadOnlyCollection<ExpType> expsB, Dictionary<ParameterExpression, ParameterExpression> pi)
            where ExpType : Expression
        {
            if ((expsA == null) != (expsB == null))
                return false;

            if (expsA == null)
                return true;

            if (expsA.Count != expsB.Count)
                return false;

            for (int i = 0, end = expsA.Count; i < end; ++i)
                if (!Visit(expsA[i], expsB[i], pi))
                    return false;

            return true;
        }

        static private bool VisitListInit(ListInitExpression expA, ListInitExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return VisitInitializers(expA.Initializers, expB.Initializers, pi) && Visit(expA.NewExpression, expB.NewExpression, pi);
        }

        private static bool VisitInitializers(ReadOnlyCollection<ElementInit> initializersA, ReadOnlyCollection<ElementInit> initializersB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            if ((initializersA) == null != (initializersB == null))
                return false;

            if (initializersA == null)
                return true;

            if (initializersA.Count != initializersB.Count)
                return false;

            for (int i = 0, end = initializersA.Count; i < end; ++i)
            {
                var initializerA = initializersA[i];
                var initializerB = initializersB[i];

                if (
                    !VisitExpressions(initializerA.Arguments, initializerB.Arguments, pi)
                    || initializerA.AddMethod != initializerB.AddMethod
                )
                    return false;
            }

            return true;
        }

        static private bool VisitMemberInit(MemberInitExpression expA, MemberInitExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return VisitMemberBindings(expA.Bindings, expB.Bindings, pi) && Visit(expA.NewExpression, expB.NewExpression, pi);
        }

        private static bool VisitMemberBindings(ReadOnlyCollection<MemberBinding> bindingsA, ReadOnlyCollection<MemberBinding> bindingsB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            if ((bindingsA) == null != (bindingsB == null))
                return false;

            if (bindingsA == null)
                return true;

            if (bindingsA.Count != bindingsB.Count)
                return false;

            for (int i = 0, end = bindingsA.Count; i < end; ++i)
                if (!VisitMemberBinding(bindingsA[i], bindingsB[i], pi))
                    return false;

            return true;
        }

        static private bool VisitInvocation(InvocationExpression expA, InvocationExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return 
                VisitExpressions(expA.Arguments,expB.Arguments, pi) 
                && Visit(expA.Expression, expB.Expression, pi);
        }

        static private bool VisitNewArray(NewArrayExpression expA, NewArrayExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return VisitExpressions(expA.Expressions, expB.Expressions, pi);                
        }

        static private bool VisitNew(NewExpression expA, NewExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                VisitExpressions(expA.Arguments,expB.Arguments, pi) 
                && expA.Constructor == expB.Constructor
                && ((expA.Members == null) == (expB.Members == null) && (expA.Members == null || expA.Members.SequenceEqual(expB.Members)));
        }

        static private bool VisitLambda(LambdaExpression expA, LambdaExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            var prmA = expA.Parameters;
            var prmB = expB.Parameters;

            if ((prmA) == null != (prmB == null))
                return false;

            if (prmA == null)
                return true;

            if (prmA.Count != prmB.Count)
                return false ;

            for( int i =0, end = prmA.Count; i < end; ++i )
                pi[prmA[i]] = prmB[i];

            return Visit(expA.Body, expB.Body, pi);
        }

        static private bool VisitMethodCall(MethodCallExpression expA, MethodCallExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                VisitExpressions(expA.Arguments,expB.Arguments, pi)
                && expA.Method == expB.Method
                && Visit(expA.Object, expB.Object,pi);
        }

        static private bool VisitMemberAccess(MemberExpression expA, MemberExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                Visit(expA.Expression, expB.Expression, pi)
                && expA.Member == expB.Member;
        }

        static private bool VisitParameter(ParameterExpression expA, ParameterExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            ParameterExpression match;

            return
                pi.TryGetValue(expA, out match)
                && object.ReferenceEquals(match,expB);
        }

        static private bool VisitConstant(ConstantExpression expA, ConstantExpression expB)
        {
            return
                object.Equals(expA.Value, expB.Value);
        }

        static private bool VisitConditional(ConditionalExpression expA, ConditionalExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                Visit(expA.IfFalse, expB.IfFalse, pi)
                && Visit(expA.IfTrue, expB.IfTrue, pi)
                && Visit(expA.Test, expB.Test, pi);
        }

        static private bool VisitTypeIs(TypeBinaryExpression expA, TypeBinaryExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                Visit(expA.Expression, expB.Expression, pi)
                && expA.TypeOperand == expB.TypeOperand;
        }

        static private bool VisitBinary(BinaryExpression expA, BinaryExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                Visit(expA.Conversion, expB.Conversion, pi)
                && expA.IsLifted == expB.IsLifted
                && expA.IsLiftedToNull == expB.IsLiftedToNull
                && Visit(expA.Left, expB.Left, pi)
                && expA.Method == expB.Method
                && Visit(expA.Right, expB.Right, pi);
        }

        static private bool VisitUnary(UnaryExpression expA, UnaryExpression expB, Dictionary<ParameterExpression, ParameterExpression> pi)
        {
            return
                expA.IsLifted == expB.IsLifted
                && expA.IsLiftedToNull == expB.IsLiftedToNull
                && expA.Method == expB.Method
                && Visit(expA.Operand, expB.Operand, pi);
        }

        #endregion

        #region Hashing methods

        static Hasher Visit(Expression exp)
        {
            if (exp == null)
                return new Hasher(459139);

            Hasher res ; 

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    res = VisitUnary((UnaryExpression)exp);
                    break;

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    res = VisitBinary((BinaryExpression)exp);
                    break;

                case ExpressionType.TypeIs:
                    res = VisitTypeIs((TypeBinaryExpression)exp);
                    break;

                case ExpressionType.Conditional:
                    res = VisitConditional((ConditionalExpression)exp); 
                    break;

                case ExpressionType.Constant:
                    res = VisitConstant((ConstantExpression)exp); 
                    break;

                case ExpressionType.Parameter:
                    res = VisitParameter(); 
                    break;

                case ExpressionType.MemberAccess:
                    res = VisitMemberAccess((MemberExpression)exp); 
                    break;

                case ExpressionType.Call:
                    res = VisitMethodCall((MethodCallExpression)exp); 
                    break;

                case ExpressionType.Lambda:
                    res = VisitLambda((LambdaExpression)exp); 
                    break;

                case ExpressionType.New:
                    res = VisitNew((NewExpression)exp); 
                    break;

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    res = VisitNewArray((NewArrayExpression)exp); 
                    break;

                case ExpressionType.Invoke:
                    res = VisitInvocation((InvocationExpression)exp); 
                    break;

                case ExpressionType.MemberInit:
                    res = VisitMemberInit((MemberInitExpression)exp); 
                    break;

                case ExpressionType.ListInit:
                    res = VisitListInit((ListInitExpression)exp); 
                    break;

                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }

            return res.AddValue( exp.NodeType ).AddRef( exp.Type );
        }

        static Hasher VisitMemberBinding(MemberBinding mb)
        {
            if (mb == null)
                return new Hasher(159249);

            Hasher res;

            switch (mb.BindingType)
            {
                case MemberBindingType.Assignment:
                    res = VisitAssignmenMB((MemberAssignment)mb);
                    break;
                case MemberBindingType.ListBinding:
                    res = VisitListMB((MemberListBinding)mb);
                    break;
                case MemberBindingType.MemberBinding:
                    res = VisitMemberMB((MemberMemberBinding)mb);
                    break;
                default:
                    throw new Exception(string.Format("Unhandled memberbinding type: '{0}'", mb.BindingType));
            }

            return res.AddValue(mb.BindingType).AddRef(mb.Member);
        }

        private static Hasher VisitAssignmenMB(MemberAssignment memberAssignment)
        { return Visit(memberAssignment.Expression); }

        private static Hasher VisitListMB(MemberListBinding memberListBinding)
        { return VisitAny(memberListBinding.Initializers); }

        private static Hasher VisitMemberMB(MemberMemberBinding memberMemberBinding)
        { return VisitAny(memberMemberBinding.Bindings, (h, b) => h.Add(VisitMemberBinding(b))); }

        static Hasher VisitExpressions<ExpType>(IEnumerable<ExpType> exps)
            where ExpType : Expression
        {
            return exps == null ? new Hasher(0x748aa9e7) : exps.Aggregate(new Hasher(98481), (h, e) => h.Add(Visit(e)));
        }

        static Hasher VisitAny<TType>(IEnumerable<TType> enm) where TType : class
        {
            return enm == null ? new Hasher(389137) : enm.Aggregate(new Hasher(0x134a8349), (h, i) => h.AddRef(i));
        }

        static Hasher VisitAny<TType>(IEnumerable<TType> enm, Func<Hasher, TType, Hasher> hash)
        {
            return enm == null ? new Hasher(193847) : enm.Aggregate(new Hasher(983091), hash);
        }


        static Hasher VisitListInit(ListInitExpression exp)
        { return VisitAny(exp.Initializers); }

        static Hasher VisitMemberInit(MemberInitExpression exp)
        {
            return
                VisitAny(exp.Bindings, (h,b) => h.Add(VisitMemberBinding(b)))
                    .Add( Visit(exp.NewExpression) );
        }

        static Hasher VisitInvocation(InvocationExpression exp)
        {
            return
                VisitExpressions(exp.Arguments)
                    .Add( Visit(exp.Expression) );
        }

        static Hasher VisitNewArray(NewArrayExpression exp)
        {
            return VisitExpressions(exp.Expressions); 
        }

        static Hasher VisitNew(NewExpression exp)
        {
            return
                VisitExpressions(exp.Arguments)
                    .AddRef(exp.Constructor)
                    .Add( VisitAny(exp.Members) );
        }


        static Hasher VisitLambda(LambdaExpression exp)
        {
            return
                Visit(exp.Body)
                    .AddValue( exp.Parameters == null ? (int)0x2347ade3 : exp.Parameters.Count );
        }

        static Hasher VisitMethodCall(MethodCallExpression exp)
        {
            return
                VisitExpressions(exp.Arguments)
                    .AddRef( exp.Method )
                    .Add( Visit(exp.Object));
        }

        static Hasher VisitMemberAccess(MemberExpression exp)
        {
            return
                Visit(exp.Expression)
                    .AddRef(exp.Member);
        }

        static Hasher VisitParameter()
        {
            return new Hasher(120047); //ignore parameter order for now.. #(a,b) => a / b == #(a,b) => b / a
        }

        static Hasher VisitConstant(ConstantExpression exp)
        {
            return Hasher.CreateFromRef(exp.Value);
        }

        static Hasher VisitConditional(ConditionalExpression exp)
        {
            return
                Visit(exp.IfFalse)
                    .Add( Visit(exp.IfTrue))
                    .Add( Visit(exp.Test));
        }

        static Hasher VisitTypeIs(TypeBinaryExpression exp)
        {
            return
                Visit(exp.Expression)
                    .AddRef(exp.TypeOperand);
        }


        static Hasher VisitBinary(BinaryExpression exp)
        {
            return
                Visit(exp.Conversion)
                    .AddValue(exp.IsLifted)
                    .AddValue(exp.IsLiftedToNull)
                    .Add( Visit(exp.Left))
                    .AddRef(exp.Method)
                    .Add( Visit(exp.Right));
        }

        static Hasher VisitUnary(UnaryExpression exp)
        {
            return
                Visit(exp.Operand)
                    .AddValue(exp.IsLifted)
                    .AddValue(exp.IsLiftedToNull)
                    .AddRef(exp.Method);
        }

        #endregion

        #region IEqualityComparer<TType> Members

        public bool Equals(TE x, TE y)
        { return Visit((Expression)(object)x, (Expression)(object)y, new Dictionary<ParameterExpression, ParameterExpression>()); }

        public int GetHashCode(TE obj)
        { return Visit((Expression)(object)obj).Value; }

        #endregion

        #region IEqualityComparer Members

        public new bool Equals(object x, object y)
        { return Equals((TE)x, (TE)y); }

        public int GetHashCode(object obj)
        { return GetHashCode((TE)obj); }

        #endregion
    }
}
