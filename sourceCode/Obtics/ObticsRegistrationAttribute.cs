using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ObticsRegistrationAttribute : Attribute
    {
    }
}
