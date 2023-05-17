using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class editGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PtsQtrs",
                table: "TeamGameStats");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGameStats_GameId",
                table: "TeamGameStats",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGameStats_Games_GameId",
                table: "TeamGameStats",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamGameStats_Games_GameId",
                table: "TeamGameStats");

            migrationBuilder.DropIndex(
                name: "IX_TeamGameStats_GameId",
                table: "TeamGameStats");

            migrationBuilder.AddColumn<string>(
                name: "PtsQtrs",
                table: "TeamGameStats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
