using System;

namespace FaasNet.StateMachine.Runtime.Exceptions
{
    public class RequestParameterException : Exception
    {
        public RequestParameterException(string message) : base(message) { }
    }
}
