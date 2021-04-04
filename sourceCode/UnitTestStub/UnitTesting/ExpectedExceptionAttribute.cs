using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TvdP.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ExpectedExceptionAttribute : Attribute
    {
        // Fields
        private Type m_exceptionType;
        private string m_message;

        // Methods
        public ExpectedExceptionAttribute(Type exceptionType)
            : this(exceptionType, string.Empty)
        {
        }

        public ExpectedExceptionAttribute(Type exceptionType, string message)
        {
            this.m_exceptionType = exceptionType;
            this.m_message = message;
        }

        // Properties
        public Type ExceptionType
        {
            get
            {
                return this.m_exceptionType;
            }
        }

        public string Message
        {
            get
            {
                return this.m_message;
            }
        }
    }


}
