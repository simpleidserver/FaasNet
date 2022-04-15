using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FaasNet.EventMesh.Runtime.Website.Migrations
{
    public partial class InitEventMesh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationDomains",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Vpn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateMachineId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootTopic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    LastEvtOffset = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrokerConfigurations",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Protocol = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerConfigurations", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ClientLst",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Urn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vpn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Purposes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLst", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "MessageDefinitionLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationDomainId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JsonSchema = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDefinitionLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VpnLst",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnLst", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRoot = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    ApplicationDomainId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Application_ApplicationDomains_ApplicationDomainId",
                        column: x => x.ApplicationDomainId,
                        principalTable: "ApplicationDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrokerConfigurationRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrokerConfigurationName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerConfigurationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokerConfigurationRecord_BrokerConfigurations_BrokerConfigurationName",
                        column: x => x.BrokerConfigurationName,
                        principalTable: "BrokerConfigurations",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSession",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Vpn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddressData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pid = table.Column<int>(type: "int", nullable: false),
                    PurposeCode = table.Column<int>(type: "int", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BufferCloudEvents = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSession_ClientLst_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientLst",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BridgeServer",
                columns: table => new
                {
                    Urn = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Vpn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VpnName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BridgeServer", x => x.Urn);
                    table.ForeignKey(
                        name: "FK_BridgeServer_VpnLst_VpnName",
                        column: x => x.VpnName,
                        principalTable: "VpnLst",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TopicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartAnchor = table.Column<int>(type: "int", nullable: false),
                    EndAnchor = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationLink_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSessionBridge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Urn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vpn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSessionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientSessionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvtPayload = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    BrokerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSessionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrokerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSessionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Offset = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topic_ClientLst_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientLst",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Topic_ClientSession_ClientSessionId",
                        column: x => x.ClientSessionId,
                        principalTable: "ClientSession",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_ApplicationDomainId",
                table: "Application",
                column: "ApplicationDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationLink_ApplicationId",
                table: "ApplicationLink",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeServer_VpnName",
                table: "BridgeServer",
                column: "VpnName");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerConfigurationRecord_BrokerConfigurationName",
                table: "BrokerConfigurationRecord",
                column: "BrokerConfigurationName");

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
                name: "ApplicationLink");

            migrationBuilder.DropTable(
                name: "BridgeServer");

            migrationBuilder.DropTable(
                name: "BrokerConfigurationRecord");

            migrationBuilder.DropTable(
                name: "ClientSessionBridge");

            migrationBuilder.DropTable(
                name: "ClientSessionHistory");

            migrationBuilder.DropTable(
                name: "ClientSessionPendingCloudEvent");

            migrationBuilder.DropTable(
                name: "MessageDefinitionLst");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "VpnLst");

            migrationBuilder.DropTable(
                name: "BrokerConfigurations");

            migrationBuilder.DropTable(
                name: "ClientSession");

            migrationBuilder.DropTable(
                name: "ApplicationDomains");

            migrationBuilder.DropTable(
                name: "ClientLst");
        }
    }
}
