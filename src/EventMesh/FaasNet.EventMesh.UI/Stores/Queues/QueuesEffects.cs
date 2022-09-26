using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.UI.Data;
using Fluxor;
using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.Stores.Queues
{
    public class QueuesEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public QueuesEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }

        [EffectMethod]
        public async Task Handle(SearchQueuesAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.SearchQueues(action.Filter, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new SearchQueuesResultAction(result));
        }

        [EffectMethod]
        public async Task Handle(AddQueueAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.AddQueue(action.Vpn, action.Name, action.TopicFilter, action.Url, action.Port, CancellationToken.None);
            if (result.Status != AddQueueStatus.SUCCESS)
            {
                dispatcher.Dispatch(new AddQueueFailureAction($"An error occured while trying to add the Queue, Error: {Enum.GetName(typeof(AddQueueStatus), result.Status)}"));
                return;
            }

            dispatcher.Dispatch(new AddQueueResultAction { Name = action.Name, TopicFilter = action.TopicFilter, Vpn = action.Vpn });
        }
    }

    public class SearchQueuesAction
    {
        public FilterQuery Filter { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class SearchQueuesResultAction
    {
        public GenericSearchQueryResult<QueueQueryResult> Queues { get; }

        public SearchQueuesResultAction(GenericSearchQueryResult<QueueQueryResult> queues)
        {
            Queues = queues;
        }
    }

    public class AddQueueAction
    {
        [Required]
        public string Vpn { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string TopicFilter { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }

        public void Reset()
        {
            Vpn = String.Empty;
            Name = String.Empty;
            TopicFilter = String.Empty;
        }
    }

    public class AddQueueResultAction
    {
        public string Vpn { get; set; }
        public string Name { get; set; }
        public string TopicFilter { get; set; }
    }

    public class AddQueueFailureAction
    {
        public AddQueueFailureAction(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
