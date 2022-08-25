using System;

namespace FaasNet.EventMesh.Client.Exceptions
{
    public class EventMeshClientException : Exception
    {
        public EventMeshClientException(string message) : base(message)
        {
        }
    }
}
