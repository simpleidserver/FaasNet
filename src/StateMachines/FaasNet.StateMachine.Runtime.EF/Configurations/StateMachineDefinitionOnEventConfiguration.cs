using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Linq;

namespace FaasNet.StateMachine.Runtime.EF.Configurations
{
    public class StateMachineDefinitionOnEventConfiguration : IEntityTypeConfiguration<StateMachineDefinitionOnEvent>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionOnEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.EventRefs)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', System.StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.EventDataFilter)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<StateMachineDefinitionEventDataFilter>(p));
            builder.HasMany(_ => _.Actions).WithOne().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
