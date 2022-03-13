using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors
{
    public interface IFunctionProcessor
    {
        StateMachineDefinitionTypes Type { get; }
        Task<JToken> Process(JToken input, StateMachineDefinitionFunction function, StateMachineInstanceState instanceState, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }
}
