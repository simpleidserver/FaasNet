namespace FaasNet.Gateway.Core.Exceptions
{
    public class ApiDefNotFoundException : NotFoundException
    {
        public ApiDefNotFoundException(string message) : base(message) { }
    }
}
