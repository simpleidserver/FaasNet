using System;

namespace FaasNet.CRDT.Client.Exceptions
{
    public class CRDTClientException : Exception
    {
        public CRDTClientException(string message, string code = null) : base(message)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
