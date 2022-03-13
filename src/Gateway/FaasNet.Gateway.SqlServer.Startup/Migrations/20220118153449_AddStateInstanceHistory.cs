using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.Gateway.SqlServer.Startup.Migrations
{
    public partial class AddStateInstanceHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowInstanceStateHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowInstanceStateId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceStateHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceStateHistory_WorkflowInstanceState_WorkflowInstanceStateId",
                        column: x => x.WorkflowInstanceStateId,
                        principalTable: "WorkflowInstanceState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceStateHistory_WorkflowInstanceStateId",
                table: "WorkflowInstanceStateHistory",
                column: "WorkflowInstanceStateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowInstanceStateHistory");
        }
    }
}
