using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Repository.Migrations.AzureMigrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gatherings",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    HostId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Location = table.Column<Point>(type: "geography", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    GroupMinimum = table.Column<int>(type: "int", nullable: false),
                    GroupMaximum = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Radius = table.Column<double>(type: "float", nullable: false),
                    IsDynamic = table.Column<bool>(type: "bit", nullable: false),
                    IsPendingDeletion = table.Column<bool>(type: "bit", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    Extroversion = table.Column<int>(type: "int", nullable: false),
                    Athleticisme = table.Column<int>(type: "int", nullable: false),
                    Openness = table.Column<int>(type: "int", nullable: false),
                    Chaos = table.Column<int>(type: "int", nullable: false),
                    Competitiveness = table.Column<int>(type: "int", nullable: false),
                    Industriousness = table.Column<int>(type: "int", nullable: false),
                    NightOwl = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gatherings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalisedEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    JoinDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Reputation = table.Column<int>(type: "int", nullable: false),
                    IsPhoneConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LockoutDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AccessTries = table.Column<int>(type: "int", nullable: false),
                    AccountStatus = table.Column<int>(type: "int", nullable: false),
                    CurrentGathering = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    IsPendingDeletion = table.Column<bool>(type: "bit", nullable: false),
                    Extroversion = table.Column<int>(type: "int", nullable: false),
                    Athleticisme = table.Column<int>(type: "int", nullable: false),
                    Openness = table.Column<int>(type: "int", nullable: false),
                    Chaos = table.Column<int>(type: "int", nullable: false),
                    Competitiveness = table.Column<int>(type: "int", nullable: false),
                    Industriousness = table.Column<int>(type: "int", nullable: false),
                    NightOwl = table.Column<int>(type: "int", nullable: false),
                    Haunt = table.Column<Point>(type: "geography", nullable: false),
                    HauntRadius = table.Column<double>(type: "float", nullable: false),
                    HauntWheight = table.Column<int>(type: "int", nullable: false),
                    CurrentLocation = table.Column<Point>(type: "geography", nullable: false),
                    CurrentRadius = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GatheringLinks",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    GatheringId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatheringLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GatheringLinks_Gatherings_GatheringId",
                        column: x => x.GatheringId,
                        principalTable: "Gatherings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_GatheringLinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "GatheringReports",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    GatheringId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FilingDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatheringReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GatheringReports_Gatherings_GatheringId",
                        column: x => x.GatheringId,
                        principalTable: "Gatherings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_GatheringReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotifierId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RecipientId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Read = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Penalties",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PenalizedId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalties_Users_PenalizedId",
                        column: x => x.PenalizedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    GatheringId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PhotoURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Gatherings_GatheringId",
                        column: x => x.GatheringId,
                        principalTable: "Gatherings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Posts_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "UserLinks",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelfId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    OtherId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLinks_Users_OtherId",
                        column: x => x.OtherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UserLinks_Users_SelfId",
                        column: x => x.SelfId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "UserReports",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SelfId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    OtherId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    GatheringId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    FilingDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReports_Gatherings_GatheringId",
                        column: x => x.GatheringId,
                        principalTable: "Gatherings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserReports_Users_OtherId",
                        column: x => x.OtherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UserReports_Users_SelfId",
                        column: x => x.SelfId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PostLinks",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    PostId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostLinks_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PostLinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GatheringLinks_GatheringId",
                table: "GatheringLinks",
                column: "GatheringId");

            migrationBuilder.CreateIndex(
                name: "IX_GatheringLinks_UserId",
                table: "GatheringLinks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GatheringReports_GatheringId",
                table: "GatheringReports",
                column: "GatheringId");

            migrationBuilder.CreateIndex(
                name: "IX_GatheringReports_UserId",
                table: "GatheringReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_PenalizedId",
                table: "Penalties",
                column: "PenalizedId");

            migrationBuilder.CreateIndex(
                name: "IX_PostLinks_PostId",
                table: "PostLinks",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostLinks_UserId_PostId",
                table: "PostLinks",
                columns: new[] { "UserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_GatheringId",
                table: "Posts",
                column: "GatheringId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_OwnerId",
                table: "Posts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinks_OtherId",
                table: "UserLinks",
                column: "OtherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinks_SelfId",
                table: "UserLinks",
                column: "SelfId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReports_GatheringId",
                table: "UserReports",
                column: "GatheringId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReports_OtherId",
                table: "UserReports",
                column: "OtherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReports_SelfId",
                table: "UserReports",
                column: "SelfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GatheringLinks");

            migrationBuilder.DropTable(
                name: "GatheringReports");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Penalties");

            migrationBuilder.DropTable(
                name: "PostLinks");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UserLinks");

            migrationBuilder.DropTable(
                name: "UserReports");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Gatherings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
