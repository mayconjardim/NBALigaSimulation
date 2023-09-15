using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class EditPlayerRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Str",
                table: "PlayerRatings",
                newName: "Stre");

            migrationBuilder.RenameColumn(
                name: "Stl",
                table: "PlayerRatings",
                newName: "Oiq");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "PlayerRatings",
                newName: "Fuzz");

            migrationBuilder.RenameColumn(
                name: "Blk",
                table: "PlayerRatings",
                newName: "Endu");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Diq",
                table: "PlayerRatings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Pos",
                table: "PlayerRatings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diq",
                table: "PlayerRatings");

            migrationBuilder.DropColumn(
                name: "Pos",
                table: "PlayerRatings");

            migrationBuilder.RenameColumn(
                name: "Stre",
                table: "PlayerRatings",
                newName: "Str");

            migrationBuilder.RenameColumn(
                name: "Oiq",
                table: "PlayerRatings",
                newName: "Stl");

            migrationBuilder.RenameColumn(
                name: "Fuzz",
                table: "PlayerRatings",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "Endu",
                table: "PlayerRatings",
                newName: "Blk");

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
