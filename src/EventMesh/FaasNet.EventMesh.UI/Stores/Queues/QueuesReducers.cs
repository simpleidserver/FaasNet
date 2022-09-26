using FaasNet.EventMesh.Client.StateMachines.Queue;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Queues
{
    public static class QueuesReducers
    {
        [ReducerMethod]
        public static QueuesState RdeduceSearchQueuesAction(QueuesState state, SearchQueuesAction action) => new(isLoading: true, queues: null);

        [ReducerMethod]
        public static QueuesState ReduceSearchQueuesResultAction(QueuesState state, SearchQueuesResultAction action) => new(isLoading: false, queues: action.Queues);

        [ReducerMethod]
        public static QueuesState ReduceAddQueueAction(QueuesState state, AddQueueAction action)
        {
            state.IsLoading = true;
            return state;
        }

        [ReducerMethod]
        public static QueuesState ReduceAddQueueFailureAction(QueuesState state, AddQueueFailureAction action)
        {
            state.IsLoading = false;
            return state;
        }

        [ReducerMethod]
        public static QueuesState ReduceAddQueueResultAction(QueuesState state, AddQueueResultAction action)
        {
            state.IsLoading = false;
            var records = state.Queues.Records.ToList();
            records.Insert(0, new QueueQueryResult { Vpn = action.Vpn, QueueName = action.Name, TopicFilter = action.TopicFilter });
            state.Queues.Records = records;
            return state;
        }
    }
}
