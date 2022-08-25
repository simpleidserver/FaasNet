using FaasNet.EventMesh.Client.Messages;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddVpnRequest request, CancellationToken cancellationToken)
        {
            // TODO : Serialize...
            /*
            var content = ConsensusPackageRequestBuilder.AppendEntry();
            var transferedRequest = new TransferedRequest
            {
                PartitionKey = "VPN",
                Content = 
            };
            await PartitionCluster.Transfer();
            */
            return null;
        }

        public class AddVpnCommand : IArrayModifierCommand<Vpn>
        {
            public string Name { get; set; }

            public void Serialize(WriteBufferContext context)
            {
                context.WriteString(Name);
            }

            public void Deserialize(ReadBufferContext context)
            {
                Name = context.NextString();
            }

            public void Apply(ICollection<Vpn> arr)
            {
                arr.Add(new Vpn { Id = Name });
            }
        }

        public class UpdateVpnCommand : IArrayModifierCommand<Vpn>
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public void Serialize(WriteBufferContext context)
            {
                context.WriteString(Name);
                context.WriteString(Description);
            }

            public void Deserialize(ReadBufferContext context)
            {
                Name = context.NextString();
                Description = context.NextString();
            }

            public void Apply(ICollection<Vpn> arr)
            {
                var record = arr.Single(a => a.Id == Name);
                record.Description = Description;
            }
        }

        public class Vpn : IEntity
        {
            public string Id { get; set; }
            public string Description { get; set; }
        }
    }
}
