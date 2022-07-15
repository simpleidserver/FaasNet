using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Client.Messages.Deltas
{
    public class GSetDelta : BaseEntityDelta
    {
        public override EntityDeltaTypes DeltaType => EntityDeltaTypes.GSet;

        public GSetDelta() { }

        public GSetDelta(ICollection<string> elements)
        {
            Elements = elements;
        }

        public ICollection<string> Elements { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Elements.Count());
            foreach (var elt in Elements) context.WriteString(elt);
        }

        public void Extract(ReadBufferContext context)
        {
            var result = new List<string>();
            var nbElt = context.NextInt();
            for (var i = 0; i < nbElt; i++) result.Add(context.NextString());
            Elements = result;
        }
    }
}
