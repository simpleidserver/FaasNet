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

        public static BaseEventMeshPackage AddClient(string seq)
        {
            return new AddClientResult(seq);
        }

        public static BaseEventMeshPackage AddClient(string seq, AddClientErrorStatus status)
        {
            return new AddClientResult(seq, status);
        }
    }
}
