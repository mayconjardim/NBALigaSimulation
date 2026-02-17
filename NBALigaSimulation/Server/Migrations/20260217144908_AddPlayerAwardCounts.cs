using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerAwardCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerAwardCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerOfTheGame = table.Column<int>(type: "INTEGER", nullable: false),
                    MVP = table.Column<int>(type: "INTEGER", nullable: false),
                    DPOY = table.Column<int>(type: "INTEGER", nullable: false),
                    ROY = table.Column<int>(type: "INTEGER", nullable: false),
                    SixthManOfTheYear = table.Column<int>(type: "INTEGER", nullable: false),
                    MIP = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerOfTheMonth = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerOfTheWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    AllStarGames = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesWon = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAwardCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerAwardCounts_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAwardCounts_PlayerId",
                table: "PlayerAwardCounts",
                column: "PlayerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerAwardCounts");
        }
    }
}
