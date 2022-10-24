using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAsyncApiResult : BaseEventMeshPackage
    {
        public GetAsyncApiResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ASYNC_API_RESULT;
        public GetAsyncApiResultStatus Status { get; set; }
        public string Document { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            if (Status == GetAsyncApiResultStatus.OK)
                context.WriteString(Document);
        }

        public GetAsyncApiResult Extract(ReadBufferContext context)
        {
            Status = (GetAsyncApiResultStatus)context.NextInt();
            if (Status == GetAsyncApiResultStatus.OK)
                Document = context.NextString();
            return this;
        }
    }

    public enum GetAsyncApiResultStatus
    {
        OK = 0,
        NOT_FOUND = 1
    }
}
