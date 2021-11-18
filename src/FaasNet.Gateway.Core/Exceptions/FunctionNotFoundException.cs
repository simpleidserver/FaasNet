using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class FunctionNotFoundException : NotFoundException
    {
        public FunctionNotFoundException(string message) : base(message)
        {
        }
    }
}
