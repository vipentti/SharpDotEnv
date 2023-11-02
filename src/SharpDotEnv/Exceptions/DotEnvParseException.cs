using System;
using System.Runtime.Serialization;

namespace SharpDotEnv.Exceptions
{
    [Serializable]
    public class DotEnvParseException : Exception
    {
        public DotEnvParseException()
        {
        }

        public DotEnvParseException(string message) : base(message)
        {
        }

        public DotEnvParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DotEnvParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
