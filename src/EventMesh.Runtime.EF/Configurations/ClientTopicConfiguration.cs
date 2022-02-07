using EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMesh.Runtime.EF.Configurations
{
    public class ClientTopicConfiguration : IEntityTypeConfiguration<ClientTopic>
    {
        public void Configure(EntityTypeBuilder<ClientTopic> builder)
        {

        }
    }
}
