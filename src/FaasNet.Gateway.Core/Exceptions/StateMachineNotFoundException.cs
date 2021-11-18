namespace FaasNet.Gateway.Core.Exceptions
{
    public class StateMachineNotFoundException: NotFoundException
    {
        public StateMachineNotFoundException(string code, string message) : base(message) { }
    }
}
