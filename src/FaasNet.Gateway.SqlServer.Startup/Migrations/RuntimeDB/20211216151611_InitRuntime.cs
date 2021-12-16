using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.Gateway.SqlServer.Startup.Migrations.RuntimeDB
{
    public partial class InitRuntime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstanceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateInstanceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConsumed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    TechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsLast = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.TechnicalId);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkflowDefTechnicalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefVersion = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OutputStr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitionEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    WorkflowDefinitionAggregateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitionEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDefinitionEvent_WorkflowDefinitions_WorkflowDefinitionAggregateTechnicalId",
                        column: x => x.WorkflowDefinitionAggregateTechnicalId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitionFunction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MetadataStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunctionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefinitionAggregateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitionFunction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDefinitionFunction_WorkflowDefinitions_WorkflowDefinitionAggregateTechnicalId",
                        column: x => x.WorkflowDefinitionAggregateTechnicalId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstanceState",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InputStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutputStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowInstanceAggregateId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceState_WorkflowInstances_WorkflowInstanceAggregateId",
                        column: x => x.WorkflowInstanceAggregateId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstanceStateEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    InputData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowInstanceStateId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceStateEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceStateEvent_WorkflowInstanceState_WorkflowInstanceStateId",
                        column: x => x.WorkflowInstanceStateId,
                        principalTable: "WorkflowInstanceState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstanceStateEventOutput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowInstanceStateEventId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceStateEventOutput", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceStateEventOutput_WorkflowInstanceStateEvent_WorkflowInstanceStateEventId",
                        column: x => x.WorkflowInstanceStateEventId,
                        principalTable: "WorkflowInstanceStateEvent",
                        principalColumn: "Id",
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
                    WorkflowDefinitionSwitchStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDataFilter = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEventCondition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitionAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunctionRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDataFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefinitionForeachStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WorkflowDefinitionOnEventId = table.Column<int>(type: "int", nullable: true),
                    WorkflowDefinitionOperationStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitionAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseWorkflowDefinitionState",
                columns: table => new
                {
                    TechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StateDataFilterInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateDataFilterOuput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefinitionAggregateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EventRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionId = table.Column<int>(type: "int", nullable: true),
                    End = table.Column<bool>(type: "bit", nullable: true),
                    Transition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exclusive = table.Column<bool>(type: "bit", nullable: true),
                    InputCollection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutputCollection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IterationParam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchSize = table.Column<int>(type: "int", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: true),
                    DataStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionMode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseWorkflowDefinitionState", x => x.TechnicalId);
                    table.ForeignKey(
                        name: "FK_BaseWorkflowDefinitionState_WorkflowDefinitionAction_ActionId",
                        column: x => x.ActionId,
                        principalTable: "WorkflowDefinitionAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseWorkflowDefinitionState_WorkflowDefinitions_WorkflowDefinitionAggregateTechnicalId",
                        column: x => x.WorkflowDefinitionAggregateTechnicalId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitionOnEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventRefs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionMode = table.Column<int>(type: "int", nullable: false),
                    EventDataFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDefinitionEventStateTechnicalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitionOnEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDefinitionOnEvent_BaseWorkflowDefinitionState_WorkflowDefinitionEventStateTechnicalId",
                        column: x => x.WorkflowDefinitionEventStateTechnicalId,
                        principalTable: "BaseWorkflowDefinitionState",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseEventCondition_WorkflowDefinitionSwitchStateTechnicalId",
                table: "BaseEventCondition",
                column: "WorkflowDefinitionSwitchStateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseWorkflowDefinitionState_ActionId",
                table: "BaseWorkflowDefinitionState",
                column: "ActionId",
                unique: true,
                filter: "[ActionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseWorkflowDefinitionState_WorkflowDefinitionAggregateTechnicalId",
                table: "BaseWorkflowDefinitionState",
                column: "WorkflowDefinitionAggregateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionAction_WorkflowDefinitionForeachStateTechnicalId",
                table: "WorkflowDefinitionAction",
                column: "WorkflowDefinitionForeachStateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionAction_WorkflowDefinitionOnEventId",
                table: "WorkflowDefinitionAction",
                column: "WorkflowDefinitionOnEventId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionAction_WorkflowDefinitionOperationStateTechnicalId",
                table: "WorkflowDefinitionAction",
                column: "WorkflowDefinitionOperationStateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionEvent_WorkflowDefinitionAggregateTechnicalId",
                table: "WorkflowDefinitionEvent",
                column: "WorkflowDefinitionAggregateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionFunction_WorkflowDefinitionAggregateTechnicalId",
                table: "WorkflowDefinitionFunction",
                column: "WorkflowDefinitionAggregateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionOnEvent_WorkflowDefinitionEventStateTechnicalId",
                table: "WorkflowDefinitionOnEvent",
                column: "WorkflowDefinitionEventStateTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceState_WorkflowInstanceAggregateId",
                table: "WorkflowInstanceState",
                column: "WorkflowInstanceAggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceStateEvent_WorkflowInstanceStateId",
                table: "WorkflowInstanceStateEvent",
                column: "WorkflowInstanceStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceStateEventOutput_WorkflowInstanceStateEventId",
                table: "WorkflowInstanceStateEventOutput",
                column: "WorkflowInstanceStateEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEventCondition_BaseWorkflowDefinitionState_WorkflowDefinitionSwitchStateTechnicalId",
                table: "BaseEventCondition",
                column: "WorkflowDefinitionSwitchStateTechnicalId",
                principalTable: "BaseWorkflowDefinitionState",
                principalColumn: "TechnicalId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowDefinitionAction_BaseWorkflowDefinitionState_WorkflowDefinitionForeachStateTechnicalId",
                table: "WorkflowDefinitionAction",
                column: "WorkflowDefinitionForeachStateTechnicalId",
                principalTable: "BaseWorkflowDefinitionState",
                principalColumn: "TechnicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowDefinitionAction_BaseWorkflowDefinitionState_WorkflowDefinitionOperationStateTechnicalId",
                table: "WorkflowDefinitionAction",
                column: "WorkflowDefinitionOperationStateTechnicalId",
                principalTable: "BaseWorkflowDefinitionState",
                principalColumn: "TechnicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowDefinitionAction_WorkflowDefinitionOnEvent_WorkflowDefinitionOnEventId",
                table: "WorkflowDefinitionAction",
                column: "WorkflowDefinitionOnEventId",
                principalTable: "WorkflowDefinitionOnEvent",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowDefinitionAction_BaseWorkflowDefinitionState_WorkflowDefinitionForeachStateTechnicalId",
                table: "WorkflowDefinitionAction");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowDefinitionAction_BaseWorkflowDefinitionState_WorkflowDefinitionOperationStateTechnicalId",
                table: "WorkflowDefinitionAction");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowDefinitionOnEvent_BaseWorkflowDefinitionState_WorkflowDefinitionEventStateTechnicalId",
                table: "WorkflowDefinitionOnEvent");

            migrationBuilder.DropTable(
                name: "BaseEventCondition");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitionEvent");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitionFunction");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceStateEventOutput");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceStateEvent");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceState");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "BaseWorkflowDefinitionState");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitionAction");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitionOnEvent");
        }
    }
}
