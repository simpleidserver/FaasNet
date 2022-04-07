using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllVpnResponse : Package
    {
        public IEnumerable<string> Vpns { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteStringArray(Vpns);
        }

        public void Extract(ReadBufferContext context)
        {
            Vpns = context.NextStringArray();
        }
    }
}
