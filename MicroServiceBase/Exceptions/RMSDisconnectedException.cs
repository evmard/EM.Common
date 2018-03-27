using System;
using System.Runtime.Serialization;

namespace MicroServiceBase.Exceptions
{
    [Serializable]
    internal class RMSDisconnectedException : Exception
    {
        public RMSDisconnectedException()
        {
        }

        public RMSDisconnectedException(string message) : base(message)
        {
        }

        public RMSDisconnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RMSDisconnectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}