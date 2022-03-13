namespace FaasNet.Gateway.Core.Exceptions
{
    public class OpenApiOperationNotFoundException : NotFoundException
    {
        public OpenApiOperationNotFoundException(string code, string message) : base(code, message) { }
    }
}
