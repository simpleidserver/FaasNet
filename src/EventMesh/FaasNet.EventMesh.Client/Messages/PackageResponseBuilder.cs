using CloudNative.CloudEvents;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public static class PackageResponseBuilder
    {
        public static BaseEventMeshPackage HeartBeat(string seq)
        {
            return new PingResult(seq);
        }

        public static BaseEventMeshPackage AddVpn(string seq)
        {
            return new AddVpnResult(seq);
        }

        public static BaseEventMeshPackage AddVpn(string seq, AddVpnErrorStatus status)
        {
            return new AddVpnResult(seq, status);
        }

        public static BaseEventMeshPackage GetAllVpn(string seq, ICollection<VpnResult> vpns)
        {
            return new GetAllVpnResult(seq)
            {
                Vpns = vpns
            };
        }

        public static BaseEventMeshPackage GetAllClient(string seq, ICollection<ClientResult> clients)
        {
            return new GetAllClientResult(seq)
            {
                Clients = clients
            };
        }

        public static BaseEventMeshPackage AddClient(string seq, string clientId, string clientSecret)
        {
            return new AddClientResult(seq)
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }

        public static BaseEventMeshPackage AddClient(string seq, AddClientErrorStatus status)
        {
            return new AddClientResult(seq, status);
        }

        public static BaseEventMeshPackage AddQueue(string seq, AddQueueStatus status = AddQueueStatus.SUCCESS)
        {
            return new AddQueueResponse(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage PublishMessage(string seq, PublishMessageStatus status = PublishMessageStatus.SUCCESS)
        {
            return new PublishMessageResult(seq, status);
        }

        public static BaseEventMeshPackage ReadMessage(string seq, CloudEvent message)
        {
            return new ReadMessageResult(seq)
            {
                Status = ReadMessageStatus.SUCCESS,
                Message = message
            };
        }

        public static BaseEventMeshPackage ReadMessage(string seq, ReadMessageStatus status)
        {
            return new ReadMessageResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage Hello(string seq, HelloMessageStatus status)
        {
            return new HelloResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage Hello(string seq, string sessionId)
        {
            return new HelloResult(seq)
            {
                SessionId = sessionId,
                Status = HelloMessageStatus.SUCCESS
            };
        }
    }
}
