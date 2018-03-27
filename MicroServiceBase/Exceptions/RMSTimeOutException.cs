using System;
using System.Runtime.Serialization;

namespace MicroServiceBase.Exceptions
{
    [Serializable]
    internal class RMSTimeOutException : Exception
    {
        public RMSTimeOutException()
        {
        }

        public RMSTimeOutException(string message) : base(message)
        {
        }

        public RMSTimeOutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RMSTimeOutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}