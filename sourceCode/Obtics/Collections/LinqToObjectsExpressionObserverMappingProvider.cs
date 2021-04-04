using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Obtics.Collections
{
    /// <summary>
    /// Object that registers expression observer mappings for LINQ to objects with an ExpressionObserverMaster.
    /// </summary>
    [ObticsRegistration]
    public sealed class LinqToObjectsExpressionObserverMappingProvider : Obtics.Values.IExpressionObserverMappingProvider
    {
        #region IExpressionObserverMappingProvider Members

        public IEnumerable<MethodInfo> GetMappings(MemberInfo memberToMap)
        {
            return 
                memberToMap.DeclaringType == typeof(Enumerable) ? Obtics.Values.ExpressionObserverMappingHelper.FindMappings(memberToMap, typeof(Obtics.Collections.ObservableEnumerable), null) :
                null
            ;
        }

        #endregion
    }
}
