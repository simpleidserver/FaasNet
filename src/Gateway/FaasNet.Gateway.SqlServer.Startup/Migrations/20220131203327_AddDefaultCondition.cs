using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.Gateway.SqlServer.Startup.Migrations
{
    public partial class AddDefaultCondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultConditionStr",
                table: "BaseWorkflowDefinitionState",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultConditionStr",
                table: "BaseWorkflowDefinitionState");
        }
    }
}
