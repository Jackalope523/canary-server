using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Links",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "JoinDate", "Name", "Passkey", "PhoneNumber", "Reputation" },
                values: new object[,]
                {
                    { new Guid("33951325-d0da-4fd5-9dfd-f7e6abee8390"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Signy of Sváfnir", "", "0", 0 },
                    { new Guid("5829377a-c9d6-4470-83ba-1390f976d958"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Huginn", "", "1", 0 },
                    { new Guid("fdaa5756-462a-4e0f-a7e8-774d1ee64ddb"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Muninn", "", "2", 0 }
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "Discriminator", "OtherId", "SelfId", "Type", "link_type" },
                values: new object[,]
                {
                    { new Guid("0374a719-0e07-4952-9d42-f97cf3b908f4"), null, new Guid("fdaa5756-462a-4e0f-a7e8-774d1ee64ddb"), new Guid("33951325-d0da-4fd5-9dfd-f7e6abee8390"), 1, "user" },
                    { new Guid("3ffd8b59-c392-434e-8f63-0d8f1141ec62"), null, new Guid("33951325-d0da-4fd5-9dfd-f7e6abee8390"), new Guid("5829377a-c9d6-4470-83ba-1390f976d958"), 0, "user" },
                    { new Guid("bc3f40b5-9377-42fb-a76f-fa35bb52883f"), null, new Guid("5829377a-c9d6-4470-83ba-1390f976d958"), new Guid("33951325-d0da-4fd5-9dfd-f7e6abee8390"), 0, "user" },
                    { new Guid("bf5653ff-6fa6-417a-90ec-5191b32b0a28"), null, new Guid("33951325-d0da-4fd5-9dfd-f7e6abee8390"), new Guid("fdaa5756-462a-4e0f-a7e8-774d1ee64ddb"), 0, "user" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Links",
                keyColumn: "Id",
                keyValue: new Guid("0374a719-0e07-4952-9d42-f97cf3b908f4"));

            migrationBuilder.DeleteData(
                table: "Links",
                keyColumn: "Id",
                keyValue: new Guid("3ffd8b59-c392-434e-8f63-0d8f1141ec62"));

            migrationBuilder.DeleteData(
                table: "Links",
                keyColumn: "Id",
                keyValue: new Guid("bc3f40b5-9377-42fb-a76f-fa35bb52883f"));

            migrationBuilder.DeleteData(
                table: "Links",
                keyColumn: "Id",
                keyValue: new Guid("bf5653ff-6fa6-417a-90ec-5191b32b0a28"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33951325-d0da-4fd5-9dfd-f7e6abee8390"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5829377a-c9d6-4470-83ba-1390f976d958"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("fdaa5756-462a-4e0f-a7e8-774d1ee64ddb"));

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Links");
        }
    }
}
