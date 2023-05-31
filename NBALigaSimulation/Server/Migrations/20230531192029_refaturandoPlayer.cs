using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class refaturandoPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Players",
                newName: "Pos");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Players",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Height",
                table: "Players",
                newName: "Hgt");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Players",
                newName: "Born_Loc");

            migrationBuilder.RenameColumn(
                name: "BornYear",
                table: "Players",
                newName: "Born_Year");

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
            migrationBuilder.RenameColumn(
                name: "Pos",
                table: "Players",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Players",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Hgt",
                table: "Players",
                newName: "Height");

            migrationBuilder.RenameColumn(
                name: "Born_Year",
                table: "Players",
                newName: "BornYear");

            migrationBuilder.RenameColumn(
                name: "Born_Loc",
                table: "Players",
                newName: "FirstName");

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
