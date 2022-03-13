using System;

namespace FaasNet.StateMachine.Runtime.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
