using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string code, string message) : base(message)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
