using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TvdP.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ClassCleanupAttribute : Attribute
    {
    }
}
