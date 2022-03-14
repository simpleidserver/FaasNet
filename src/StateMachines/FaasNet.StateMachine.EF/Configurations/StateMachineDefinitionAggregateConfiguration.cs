using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionAggregateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionAggregate>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionAggregate> builder)
        {
            builder.HasKey(_ => _.TechnicalId);
            builder.Property(p => p.Start)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<StateMachineDefinitionStartState>(p));
            builder.HasMany(_ => _.States).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(_ => _.Functions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(_ => _.Events).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
