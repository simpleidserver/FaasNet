namespace FaasNet.Gateway.Core.Exceptions
{
    public class FunctionNotFoundException : NotFoundException
    {
        public FunctionNotFoundException(string code, string message) : base(code, message) { }
    }
}
