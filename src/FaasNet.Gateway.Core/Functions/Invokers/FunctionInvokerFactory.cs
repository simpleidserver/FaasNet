using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Functions.Invokers
{
    public class FunctionInvokerFactory : IFunctionInvokerFactory
    {
        private readonly IEnumerable<IFunctionInvoker> _invokers;

        public FunctionInvokerFactory(IEnumerable<IFunctionInvoker> invokers)
        {
            _invokers = invokers;
        }

        public IFunctionInvoker Build(string provider)
        {
            return _invokers.FirstOrDefault(i => i.Provider == provider);
        }
    }
}
