using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.AzureMigrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Telegrams");

            migrationBuilder.AddColumn<int>(
                name: "Message",
                table: "Telegrams",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Telegrams");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Telegrams",
                type: "nvarchar(5000)",
                nullable: true);
        }
    }
}
