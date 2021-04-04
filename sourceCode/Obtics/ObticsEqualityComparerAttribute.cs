using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Obtics
{

    /// <summary>
    /// Register an ExpressionObserver mapping for the member. This mapping is available in all ExpressionObserver(Object)s and can not be removed by an ExpressionObserverMaster.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Enum| AttributeTargets.Struct, AllowMultiple=false, Inherited=true)]
    public class ObticsEqualityComparerAttribute : Attribute
    {
        Type _EqualityComparerType;

        public Type EqualityComparerType { get { return _EqualityComparerType; } }

        /// <summary>
        /// Register a specific equality comparer for obtics to use when comparing item of or derived of the type.
        /// </summary>
        /// <param name="targetMethods">The target methods the member is mapped to.</param>
        public ObticsEqualityComparerAttribute(Type equalityComparer)
        {
            if (equalityComparer == null)
                throw new ArgumentNullException("equalityComparer");

            if (!typeof(IEqualityComparer).IsAssignableFrom(equalityComparer))
                throw new ArgumentException( string.Format("equalityComparer must implement {0}", typeof(IEqualityComparer).FullName), "equalityComparer");

            if(equalityComparer.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("equalityComparer must have a default constructor", "equalityComparer");

            _EqualityComparerType = equalityComparer;
        }
    }
}
