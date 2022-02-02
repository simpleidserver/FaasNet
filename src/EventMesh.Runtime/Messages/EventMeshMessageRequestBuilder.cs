using System;
using System.Text;

namespace EventMesh.Runtime.Messages
{
    public static class EventMeshMessageRequestBuilder
    {
        private const int SEQ_LENGTH = 10;

        public static EventMeshPackage HeartBeat()
        {
            var result = new EventMeshPackage
            {
                Header = new EventMeshHeader(EventMeshCommands.HEARTBEAT_REQUEST, EventMeshHeaderStatus.SUCCESS, GenerateRandomSeq())
            };
            return result;
        }

        private static string GenerateRandomSeq()
        {
            var builder = new StringBuilder(SEQ_LENGTH);
            var rnd = new Random();
            for(int i = 0; i < SEQ_LENGTH; i++)
            {
                builder.Append((char)rnd.Next(48, 57));
            }

            return builder.ToString();
        }
    }
}
