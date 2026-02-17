using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Shared.Components;

public partial class Leaderboards
{
    private List<PlayerRegularStatsDto> _topScorers;
    private List<PlayerRegularStatsDto> _topRebounds;
    private List<PlayerRegularStatsDto> _topAssists;
    private int _season;
    private string _message = string.Empty;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        _topScorers = new List<PlayerRegularStatsDto>();
        _topRebounds = new List<PlayerRegularStatsDto>();
        _topAssists = new List<PlayerRegularStatsDto>();

        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        if (string.IsNullOrEmpty(seasonStr) || !int.TryParse(seasonStr, out _season))
        {
            _message = "Selecione uma temporada.";
            _loaded = true;
            return;
        }

        const int topCount = 5;
        const bool descending = true; // isAscending=true no servidor = OrderByDescending (maior primeiro)

        var ptsTask = StatsService.GetAllPlayerRegularStats(1, topCount, _season, descending, null, null);
        var rebTask = StatsService.GetAllPlayerRegularStats(1, topCount, _season, descending, null, "RPG");
        var astTask = StatsService.GetAllPlayerRegularStats(1, topCount, _season, descending, null, "APG");

        await Task.WhenAll(ptsTask, rebTask, astTask);

        if (ptsTask.Result.Success && ptsTask.Result.Data?.Response != null)
            _topScorers = ptsTask.Result.Data.Response;
        if (rebTask.Result.Success && rebTask.Result.Data?.Response != null)
            _topRebounds = rebTask.Result.Data.Response;
        if (astTask.Result.Success && astTask.Result.Data?.Response != null)
            _topAssists = astTask.Result.Data.Response;

        _loaded = true;
    }
}
