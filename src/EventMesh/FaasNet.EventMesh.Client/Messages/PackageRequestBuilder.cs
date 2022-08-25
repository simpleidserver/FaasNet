using System;
using System.Text;

namespace FaasNet.EventMesh.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static BaseEventMeshPackage HeartBeat()
        {
            return new PingRequest(GenerateRandomSeq());
        }

        private static string GenerateRandomSeq()
        {
            var builder = new StringBuilder(10);
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
                builder.Append((char)rnd.Next(48, 57));
            return builder.ToString();
        }
    }
}
