using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.TestMigrations
{
    /// <inheritdoc />
    public partial class FixDeleteBehaviour : Migration
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
                name: "FK_Feedback_Users_UserId",
                table: "Feedback");

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
                name: "FK_UserReports_Gatherings_GatheringId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_OtherId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_SelfId",
                table: "UserReports");

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
                name: "FK_Feedback_Users_UserId",
                table: "Feedback",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Gatherings_Users_HostId",
                table: "Gatherings",
                column: "HostId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Gatherings_GatheringId",
                table: "Snapshots",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Telegrams_Users_NotifierId",
                table: "Telegrams",
                column: "NotifierId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Telegrams_Users_RecipientId",
                table: "Telegrams",
                column: "RecipientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_UserReports_Gatherings_GatheringId",
                table: "UserReports",
                column: "GatheringId",
                principalTable: "Gatherings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                onDelete: ReferentialAction.SetNull);
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
                name: "FK_Feedback_Users_UserId",
                table: "Feedback");

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
                name: "FK_UserReports_Gatherings_GatheringId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_OtherId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_Users_SelfId",
                table: "UserReports");

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
                name: "FK_Feedback_Users_UserId",
                table: "Feedback",
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
                name: "FK_UserReports_Gatherings_GatheringId",
                table: "UserReports",
                column: "GatheringId",
                principalTable: "Gatherings",
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
    }
}
