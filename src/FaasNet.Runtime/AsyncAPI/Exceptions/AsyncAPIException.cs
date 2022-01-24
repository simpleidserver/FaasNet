using System;

namespace FaasNet.Runtime.AsyncAPI.Exceptions
{
    public class AsyncAPIException : Exception
    {
        public AsyncAPIException(string message) : base(message) { }
    }
}
