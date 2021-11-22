using FaasNet.Gateway.Core.Functions.Invokers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Aws
{
    public class AwsFunctionInvoker : IFunctionInvoker
    {
        public string Provider => "aws";

        public Task InitAudit(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JToken> Invoke(string id, JToken input, JObject configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Publish(string id, JObject parameter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAudit(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Unpublish(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
