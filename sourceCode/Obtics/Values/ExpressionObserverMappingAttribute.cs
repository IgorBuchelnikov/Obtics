using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Obtics.Values
{
    /// <summary>
    /// Register an ExpressionObserver mapping for the member. This mapping is available in all ExpressionObserver(Object)s and can not be removed by an ExpressionObserverMaster.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public class ExpressionObserverMappingAttribute : Attribute
    {
        MethodInfo[] _TargetMethods;
        Type _TargetMethodsDeclaringClass;
        string _TargetMethodsName;

        //Can not have a (usefull) constructor that accepts te target methods directly.
        //It would not be possible to use this constructor when anotating a member.

        /// <summary>
        /// Register a declaring class to find the target methods in.
        /// </summary>
        /// <param name="targetMethodsDeclaringClass">The class to search for target methods.</param>
        /// <remarks>
        /// Target methods must be public static members of the declaring class and have the same name as the mapped member.
        /// </remarks>
        public ExpressionObserverMappingAttribute(Type targetMethodsDeclaringClass)
        {
            if (targetMethodsDeclaringClass == null)
                throw new ArgumentNullException("targetMethodsDeclaringClass");

            _TargetMethodsDeclaringClass = targetMethodsDeclaringClass;
            _TargetMethodsName = null;
        }

        /// <summary>
        /// Register a declaring class to find the target methods in.
        /// </summary>
        /// <param name="targetMethodsDeclaringClass">The class to search for target methods.</param>
        /// <param name="targetMethodsName">Name of the target methods.</param>
        /// <remarks>
        /// Target methods must be public static members of the declaring class and have the same name as the mapped member.
        /// </remarks>
        public ExpressionObserverMappingAttribute(Type targetMethodsDeclaringClass, string targetMethodsName)
        {
            if (targetMethodsDeclaringClass == null)
                throw new ArgumentNullException("targetMethodsDeclaringClass");

            if (targetMethodsName == null)
                throw new ArgumentNullException("targetMethodsName");

            _TargetMethodsDeclaringClass = targetMethodsDeclaringClass;
            _TargetMethodsName = targetMethodsName;
        }

        internal IEnumerable<MethodInfo> GetMappedMethods(MemberInfo sourceMember)
        {
            if (_TargetMethods == null)
            {
                var sourceMemberDeclaringType = sourceMember.DeclaringType;

                if (sourceMemberDeclaringType.IsGenericType && !sourceMemberDeclaringType.IsGenericTypeDefinition)
                {                    
                    sourceMember =
                        sourceMemberDeclaringType
                            .GetGenericTypeDefinition()
                            .FindMembers(
                                sourceMember.MemberType,
                                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public,
                                (MemberFilter)delegate(MemberInfo mInfo, object obj) { return mInfo.MetadataToken == (int)obj; },
                                sourceMember.MetadataToken
                            )
                            .FirstOrDefault() 
                        ?? sourceMember;
                }
                else
                {
                    var sourceMethod = sourceMember as MethodInfo;

                    if (sourceMethod != null && sourceMethod.IsGenericMethod)
                        sourceMember = sourceMethod.GetGenericMethodDefinition();
                }

                var enm = ExpressionObserverMappingHelper.FindMappings(sourceMember, _TargetMethodsDeclaringClass, _TargetMethodsName);
                _TargetMethods = enm == null ? new MethodInfo[0] : enm.ToArray();
            }

            return _TargetMethods;
        }
    }
}
