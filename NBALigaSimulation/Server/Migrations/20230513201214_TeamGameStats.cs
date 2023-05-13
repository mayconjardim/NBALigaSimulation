using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class TeamGameStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamGameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
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
                    Trb = table.Column<int>(type: "INTEGER", nullable: false),
                    Ast = table.Column<int>(type: "INTEGER", nullable: false),
                    Tov = table.Column<int>(type: "INTEGER", nullable: false),
                    Stl = table.Column<int>(type: "INTEGER", nullable: false),
                    Blk = table.Column<int>(type: "INTEGER", nullable: false),
                    Pf = table.Column<int>(type: "INTEGER", nullable: false),
                    Pts = table.Column<int>(type: "INTEGER", nullable: false),
                    PtsQtrs = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGameStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamGameStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamGameStats_TeamId",
                table: "TeamGameStats",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamGameStats");
        }
    }
}
