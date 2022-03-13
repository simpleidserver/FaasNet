using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.Gateway.SqlServer.Startup.Migrations
{
    public partial class AddParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "WorkflowInstances");
        }
    }
}
