namespace NBALigaSimulation.Client.Pages.Stats
{
    partial class TeamLeaders
    {


        private Dictionary<string, List<TeamRegularStatsDto>> categoryLists = new Dictionary<string, List<TeamRegularStatsDto>>();
        private string message = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var result = await StatsService.GetAllTeamRegularStats();

            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                var sortedStats = result.Data.ToList();
                double pts, allowedPts, reb, ast, blk, stl, to, fg, fga, fgPct, tp, tpa, tpPct, ft, fta, ftPct, off, deff;

                categoryLists["Pts"] = sortedStats.OrderByDescending(t => double.TryParse(t.PtsPG, out pts) ? pts : 0).Take(10).ToList();
                categoryLists["AllowedPts"] = sortedStats.OrderBy(t => double.TryParse(t.AllowedPtsPG, out allowedPts) ? allowedPts : 0).Take(10).ToList();
                categoryLists["Reb"] = sortedStats.OrderByDescending(t => double.TryParse(t.RebPg, out reb) ? reb : 0).Take(10).ToList();
                categoryLists["Ast"] = sortedStats.OrderByDescending(t => double.TryParse(t.AstPg, out ast) ? ast : 0).Take(10).ToList();
                categoryLists["Blk"] = sortedStats.OrderByDescending(t => double.TryParse(t.BlkPg, out blk) ? blk : 0).Take(10).ToList();
                categoryLists["Stl"] = sortedStats.OrderByDescending(t => double.TryParse(t.StlPg, out stl) ? stl : 0).Take(10).ToList();
                categoryLists["TO"] = sortedStats.OrderBy(t => double.TryParse(t.TOPg, out to) ? to : 0).Take(10).ToList();
                categoryLists["FG"] = sortedStats.OrderByDescending(t => double.TryParse(t.FgPg, out fg) ? fg : 0).Take(10).ToList();
                categoryLists["FGA"] = sortedStats.OrderByDescending(t => double.TryParse(t.FgaPg, out fga) ? fga : 0).Take(10).ToList();
                categoryLists["FGPct"] = sortedStats.OrderByDescending(t => double.TryParse(t.FgPct, out fgPct) ? fgPct : 0).Take(10).ToList();
                categoryLists["TP"] = sortedStats.OrderByDescending(t => double.TryParse(t.TPPg, out tp) ? tp : 0).Take(10).ToList();
                categoryLists["TPA"] = sortedStats.OrderByDescending(t => double.TryParse(t.TPaPg, out tpa) ? tpa : 0).Take(10).ToList();
                categoryLists["TPPct"] = sortedStats.OrderByDescending(t => double.TryParse(t.TPPct, out tpPct) ? tpPct : 0).Take(10).ToList();
                categoryLists["FT"] = sortedStats.OrderByDescending(t => double.TryParse(t.FTPg, out ft) ? ft : 0).Take(10).ToList();
                categoryLists["FTA"] = sortedStats.OrderByDescending(t => double.TryParse(t.FTaPg, out fta) ? fta : 0).Take(10).ToList();
                categoryLists["FTPct"] = sortedStats.OrderByDescending(t => double.TryParse(t.FTPct, out ftPct) ? ftPct : 0).Take(10).ToList();
                categoryLists["OFF"] = sortedStats.OrderByDescending(t => double.TryParse(t.OffensiveEfficiency, out off) ? off : 0).Take(10).ToList();
                categoryLists["DFF"] = sortedStats.OrderBy(t => double.TryParse(t.DefensiveEfficiency, out deff) ? deff : 0).Take(10).ToList();

            }
        }

    }
}
