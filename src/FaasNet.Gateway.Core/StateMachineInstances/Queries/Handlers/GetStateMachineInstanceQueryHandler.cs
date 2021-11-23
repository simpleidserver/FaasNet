using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Gateway.Core.StateMachineInstances.Results;
using FaasNet.Runtime.Persistence;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.StateMachineInstances.Queries.Handlers
{
    public class GetStateMachineInstanceQueryHandler : IRequestHandler<GetStateMachineInstanceQuery, StateMachineInstanceResult>
    {
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

        public GetStateMachineInstanceQueryHandler(IWorkflowInstanceRepository workflowInstanceRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public Task<StateMachineInstanceResult> Handle(GetStateMachineInstanceQuery request, CancellationToken cancellationToken)
        {
            var instance = _workflowInstanceRepository.Query().FirstOrDefault(w => w.Id == request.Id);
            if (instance == null)
            {
                throw new StateMachineInstanceNotFoundException(ErrorCodes.InvalidStateMachineInstanceId, Global.UnknownStateMachineInstance);
            }

            return Task.FromResult(StateMachineInstanceResult.Build(instance));
        }
    }
}
