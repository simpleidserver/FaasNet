using System;

namespace FaasNet.Domain.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string code, string message) : base(message) { }
    }
}
