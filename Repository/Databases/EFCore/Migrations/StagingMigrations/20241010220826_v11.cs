using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.StagingMigrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Gatherings_Users_HostId",
                table: "Gatherings");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotReports_Snapshots_SnapshotId",
                table: "SnapshotReports");

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Feedback",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gatherings_Users_HostId",
                table: "Gatherings",
                column: "HostId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks",
                column: "SnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotReports_Snapshots_SnapshotId",
                table: "SnapshotReports",
                column: "SnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Gatherings_Users_HostId",
                table: "Gatherings");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotReports_Snapshots_SnapshotId",
                table: "SnapshotReports");

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Feedback",
                type: "decimal(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gatherings_Users_HostId",
                table: "Gatherings",
                column: "HostId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks",
                column: "SnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotReports_Snapshots_SnapshotId",
                table: "SnapshotReports",
                column: "SnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id");
        }
    }
}
