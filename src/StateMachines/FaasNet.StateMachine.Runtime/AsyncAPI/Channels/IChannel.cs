using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models;
using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.Channels
{
    public interface IChannel
    {
        string Protocol { get; }
        Task Invoke(JToken input, Server server, ChannelBindings channelBindings, OperationBindings operationBinding, IEnumerable<SecurityScheme> securitySchemes, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }
}
