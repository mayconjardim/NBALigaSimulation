using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class CompletedToSeason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeadlineCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DraftCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FirstRoundCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FourthRoundCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LotteryCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RegularCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SecondRoundCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TcCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThirdRoundCompleted",
                table: "Seasons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

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
            migrationBuilder.DropColumn(
                name: "DeadlineCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "DraftCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "FirstRoundCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "FourthRoundCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "LotteryCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "RegularCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "SecondRoundCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "TcCompleted",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "ThirdRoundCompleted",
                table: "Seasons");

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
