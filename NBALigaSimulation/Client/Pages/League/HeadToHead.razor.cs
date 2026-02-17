using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.League;

public partial class HeadToHead
{
    private List<TeamSimpleDto> _teams = new();
    private int? _teamAId;
    private int? _teamBId;
    private List<GameCompleteDto> _games = new();
    private string _teamAAbrv = "";
    private string _teamBAbrv = "";
    private int _winsA;
    private int _winsB;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var result = await TeamService.GetAllTeams();
        if (result.Success && result.Data != null)
            _teams = result.Data.OrderBy(t => t.Name).ToList();
    }

    private async Task LoadHeadToHead()
    {
        _games.Clear();
        _winsA = 0;
        _winsB = 0;
        _loaded = false;
        if (!_teamAId.HasValue || !_teamBId.HasValue || _teamAId == _teamBId)
            return;

        var teamA = _teams.FirstOrDefault(t => t.Id == _teamAId);
        var teamB = _teams.FirstOrDefault(t => t.Id == _teamBId);
        if (teamA == null || teamB == null) return;

        _teamAAbrv = teamA.Abrv;
        _teamBAbrv = teamB.Abrv;

        var result = await GameService.GetGamesBetweenTeams(_teamAId.Value, _teamBId.Value);
        if (result.Success && result.Data != null)
        {
            _games = result.Data.Where(g => g.HomeTeamScore > 0 || g.AwayTeamScore > 0).ToList();
            foreach (var g in _games)
            {
                bool teamAIsHome = g.HomeTeamId == _teamAId;
                int homeScore = g.HomeTeamScore;
                int awayScore = g.AwayTeamScore;
                if (teamAIsHome)
                {
                    if (homeScore > awayScore) _winsA++;
                    else _winsB++;
                }
                else
                {
                    if (awayScore > homeScore) _winsA++;
                    else _winsB++;
                }
            }
        }
        _loaded = true;
    }

    private string GetScoreClass(GameCompleteDto game)
    {
        if (game.HomeTeamScore == 0) return "";
        bool teamAWins = (_teamAId == game.HomeTeamId && game.HomeTeamScore > game.AwayTeamScore) ||
                         (_teamAId == game.AwayTeamId && game.AwayTeamScore > game.HomeTeamScore);
        return teamAWins ? "text-win" : "text-loss";
    }

    private void GoToGame(int gameId) => NavigationManager.NavigateTo($"/game/{gameId}", forceLoad: true);
}
