using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Queues
{
    [FeatureState]
    public class QueuesState
    {
        public QueuesState()
        {

        }

        public GenericSearchQueryResult<QueueQueryResult> Queues { get; set; } = new GenericSearchQueryResult<QueueQueryResult>();
        public bool IsLoading { get; set; }

        public QueuesState(bool isLoading, GenericSearchQueryResult<QueueQueryResult> queues)
        {
            IsLoading = isLoading;
            Queues = queues;
        }
    }
}
