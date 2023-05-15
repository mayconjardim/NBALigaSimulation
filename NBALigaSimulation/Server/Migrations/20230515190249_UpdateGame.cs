using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePlayByPlay");

            migrationBuilder.DropColumn(
                name: "AwayOT",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "AwayQ1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "AwayQ2",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "AwayQ3",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "AwayQ4",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeOT",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeQ1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeQ2",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeQ3",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeQ4",
                table: "Games");

            migrationBuilder.AlterColumn<double>(
                name: "CourtTime",
                table: "PlayerGameStats",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<double>(
                name: "BenchTime",
                table: "PlayerGameStats",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CourtTime",
                table: "PlayerGameStats",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "BenchTime",
                table: "PlayerGameStats",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "AwayOT",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayQ1",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayQ2",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayQ3",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayQ4",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeOT",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeQ1",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeQ2",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeQ3",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeQ4",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
