using System;

namespace FaasNet.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string code, string message) : base(message)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
