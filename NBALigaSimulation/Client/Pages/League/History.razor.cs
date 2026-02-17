using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.League;
using NBALigaSimulation.Shared.Dtos.Playoffs;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Pages.League;

public partial class History
{
    private List<SeasonHistoryDto>? _list;
    private bool _loading = true;
    private string _selectedSeason = "";
    private int? _selectedSeasonInt => string.IsNullOrWhiteSpace(_selectedSeason) || !int.TryParse(_selectedSeason, out int s) ? null : s;
    private List<TeamRegularStatsDto>? _standings;
    private bool _standingsLoading;

    private string _activeTab = "standings";
    private List<PlayoffsDto>? _playoffs;
    private bool _playoffsLoading;
    private PageableResponse<PlayerRegularStatsDto>? _regularStatsResponse;
    private bool _playerRegularLoading;
    private List<PlayerPlayoffsStatsDto>? _playerPlayoffsStats;
    private bool _playerPlayoffsLoading;

    private static readonly Dictionary<int, string> RoundsOrder = new()
    {
        { 1, "1ª Rodada" },
        { 2, "Semi-finais de Conferência" },
        { 3, "Finais de Conferência" },
        { 4, "Finais da NBA" }
    };

    private int _currentPageRegular = 1;
    private int _pageSizeRegular = 25;
    private string _sortedColumnRegular = "PPG";
    private bool _isAscendingRegular = false;
    private string _positionRegular = "";

    private int _currentPagePlayoffs = 1;
    private int _pageSizePlayoffs = 25;
    private string _sortedColumnPlayoffs = "PPG";
    private bool _isAscendingPlayoffs = false;

    private static readonly List<string> Positions = new() { "C", "FC", "PF", "F", "SF", "GF", "G", "SG", "PG" };
    private static readonly List<int> PageSizeOptions = new() { 10, 25, 50 };

    protected override async Task OnInitializedAsync()
    {
        var result = await LeagueService.GetSeasonHistory();
        if (result.Success && result.Data != null)
            _list = result.Data;
        else
            _list = new List<SeasonHistoryDto>();
        _loading = false;
    }

    private async Task OnSeasonSelected()
    {
        _standings = null;
        _playoffs = null;
        _regularStatsResponse = null;
        _playerPlayoffsStats = null;
        _currentPageRegular = 1;
        _currentPagePlayoffs = 1;
        if (!_selectedSeasonInt.HasValue) return;
        _standingsLoading = true;
        var result = await StatsService.GetAllTeamRegularStats(_selectedSeasonInt.Value, false, null);
        _standingsLoading = false;
        if (result.Success && result.Data != null)
            _standings = result.Data.OrderBy(t => t.TeamConference).ThenBy(t => t.ConfRank).ToList();
    }

    private async Task OnTabSelect(string tab)
    {
        _activeTab = tab;
        if (!_selectedSeasonInt.HasValue) return;
        if (tab == "playoffs" && _playoffs == null)
            await LoadPlayoffs();
        else if (tab == "regular" && _regularStatsResponse == null)
            await LoadPlayerRegularStatsPage();
        else if (tab == "playoffs-stats" && _playerPlayoffsStats == null)
            await LoadPlayerPlayoffsStats();
    }

    private async Task LoadPlayoffs()
    {
        if (!_selectedSeasonInt.HasValue) return;
        _playoffsLoading = true;
        var result = await PlayoffsService.GetPlayoffsBySeason(_selectedSeasonInt.Value);
        _playoffsLoading = false;
        _playoffs = result.Success && result.Data != null ? result.Data : new List<PlayoffsDto>();
    }

    private async Task LoadPlayerRegularStatsPage()
    {
        if (!_selectedSeasonInt.HasValue) return;
        _playerRegularLoading = true;
        var result = await StatsService.GetAllPlayerRegularStats(
            _currentPageRegular, _pageSizeRegular, _selectedSeasonInt.Value,
            _isAscendingRegular, string.IsNullOrEmpty(_positionRegular) ? null : _positionRegular, _sortedColumnRegular);
        _playerRegularLoading = false;
        if (result.Success && result.Data != null)
            _regularStatsResponse = result.Data;
        else
            _regularStatsResponse = null;
    }

    private async Task SortRegularColumn(string column)
    {
        if (column == _sortedColumnRegular)
            _isAscendingRegular = !_isAscendingRegular;
        else
        {
            _sortedColumnRegular = column;
            _isAscendingRegular = false;
        }
        _currentPageRegular = 1;
        await LoadPlayerRegularStatsPage();
    }

    private string GetRegularSortIcon(string column)
    {
        if (column != _sortedColumnRegular) return "";
        return _isAscendingRegular ? "bi-sort-up" : "bi-sort-down";
    }

    private async Task ChangePageRegular(int page)
    {
        _currentPageRegular = page;
        await LoadPlayerRegularStatsPage();
    }

    private async Task HandleRegularPositionChange(ChangeEventArgs e)
    {
        _positionRegular = e.Value?.ToString() ?? "";
        _currentPageRegular = 1;
        await LoadPlayerRegularStatsPage();
    }

