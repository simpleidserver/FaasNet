﻿// <auto-generated />
using FaasNet.EventStore.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FaasNet.StateMachine.ExporterHost.Migrations
{
    [DbContext(typeof(EventStoreDBContext))]
    partial class EventStoreDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("FaasNet.EventStore.EF.Models.Snapshot", b =>
                {
                    b.Property<string>("AggregateId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LastEvtOffset")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SerializedContent")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("AggregateId", "Version");

                    b.ToTable("Snapshots");
                });

            modelBuilder.Entity("FaasNet.EventStore.EF.Models.Subscription", b =>
                {
                    b.Property<string>("GroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TopicName")
                        .HasColumnType("TEXT");

                    b.Property<long>("Offset")
                        .HasColumnType("INTEGER");

                    b.HasKey("GroupId", "TopicName");

                    b.ToTable("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
