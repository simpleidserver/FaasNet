namespace EventMesh.Runtime.Exceptions
{
    public class RuntimeClientSessionClosedException : RuntimeClientException
    {
        public RuntimeClientSessionClosedException(string message) : base(message) { }
    }
}
