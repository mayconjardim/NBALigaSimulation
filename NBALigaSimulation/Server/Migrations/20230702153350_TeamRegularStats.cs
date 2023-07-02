using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class TeamRegularStats : Migration
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

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TeamRegularStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    ConfRank = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeWins = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeLosses = table.Column<int>(type: "INTEGER", nullable: false),
                    RoadWins = table.Column<int>(type: "INTEGER", nullable: false),
                    RoadLosses = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_TeamRegularStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamRegularStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamRegularStats_TeamId",
                table: "TeamRegularStats",
                column: "TeamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamRegularStats");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Games");

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
