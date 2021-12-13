using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.Gateway.SqlServer.Startup.Migrations
{
    public partial class AddColumnsWorkflowInstance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkflowDefDescription",
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkflowDefName",
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkflowDefDescription",
                table: "WorkflowInstances");

            migrationBuilder.DropColumn(
                name: "WorkflowDefName",
                table: "WorkflowInstances");
        }
    }
}
