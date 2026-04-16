using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TippPlattform.Migrations
{
    /// <inheritdoc />
    public partial class AddSpieltagColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Spieltag",
                table: "spiele",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Spieltag",
                table: "spiele");
        }
    }
}
