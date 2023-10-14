using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class removeFuzzRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fuzz",
                table: "PlayerRatings");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Fuzz",
                table: "PlayerRatings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
