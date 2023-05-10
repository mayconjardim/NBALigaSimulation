using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class Game : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tp",
                table: "Ratings",
                newName: "Tp");

            migrationBuilder.RenameColumn(
                name: "str",
                table: "Ratings",
                newName: "Str");

            migrationBuilder.RenameColumn(
                name: "stl",
                table: "Ratings",
                newName: "Stl");

            migrationBuilder.RenameColumn(
                name: "spd",
                table: "Ratings",
                newName: "Spd");

            migrationBuilder.RenameColumn(
                name: "reb",
                table: "Ratings",
                newName: "Reb");

            migrationBuilder.RenameColumn(
                name: "pss",
                table: "Ratings",
                newName: "Pss");

            migrationBuilder.RenameColumn(
                name: "jmp",
                table: "Ratings",
                newName: "Jmp");

            migrationBuilder.RenameColumn(
                name: "ins",
                table: "Ratings",
                newName: "Ins");

            migrationBuilder.RenameColumn(
                name: "hgt",
                table: "Ratings",
                newName: "Hgt");

            migrationBuilder.RenameColumn(
                name: "ft",
                table: "Ratings",
                newName: "Ft");

            migrationBuilder.RenameColumn(
                name: "fg",
                table: "Ratings",
                newName: "Fg");

            migrationBuilder.RenameColumn(
                name: "end",
                table: "Ratings",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "drb",
                table: "Ratings",
                newName: "Drb");

            migrationBuilder.RenameColumn(
                name: "dnk",
                table: "Ratings",
                newName: "Dnk");

            migrationBuilder.RenameColumn(
                name: "blk",
                table: "Ratings",
                newName: "Blk");

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HomeTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeQ1 = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeQ2 = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeQ3 = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeQ4 = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeOT = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayQ1 = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayQ2 = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayQ3 = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayQ4 = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayOT = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GamePlayByPlay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Play = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayByPlay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePlayByPlay_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayByPlay_GameId",
                table: "GamePlayByPlay",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayTeamId",
                table: "Games",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomeTeamId",
                table: "Games",
                column: "HomeTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePlayByPlay");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.RenameColumn(
                name: "Tp",
                table: "Ratings",
                newName: "tp");

            migrationBuilder.RenameColumn(
                name: "Str",
                table: "Ratings",
                newName: "str");

            migrationBuilder.RenameColumn(
                name: "Stl",
                table: "Ratings",
                newName: "stl");

            migrationBuilder.RenameColumn(
                name: "Spd",
                table: "Ratings",
                newName: "spd");

            migrationBuilder.RenameColumn(
                name: "Reb",
                table: "Ratings",
                newName: "reb");

            migrationBuilder.RenameColumn(
                name: "Pss",
                table: "Ratings",
                newName: "pss");

            migrationBuilder.RenameColumn(
                name: "Jmp",
                table: "Ratings",
                newName: "jmp");

            migrationBuilder.RenameColumn(
                name: "Ins",
                table: "Ratings",
                newName: "ins");

            migrationBuilder.RenameColumn(
                name: "Hgt",
                table: "Ratings",
                newName: "hgt");

            migrationBuilder.RenameColumn(
                name: "Ft",
                table: "Ratings",
                newName: "ft");

            migrationBuilder.RenameColumn(
                name: "Fg",
                table: "Ratings",
                newName: "fg");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Ratings",
                newName: "end");

            migrationBuilder.RenameColumn(
                name: "Drb",
                table: "Ratings",
                newName: "drb");

            migrationBuilder.RenameColumn(
                name: "Dnk",
                table: "Ratings",
                newName: "dnk");

            migrationBuilder.RenameColumn(
                name: "Blk",
                table: "Ratings",
                newName: "blk");
        }
    }
}
