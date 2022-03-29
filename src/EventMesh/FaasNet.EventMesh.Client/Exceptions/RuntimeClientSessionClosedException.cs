namespace FaasNet.EventMesh.Client.Exceptions
{
    public class RuntimeClientSessionClosedException : RuntimeClientException
    {
        public RuntimeClientSessionClosedException(string message) : base(message) { }
    }
}
