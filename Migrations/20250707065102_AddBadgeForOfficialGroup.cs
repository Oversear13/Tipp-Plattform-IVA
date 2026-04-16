using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TippPlattform.Migrations
{
    /// <inheritdoc />
    public partial class AddBadgeForOfficialGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Badge",
                table: "tippgruppen",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Badge",
                table: "tippgruppen");
        }
    }
}
