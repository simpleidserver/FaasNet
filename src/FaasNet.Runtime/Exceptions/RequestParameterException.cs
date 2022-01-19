using System;

namespace FaasNet.Runtime.Exceptions
{
    public class RequestParameterException : Exception
    {
        public RequestParameterException(string message) : base(message) { }
    }
}
