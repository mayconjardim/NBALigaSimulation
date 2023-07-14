namespace NBALigaSimulation.Client.Pages.Playoffs
{
	partial class PlayoffsLeaders
	{


		private Dictionary<string, List<PlayerPlayoffsStatsDto>> categoryLists = new Dictionary<string, List<PlayerPlayoffsStatsDto>>();
		private string message = string.Empty;

		protected override async Task OnInitializedAsync()
		{
			var result = await StatsService.GetAllPlayerPlayoffsStats();

			if (!result.Success)
			{
				message = result.Message;
			}
			else
			{
				var sortedStats = result.Data.ToList();
				double pts, reb, ast, blk, stl, min, fgPct, tpPct, ftPct;

				categoryLists["Pts"] = sortedStats.OrderByDescending(t => double.TryParse(t.PtsPG, out pts) ? pts : 0).Take(10).ToList();
				categoryLists["Reb"] = sortedStats.OrderByDescending(t => double.TryParse(t.TRebPG, out reb) ? reb : 0).Take(10).ToList();
				categoryLists["Ast"] = sortedStats.OrderByDescending(t => double.TryParse(t.AstPG, out ast) ? ast : 0).Take(10).ToList();
				categoryLists["Blk"] = sortedStats.OrderByDescending(t => double.TryParse(t.BlkPG, out blk) ? blk : 0).Take(10).ToList();
				categoryLists["Stl"] = sortedStats.OrderByDescending(t => double.TryParse(t.StlPG, out stl) ? stl : 0).Take(10).ToList();
				categoryLists["Min"] = sortedStats.OrderByDescending(t => double.TryParse(t.MinPG, out min) ? min : 0).Take(10).ToList();
				categoryLists["FGPct"] = sortedStats.OrderByDescending(t => double.TryParse(t.FgPct, out fgPct) ? fgPct : 0).Take(10).ToList();
				categoryLists["TPPct"] = sortedStats.Where(t => t.Tpa > 5).OrderByDescending(t => double.TryParse(t.TpPct, out tpPct) ? tpPct : 0).Take(10).ToList();
				categoryLists["FTPct"] = sortedStats.Where(t => t.Fta > 5).OrderByDescending(t => double.TryParse(t.FtPct, out ftPct) ? ftPct : 0).Take(10).ToList();
			}
		}

	}

}
