using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class FunctionNotFoundException : Exception
    {
        public FunctionNotFoundException(string message) : base(message)
        {
        }
    }
}
