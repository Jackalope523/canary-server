using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Repository.Migrations
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
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                columns: new[] { "Id", "Description", "EndTime", "GroupMaximum", "GroupMinimum", "HostId", "IsEventOpen", "Location", "Name", "StartTime", "Type" },
                values: new object[,]
                {
                    { new Guid("a658d8a4-8dbe-4cc5-99cc-1d9a7c1ace77"), "still nothing interesting", new DateTimeOffset(new DateTime(800, 11, 4, 11, 3, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 0, new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "Then There Were Two", new DateTimeOffset(new DateTime(800, 11, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "skiing,drinks,rager" },
                    { new Guid("ca8e2ea7-204f-4ac5-bb1e-6b7c1752ec8e"), "something interesting", null, 0, 0, new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (23.4413325 -76.0092066)"), "Masquerade", new DateTimeOffset(new DateTime(2025, 6, 25, 17, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "chill,drinks" },
                    { new Guid("d99961f5-1332-4bf3-80a0-2fd9d27e5d4e"), "nothing interesting", new DateTimeOffset(new DateTime(800, 4, 3, 1, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 0, new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), false, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4237;POINT (0 0)"), "The First Few", new DateTimeOffset(new DateTime(800, 4, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "campfire,stories" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessTries", "AccountStatus", "DateOfBirth", "Email", "IsEmailConfirmed", "IsPhoneConfirmed", "JoinDate", "LockoutDate", "Name", "NormalisedEmail", "PhoneNumber", "Reputation", "SecurityStamp" },
                values: new object[,]
                {
                    { new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), 0, 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Huginn", "", "1", 0, "8123c3e1-85ab-48d2-b2df-be704b35f133" },
                    { new Guid("c75de3b0-f166-4894-bee5-98cff6ade6ce"), 0, 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Muninn", "", "2", 0, "6718008d-be8a-4a73-a3dd-0518ec7a3d77" },
                    { new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), 0, 0, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Signy of Sváfnir", "", "0", 0, "266a9a20-36ab-49cc-aa1d-ee0526858bbb" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "EventId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("1c5be907-e057-4c08-9ca0-a8e646a4fe01"), new Guid("ca8e2ea7-204f-4ac5-bb1e-6b7c1752ec8e"), new Guid("c75de3b0-f166-4894-bee5-98cff6ade6ce"), 1, "event" },
                    { new Guid("495f866c-3f1f-4f6a-b058-86fb214977d2"), new Guid("ca8e2ea7-204f-4ac5-bb1e-6b7c1752ec8e"), new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), 1, "event" },
                    { new Guid("67586de8-db04-4802-814b-78c665ed8cbd"), new Guid("d99961f5-1332-4bf3-80a0-2fd9d27e5d4e"), new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), 0, "event" },
                    { new Guid("6e64b50d-596e-49d9-95cb-97b73b86f766"), new Guid("ca8e2ea7-204f-4ac5-bb1e-6b7c1752ec8e"), new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), 1, "event" },
                    { new Guid("d3c8b3cc-93b2-4916-8ea4-32355687ecda"), new Guid("a658d8a4-8dbe-4cc5-99cc-1d9a7c1ace77"), new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), 0, "event" },
                    { new Guid("d404b40a-5405-4b3c-ace2-cd086efbbb07"), new Guid("d99961f5-1332-4bf3-80a0-2fd9d27e5d4e"), new Guid("c75de3b0-f166-4894-bee5-98cff6ade6ce"), 0, "event" },
                    { new Guid("fa59fbea-17c3-412e-9a63-d837020aa19f"), new Guid("d99961f5-1332-4bf3-80a0-2fd9d27e5d4e"), new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), 0, "event" },
                    { new Guid("fc018845-eab2-43b1-8de5-c85559b9082e"), new Guid("a658d8a4-8dbe-4cc5-99cc-1d9a7c1ace77"), new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), 0, "event" }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "OtherId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("02cac8f7-bb56-4807-a510-e1e6a0de3fb0"), new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), 0, "user" },
                    { new Guid("22400f6e-c402-4fdf-998b-b536207b4d4d"), new Guid("3dcf1d47-7322-439a-8011-9091a0ba8b99"), new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), 0, "user" },
                    { new Guid("6b5b70aa-ade5-4266-b8de-a71a8d119d68"), new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), new Guid("c75de3b0-f166-4894-bee5-98cff6ade6ce"), 0, "user" },
                    { new Guid("af62a095-506a-4ec5-8e90-cb33beea92b6"), new Guid("c75de3b0-f166-4894-bee5-98cff6ade6ce"), new Guid("d365de18-b3cf-4945-92ba-0bd4db2b4b18"), 1, "user" }
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
