using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Core.StateMachineInstances.Results;
using FaasNet.StateMachine.Runtime.Persistence;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Queries.Handlers
{
    public class GetStateMachineInstanceQueryHandler : IRequestHandler<GetStateMachineInstanceQuery, StateMachineInstanceDetailsResult>
    {
        private readonly IStateMachineInstanceRepository _workflowInstanceRepository;

        public GetStateMachineInstanceQueryHandler(IStateMachineInstanceRepository workflowInstanceRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public Task<StateMachineInstanceDetailsResult> Handle(GetStateMachineInstanceQuery request, CancellationToken cancellationToken)
        {
            var instance = _workflowInstanceRepository.Query().FirstOrDefault(w => w.Id == request.Id);
            if (instance == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_STATEMACHINE_INSTANCE, string.Format(Global.UnknownStateMachineInstance, request.Id));
            }

            return Task.FromResult(StateMachineInstanceDetailsResult.ToDto(instance));
        }
    }
}
