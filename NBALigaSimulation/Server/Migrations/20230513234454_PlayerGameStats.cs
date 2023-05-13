using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class PlayerGameStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerGameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Gs = table.Column<int>(type: "INTEGER", nullable: false),
                    Min = table.Column<double>(type: "REAL", nullable: false),
                    Fg = table.Column<int>(type: "INTEGER", nullable: false),
                    Fga = table.Column<int>(type: "INTEGER", nullable: false),
                    FgAtRim = table.Column<int>(type: "INTEGER", nullable: false),
                    FgaAtRim = table.Column<int>(type: "INTEGER", nullable: false),
                    FgLowPost = table.Column<int>(type: "INTEGER", nullable: false),
                    FgaLowPost = table.Column<int>(type: "INTEGER", nullable: false),
                    FgMidRange = table.Column<int>(type: "INTEGER", nullable: false),
                    FgaMidRange = table.Column<int>(type: "INTEGER", nullable: false),
                    Tp = table.Column<int>(type: "INTEGER", nullable: false),
                    Tpa = table.Column<int>(type: "INTEGER", nullable: false),
                    Ft = table.Column<int>(type: "INTEGER", nullable: false),
                    Fta = table.Column<int>(type: "INTEGER", nullable: false),
                    Orb = table.Column<int>(type: "INTEGER", nullable: false),
                    Drb = table.Column<int>(type: "INTEGER", nullable: false),
                    Ast = table.Column<int>(type: "INTEGER", nullable: false),
                    Tov = table.Column<int>(type: "INTEGER", nullable: false),
                    Stl = table.Column<int>(type: "INTEGER", nullable: false),
                    Blk = table.Column<int>(type: "INTEGER", nullable: false),
                    Pf = table.Column<int>(type: "INTEGER", nullable: false),
                    Pts = table.Column<int>(type: "INTEGER", nullable: false),
                    Trb = table.Column<int>(type: "INTEGER", nullable: false),
                    CourtTime = table.Column<int>(type: "INTEGER", nullable: false),
                    BenchTime = table.Column<int>(type: "INTEGER", nullable: false),
                    Energy = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerGameStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameStats_PlayerId",
                table: "PlayerGameStats",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerGameStats");
        }
    }
}
