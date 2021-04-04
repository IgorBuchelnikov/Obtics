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
    public class AssertFailedException : Exception
    {
        // Methods
        public AssertFailedException()
        {
        }

        public AssertFailedException(string message)
            : base(message)
        {
        }


        public AssertFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

#if !SILVERLIGHT
        protected AssertFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }


}
