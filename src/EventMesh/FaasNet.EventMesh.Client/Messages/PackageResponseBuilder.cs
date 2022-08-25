namespace FaasNet.EventMesh.Client.Messages
{
    public static class PackageResponseBuilder
    {
        public static BaseEventMeshPackage HeartBeat(string seq)
        {
            return new PingResult(seq);
        }
    }
}
