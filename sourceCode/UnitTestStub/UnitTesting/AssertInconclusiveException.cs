using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TvdP.UnitTesting
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class AssertInconclusiveException : Exception
    {
        // Methods
        public AssertInconclusiveException()
        {
        }

        public AssertInconclusiveException(string msg)
            : base(msg)
        {
        }

#if !SILVERLIGHT
        protected AssertInconclusiveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        public AssertInconclusiveException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }

}
