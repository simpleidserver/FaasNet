using System;

namespace FaasNet.Runtime.Exceptions
{
    public class OpenAPIException : Exception
    {
        public OpenAPIException(string message) : base(message) { }
    }
}
