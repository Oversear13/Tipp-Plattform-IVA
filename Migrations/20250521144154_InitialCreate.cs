using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TippPlattform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "liga",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    api_liga_id = table.Column<int>(type: "int", nullable: false),
                    liga_name = table.Column<string>(type: "character varying(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("liga_pkey", x => x.id);
                    table.UniqueConstraint("AK_liga_api_liga_id", x => x.api_liga_id);
                });

            migrationBuilder.CreateTable(
                name: "sporttypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "character varying(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sporttypes_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 30, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    role = table.Column<string>(type: "character varying(255)", nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 40, nullable: false),
                    geburtstag = table.Column<DateTime>(type: "datetime", nullable: true),
                    telefon_nummer = table.Column<string>(type: "character varying(255)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mannschaft",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    api_mannschaft_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", nullable: true),
                    rang = table.Column<int>(type: "int", nullable: true),
                    liga_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mannschaft_pkey", x => x.id);
                    table.ForeignKey(
                        name: "mannschaft_liga_id_fkey",
                        column: x => x.liga_id,
                        principalTable: "liga",
                        principalColumn: "api_liga_id");
                });

            migrationBuilder.CreateTable(
                name: "tippgruppen",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "character varying(255)", nullable: true),
                    beschreibung = table.Column<string>(type: "character varying(255)", nullable: true),
                    sporttype_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    passwort = table.Column<string>(type: "character varying(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tippgruppen_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tippgruppen_sporttype_id_fkey",
                        column: x => x.sporttype_id,
                        principalTable: "sporttypes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "spiele",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    api_spiel_id = table.Column<int>(type: "int", nullable: false),
                    teamAId = table.Column<int>(type: "integer", nullable: false),
                    teamBId = table.Column<int>(type: "integer", nullable: false),
                    teama_score = table.Column<int>(type: "int", nullable: true),
                    teamb_score = table.Column<int>(type: "int", nullable: true),
                    spiel_beginn = table.Column<DateTime>(type: "datetime", nullable: true),
                    spiel_ende = table.Column<DateTime>(type: "datetime", nullable: true),
                    liga_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("spiele_pkey", x => x.id);
                    table.ForeignKey(
                        name: "spiele_liga_id_fkey",
                        column: x => x.liga_id,
                        principalTable: "liga",
                        principalColumn: "api_liga_id");
                    table.ForeignKey(
                        name: "spiele_teamAId_fkey",
                        column: x => x.teamAId,
                        principalTable: "mannschaft",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "spiele_teamBId_fkey",
                        column: x => x.teamBId,
                        principalTable: "mannschaft",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "beitritte",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    tippgruppe_id = table.Column<int>(type: "int", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("beitritte_pkey", x => x.id);
                    table.ForeignKey(
                        name: "beitritte_tippgruppe_id_fkey",
                        column: x => x.tippgruppe_id,
                        principalTable: "tippgruppen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "beitritte_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PunkteRegel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "character varying(255)", nullable: false),
                    Beschreibung = table.Column<string>(type: "character varying(1000)", nullable: true),
                    Quote1 = table.Column<int>(type: "int", nullable: false),
                    Quote2 = table.Column<int>(type: "int", nullable: false),
                    Quote3 = table.Column<int>(type: "int", nullable: false),
                    Quote4 = table.Column<int>(type: "int", nullable: false),
                    Tippgruppe_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunkteRegel", x => x.Id);
                    table.ForeignKey(
                        name: "tippgruppe_punkteregeln_id_fkey",
                        column: x => x.Tippgruppe_Id,
                        principalTable: "tippgruppen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tippgruppe_admin",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    tippgruppe_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tippgruppe_admin_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tippgruppe_admin_tippgruppe_id_fkey",
                        column: x => x.tippgruppe_id,
                        principalTable: "tippgruppen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tippgruppe_admin_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tippscheine",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    spiel_id = table.Column<int>(type: "int", nullable: false),
                    tippgruppe_id = table.Column<int>(type: "int", nullable: false),
                    tipp_a = table.Column<int>(type: "int", nullable: false),
                    tipp_b = table.Column<int>(type: "int", nullable: false),
                    quote1 = table.Column<int>(type: "int", nullable: false),
                    quote2 = table.Column<int>(type: "int", nullable: false),
                    quote3 = table.Column<int>(type: "int", nullable: false),
                    quote4 = table.Column<int>(type: "int", nullable: false),
                    PaidOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Points = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tippscheine_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tippscheine_spiel_id_fkey",
                        column: x => x.spiel_id,
                        principalTable: "spiele",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tippscheine_tippgruppe_id_fkey",
                        column: x => x.tippgruppe_id,
                        principalTable: "tippgruppen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tippscheine_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spiele_in_tippgruppe",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    spiel_id = table.Column<int>(type: "int", nullable: false),
                    tippgruppe_id = table.Column<int>(type: "int", nullable: false),
                    punkteregel_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("spiele_in_tippgruppe_pkey", x => x.id);
                    table.ForeignKey(
                        name: "spiele_in_tippgruppe_punkteregel_id_fkey",
                        column: x => x.punkteregel_id,
                        principalTable: "PunkteRegel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "spiele_in_tippgruppe_spiel_id_fkey",
                        column: x => x.spiel_id,
                        principalTable: "spiele",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "spiele_in_tippgruppe_tippgruppe_id_fkey",
                        column: x => x.tippgruppe_id,
                        principalTable: "tippgruppen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_beitritte_tippgruppe_id",
                table: "beitritte",
                column: "tippgruppe_id");

            migrationBuilder.CreateIndex(
                name: "IX_beitritte_user_id",
                table: "beitritte",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mannschaft_liga_id",
                table: "mannschaft",
                column: "liga_id");

            migrationBuilder.CreateIndex(
                name: "IX_PunkteRegel_Tippgruppe_Id",
                table: "PunkteRegel",
                column: "Tippgruppe_Id");

            migrationBuilder.CreateIndex(
                name: "IX_spiele_liga_id",
                table: "spiele",
                column: "liga_id");

            migrationBuilder.CreateIndex(
                name: "IX_spiele_teamAId",
                table: "spiele",
                column: "teamAId");

            migrationBuilder.CreateIndex(
                name: "IX_spiele_teamBId",
                table: "spiele",
                column: "teamBId");

            migrationBuilder.CreateIndex(
                name: "IX_spiele_in_tippgruppe_punkteregel_id",
                table: "spiele_in_tippgruppe",
                column: "punkteregel_id");

            migrationBuilder.CreateIndex(
                name: "IX_spiele_in_tippgruppe_spiel_id",
                table: "spiele_in_tippgruppe",
                column: "spiel_id");

            migrationBuilder.CreateIndex(
                name: "IX_spiele_in_tippgruppe_tippgruppe_id",
                table: "spiele_in_tippgruppe",
                column: "tippgruppe_id");

            migrationBuilder.CreateIndex(
                name: "IX_tippgruppe_admin_tippgruppe_id",
                table: "tippgruppe_admin",
                column: "tippgruppe_id");

            migrationBuilder.CreateIndex(
                name: "IX_tippgruppe_admin_user_id",
                table: "tippgruppe_admin",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tippgruppen_sporttype_id",
                table: "tippgruppen",
                column: "sporttype_id");

            migrationBuilder.CreateIndex(
                name: "IX_tippscheine_spiel_id",
                table: "tippscheine",
                column: "spiel_id");

            migrationBuilder.CreateIndex(
                name: "IX_tippscheine_tippgruppe_id",
                table: "tippscheine",
                column: "tippgruppe_id");

            migrationBuilder.CreateIndex(
                name: "IX_tippscheine_user_id",
                table: "tippscheine",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "beitritte");

            migrationBuilder.DropTable(
                name: "spiele_in_tippgruppe");

            migrationBuilder.DropTable(
                name: "tippgruppe_admin");

            migrationBuilder.DropTable(
                name: "tippscheine");

            migrationBuilder.DropTable(
                name: "PunkteRegel");

            migrationBuilder.DropTable(
                name: "spiele");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "tippgruppen");

            migrationBuilder.DropTable(
                name: "mannschaft");

            migrationBuilder.DropTable(
                name: "sporttypes");

            migrationBuilder.DropTable(
                name: "liga");
        }
    }
}
