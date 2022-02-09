using System;

namespace EventMesh.Runtime.Exceptions
{
    public class RuntimeClientException : Exception
    {
        public RuntimeClientException(string message) : base(message)
        {
        }
    }
}
