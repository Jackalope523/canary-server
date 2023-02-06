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
                    Discriminator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    link_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OtherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true)
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
                    { new Guid("2f07003e-a3ca-4688-b1e8-1e4373afe615"), "something interesting", null, "chill,drinks", 0, 0, new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (23.4413325 -76.0092066)"), "Masquerade", new DateTimeOffset(new DateTime(2025, 6, 25, 17, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("89d9f649-cb8e-4f97-9014-da3576c4d0d8"), "still nothing interesting", new DateTimeOffset(new DateTime(800, 11, 4, 11, 3, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "skiing,drinks,rager", 0, 0, new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "Then There Were Two", new DateTimeOffset(new DateTime(800, 11, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("8b004d57-ec57-4293-be01-3760a1be26c8"), "nothing interesting", new DateTimeOffset(new DateTime(800, 4, 3, 1, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "campfire,stories", 0, 0, new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "The First Few", new DateTimeOffset(new DateTime(800, 4, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessTries", "DateOfBirth", "Email", "IsEmailConfirmed", "IsPhoneConfirmed", "JoinDate", "LockoutDate", "Name", "PhoneNumber", "Reputation", "SecurityStamp" },
                values: new object[,]
                {
                    { new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Huginn", "1", 0, "604a41c9-1d3f-4e31-8576-184807b7835a" },
                    { new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Signy of Sváfnir", "0", 0, "99669718-9f64-4a64-9067-45cfc0ee19ef" },
                    { new Guid("ecb85ed0-1628-4ae2-81bb-0e9bb3bdb6b8"), 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Muninn", "2", 0, "f3442a23-3b33-4cf7-95d2-e0bd3ac379cb" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "Discriminator", "EventId", "SelfId", "link_type" },
                values: new object[,]
                {
                    { new Guid("13b3ccb5-bd9e-422d-9cfe-72c82a973c9c"), null, new Guid("8b004d57-ec57-4293-be01-3760a1be26c8"), new Guid("ecb85ed0-1628-4ae2-81bb-0e9bb3bdb6b8"), "event" },
                    { new Guid("3f5c5b71-b28a-4379-b1a7-6ac808e4d190"), null, new Guid("2f07003e-a3ca-4688-b1e8-1e4373afe615"), new Guid("ecb85ed0-1628-4ae2-81bb-0e9bb3bdb6b8"), "event" },
                    { new Guid("748b5d38-286e-4917-83a7-50d73f63058a"), null, new Guid("8b004d57-ec57-4293-be01-3760a1be26c8"), new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), "event" },
                    { new Guid("99861b00-fdd1-4f4b-881a-b00fa227a5a8"), null, new Guid("2f07003e-a3ca-4688-b1e8-1e4373afe615"), new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), "event" },
                    { new Guid("c44be1cb-b8ce-4349-a9f2-dc16d13c6063"), null, new Guid("2f07003e-a3ca-4688-b1e8-1e4373afe615"), new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), "event" },
                    { new Guid("c931bfbb-136f-4bf6-b8ea-a37c913c12ca"), null, new Guid("8b004d57-ec57-4293-be01-3760a1be26c8"), new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), "event" },
                    { new Guid("e20dbc09-aa27-4cf4-84a3-abb5a8d941dd"), null, new Guid("89d9f649-cb8e-4f97-9014-da3576c4d0d8"), new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), "event" },
                    { new Guid("f2180438-d129-4877-af66-07626068ef8b"), null, new Guid("89d9f649-cb8e-4f97-9014-da3576c4d0d8"), new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), "event" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "Discriminator", "OtherId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("135661a5-7b71-44cc-ad0b-f83e16a3c36a"), null, new Guid("ecb85ed0-1628-4ae2-81bb-0e9bb3bdb6b8"), new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), 1, "user" },
                    { new Guid("2d99ec49-5ff1-40bf-ac22-8115cbb6d76e"), null, new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), new Guid("ecb85ed0-1628-4ae2-81bb-0e9bb3bdb6b8"), 0, "user" },
                    { new Guid("e7a57830-7600-42ad-a138-728f638d84dd"), null, new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), 0, "user" },
                    { new Guid("f4841190-57aa-440a-8d4f-a8f124629fbd"), null, new Guid("73acf8c3-a2fd-41d3-af27-43215fd0637f"), new Guid("564bb3d4-c2cf-4278-9a0c-68da3eb782b3"), 0, "user" }
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
