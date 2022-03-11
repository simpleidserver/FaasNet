using System;

namespace FaasNet.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string code, string message) : base(message)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
