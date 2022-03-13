using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string code, string message) : base(message)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
