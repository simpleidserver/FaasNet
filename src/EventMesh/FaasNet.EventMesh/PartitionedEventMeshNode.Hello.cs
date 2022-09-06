using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(HelloRequest helloRequest, CancellationToken cancellationToken)
        {
            var client = await GetStateMachine<ClientStateMachine>(CLIENT_PARTITION_KEY, helloRequest.ClientId, cancellationToken);
            if (client == null) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.UNKNOWN_CLIENT);
            if (client.CheckPassword(helloRequest.ClientSecret)) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.BAD_CREDENTIALS);
            if (!client.HasPurpose(helloRequest.Purpose)) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.BAD_PURPOSE);
            var sessionId = Guid.NewGuid().ToString();
            var expirationTime = TimeSpan.FromTicks(DateTime.UtcNow.AddMilliseconds(client.SessionExpirationTimeMS).Ticks);
            var addSessionCommand = new AddSessionCommand { ClientId = helloRequest.ClientId, ClientPurpose = helloRequest.Purpose, ExpirationTime = expirationTime, TopicFilter = helloRequest.TopicFilter };
            await Send(SESSION_PARTITION_KEY, sessionId, addSessionCommand, cancellationToken);
            return PackageResponseBuilder.Hello(helloRequest.Seq, sessionId);
        }
    }
}