using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class addPotRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PtsQtrs");

            migrationBuilder.AddColumn<int>(
                name: "Pot",
                table: "Ratings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "Energy",
                table: "PlayerGameStats",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pot",
                table: "Ratings");

            migrationBuilder.AlterColumn<int>(
                name: "Energy",
                table: "PlayerGameStats",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.CreateTable(
                name: "PtsQtrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamGameStatsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    QuarterNumber = table.Column<int>(type: "INTEGER", nullable: false)
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
                name: "IX_PtsQtrs_TeamGameStatsId",
                table: "PtsQtrs",
                column: "TeamGameStatsId");
        }
    }
}
