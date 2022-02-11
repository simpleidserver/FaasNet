using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventMesh.Runtime.Server.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BridgeServers",
                columns: table => new
                {
                    Urn = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BridgeServers", x => x.Urn);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    Urn = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "ClientSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IPAddressData = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Environment = table.Column<string>(type: "TEXT", nullable: true),
                    Pid = table.Column<int>(type: "INTEGER", nullable: false),
                    PurposeCode = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Seq = table.Column<string>(type: "TEXT", nullable: true),
                    BufferCloudEvents = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSession_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSessionBridge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Urn = table.Column<string>(type: "TEXT", nullable: true),
                    ClientSessionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSessionBridge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSessionBridge_ClientSession_ClientSessionId",
                        column: x => x.ClientSessionId,
                        principalTable: "ClientSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSessionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientSessionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSessionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSessionHistory_ClientSession_ClientSessionId",
                        column: x => x.ClientSessionId,
                        principalTable: "ClientSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSessionPendingCloudEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EvtPayload = table.Column<byte[]>(type: "BLOB", nullable: true),
                    BrokerName = table.Column<string>(type: "TEXT", nullable: true),
                    Topic = table.Column<string>(type: "TEXT", nullable: true),
                    ClientSessionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSessionPendingCloudEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSessionPendingCloudEvent_ClientSession_ClientSessionId",
                        column: x => x.ClientSessionId,
                        principalTable: "ClientSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    BrokerName = table.Column<string>(type: "TEXT", nullable: true),
                    ClientSessionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    Offset = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topic_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Topic_ClientSession_ClientSessionId",
                        column: x => x.ClientSessionId,
                        principalTable: "ClientSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientSession_ClientId",
                table: "ClientSession",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSessionBridge_ClientSessionId",
                table: "ClientSessionBridge",
                column: "ClientSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSessionHistory_ClientSessionId",
                table: "ClientSessionHistory",
                column: "ClientSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSessionPendingCloudEvent_ClientSessionId",
                table: "ClientSessionPendingCloudEvent",
                column: "ClientSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_ClientId",
                table: "Topic",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_ClientSessionId",
                table: "Topic",
                column: "ClientSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BridgeServers");

            migrationBuilder.DropTable(
                name: "ClientSessionBridge");

            migrationBuilder.DropTable(
                name: "ClientSessionHistory");

            migrationBuilder.DropTable(
                name: "ClientSessionPendingCloudEvent");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "ClientSession");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
