using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Databases.EFCore.Migrations.StagingMigrations
{
    /// <inheritdoc />
    public partial class Decay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Decay",
                table: "Gatherings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Decay",
                table: "Gatherings");
        }
    }
}
