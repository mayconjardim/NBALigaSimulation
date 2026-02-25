using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class ExpandNewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "GameNews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "GameNews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "GameNews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkEntityType",
                table: "GameNews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LinkEntityId",
                table: "GameNews",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Type", table: "GameNews");
            migrationBuilder.DropColumn(name: "Summary", table: "GameNews");
            migrationBuilder.DropColumn(name: "ImageUrl", table: "GameNews");
            migrationBuilder.DropColumn(name: "LinkEntityType", table: "GameNews");
            migrationBuilder.DropColumn(name: "LinkEntityId", table: "GameNews");
        }
    }
}
