using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.TestMigrations
{
    /// <inheritdoc />
    public partial class SnapshotReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Snapshots_PostId",
                table: "SnapshotLinks");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "SnapshotLinks",
                newName: "SnapshotId");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotLinks_UserId_PostId",
                table: "SnapshotLinks",
                newName: "IX_SnapshotLinks_UserId_SnapshotId");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotLinks_PostId",
                table: "SnapshotLinks",
                newName: "IX_SnapshotLinks_SnapshotId");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Banners",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SnapshotReports",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SnapshotId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    FilingDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapshotReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapshotReports_Snapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "Snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SnapshotReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SnapshotReports_SnapshotId",
                table: "SnapshotReports",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapshotReports_UserId",
                table: "SnapshotReports",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks",
                column: "SnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks");

            migrationBuilder.DropTable(
                name: "SnapshotReports");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Banners");

            migrationBuilder.RenameColumn(
                name: "SnapshotId",
                table: "SnapshotLinks",
                newName: "PostId");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotLinks_UserId_SnapshotId",
                table: "SnapshotLinks",
                newName: "IX_SnapshotLinks_UserId_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_SnapshotLinks_SnapshotId",
                table: "SnapshotLinks",
                newName: "IX_SnapshotLinks_PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotLinks_Snapshots_PostId",
                table: "SnapshotLinks",
                column: "PostId",
                principalTable: "Snapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
