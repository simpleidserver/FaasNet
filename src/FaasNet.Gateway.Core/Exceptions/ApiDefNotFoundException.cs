using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class ApiDefNotFoundException : Exception
    {
        public ApiDefNotFoundException(string message) : base(message) { }
    }
}
