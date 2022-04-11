using FaasNet.Domain;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using Microsoft.Extensions.Options;
using Nest;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachineInstance.Persistence.ES.Repositories
{
    public class ESStateMachineInstanceRepository : IStateMachineInstanceRepository
    {
        private readonly ElasticClient _esClient;
        private readonly StateMachineInstancePersistenceESOptions _options;

        public ESStateMachineInstanceRepository(IOptions<StateMachineInstancePersistenceESOptions> options)
        {
            _options = options.Value;
            _esClient = new ElasticClient(options.Value.Settings);
        }

        public async Task<StateMachineInstanceAggregate> Get(string id, CancellationToken cancellationToken)
        {
            var result = await _esClient.GetAsync<StateMachineInstanceAggregate>(id, o => o.Index(_options.IndexName), cancellationToken);
            return result.Source;
        }

        public async Task Add(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            await _esClient.IndexAsync(workflowInstance, i => i.Index(_options.IndexName)
                .Id(workflowInstance.Id), cancellationToken);
        }

        public async Task Update(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            await _esClient.UpdateAsync<StateMachineInstanceAggregate>(workflowInstance.Id, u => u.Index(_options.IndexName)
                .Doc(workflowInstance), cancellationToken);
        }

        public async Task<BaseSearchResult<StateMachineInstanceAggregate>> Search(SearchWorkflowInstanceParameter parameter, CancellationToken cancellationToken)
        {
            var searchResponse = await _esClient.SearchAsync<StateMachineInstanceAggregate>(s => 
                s.From(parameter.StartIndex)
                .Size(parameter.Count)
                .Query(q => q.Match(i => i.Field(f => f.Vpn).Query(parameter.Vpn)))
                .Sort(q => q.Descending(f => f.CreateDateTime))
                .Index(_options.IndexName));
            return new BaseSearchResult<StateMachineInstanceAggregate>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = searchResponse.Total,
                Content = searchResponse.Documents.ToList()
            };
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
