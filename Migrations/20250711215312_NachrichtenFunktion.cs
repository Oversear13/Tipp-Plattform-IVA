using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TippPlattform.Migrations
{
    /// <inheritdoc />
    public partial class NachrichtenFunktion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nachrichten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nachrichtentext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    EmpfaengerId = table.Column<int>(type: "int", nullable: false),
                    SendDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GelesenDatum = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nachrichten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nachrichten_users_EmpfaengerId",
                        column: x => x.EmpfaengerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nachrichten_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nachrichten_EmpfaengerId",
                table: "Nachrichten",
                column: "EmpfaengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Nachrichten_SenderId",
                table: "Nachrichten",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nachrichten");
        }
    }
}
