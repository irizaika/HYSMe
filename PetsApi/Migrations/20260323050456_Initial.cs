using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Breed = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    DateLost = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LastSeenAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_Latitude_Longitude",
                table: "Pets",
                columns: new[] { "Latitude", "Longitude" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pets");
        }
    }
}
