using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Client.Transports;
using System.Collections.Generic;

namespace FaasNet.Peer.Client
{
    public abstract class BasePartitionedPeerClient : BasePeerClient, IPartitionedPeerClient
    {
        public BasePartitionedPeerClient(IClientTransport transport) : base(transport) { }

        public PartitionedPeerClientTypes ClientType { get; set; } = PartitionedPeerClientTypes.DIRECT;
        public string PartitionKey { get; set; }

        public void Transfer(string partitionKey)
        {
            ClientType = PartitionedPeerClientTypes.TRANSFERED;
            PartitionKey = partitionKey;
        }

        public void Broadcast()
        {
            ClientType = PartitionedPeerClientTypes.BROADCAST;
        }

        protected byte[] SerializeRequest(BasePeerPackage package)
        {
            var writeBufferCtx = new WriteBufferContext();
            package.SerializeEnvelope(writeBufferCtx);
            var packageBuffer = writeBufferCtx.Buffer.ToArray();
            switch(ClientType)
            {
                case PartitionedPeerClientTypes.DIRECT:
                    return packageBuffer;
                case PartitionedPeerClientTypes.TRANSFERED:
                    writeBufferCtx = new WriteBufferContext();
                    var transferedPackage = PartitionPackageRequestBuilder.Transfer(PartitionKey, packageBuffer);
                    transferedPackage.SerializeEnvelope(writeBufferCtx);
                    return writeBufferCtx.Buffer.ToArray();
                default:
                    writeBufferCtx = new WriteBufferContext();
                    var broadcastRequest = PartitionPackageRequestBuilder.Broadcast(packageBuffer);
                    broadcastRequest.SerializeEnvelope(writeBufferCtx);
                    return writeBufferCtx.Buffer.ToArray();
            }
        }

        protected IEnumerable<TResult> DeserializeResult<T, TResult>(byte[] result)
        {
            var readBufferCtx = new ReadBufferContext(result);
            var deserializeTypeMethodInfo = typeof(T).GetMethod("Deserialize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            switch (ClientType)
            {
                case PartitionedPeerClientTypes.BROADCAST:
                    var cmd = BasePartitionedRequest.Deserialize(readBufferCtx) as BroadcastResult;
                    var res = new List<TResult>();
                    foreach(var content in cmd.ContentLst)
                    {
                        readBufferCtx = new ReadBufferContext(content);
                        res.Add((TResult)deserializeTypeMethodInfo.Invoke(null, new object[] { readBufferCtx, false }));
                    }

                    return res;
                default:
                    return new TResult[]
                    { 
                        (TResult)deserializeTypeMethodInfo.Invoke(null, new object[] { readBufferCtx, false }) 
                    };
            }
        }
    }
}
