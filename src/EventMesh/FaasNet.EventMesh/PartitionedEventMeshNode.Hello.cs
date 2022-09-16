using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(HelloRequest helloRequest, CancellationToken cancellationToken)
        {
            var client = await Query<ClientQueryResult>(CLIENT_PARTITION_KEY, new GetClientQuery { Id = helloRequest.ClientId }, cancellationToken);
            if (client == null) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.UNKNOWN_CLIENT);
            if (PasswordHelper.CheckPassword(helloRequest.ClientSecret, client.ClientSecret)) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.BAD_CREDENTIALS);
            if (!client.Purposes.Contains(helloRequest.Purpose)) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.BAD_PURPOSE);
            var sessionId = Guid.NewGuid().ToString();
            var expirationTime = TimeSpan.FromTicks(DateTime.UtcNow.AddMilliseconds(client.SessionExpirationTimeMS).Ticks);
            var addSessionCommand = new AddSessionCommand { ClientId = helloRequest.ClientId, ClientPurpose = helloRequest.Purpose, ExpirationTime = expirationTime, QueueName = helloRequest.QueueName };
            var result = await Send(SESSION_PARTITION_KEY, addSessionCommand, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.NOLEADER);
            return PackageResponseBuilder.Hello(helloRequest.Seq, sessionId);
        }
    }
}