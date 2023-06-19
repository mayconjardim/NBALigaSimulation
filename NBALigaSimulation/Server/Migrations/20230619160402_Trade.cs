using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class Trade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayByPlays_Games_GameSimId",
                table: "PlayByPlays");

            migrationBuilder.DropIndex(
                name: "IX_PlayByPlays_GameSimId",
                table: "PlayByPlays");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "PlayByPlays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETDATE()"),
                    TeamOneId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamTwoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_Teams_TeamOneId",
                        column: x => x.TeamOneId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Teams_TeamTwoId",
                        column: x => x.TeamTwoId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradePlayer",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TradePlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePlayer", x => new { x.PlayerId, x.TradePlayerId });
                    table.ForeignKey(
                        name: "FK_TradePlayer_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradePlayer_Trades_TradePlayerId",
                        column: x => x.TradePlayerId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayByPlays_GameId",
                table: "PlayByPlays",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_TradePlayer_TradePlayerId",
                table: "TradePlayer",
                column: "TradePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TeamOneId",
                table: "Trades",
                column: "TeamOneId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TeamTwoId",
                table: "Trades",
                column: "TeamTwoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayByPlays_Games_GameId",
                table: "PlayByPlays",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayByPlays_Games_GameId",
                table: "PlayByPlays");

            migrationBuilder.DropTable(
                name: "TradePlayer");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_PlayByPlays_GameId",
                table: "PlayByPlays");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "PlayByPlays");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayByPlays_GameSimId",
                table: "PlayByPlays",
                column: "GameSimId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayByPlays_Games_GameSimId",
                table: "PlayByPlays",
                column: "GameSimId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
