﻿// <auto-generated />
using System;
using FaasNet.StateMachine.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FaasNet.StateMachine.SqlServer.Startup.Migrations
{
    [DbContext(typeof(RuntimeDBContext))]
    [Migration("20220414120914_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseEventCondition", b =>
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

                    b.Property<string>("StateMachineDefinitionSwitchStateTechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Transition")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StateMachineDefinitionSwitchStateTechnicalId");

                    b.ToTable("BaseEventCondition");

                    b.HasDiscriminator<int>("ConditionType");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState", b =>
                {
                    b.Property<string>("TechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateDataFilterInput")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateDataFilterOuput")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateMachineDefinitionAggregateTechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("TechnicalId");

                    b.HasIndex("StateMachineDefinitionAggregateTechnicalId");

                    b.ToTable("BaseStateMachineDefinitionState");

                    b.HasDiscriminator<int>("Type");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAction", b =>
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

                    b.Property<string>("StateMachineDefinitionForeachStateTechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("StateMachineDefinitionOnEventId")
                        .HasColumnType("int");

                    b.Property<string>("StateMachineDefinitionOperationStateTechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("StateMachineDefinitionForeachStateTechnicalId");

                    b.HasIndex("StateMachineDefinitionOnEventId");

                    b.HasIndex("StateMachineDefinitionOperationStateTechnicalId");

                    b.ToTable("StateMachineDefinitionAction");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAggregate", b =>
                {
                    b.Property<string>("TechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ApplicationDomainId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsLast")
                        .HasColumnType("bit");

                    b.Property<int>("LastEvtOffset")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RootTopic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Start")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("Vpn")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TechnicalId");

                    b.ToTable("WorkflowDefinitions");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<string>("MetadataStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateMachineDefinitionAggregateTechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StateMachineDefinitionAggregateTechnicalId");

                    b.ToTable("StateMachineDefinitionEvent");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionFunction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FunctionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MetadataStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Operation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateMachineDefinitionAggregateTechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StateMachineDefinitionAggregateTechnicalId");

                    b.ToTable("StateMachineDefinitionFunction");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionOnEvent", b =>
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

                    b.Property<string>("StateMachineDefinitionEventStateTechnicalId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("StateMachineDefinitionEventStateTechnicalId");

                    b.ToTable("StateMachineDefinitionOnEvent");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionSwitchDataCondition", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseEventCondition");

                    b.Property<string>("Condition")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionSwitchEventCondition", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseEventCondition");

                    b.Property<string>("EventDataFilter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventRef")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionCallbackState", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState");

                    b.Property<int?>("ActionId")
                        .HasColumnType("int");

                    b.Property<bool>("End")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("bit");

                    b.Property<string>("EventDataFilter")
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionEventState", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState");

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

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionForeachState", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState");

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

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionInjectState", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState");

                    b.Property<string>("DataStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("End")
                        .HasColumnType("bit");

                    b.Property<string>("Transition")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(5);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionOperationState", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState");

                    b.Property<int>("ActionMode")
                        .HasColumnType("int");

                    b.Property<bool>("End")
                        .HasColumnType("bit");

                    b.Property<string>("Transition")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionSwitchState", b =>
                {
                    b.HasBaseType("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState");

                    b.Property<string>("DefaultConditionStr")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseEventCondition", b =>
                {
                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionSwitchState", null)
                        .WithMany("Conditions")
                        .HasForeignKey("StateMachineDefinitionSwitchStateTechnicalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.BaseStateMachineDefinitionState", b =>
                {
                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAggregate", null)
                        .WithMany("States")
                        .HasForeignKey("StateMachineDefinitionAggregateTechnicalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAction", b =>
                {
                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionForeachState", null)
                        .WithMany("Actions")
                        .HasForeignKey("StateMachineDefinitionForeachStateTechnicalId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionOnEvent", null)
                        .WithMany("Actions")
                        .HasForeignKey("StateMachineDefinitionOnEventId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionOperationState", null)
                        .WithMany("Actions")
                        .HasForeignKey("StateMachineDefinitionOperationStateTechnicalId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionEvent", b =>
                {
                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAggregate", null)
                        .WithMany("Events")
                        .HasForeignKey("StateMachineDefinitionAggregateTechnicalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionFunction", b =>
                {
                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAggregate", null)
                        .WithMany("Functions")
                        .HasForeignKey("StateMachineDefinitionAggregateTechnicalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionOnEvent", b =>
                {
                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionEventState", null)
                        .WithMany("OnEvents")
                        .HasForeignKey("StateMachineDefinitionEventStateTechnicalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionCallbackState", b =>
                {
                    b.HasOne("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAction", "Action")
                        .WithOne("CallbackState")
                        .HasForeignKey("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionCallbackState", "ActionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Action");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAction", b =>
                {
                    b.Navigation("CallbackState");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionAggregate", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Functions");

                    b.Navigation("States");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionOnEvent", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionEventState", b =>
                {
                    b.Navigation("OnEvents");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionForeachState", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionOperationState", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("FaasNet.StateMachine.Runtime.Domains.Definitions.StateMachineDefinitionSwitchState", b =>
                {
                    b.Navigation("Conditions");
                });
#pragma warning restore 612, 618
        }
    }
}