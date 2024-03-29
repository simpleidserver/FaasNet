﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.StateMachine.SqlServer.Startup.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    TechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsLast = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vpn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationDomainId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootTopic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    LastEvtOffset = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.TechnicalId);
                });

            migrationBuilder.CreateTable(
                name: "StateMachineDefinitionEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    MetadataStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateMachineDefinitionAggregateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMachineDefinitionEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateMachineDefinitionEvent_WorkflowDefinitions_StateMachineDefinitionAggregateTechnicalId",
                        column: x => x.StateMachineDefinitionAggregateTechnicalId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StateMachineDefinitionFunction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MetadataStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunctionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateMachineDefinitionAggregateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMachineDefinitionFunction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateMachineDefinitionFunction_WorkflowDefinitions_StateMachineDefinitionAggregateTechnicalId",
                        column: x => x.StateMachineDefinitionAggregateTechnicalId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaseEventCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConditionType = table.Column<int>(type: "int", nullable: false),
                    Transition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    End = table.Column<bool>(type: "bit", nullable: false),
                    StateMachineDefinitionSwitchStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDataFilter = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEventCondition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateMachineDefinitionAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunctionRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDataFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateMachineDefinitionForeachStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StateMachineDefinitionOnEventId = table.Column<int>(type: "int", nullable: true),
                    StateMachineDefinitionOperationStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMachineDefinitionAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseStateMachineDefinitionState",
                columns: table => new
                {
                    TechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StateDataFilterInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateDataFilterOuput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateMachineDefinitionAggregateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EventRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionId = table.Column<int>(type: "int", nullable: true),
                    EventDataFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    End = table.Column<bool>(type: "bit", nullable: true),
                    Transition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exclusive = table.Column<bool>(type: "bit", nullable: true),
                    InputCollection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutputCollection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IterationParam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchSize = table.Column<int>(type: "int", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: true),
                    DataStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionMode = table.Column<int>(type: "int", nullable: true),
                    DefaultConditionStr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseStateMachineDefinitionState", x => x.TechnicalId);
                    table.ForeignKey(
                        name: "FK_BaseStateMachineDefinitionState_StateMachineDefinitionAction_ActionId",
                        column: x => x.ActionId,
                        principalTable: "StateMachineDefinitionAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseStateMachineDefinitionState_WorkflowDefinitions_StateMachineDefinitionAggregateTechnicalId",
                        column: x => x.StateMachineDefinitionAggregateTechnicalId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StateMachineDefinitionOnEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventRefs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionMode = table.Column<int>(type: "int", nullable: false),
                    EventDataFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateMachineDefinitionEventStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMachineDefinitionOnEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateMachineDefinitionOnEvent_BaseStateMachineDefinitionState_StateMachineDefinitionEventStateTechnicalId",
                        column: x => x.StateMachineDefinitionEventStateTechnicalId,
                        principalTable: "BaseStateMachineDefinitionState",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseEventCondition_StateMachineDefinitionSwitchStateTechnicalId",
                table: "BaseEventCondition",
                column: "StateMachineDefinitionSwitchStateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseStateMachineDefinitionState_ActionId",
                table: "BaseStateMachineDefinitionState",
                column: "ActionId",
                unique: true,
                filter: "[ActionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseStateMachineDefinitionState_StateMachineDefinitionAggregateTechnicalId",
                table: "BaseStateMachineDefinitionState",
                column: "StateMachineDefinitionAggregateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineDefinitionAction_StateMachineDefinitionForeachStateTechnicalId",
                table: "StateMachineDefinitionAction",
                column: "StateMachineDefinitionForeachStateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineDefinitionAction_StateMachineDefinitionOnEventId",
                table: "StateMachineDefinitionAction",
                column: "StateMachineDefinitionOnEventId");

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineDefinitionAction_StateMachineDefinitionOperationStateTechnicalId",
                table: "StateMachineDefinitionAction",
                column: "StateMachineDefinitionOperationStateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineDefinitionEvent_StateMachineDefinitionAggregateTechnicalId",
                table: "StateMachineDefinitionEvent",
                column: "StateMachineDefinitionAggregateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineDefinitionFunction_StateMachineDefinitionAggregateTechnicalId",
                table: "StateMachineDefinitionFunction",
                column: "StateMachineDefinitionAggregateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineDefinitionOnEvent_StateMachineDefinitionEventStateTechnicalId",
                table: "StateMachineDefinitionOnEvent",
                column: "StateMachineDefinitionEventStateTechnicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEventCondition_BaseStateMachineDefinitionState_StateMachineDefinitionSwitchStateTechnicalId",
                table: "BaseEventCondition",
                column: "StateMachineDefinitionSwitchStateTechnicalId",
                principalTable: "BaseStateMachineDefinitionState",
                principalColumn: "TechnicalId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StateMachineDefinitionAction_BaseStateMachineDefinitionState_StateMachineDefinitionForeachStateTechnicalId",
                table: "StateMachineDefinitionAction",
                column: "StateMachineDefinitionForeachStateTechnicalId",
                principalTable: "BaseStateMachineDefinitionState",
                principalColumn: "TechnicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_StateMachineDefinitionAction_BaseStateMachineDefinitionState_StateMachineDefinitionOperationStateTechnicalId",
                table: "StateMachineDefinitionAction",
                column: "StateMachineDefinitionOperationStateTechnicalId",
                principalTable: "BaseStateMachineDefinitionState",
                principalColumn: "TechnicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_StateMachineDefinitionAction_StateMachineDefinitionOnEvent_StateMachineDefinitionOnEventId",
                table: "StateMachineDefinitionAction",
                column: "StateMachineDefinitionOnEventId",
                principalTable: "StateMachineDefinitionOnEvent",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StateMachineDefinitionAction_BaseStateMachineDefinitionState_StateMachineDefinitionForeachStateTechnicalId",
                table: "StateMachineDefinitionAction");

            migrationBuilder.DropForeignKey(
                name: "FK_StateMachineDefinitionAction_BaseStateMachineDefinitionState_StateMachineDefinitionOperationStateTechnicalId",
                table: "StateMachineDefinitionAction");

            migrationBuilder.DropForeignKey(
                name: "FK_StateMachineDefinitionOnEvent_BaseStateMachineDefinitionState_StateMachineDefinitionEventStateTechnicalId",
                table: "StateMachineDefinitionOnEvent");

            migrationBuilder.DropTable(
                name: "BaseEventCondition");

            migrationBuilder.DropTable(
                name: "StateMachineDefinitionEvent");

            migrationBuilder.DropTable(
                name: "StateMachineDefinitionFunction");

            migrationBuilder.DropTable(
                name: "BaseStateMachineDefinitionState");

            migrationBuilder.DropTable(
                name: "StateMachineDefinitionAction");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "StateMachineDefinitionOnEvent");
        }
    }
}
