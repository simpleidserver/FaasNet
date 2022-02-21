using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class EventMeshSeverUnreachableException : Exception
    {
        public EventMeshSeverUnreachableException(string message) : base(message) { }
    }
}
