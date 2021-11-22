namespace FaasNet.Gateway.Core.Functions.Invokers
{
    public interface IFunctionInvokerFactory
    {
        IFunctionInvoker Build(string provider);
    }
}
