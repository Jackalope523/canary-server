using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.AzureMigrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserLinks_SelfId",
                table: "UserLinks");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinks_SelfId_OtherId",
                table: "UserLinks",
                columns: new[] { "SelfId", "OtherId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserLinks_SelfId_OtherId",
                table: "UserLinks");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinks_SelfId",
                table: "UserLinks",
                column: "SelfId");
        }
    }
}
