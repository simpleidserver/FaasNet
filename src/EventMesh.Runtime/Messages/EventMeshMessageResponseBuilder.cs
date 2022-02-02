namespace EventMesh.Runtime.Messages
{
    public static class EventMeshMessageResponseBuilder
    {
        public static EventMeshPackage HeartBeat(string seq)
        {
            var result = new EventMeshPackage
            {
                Header = new EventMeshHeader(EventMeshCommands.HEARTBEAT_RESPONSE, EventMeshHeaderStatus.SUCCESS, seq)
            };
            return result;
        }
    }
}
