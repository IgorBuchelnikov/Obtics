using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Obtics.Values
{
    /// <summary>
    /// Interface for objects that can map members to live methods.
    /// </summary>
    public interface IExpressionObserverMappingProvider
    {
        /// <summary>
        /// Gives live mappings for members.
        /// </summary>
        /// <param name="memberToMap">The member (field, property or method) mapped methods should be returned for.</param>
        IEnumerable<MethodInfo> GetMappings(MemberInfo memberToMap);
    }
}
