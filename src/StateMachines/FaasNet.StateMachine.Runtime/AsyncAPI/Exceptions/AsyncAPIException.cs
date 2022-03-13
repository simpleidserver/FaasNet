using System;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.Exceptions
{
    public class AsyncAPIException : Exception
    {
        public AsyncAPIException(string message) : base(message) { }
    }
}
