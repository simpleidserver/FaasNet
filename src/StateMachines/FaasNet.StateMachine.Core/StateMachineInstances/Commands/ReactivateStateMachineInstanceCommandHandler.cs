using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using static FaasNet.StateMachine.WorkerHost.StateMachine;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Commands
{
    public class ReactivateStateMachineInstanceCommandHandler : IRequestHandler<ReactivateStateMachineInstanceCommand>
    {
        private readonly StateMachineOptions _options;

        public ReactivateStateMachineInstanceCommandHandler(IOptions<StateMachineOptions> options)
        {
            _options = options.Value;
        }

        public async Task<Unit> Handle(ReactivateStateMachineInstanceCommand request, CancellationToken cancellationToken)
        {
            var channel = GrpcChannel.ForAddress(_options.StateMachineWorkerUrl);
            var client = new StateMachineClient(channel);
            await client.ReactivateAsync(new WorkerHost.ReactivateRequest
            {
                Id = request.Id
            });
            return Unit.Value;
        }
    }

    public class ReactivateStateMachineInstanceCommand : IRequest
    {
        public string Id { get; set; }
    }
}
