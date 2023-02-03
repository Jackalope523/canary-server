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
                    Location = table.Column<Point>(type: "geography", nullable: false)
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
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                table: "Users",
                columns: new[] { "Id", "AccessTries", "DateOfBirth", "Email", "IsEmailConfirmed", "IsPhoneConfirmed", "JoinDate", "LockoutDate", "Name", "PhoneNumber", "Reputation", "SecurityStamp" },
                values: new object[] { new Guid("5d4f8ae3-3414-445d-8a82-aeb632bfba91"), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Huginn", "1", 0, "01d89e9b-d1b1-460c-92c1-a61420692faf" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessTries", "DateOfBirth", "Email", "IsEmailConfirmed", "IsPhoneConfirmed", "JoinDate", "LockoutDate", "Name", "PhoneNumber", "Reputation", "SecurityStamp" },
                values: new object[] { new Guid("d047c5e7-8f32-474f-a466-66989c934b05"), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Signy of Sváfnir", "0", 0, "b77d02dd-352f-4539-885c-2ddff6b9b462" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessTries", "DateOfBirth", "Email", "IsEmailConfirmed", "IsPhoneConfirmed", "JoinDate", "LockoutDate", "Name", "PhoneNumber", "Reputation", "SecurityStamp" },
                values: new object[] { new Guid("d9e5aed6-e9bc-4986-8f46-9c5aaf32f162"), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Muninn", "2", 0, "266a3179-8a1a-4324-8235-11f54f3bb42c" });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "Discriminator", "OtherId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("07255625-ec32-45f3-b43d-7e0e06acf2e3"), null, new Guid("d047c5e7-8f32-474f-a466-66989c934b05"), new Guid("d9e5aed6-e9bc-4986-8f46-9c5aaf32f162"), 0, "user" },
                    { new Guid("c84ed896-d491-4fde-ad20-46856a88d7aa"), null, new Guid("5d4f8ae3-3414-445d-8a82-aeb632bfba91"), new Guid("d047c5e7-8f32-474f-a466-66989c934b05"), 0, "user" },
                    { new Guid("d2b05d8a-e3f2-43f6-a283-681deec2b607"), null, new Guid("d047c5e7-8f32-474f-a466-66989c934b05"), new Guid("5d4f8ae3-3414-445d-8a82-aeb632bfba91"), 0, "user" },
                    { new Guid("db9de99d-4c54-4a6e-a573-830ed264ff13"), null, new Guid("d9e5aed6-e9bc-4986-8f46-9c5aaf32f162"), new Guid("d047c5e7-8f32-474f-a466-66989c934b05"), 1, "user" }
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
