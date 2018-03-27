using System;
using System.Runtime.Serialization;

namespace MicroServiceBase.Exceptions
{
    [Serializable]
    internal class RMSContractException : Exception
    {
        public RMSContractException()
        {
        }

        public RMSContractException(string message) : base(message)
        {
        }

        public RMSContractException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RMSContractException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}