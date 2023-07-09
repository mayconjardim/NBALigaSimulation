using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class Playoffs : Migration
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
                name: "Playoffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    SeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Complete = table.Column<bool>(type: "INTEGER", nullable: true),
                    TeamOneId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamTwoId = table.Column<int>(type: "INTEGER", nullable: false),
                    WinsTeamOne = table.Column<int>(type: "INTEGER", nullable: false),
                    WinsTeamTwo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playoffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playoffs_Teams_TeamOneId",
                        column: x => x.TeamOneId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Playoffs_Teams_TeamTwoId",
                        column: x => x.TeamTwoId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Playoffs_TeamOneId",
                table: "Playoffs",
                column: "TeamOneId");

            migrationBuilder.CreateIndex(
                name: "IX_Playoffs_TeamTwoId",
                table: "Playoffs",
                column: "TeamTwoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Playoffs");

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
