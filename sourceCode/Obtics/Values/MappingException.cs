﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Obtics.Values
{
    /// <summary>
    /// This exception is raised by <see cref="ExpressionObserverMappingHelper"/> if a sertain member mapping has been given but the
    /// mapping does not match the member or its sibling mappings.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class MappingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the Obtics.Values.MappingException class.
        /// </summary>
        public MappingException()
        { }

        /// <summary>
        ///     Initializes a new instance of the Obtics.Values.MappingException class with a specified
        ///     error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MappingException(string message)
            : base(message)
        { }

#if !SILVERLIGHT
        /// <summary>
        ///     Initializes a new instance of the Obtics.Values.MappingException class with serialized
        ///     data.
        /// </summary>
        /// <param name="info">
        /// The System.Runtime.Serialization.SerializationInfo that holds the serialized
        /// object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The System.Runtime.Serialization.StreamingContext that contains contextual 
        /// information about the source or destination.
        /// </param>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The info parameter is null.</exception>
        protected MappingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif

        /// <summary>
        /// Initializes a new instance of the Obtics.Values.MappingException class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        ///     (Nothing in Visual Basic) if no inner exception is specified.</param>
        public MappingException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}
