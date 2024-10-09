using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.AzureMigrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GatheringLinks_Banners_BannerId",
                table: "GatheringLinks");

            migrationBuilder.RenameTable(
                name: "Notes",
                newName: "Telegrams");

            migrationBuilder.DropIndex(
                name: "IX_GatheringLinks_BannerId",
                table: "GatheringLinks");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Snapshots");

            migrationBuilder.DropColumn(
                name: "PhotoURL",
                table: "Snapshots");

            migrationBuilder.DropColumn(
                name: "HeroImageURL",
                table: "Gatherings");

            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "GatheringLinks");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Pseudonym",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TimeOfUserAgreement",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Gatherings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameIndex(
                name: "IX_Notes_UserId",
                table: "Telegrams",
                newName: "IX_Telegrams_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Telegrams",
                newName: "Notes");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Pseudonym",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TimeOfUserAgreement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Gatherings");

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Snapshots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhotoURL",
                table: "Snapshots",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HeroImageURL",
                table: "Gatherings",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BannerId",
                table: "GatheringLinks",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GatheringLinks_BannerId",
                table: "GatheringLinks",
                column: "BannerId");

            migrationBuilder.RenameIndex(
               name: "IX_Telegrams_UserId",
               table: "Notes",
               newName: "IX_Notes_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringLinks_Banners_BannerId",
                table: "GatheringLinks",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id");
        }
    }
}
