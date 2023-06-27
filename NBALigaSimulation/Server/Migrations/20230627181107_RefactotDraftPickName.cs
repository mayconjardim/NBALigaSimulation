using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class RefactotDraftPickName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DraftPicks_Teams_TeamId",
                table: "DraftPicks");

            migrationBuilder.DropForeignKey(
                name: "FK_TradePicks_DraftPicks_DraftPickId",
                table: "TradePicks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DraftPicks",
                table: "DraftPicks");

            migrationBuilder.RenameTable(
                name: "DraftPicks",
                newName: "TeamDraftPicks");

            migrationBuilder.RenameIndex(
                name: "IX_DraftPicks_TeamId",
                table: "TeamDraftPicks",
                newName: "IX_TeamDraftPicks_TeamId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamDraftPicks",
                table: "TeamDraftPicks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamDraftPicks_Teams_TeamId",
                table: "TeamDraftPicks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TradePicks_TeamDraftPicks_DraftPickId",
                table: "TradePicks",
                column: "DraftPickId",
                principalTable: "TeamDraftPicks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamDraftPicks_Teams_TeamId",
                table: "TeamDraftPicks");

            migrationBuilder.DropForeignKey(
                name: "FK_TradePicks_TeamDraftPicks_DraftPickId",
                table: "TradePicks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamDraftPicks",
                table: "TeamDraftPicks");

            migrationBuilder.RenameTable(
                name: "TeamDraftPicks",
                newName: "DraftPicks");

            migrationBuilder.RenameIndex(
                name: "IX_TeamDraftPicks_TeamId",
                table: "DraftPicks",
                newName: "IX_DraftPicks_TeamId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DraftPicks",
                table: "DraftPicks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DraftPicks_Teams_TeamId",
                table: "DraftPicks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TradePicks_DraftPicks_DraftPickId",
                table: "TradePicks",
                column: "DraftPickId",
                principalTable: "DraftPicks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
