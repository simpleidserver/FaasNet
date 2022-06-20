using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllBridgeResponse : Package
    {
        public IEnumerable<BridgeServerResponse> Servers { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(Servers.Count());
            foreach(var server in Servers)
            {
                server.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            var result = new List<BridgeServerResponse>();
            int nbServers = context.NextInt();
            for(int i = 0; i < nbServers; i++)
            {
                result.Add(BridgeServerResponse.Deserialize(context));
            }

            Servers = result;
        }
    }

    public class BridgeServerResponse
    {
        public string SourceVpn { get; set; }
        public string TargetUrn { get; set; }
        public int TargetPort { get; set; }
        public string TargetVpn { get; set; }
        public string TargetClientId { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(SourceVpn);
            context.WriteString(TargetUrn);
            context.WriteInteger(TargetPort);
            context.WriteString(TargetVpn);
            context.WriteString(TargetClientId);
        }

        public static BridgeServerResponse Deserialize(ReadBufferContext context)
        {
            return new BridgeServerResponse
            {
                SourceVpn = context.NextString(),
                TargetUrn = context.NextString(),
                TargetPort = context.NextInt(),
                TargetVpn = context.NextString(),
                TargetClientId = context.NextString()
            };
        }
    }
}
