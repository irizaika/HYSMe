using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsApi.Migrations
{
    /// <inheritdoc />
    public partial class SightingTableChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Sightings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Sightings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterEmail",
                table: "Sightings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterName",
                table: "Sightings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Sightings");

            migrationBuilder.DropColumn(
                name: "ReporterEmail",
                table: "Sightings");

            migrationBuilder.DropColumn(
                name: "ReporterName",
                table: "Sightings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Sightings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
