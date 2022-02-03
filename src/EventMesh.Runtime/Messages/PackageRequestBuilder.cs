using System;
using System.Text;

namespace EventMesh.Runtime.Messages
{
    public static class PackageRequestBuilder
    {
        private const int SEQ_LENGTH = 10;

        public static Package HeartBeat()
        {
            var result = new Package
            {
                Header = new Header(Commands.HEARTBEAT_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq())
            };
            return result;
        }

        public static HelloRequest Hello(UserAgent userAgent)
        {
            var result = new HelloRequest
            {
                Header = new Header(Commands.HELLO_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq()),
                UserAgent = userAgent
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
