using System;

namespace FaasNet.Runtime.Exceptions
{
    public class ProcessorException : Exception
    {
        public ProcessorException(string message) : base(message) { }
    }
}