    private async Task HandleRegularPageSizeChange(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out int size))
        {
            _pageSizeRegular = size;
            _currentPageRegular = 1;
            await LoadPlayerRegularStatsPage();
        }
    }

    private async Task LoadPlayerPlayoffsStats()
    {
        if (!_selectedSeasonInt.HasValue) return;
        _playerPlayoffsLoading = true;
        var result = await StatsService.GetAllPlayerPlayoffsStatsBySeason(_selectedSeasonInt.Value);
        _playerPlayoffsLoading = false;
        _playerPlayoffsStats = result.Success && result.Data != null ? result.Data : new List<PlayerPlayoffsStatsDto>();
        _currentPagePlayoffs = 1;
    }

    private IEnumerable<PlayerPlayoffsStatsDto> GetPlayoffsStatsSorted()
    {
        if (_playerPlayoffsStats == null || !_playerPlayoffsStats.Any()) return Enumerable.Empty<PlayerPlayoffsStatsDto>();
        var sorted = _sortedColumnPlayoffs switch
        {
            "GP" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games) : _playerPlayoffsStats.OrderByDescending(p => p.Games),
            "MIN" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? p.Min / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? p.Min / p.Games : 0),
            "PPG" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Pts / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Pts / p.Games : 0),
            "APG" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Ast / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Ast / p.Games : 0),
            "RPG" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Trb / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Trb / p.Games : 0),
            "ORB" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Orb / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Orb / p.Games : 0),
            "DRB" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Drb / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Drb / p.Games : 0),
            "SPG" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Stl / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Stl / p.Games : 0),
            "BPG" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Blk / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Blk / p.Games : 0),
            "TPG" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Tov / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Tov / p.Games : 0),
            "FPG" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Games > 0 ? (double)p.Pf / p.Games : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Pf / p.Games : 0),
            "FG%" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Fga > 0 ? (double)p.Fg / p.Fga * 100 : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Fga > 0 ? (double)p.Fg / p.Fga * 100 : 0),
            "FT%" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Fta > 0 ? (double)p.Ft / p.Fta * 100 : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Fta > 0 ? (double)p.Ft / p.Fta * 100 : 0),
            "3P%" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => p.Tpa > 0 ? (double)p.Tp / p.Tpa * 100 : 0) : _playerPlayoffsStats.OrderByDescending(p => p.Tpa > 0 ? (double)p.Tp / p.Tpa * 100 : 0),
            "TS%" => _isAscendingPlayoffs ? _playerPlayoffsStats.OrderBy(p => (p.Fga + 0.44 * p.Fta) > 0 ? (double)p.Pts / (2.0 * (p.Fga + 0.44 * p.Fta)) * 100 : 0) : _playerPlayoffsStats.OrderByDescending(p => (p.Fga + 0.44 * p.Fta) > 0 ? (double)p.Pts / (2.0 * (p.Fga + 0.44 * p.Fta)) * 100 : 0),
            _ => _playerPlayoffsStats.OrderByDescending(p => p.Games > 0 ? (double)p.Pts / p.Games : 0)
        };
        return sorted;
    }

    private List<PlayerPlayoffsStatsDto> GetPlayoffsStatsPage()
    {
        var sorted = GetPlayoffsStatsSorted().ToList();
        return sorted.Skip((_currentPagePlayoffs - 1) * _pageSizePlayoffs).Take(_pageSizePlayoffs).ToList();
    }

    private int GetPlayoffsStatsTotalPages()
    {
        if (_playerPlayoffsStats == null || !_playerPlayoffsStats.Any()) return 0;
        return (int)Math.Ceiling(_playerPlayoffsStats.Count / (double)_pageSizePlayoffs);
    }

    private void SortPlayoffsColumn(string column)
    {
        if (column == _sortedColumnPlayoffs)
            _isAscendingPlayoffs = !_isAscendingPlayoffs;
        else
        {
            _sortedColumnPlayoffs = column;
            _isAscendingPlayoffs = false;
        }
        _currentPagePlayoffs = 1;
    }

    private string GetPlayoffsSortIcon(string column)
    {
        if (column != _sortedColumnPlayoffs) return "";
        return _isAscendingPlayoffs ? "bi-sort-up" : "bi-sort-down";
    }

    private void ChangePagePlayoffs(int page)
    {
        _currentPagePlayoffs = page;
        StateHasChanged();
    }

    private void HandlePlayoffsPageSizeChange(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out int size))
        {
            _pageSizePlayoffs = size;
            _currentPagePlayoffs = 1;
        }
    }

    private static Dictionary<int, List<PlayoffsDto>> GroupPlayoffsByRound(List<PlayoffsDto> playoffs)
    {
        var dict = new Dictionary<int, List<PlayoffsDto>>();
        foreach (var serie in playoffs)
        {
            int round = serie.SeriesId switch
            {
                >= 1 and <= 8 => 1,
                >= 9 and <= 12 => 2,
                13 or 14 => 3,
                15 => 4,
                _ => 1
            };
            if (!dict.ContainsKey(round))
                dict[round] = new List<PlayoffsDto>();
            dict[round].Add(serie);
        }
        return dict;
    }

    private static SeasonAwardDto? GetAward(IList<SeasonAwardDto> awards, string awardName)
    {
        return awards?.FirstOrDefault(a => string.Equals(a.AwardName, awardName, StringComparison.OrdinalIgnoreCase));
    }
}
