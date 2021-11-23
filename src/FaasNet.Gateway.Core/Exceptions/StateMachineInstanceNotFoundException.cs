namespace FaasNet.Gateway.Core.Exceptions
{
    public class StateMachineInstanceNotFoundException : NotFoundException
    {
        public StateMachineInstanceNotFoundException(string code, string message) : base(code, message)
        {
        }
    }
}
