﻿// <auto-generated />
using System;
using FaasNet.Runtime.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FaasNet.Gateway.SqlServer.Startup.Migrations
{
    [DbContext(typeof(RuntimeDBContext))]
    partial class RuntimeDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.BaseEventCondition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConditionType")
                        .HasColumnType("int");

                    b.Property<bool>("End")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Transition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkflowDefinitionSwitchStateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionSwitchStateId");

                    b.ToTable("BaseEventCondition");

                    b.HasDiscriminator<int>("ConditionType");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsRootState")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateDataFilterInput")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateDataFilterOuput")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowDefinitionAggregateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionAggregateId");

                    b.ToTable("BaseWorkflowDefinitionState");

                    b.HasDiscriminator<int>("Type");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionDataFilter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FunctionRef")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkflowDefinitionForeachStateId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("WorkflowDefinitionOnEventId")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowDefinitionOperationStateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionForeachStateId");

                    b.HasIndex("WorkflowDefinitionOnEventId");

                    b.HasIndex("WorkflowDefinitionOperationStateId");

                    b.ToTable("WorkflowDefinitionAction");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAggregate", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Start")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WorkflowDefinitions");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkflowDefinitionAggregateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionAggregateId");

                    b.ToTable("WorkflowDefinitionEvent");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionFunction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("MetadataStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Operation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowDefinitionAggregateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionAggregateId");

                    b.ToTable("WorkflowDefinitionFunction");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionOnEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActionMode")
                        .HasColumnType("int");

                    b.Property<string>("EventDataFilter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventRefs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkflowDefinitionEventStateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionEventStateId");

                    b.ToTable("WorkflowDefinitionOnEvent");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceAggregate", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("OutputStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowDefId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkflowDefVersion")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WorkflowInstances");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceState", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DefId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InputStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OutputStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowInstanceAggregateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowInstanceAggregateId");

                    b.ToTable("WorkflowInstanceState");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceStateEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("InputData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkflowInstanceStateId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowInstanceStateId");

                    b.ToTable("WorkflowInstanceStateEvent");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceStateEventOutput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Data")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<int?>("WorkflowInstanceStateEventId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowInstanceStateEventId");

                    b.ToTable("WorkflowInstanceStateEventOutput");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Subscriptions.CloudEventSubscriptionAggregate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsConsumed")
                        .HasColumnType("bit");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateInstanceId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkflowInstanceId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionSwitchDataCondition", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseEventCondition");

                    b.Property<string>("Condition")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionSwitchEventCondition", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseEventCondition");

                    b.Property<string>("EventDataFilter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventRef")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionCallbackState", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState");

                    b.Property<int?>("ActionId")
                        .HasColumnType("int");

                    b.Property<bool>("End")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("bit");

                    b.Property<string>("EventRef")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Transition")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("ActionId")
                        .IsUnique()
                        .HasFilter("[ActionId] IS NOT NULL");

                    b.HasDiscriminator().HasValue(7);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionEventState", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState");

                    b.Property<bool>("End")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("bit");

                    b.Property<bool>("Exclusive")
                        .HasColumnType("bit");

                    b.Property<string>("Transition")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionForeachState", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState");

                    b.Property<int?>("BatchSize")
                        .HasColumnType("int");

                    b.Property<bool>("End")
                        .HasColumnType("bit");

                    b.Property<string>("InputCollection")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IterationParam")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Mode")
                        .HasColumnType("int");

                    b.Property<string>("OutputCollection")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Transition")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(6);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionInjectState", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState");

                    b.Property<string>("DataStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("End")
                        .HasColumnType("bit");

                    b.Property<string>("Transition")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(5);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionOperationState", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState");

                    b.Property<int>("ActionMode")
                        .HasColumnType("int");

                    b.Property<bool>("End")
                        .HasColumnType("bit");

                    b.Property<string>("Transition")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionSwitchState", b =>
                {
                    b.HasBaseType("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.BaseEventCondition", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionSwitchState", null)
                        .WithMany("Conditions")
                        .HasForeignKey("WorkflowDefinitionSwitchStateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.BaseWorkflowDefinitionState", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAggregate", null)
                        .WithMany("States")
                        .HasForeignKey("WorkflowDefinitionAggregateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAction", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionForeachState", null)
                        .WithMany("Actions")
                        .HasForeignKey("WorkflowDefinitionForeachStateId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionOnEvent", null)
                        .WithMany("Actions")
                        .HasForeignKey("WorkflowDefinitionOnEventId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionOperationState", null)
                        .WithMany("Actions")
                        .HasForeignKey("WorkflowDefinitionOperationStateId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionEvent", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAggregate", null)
                        .WithMany("Events")
                        .HasForeignKey("WorkflowDefinitionAggregateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionFunction", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAggregate", null)
                        .WithMany("Functions")
                        .HasForeignKey("WorkflowDefinitionAggregateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionOnEvent", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionEventState", null)
                        .WithMany("OnEvents")
                        .HasForeignKey("WorkflowDefinitionEventStateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceState", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Instances.WorkflowInstanceAggregate", null)
                        .WithMany("States")
                        .HasForeignKey("WorkflowInstanceAggregateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceStateEvent", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Instances.WorkflowInstanceState", null)
                        .WithMany("Events")
                        .HasForeignKey("WorkflowInstanceStateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceStateEventOutput", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Instances.WorkflowInstanceStateEvent", null)
                        .WithMany("OutputLst")
                        .HasForeignKey("WorkflowInstanceStateEventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionCallbackState", b =>
                {
                    b.HasOne("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAction", "Action")
                        .WithOne("CallbackState")
                        .HasForeignKey("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionCallbackState", "ActionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Action");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAction", b =>
                {
                    b.Navigation("CallbackState");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionAggregate", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Functions");

                    b.Navigation("States");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionOnEvent", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceAggregate", b =>
                {
                    b.Navigation("States");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceState", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Instances.WorkflowInstanceStateEvent", b =>
                {
                    b.Navigation("OutputLst");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionEventState", b =>
                {
                    b.Navigation("OnEvents");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionForeachState", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionOperationState", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("FaasNet.Runtime.Domains.Definitions.WorkflowDefinitionSwitchState", b =>
                {
                    b.Navigation("Conditions");
                });
#pragma warning restore 612, 618
        }
    }
}
