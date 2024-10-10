using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.ProductionMigrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerLinks_Banners_BannerId",
                table: "BannerLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringLinks_Gatherings_GatheringId",
                table: "GatheringLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringLinks_Users_UserId",
                table: "GatheringLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringReports_Gatherings_GatheringId",
                table: "GatheringReports");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringReports_Users_UserId",
                table: "GatheringReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Gatherings_Users_HostId",
                table: "Gatherings");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestClearances_Gatherings_GatheringId",
                table: "GuestClearances");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestClearances_Users_UserId",
                table: "GuestClearances");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_Users_PenalizedId",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Users_UserId",
                table: "SnapshotLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotReports_Users_UserId",
                table: "SnapshotReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Gatherings_GatheringId",
                table: "Snapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Users_OwnerId",
                table: "Snapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_UserId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Telegrams_Users_UserId",
                table: "Telegrams");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLinks_Users_OtherId",
                table: "UserLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLinks_Users_SelfId",
                table: "UserLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_OtherId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_SelfId",
                table: "UserReports");

            migrationBuilder.DropIndex(
                name: "IX_Telegrams_UserId",
                table: "Telegrams");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Telegrams");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "Subscriptions");

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedback_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Telegrams_NotifierId",
                table: "Telegrams",
                column: "NotifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Telegrams_RecipientId",
                table: "Telegrams",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserId",
                table: "Feedback",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLinks_Banners_BannerId",
                table: "BannerLinks",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringLinks_Gatherings_GatheringId",
                table: "GatheringLinks",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringLinks_Users_UserId",
                table: "GatheringLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringReports_Gatherings_GatheringId",
                table: "GatheringReports",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringReports_Users_UserId",
                table: "GatheringReports",
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
                name: "FK_GuestClearances_Gatherings_GatheringId",
                table: "GuestClearances",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestClearances_Users_UserId",
                table: "GuestClearances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_Users_PenalizedId",
                table: "Penalties",
                column: "PenalizedId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks",
                column: "SnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotLinks_Users_UserId",
                table: "SnapshotLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotReports_Users_UserId",
                table: "SnapshotReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Gatherings_GatheringId",
                table: "Snapshots",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Users_OwnerId",
                table: "Snapshots",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_UserId",
                table: "Subscriptions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Telegrams_Users_NotifierId",
                table: "Telegrams",
                column: "NotifierId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Telegrams_Users_RecipientId",
                table: "Telegrams",
                column: "RecipientId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLinks_Users_OtherId",
                table: "UserLinks",
                column: "OtherId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLinks_Users_SelfId",
                table: "UserLinks",
                column: "SelfId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_Users_OtherId",
                table: "UserReports",
                column: "OtherId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_Users_SelfId",
                table: "UserReports",
                column: "SelfId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerLinks_Banners_BannerId",
                table: "BannerLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringLinks_Gatherings_GatheringId",
                table: "GatheringLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringLinks_Users_UserId",
                table: "GatheringLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringReports_Gatherings_GatheringId",
                table: "GatheringReports");

            migrationBuilder.DropForeignKey(
                name: "FK_GatheringReports_Users_UserId",
                table: "GatheringReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Gatherings_Users_HostId",
                table: "Gatherings");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestClearances_Gatherings_GatheringId",
                table: "GuestClearances");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestClearances_Users_UserId",
                table: "GuestClearances");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_Users_PenalizedId",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Snapshots_SnapshotId",
                table: "SnapshotLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotLinks_Users_UserId",
                table: "SnapshotLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_SnapshotReports_Users_UserId",
                table: "SnapshotReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Gatherings_GatheringId",
                table: "Snapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Users_OwnerId",
                table: "Snapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_UserId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Telegrams_Users_NotifierId",
                table: "Telegrams");

            migrationBuilder.DropForeignKey(
                name: "FK_Telegrams_Users_RecipientId",
                table: "Telegrams");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLinks_Users_OtherId",
                table: "UserLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLinks_Users_SelfId",
                table: "UserLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_OtherId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_SelfId",
                table: "UserReports");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Telegrams_NotifierId",
                table: "Telegrams");

            migrationBuilder.DropIndex(
                name: "IX_Telegrams_RecipientId",
                table: "Telegrams");

            migrationBuilder.AddColumn<decimal>(
                name: "UserId",
                table: "Telegrams",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceType",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Telegrams_UserId",
                table: "Telegrams",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLinks_Banners_BannerId",
                table: "BannerLinks",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLinks_Users_UserId",
                table: "BannerLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringLinks_Gatherings_GatheringId",
                table: "GatheringLinks",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringLinks_Users_UserId",
                table: "GatheringLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringReports_Gatherings_GatheringId",
                table: "GatheringReports",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GatheringReports_Users_UserId",
                table: "GatheringReports",
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
                name: "FK_GuestClearances_Gatherings_GatheringId",
                table: "GuestClearances",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestClearances_Users_UserId",
                table: "GuestClearances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_Users_PenalizedId",
                table: "Penalties",
                column: "PenalizedId",
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
                name: "FK_SnapshotLinks_Users_UserId",
                table: "SnapshotLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SnapshotReports_Users_UserId",
                table: "SnapshotReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Gatherings_GatheringId",
                table: "Snapshots",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Users_OwnerId",
                table: "Snapshots",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_UserId",
                table: "Subscriptions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Telegrams_Users_UserId",
                table: "Telegrams",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLinks_Users_OtherId",
                table: "UserLinks",
                column: "OtherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLinks_Users_SelfId",
                table: "UserLinks",
                column: "SelfId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_Users_OtherId",
                table: "UserReports",
                column: "OtherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_Users_SelfId",
                table: "UserReports",
                column: "SelfId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
