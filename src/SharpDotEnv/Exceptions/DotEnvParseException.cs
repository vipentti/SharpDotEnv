﻿using System;
using System.Runtime.Serialization;

namespace SharpDotEnv.Exceptions
{
    /// <summary>
    /// Represents errors that occur during parsing of dotenv files.
    /// </summary>
    [Serializable]
    public class DotEnvParseException : Exception
    {
        /// <summary>
        /// Initialize new instance
        /// </summary>
        public DotEnvParseException() { }

        /// <summary>
        /// Initialize new instance with the specified error message
        /// </summary>
        /// <param name="message">The error message</param>
        public DotEnvParseException(string message)
            : base(message) { }

        /// <summary>
        /// Initialize new instance with the specified error message and a reference to an inner exception
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public DotEnvParseException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <inheritdoc />
        protected DotEnvParseException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
