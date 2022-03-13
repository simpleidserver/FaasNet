using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class ClientTopicConfiguration : IEntityTypeConfiguration<ClientTopic>
    {
        public void Configure(EntityTypeBuilder<ClientTopic> builder)
        {

        }
    }
}
