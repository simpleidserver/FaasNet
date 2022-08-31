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
    }
}
