using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Location = table.Column<Point>(type: "geography", nullable: false),
                    IsEventOpen = table.Column<bool>(type: "bit", nullable: false),
                    GroupMinimum = table.Column<int>(type: "int", nullable: false),
                    GroupMaximum = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    JoinDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Reputation = table.Column<int>(type: "int", nullable: false),
                    NormalisedEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPhoneConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LockoutDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AccessTries = table.Column<int>(type: "int", nullable: false),
                    AccountStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    link_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    OtherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Links_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Links_Users_SelfId",
                        column: x => x.SelfId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Description", "EndTime", "EventType", "GroupMaximum", "GroupMinimum", "HostId", "IsEventOpen", "Location", "Name", "StartTime" },
                values: new object[,]
                {
                    { new Guid("43a9001a-c220-42c0-8db2-78e448340ef7"), "still nothing interesting", new DateTimeOffset(new DateTime(800, 11, 4, 11, 3, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "skiing,drinks,rager", 0, 0, new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "Then There Were Two", new DateTimeOffset(new DateTime(800, 11, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("72b1c2e9-88ca-477a-869e-71eeb555d813"), "something interesting", null, "chill,drinks", 0, 0, new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (23.4413325 -76.0092066)"), "Masquerade", new DateTimeOffset(new DateTime(2025, 6, 25, 17, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b98dc4a7-66fd-41be-9ac1-70ef12875472"), "nothing interesting", new DateTimeOffset(new DateTime(800, 4, 3, 1, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "campfire,stories", 0, 0, new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "The First Few", new DateTimeOffset(new DateTime(800, 4, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessTries", "AccountStatus", "DateOfBirth", "Email", "IsEmailConfirmed", "IsPhoneConfirmed", "JoinDate", "LockoutDate", "Name", "NormalisedEmail", "PhoneNumber", "Reputation", "SecurityStamp" },
                values: new object[,]
                {
                    { new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), 0, 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Signy of Sváfnir", "", "0", 0, "b6462001-08c7-43ef-bff8-824ef5819d95" },
                    { new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), 0, 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Huginn", "", "1", 0, "675a78f2-5038-4c51-b9a9-f639e194d2de" },
                    { new Guid("b4940b9e-bb9b-49dc-a9ee-458a3feb76f5"), 0, 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Muninn", "", "2", 0, "dc97fd04-ec68-41d0-80ad-20df7c858ea9" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "EventId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("34621ad4-75cc-49bb-a046-4d93a8d42492"), new Guid("43a9001a-c220-42c0-8db2-78e448340ef7"), new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), 0, "event" },
                    { new Guid("699ba422-0ad7-4439-96ff-db6f67d44ce1"), new Guid("72b1c2e9-88ca-477a-869e-71eeb555d813"), new Guid("b4940b9e-bb9b-49dc-a9ee-458a3feb76f5"), 1, "event" },
                    { new Guid("7ac4cb6f-1d4a-4bea-aa4f-8e129bead1b5"), new Guid("b98dc4a7-66fd-41be-9ac1-70ef12875472"), new Guid("b4940b9e-bb9b-49dc-a9ee-458a3feb76f5"), 0, "event" },
                    { new Guid("9989ae13-874e-42ca-b73a-493e9f624f21"), new Guid("b98dc4a7-66fd-41be-9ac1-70ef12875472"), new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), 0, "event" },
                    { new Guid("a9d20a93-b0b1-4cca-9b73-9cca7c5c10a3"), new Guid("b98dc4a7-66fd-41be-9ac1-70ef12875472"), new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), 0, "event" },
                    { new Guid("bf256884-4360-4d3a-a669-7f964eaeadfb"), new Guid("43a9001a-c220-42c0-8db2-78e448340ef7"), new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), 0, "event" },
                    { new Guid("c16f67ad-e638-40a7-9e60-3851276dddfd"), new Guid("72b1c2e9-88ca-477a-869e-71eeb555d813"), new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), 1, "event" },
                    { new Guid("e96c1719-6121-4877-9793-20ec046d8d28"), new Guid("72b1c2e9-88ca-477a-869e-71eeb555d813"), new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), 1, "event" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "OtherId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("8942bda0-aab2-4219-8e4f-dd0c5d3f8374"), new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), new Guid("b4940b9e-bb9b-49dc-a9ee-458a3feb76f5"), 0, "user" },
                    { new Guid("96e2a779-fd82-4018-9565-a3c2612be074"), new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), 0, "user" },
                    { new Guid("a24b5b3e-0c7a-4d66-a839-ba66cdb0cbdf"), new Guid("b4940b9e-bb9b-49dc-a9ee-458a3feb76f5"), new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), 1, "user" },
                    { new Guid("b3c3f836-7640-4f59-803c-0ebaf212ba90"), new Guid("53c8a2cf-46dd-435c-a5dd-6130c2c3d408"), new Guid("5df33e44-2e1a-4cb7-8296-bd098f6e32b9"), 0, "user" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Links_EventId",
                table: "Links",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Links_SelfId",
                table: "Links",
                column: "SelfId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
