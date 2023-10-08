using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class OnOffPlayByPlay : Migration
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
                name: "Off",
                table: "PlayByPlays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "On",
                table: "PlayByPlays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Off",
                table: "PlayByPlays");

            migrationBuilder.DropColumn(
                name: "On",
                table: "PlayByPlays");

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
