using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsApi.Migrations
{
    /// <inheritdoc />
    public partial class SightingTableChange2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeenAddress",
                table: "Sightings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeenAddress",
                table: "Sightings");
        }
    }
}
