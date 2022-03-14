using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.EventMesh.SqlServer.Startup.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventMeshServers",
                columns: table => new
                {
                    Urn = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    CountryIsoCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMeshServers", x => new { x.Urn, x.Port });
                });

            migrationBuilder.CreateTable(
                name: "EventMeshServerBridge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Urn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    EventMeshServerAggregatePort = table.Column<int>(type: "int", nullable: true),
                    EventMeshServerAggregateUrn = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMeshServerBridge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMeshServerBridge_EventMeshServers_EventMeshServerAggregateUrn_EventMeshServerAggregatePort",
                        columns: x => new { x.EventMeshServerAggregateUrn, x.EventMeshServerAggregatePort },
                        principalTable: "EventMeshServers",
                        principalColumns: new[] { "Urn", "Port" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventMeshServerBridge_EventMeshServerAggregateUrn_EventMeshServerAggregatePort",
                table: "EventMeshServerBridge",
                columns: new[] { "EventMeshServerAggregateUrn", "EventMeshServerAggregatePort" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventMeshServerBridge");

            migrationBuilder.DropTable(
                name: "EventMeshServers");
        }
    }
}
