using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class addPlayByPly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayByPlays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSimId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    T = table.Column<double>(type: "REAL", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayByPlays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayByPlays_Games_GameSimId",
                        column: x => x.GameSimId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PtsQtrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamGameStatsId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuarterNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtsQtrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PtsQtrs_TeamGameStats_TeamGameStatsId",
                        column: x => x.TeamGameStatsId,
                        principalTable: "TeamGameStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayByPlays_GameSimId",
                table: "PlayByPlays",
                column: "GameSimId");

            migrationBuilder.CreateIndex(
                name: "IX_PtsQtrs_TeamGameStatsId",
                table: "PtsQtrs",
                column: "TeamGameStatsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayByPlays");

            migrationBuilder.DropTable(
                name: "PtsQtrs");
        }
    }
}
