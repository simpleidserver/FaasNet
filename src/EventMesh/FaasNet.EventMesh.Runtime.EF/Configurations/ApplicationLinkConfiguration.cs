using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class ApplicationLinkConfiguration : IEntityTypeConfiguration<ApplicationLink>
    {
        public void Configure(EntityTypeBuilder<ApplicationLink> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
