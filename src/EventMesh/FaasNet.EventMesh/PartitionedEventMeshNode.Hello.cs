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
            var client = await Query<GetClientQueryResult>(PartitionNames.CLIENT_PARTITION_KEY, new GetClientQuery { Id = helloRequest.ClientId, Vpn = helloRequest.Vpn }, cancellationToken);
            if (!client.Success) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.UNKNOWN_CLIENT);
            if (!PasswordHelper.CheckPassword(helloRequest.ClientSecret, client.Client.ClientSecret)) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.BAD_CREDENTIALS);
            if (!client.Client.Purposes.Contains(helloRequest.Purpose)) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.BAD_PURPOSE);
            var sessionId = Guid.NewGuid().ToString();
            var expirationTime = TimeSpan.FromTicks(DateTime.UtcNow.AddMilliseconds(client.Client.SessionExpirationTimeMS).Ticks);
            var addSessionCommand = new AddSessionCommand 
            { 
                Id = sessionId, 
                ClientId = helloRequest.ClientId, 
                Vpn = helloRequest.Vpn,
                ClientPurpose = helloRequest.Purpose, 
                ExpirationTime = expirationTime, 
                QueueName = helloRequest.QueueName 
            };
            var result = await Send(PartitionNames.SESSION_PARTITION_KEY, addSessionCommand, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.Hello(helloRequest.Seq, HelloMessageStatus.NOLEADER);
            return PackageResponseBuilder.Hello(helloRequest.Seq, sessionId);
        }
    }
}