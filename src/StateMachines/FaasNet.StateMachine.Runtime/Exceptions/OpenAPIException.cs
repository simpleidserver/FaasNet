using System;

namespace FaasNet.StateMachine.Runtime.Exceptions
{
    public class OpenAPIException : Exception
    {
        public OpenAPIException(string message) : base(message) { }
    }
}
