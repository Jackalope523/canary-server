using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.AzureMigrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
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
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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
