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
                    AccessTries = table.Column<int>(type: "int", nullable: false)
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
                    { new Guid("22ea9e48-6528-4e8e-94b5-6a340431a140"), "nothing interesting", new DateTimeOffset(new DateTime(800, 4, 3, 1, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "campfire,stories", 0, 0, new Guid("2fe22508-b1c7-4243-9070-926172684da2"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "The First Few", new DateTimeOffset(new DateTime(800, 4, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("540ec6e6-5342-4350-92ec-a94e684b5df3"), "something interesting", null, "chill,drinks", 0, 0, new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (23.4413325 -76.0092066)"), "Masquerade", new DateTimeOffset(new DateTime(2025, 6, 25, 17, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("903aea86-698d-4f18-ba24-be6ee3d16566"), "still nothing interesting", new DateTimeOffset(new DateTime(800, 11, 4, 11, 3, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "skiing,drinks,rager", 0, 0, new Guid("2fe22508-b1c7-4243-9070-926172684da2"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "Then There Were Two", new DateTimeOffset(new DateTime(800, 11, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessTries", "DateOfBirth", "Email", "IsEmailConfirmed", "IsPhoneConfirmed", "JoinDate", "LockoutDate", "Name", "NormalisedEmail", "PhoneNumber", "Reputation", "SecurityStamp" },
                values: new object[,]
                {
                    { new Guid("2fe22508-b1c7-4243-9070-926172684da2"), 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Huginn", "", "1", 0, "260e2456-8712-4c3f-b68c-7b5d483961e6" },
                    { new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Signy of Sváfnir", "", "0", 0, "25370e4b-384f-41be-80f6-97e4abcdff52" },
                    { new Guid("d6430eeb-a7cc-4ef9-a851-6cde9376b9bf"), 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Muninn", "", "2", 0, "16c5a432-2da9-4380-82f4-4f30131fd200" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "EventId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("03b1d5fa-6d91-4fbd-b88d-5e4700030f0c"), new Guid("22ea9e48-6528-4e8e-94b5-6a340431a140"), new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), 0, "event" },
                    { new Guid("19c141ab-3f37-427c-ba5b-e8fbc87e9cd2"), new Guid("22ea9e48-6528-4e8e-94b5-6a340431a140"), new Guid("d6430eeb-a7cc-4ef9-a851-6cde9376b9bf"), 0, "event" },
                    { new Guid("56c3dad2-8a5b-4c2c-9472-86427844dad9"), new Guid("22ea9e48-6528-4e8e-94b5-6a340431a140"), new Guid("2fe22508-b1c7-4243-9070-926172684da2"), 0, "event" },
                    { new Guid("6ba2518b-0352-4769-a453-5cec43bdaeb9"), new Guid("903aea86-698d-4f18-ba24-be6ee3d16566"), new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), 0, "event" },
                    { new Guid("6bb343da-74dc-4584-9ff9-e732cc457854"), new Guid("540ec6e6-5342-4350-92ec-a94e684b5df3"), new Guid("2fe22508-b1c7-4243-9070-926172684da2"), 1, "event" },
                    { new Guid("7fbf2eb9-3e9e-4782-b1d8-101b315c9d19"), new Guid("540ec6e6-5342-4350-92ec-a94e684b5df3"), new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), 1, "event" },
                    { new Guid("b2862eb0-1407-4b2e-8410-d6f3854134b2"), new Guid("903aea86-698d-4f18-ba24-be6ee3d16566"), new Guid("2fe22508-b1c7-4243-9070-926172684da2"), 0, "event" },
                    { new Guid("bdd00760-019b-42ca-b27b-4325fc001560"), new Guid("540ec6e6-5342-4350-92ec-a94e684b5df3"), new Guid("d6430eeb-a7cc-4ef9-a851-6cde9376b9bf"), 1, "event" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "OtherId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("26863755-e02b-4d84-8678-89b3b86d2310"), new Guid("2fe22508-b1c7-4243-9070-926172684da2"), new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), 0, "user" },
                    { new Guid("298a9a8d-b2f2-4df6-a043-26a8c7f848d2"), new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), new Guid("d6430eeb-a7cc-4ef9-a851-6cde9376b9bf"), 0, "user" },
                    { new Guid("5d1ed340-a6e7-41c3-9d61-1da12bb60ad2"), new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), new Guid("2fe22508-b1c7-4243-9070-926172684da2"), 0, "user" },
                    { new Guid("bad9512b-5f0a-4165-88bf-893c2ddc3118"), new Guid("d6430eeb-a7cc-4ef9-a851-6cde9376b9bf"), new Guid("8b043af7-c35b-463e-a2eb-b9cb36d38426"), 1, "user" }
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
