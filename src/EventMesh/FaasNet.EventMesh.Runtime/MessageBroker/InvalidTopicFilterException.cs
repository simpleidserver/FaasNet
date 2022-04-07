using System;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class InvalidTopicFilterException : Exception
    {
        public InvalidTopicFilterException(string message) : base(message) { }
    }
}
