using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TippPlattform.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "telefon_nummer",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "telefon_nummer",
                table: "users",
                type: "character varying(255)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}
