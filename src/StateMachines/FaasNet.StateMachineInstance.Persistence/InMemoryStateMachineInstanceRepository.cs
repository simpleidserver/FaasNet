using FaasNet.Common.Extensions;
using FaasNet.Domain;
using FaasNet.Domain.Extensions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachineInstance.Persistence
{
    public class InMemoryStateMachineInstanceRepository : IStateMachineInstanceRepository
    {
        private static Dictionary<string, string> MAPPING_STATEMACHINEINSTANCE_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "workflowDefId", "WorkflowDefId" },
            { "workflowDefName", "WorkflowDefName" },
            { "workflowDefDescription", "WorkflowDefDescription" },
            { "createDateTime", "CreateDateTime" }
        };
        private readonly ConcurrentBag<StateMachineInstanceAggregate> _instances;

        public InMemoryStateMachineInstanceRepository(ConcurrentBag<StateMachineInstanceAggregate> instances)
        {
            _instances = instances;
        }

        public Task Add(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _instances.Add((StateMachineInstanceAggregate)workflowInstance.Clone());
            return Task.CompletedTask;
        }

        public Task<StateMachineInstanceAggregate> Get(string id, CancellationToken cancellationToken)
        {
            var record = _instances.FirstOrDefault(i => i.Id == id);
            if (record != null)
            {
                record = (StateMachineInstanceAggregate)record.Clone();
            }

            return Task.FromResult(record);
        }

        public Task<BaseSearchResult<StateMachineInstanceAggregate>> Search(SearchWorkflowInstanceParameter parameter, CancellationToken cancellationToken)
        {
            var query = _instances.Where(i => i.Vpn == parameter.Vpn).AsQueryable();
            if (MAPPING_STATEMACHINEINSTANCE_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                query = query.InvokeOrderBy(MAPPING_STATEMACHINEINSTANCE_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = query.Count();
            query = query.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new BaseSearchResult<StateMachineInstanceAggregate>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = query.ToList()
            });
        }

        public Task Update(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            var currentInstance = _instances.First(i => i.Id == workflowInstance.Id);
            _instances.Remove(_instances.First(i => i.Id == workflowInstance.Id));
            var newRecord = (StateMachineInstanceAggregate)workflowInstance.Clone();
            _instances.Add(newRecord);
            return Task.CompletedTask;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
