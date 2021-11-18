using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
