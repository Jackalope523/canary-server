using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations.TestDBMigrations
{
    /// <inheritdoc />
    public partial class AddReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Events_EventId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Users_OtherId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Users_SelfId",
                table: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.RenameTable(
                name: "Report",
                newName: "Reports");

            migrationBuilder.RenameIndex(
                name: "IX_Report_SelfId",
                table: "Reports",
                newName: "IX_Reports_SelfId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_OtherId",
                table: "Reports",
                newName: "IX_Reports_OtherId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_EventId",
                table: "Reports",
                newName: "IX_Reports_EventId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FilingDate",
                table: "Reports",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Events_EventId",
                table: "Reports",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_OtherId",
                table: "Reports",
                column: "OtherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_SelfId",
                table: "Reports",
                column: "SelfId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Events_EventId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_OtherId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_SelfId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "FilingDate",
                table: "Reports");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "Report");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_SelfId",
                table: "Report",
                newName: "IX_Report_SelfId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_OtherId",
                table: "Report",
                newName: "IX_Report_OtherId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_EventId",
                table: "Report",
                newName: "IX_Report_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Events_EventId",
                table: "Report",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Users_OtherId",
                table: "Report",
                column: "OtherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Users_SelfId",
                table: "Report",
                column: "SelfId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
