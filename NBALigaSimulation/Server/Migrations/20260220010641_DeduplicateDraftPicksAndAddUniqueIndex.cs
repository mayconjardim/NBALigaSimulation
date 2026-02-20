using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class DeduplicateDraftPicksAndAddUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove duplicatas: mantém um pick por (Original, Year, Round)
            migrationBuilder.Sql(@"
                CREATE TEMP TABLE IF NOT EXISTS _DedupKeep AS 
                SELECT MIN(Id) as KeepId, Original, Year, Round 
                FROM TeamDraftPicks 
                GROUP BY Original, Year, Round
            ");
            migrationBuilder.Sql(@"
                CREATE TEMP TABLE IF NOT EXISTS _DedupMap AS
                SELECT t.Id as DupId, k.KeepId 
                FROM TeamDraftPicks t
                JOIN _DedupKeep k ON t.Original = k.Original AND t.Year = k.Year AND t.Round = k.Round
                WHERE t.Id != k.KeepId
            ");
            migrationBuilder.Sql(@"
                DELETE FROM TradePicks 
                WHERE rowid IN (
                    SELECT tp.rowid FROM TradePicks tp
                    INNER JOIN _DedupMap dm ON tp.DraftPickId = dm.DupId
                    WHERE EXISTS (
                        SELECT 1 FROM TradePicks t2 
                        WHERE t2.DraftPickId = dm.KeepId AND t2.TradePickId = tp.TradePickId
                    )
                )
            ");
            migrationBuilder.Sql(@"
                UPDATE TradePicks 
                SET DraftPickId = (SELECT KeepId FROM _DedupMap WHERE DupId = TradePicks.DraftPickId)
                WHERE DraftPickId IN (SELECT DupId FROM _DedupMap)
            ");
            migrationBuilder.Sql(@"
                DELETE FROM TeamDraftPicks 
                WHERE Id IN (SELECT DupId FROM _DedupMap)
            ");

            migrationBuilder.CreateIndex(
                name: "IX_TeamDraftPicks_Original_Year_Round",
                table: "TeamDraftPicks",
                columns: new[] { "Original", "Year", "Round" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamDraftPicks_Original_Year_Round",
                table: "TeamDraftPicks");
        }
    }
}
