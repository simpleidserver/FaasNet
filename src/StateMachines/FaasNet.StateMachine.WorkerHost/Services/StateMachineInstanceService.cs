using FaasNet.StateMachine.Runtime.Serializer;
using FaasNet.StateMachine.Worker;
using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.WorkerHost.Services
{
    public class StateMachineInstanceService : StateMachine.StateMachineBase
    {
        private readonly IStateMachineLauncher _stateMachineLauncher;

        public StateMachineInstanceService(IStateMachineLauncher stateMachineLauncher)
        {
            _stateMachineLauncher = stateMachineLauncher;
        }

        public override async Task<LaunchStateMachineDefResult> Launch(StateMachineDef request, ServerCallContext context)
        {
            var runtimeSerializer = new RuntimeSerializer();
            var stateMachineDefinition = runtimeSerializer.DeserializeYaml(request.Yaml);
            stateMachineDefinition.TechnicalId = request.WorkflowDefTechnicalId;
            var instance = await _stateMachineLauncher.InstanciateAndLaunch(stateMachineDefinition, request.Input, CancellationToken.None);
            return new LaunchStateMachineDefResult
            {
                Id = instance.Id,
                LaunchDateTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
            };
        }
    }
}
