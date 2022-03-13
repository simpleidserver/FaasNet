using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string code, string message) : base(message) 
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
