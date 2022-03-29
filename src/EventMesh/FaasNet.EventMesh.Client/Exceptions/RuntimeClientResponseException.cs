using FaasNet.EventMesh.Client.Messages;

namespace FaasNet.EventMesh.Client.Exceptions
{
    public class RuntimeClientResponseException : RuntimeClientException
    {
        public RuntimeClientResponseException(HeaderStatus status, Errors error, string message = null) : base(message) 
        {
            Status = status;
            Error = error;
        }

        public HeaderStatus Status { get; private set; }
        public Errors Error { get; private set; }
    }
}
