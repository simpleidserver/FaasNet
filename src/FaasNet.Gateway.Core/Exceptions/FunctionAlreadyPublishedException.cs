using System;

namespace FaasNet.Gateway.Core.Exceptions
{
    public class FunctionAlreadyPublishedException : Exception
    {
        public FunctionAlreadyPublishedException(string code, string message, string functionId) : base(message)
        {
            Code = code;
            FunctionId = functionId;
        }

        public string Code { get; set; }
        public string FunctionId { get; set; }
    }
}
