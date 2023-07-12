using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class TeamAndPlayerPlayoffsStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "PlayerPlayoffsStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamAbrv = table.Column<string>(type: "TEXT", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Games = table.Column<int>(type: "INTEGER", nullable: false),
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
                    Trb = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPlayoffsStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPlayoffsStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamPlayoffsStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    ConfRank = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayoffWins = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayoffLosses = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Steals = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedStealS = table.Column<int>(type: "INTEGER", nullable: false),
                    Rebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedRebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    Assists = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedAssists = table.Column<int>(type: "INTEGER", nullable: false),
                    Blocks = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedBlocks = table.Column<int>(type: "INTEGER", nullable: false),
                    Turnovers = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedTurnovers = table.Column<int>(type: "INTEGER", nullable: false),
                    FGA = table.Column<int>(type: "INTEGER", nullable: false),
                    FGM = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedFGA = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedFGM = table.Column<int>(type: "INTEGER", nullable: false),
                    TPA = table.Column<int>(type: "INTEGER", nullable: false),
                    TPM = table.Column<int>(type: "INTEGER", nullable: false),
                    Allowed3PA = table.Column<int>(type: "INTEGER", nullable: false),
                    Allowed3PM = table.Column<int>(type: "INTEGER", nullable: false),
                    FTM = table.Column<int>(type: "INTEGER", nullable: false),
                    FTA = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedFTM = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedFTA = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayoffsStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPlayoffsStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPlayoffsStats_PlayerId",
                table: "PlayerPlayoffsStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayoffsStats_TeamId",
                table: "TeamPlayoffsStats",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerPlayoffsStats");

            migrationBuilder.DropTable(
                name: "TeamPlayoffsStats");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
