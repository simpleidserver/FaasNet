using System;

namespace FaasNet.StateMachine.Runtime.Exceptions
{
    public class ProcessorException : Exception
    {
        public ProcessorException(string message) : base(message) { }
    }
}
