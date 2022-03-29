using System;

namespace FaasNet.EventMesh.Client.Exceptions
{
    public class RuntimeClientException : Exception
    {
        public RuntimeClientException(string message) : base(message)
        {
        }
    }
}
