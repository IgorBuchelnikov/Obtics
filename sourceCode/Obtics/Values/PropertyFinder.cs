using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Obtics.Values
{
    internal static class PropertyFinder
    {
        public static PropertyInfo FindProperty<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            var memberExp = exp.Body as MemberExpression;

            if (memberExp != null && memberExp.Expression == exp.Parameters[0])
            {
                var property = memberExp.Member as PropertyInfo;

                if(property != null)
                    return property;
            }

            throw new ArgumentException("Lambda expresion must be of form x => x.p where p is a property.");
        }
    }
}
